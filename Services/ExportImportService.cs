using Korean_Vocabulary_new.Models;
using System.Text;
using System.Text.Json;
using Microsoft.Maui.Storage;
using Microsoft.Maui.ApplicationModel.DataTransfer;
#if ANDROID
using Android.Content;
using Android.OS;
using Android.Provider;
#endif

namespace Korean_Vocabulary_new.Services
{
    public class ExportImportService
    {
        private readonly DatabaseService _databaseService;

        public ExportImportService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
        }

        public async Task<bool> ExportAsync()
        {
            try
            {
                var words = await _databaseService.GetAllWordsAsync();

                if (words.Count == 0)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Thông báo",
                        "Không có từ vựng để export",
                        "OK");
                    return false;
                }

                // Create simplified word objects for export (without Id and database-specific fields)
                var exportData = words.Select(w => new
                {
                    KoreanWord = w.KoreanWord,
                    VietnameseMeaning = w.VietnameseMeaning,
                    Pronunciation = w.Pronunciation,
                    WordType = w.WordType,
                    Category = w.Category,
                    DifficultyLevel = w.DifficultyLevel,
                    ExampleSentence = w.ExampleSentence,
                    ExampleTranslation = w.ExampleTranslation,
                    IsFavorite = w.IsFavorite
                }).ToList();

                var json = JsonSerializer.Serialize(exportData, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });

                var fileName = $"korean_vocabulary_export_{DateTime.Now:yyyyMMdd_HHmmss}.json";
                string filePath;

#if ANDROID
                // Save to Downloads folder on Android
                filePath = await SaveToDownloadsAsync(fileName, json);
#else
                // Save to AppDataDirectory on other platforms
                filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
                await File.WriteAllTextAsync(filePath, json, Encoding.UTF8);
#endif

                await Application.Current!.MainPage!.DisplayAlert(
                    "Thành công",
                    $"Đã export {words.Count} từ vựng ra file JSON:\n{fileName}\n\nFile được lưu tại: {filePath}",
                    "OK");

                // Try to share the file
                await ShareFileAsync(filePath, fileName);

                return true;
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Lỗi",
                    $"Không thể export file: {ex.Message}",
                    "OK");
                return false;
            }
        }

        public async Task<bool> ImportAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Chọn file JSON để import",
                    FileTypes = new FilePickerFileType(
                        new Dictionary<DevicePlatform, IEnumerable<string>>
                        {
                            { DevicePlatform.Android, new[] { "application/json", "json" } },
                            { DevicePlatform.iOS, new[] { "public.json" } },
                            { DevicePlatform.WinUI, new[] { ".json" } },
                            { DevicePlatform.macOS, new[] { "json" } }
                        })
                });

                if (result == null)
                    return false;

                using var stream = await result.OpenReadAsync();
                using var reader = new StreamReader(stream);
                var jsonContent = await reader.ReadToEndAsync();

                var importData = JsonSerializer.Deserialize<List<ImportWordData>>(jsonContent);

                if (importData == null || importData.Count == 0)
                {
                    await Application.Current!.MainPage!.DisplayAlert(
                        "Lỗi",
                        "File JSON không hợp lệ hoặc trống",
                        "OK");
                    return false;
                }

                return await ProcessImportAsync(importData);
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Lỗi",
                    $"Không thể import file JSON: {ex.Message}",
                    "OK");
                return false;
            }
        }

        private async Task<bool> ProcessImportAsync(List<ImportWordData> importData)
        {
            var action = await Application.Current!.MainPage!.DisplayActionSheet(
                "Chọn phương thức import",
                "Hủy",
                null,
                "Merge (Giữ từ cũ, thêm mới)",
                "Replace (Xóa tất cả, import mới)");

            if (action == null || action == "Hủy")
                return false;

            try
            {
                int importedCount = 0;
                int skippedCount = 0;

                if (action == "Replace (Xóa tất cả, import mới)")
                {
                    // Delete all existing words
                    var allWords = await _databaseService.GetAllWordsAsync();
                    foreach (var word in allWords)
                    {
                        await _databaseService.DeleteWordAsync(word);
                    }
                }

                // Import words
                foreach (var importWord in importData)
                {
                    if (string.IsNullOrWhiteSpace(importWord.KoreanWord) ||
                        string.IsNullOrWhiteSpace(importWord.VietnameseMeaning))
                    {
                        skippedCount++;
                        continue;
                    }

                    // Check if word already exists (only if Merge mode)
                    if (action == "Merge (Giữ từ cũ, thêm mới)")
                    {

                        var existingWord = (await _databaseService.GetAllWordsAsync())
                            .FirstOrDefault(x => x.KoreanWord.Equals(importWord.KoreanWord, StringComparison.OrdinalIgnoreCase));
                        if (existingWord != null)
                        {
                            skippedCount++;
                            if (string.IsNullOrWhiteSpace(importWord.Category))
                            {
                                continue;
                            }

                            if (string.IsNullOrWhiteSpace(existingWord.Category))
                            {
                                existingWord.Category = importWord.Category;

                                await _databaseService.SaveWordAsync(existingWord);
                                continue;
                            }

                            if (existingWord.Category.ToLower().Contains(importWord.Category!.ToLower()))
                            {
                                continue;
                            }
                            existingWord.Category += ", " + importWord.Category;
                            await _databaseService.SaveWordAsync(existingWord);
                            continue;
                        }
                    }

                    var newWord = new VocabularyWord
                    {
                        KoreanWord = importWord.KoreanWord.Trim(),
                        VietnameseMeaning = importWord.VietnameseMeaning.Trim(),
                        Pronunciation = importWord.Pronunciation?.Trim(),
                        WordType = importWord.WordType?.Trim(),
                        Category = importWord.Category?.Trim(),
                        DifficultyLevel = importWord.DifficultyLevel,
                        ExampleSentence = importWord.ExampleSentence?.Trim(),
                        ExampleTranslation = importWord.ExampleTranslation?.Trim(),
                        IsFavorite = importWord.IsFavorite,
                        CreatedDate = DateTime.Now
                    };

                    await _databaseService.SaveWordAsync(newWord);
                    importedCount++;
                }

                await Application.Current!.MainPage!.DisplayAlert(
                    "Thành công",
                    $"Đã import {importedCount} từ vựng.\nBỏ qua {skippedCount} từ (trùng hoặc không hợp lệ).",
                    "OK");

                return true;
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Lỗi",
                    $"Lỗi khi import: {ex.Message}",
                    "OK");
                return false;
            }
        }

        private async Task ShareFileAsync(string filePath, string fileName)
        {
            try
            {
                if (!File.Exists(filePath))
                    return;

                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = $"Chia sẻ {fileName}",
                    File = new ShareFile(filePath)
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Share file error: {ex.Message}");
                // Don't show error to user for share failure
            }
        }

#if ANDROID
        private async Task<string> SaveToDownloadsAsync(string fileName, string content)
        {
            try
            {
                string? savedPath = await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    try
                    {
                        var context = Microsoft.Maui.ApplicationModel.Platform.CurrentActivity?.ApplicationContext ?? 
                                     Android.App.Application.Context;

                        if (context == null)
                        {
                            return null;
                        }

                        // Use MediaStore API for Android 10+ (API 29+)
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.Q)
                        {
                            var contentValues = new ContentValues();
                            contentValues.Put(MediaStore.IMediaColumns.DisplayName, fileName);
                            contentValues.Put(MediaStore.IMediaColumns.MimeType, "application/json");
                            contentValues.Put(MediaStore.IMediaColumns.RelativePath, Android.OS.Environment.DirectoryDownloads);

                            var resolver = context.ContentResolver;
                            var uri = resolver.Insert(MediaStore.Downloads.ExternalContentUri, contentValues);

                            if (uri != null)
                            {
                                using var stream = resolver.OpenOutputStream(uri);
                                if (stream != null)
                                {
                                    var bytes = Encoding.UTF8.GetBytes(content);
                                    await stream.WriteAsync(bytes, 0, bytes.Length);
                                    await stream.FlushAsync();
                                    return $"Downloads/{fileName}";
                                }
                            }
                        }
                        else
                        {
                            // For Android 9 and below, use traditional file system
                            var downloadsDir = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
                            if (downloadsDir != null && downloadsDir.Exists())
                            {
                                var downloadsPath = Path.Combine(downloadsDir.AbsolutePath, fileName);
                                await File.WriteAllTextAsync(downloadsPath, content, Encoding.UTF8);
                                if (File.Exists(downloadsPath))
                                {
                                    return downloadsPath;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"SaveToDownloads error: {ex.Message}");
                    }
                    return null;
                });

                // If save to Downloads succeeded, return the path
                if (!string.IsNullOrEmpty(savedPath))
                {
                    return savedPath;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveToDownloads outer error: {ex.Message}");
            }

            // Fallback to AppDataDirectory if Downloads folder fails
            var fallbackPath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            await File.WriteAllTextAsync(fallbackPath, content, Encoding.UTF8);
            return fallbackPath;
        }
#endif

        private class ImportWordData
        {
            public string KoreanWord { get; set; } = string.Empty;
            public string VietnameseMeaning { get; set; } = string.Empty;
            public string? Pronunciation { get; set; }
            public string? WordType { get; set; }
            public string? Category { get; set; }
            public int DifficultyLevel { get; set; } = 1;
            public string? ExampleSentence { get; set; }
            public string? ExampleTranslation { get; set; }
            public bool IsFavorite { get; set; }
        }
    }
}
