using Korean_Vocabulary_new.Models;
using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Korean_Vocabulary_new.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection? _database;

        private readonly SemaphoreSlim _initSemaphore = new SemaphoreSlim(1, 1);
        private bool _isInitialized = false;

        public DatabaseService()
        {
            // Database will be initialized via InitializeAsync() called from App.OnStart()
        }

        public async Task InitializeAsync()
        {
            if (_isInitialized)
                return;

            await _initSemaphore.WaitAsync();
            try
            {
                if (_isInitialized)
                    return;

                var databasePath = Path.Combine(FileSystem.AppDataDirectory, "korean_vocabulary.db");
                _database = new SQLiteAsyncConnection(databasePath);

                // Create tables
                await _database.CreateTableAsync<VocabularyWord>();
                await _database.CreateTableAsync<Category>();

                // Insert default categories if they don't exist
                await SeedDefaultCategories();

                _isInitialized = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine($"Database initialization error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                // Don't throw - let app continue, database will be initialized on next attempt
                // This prevents app from crashing on startup
            }
            finally
            {
                _initSemaphore.Release();
            }
        }

        private async Task SeedDefaultCategories()
        {
            var existingCategories = await _database!.Table<Category>().CountAsync();
            if (existingCategories == 0)
            {
                var defaultCategories = new List<Category>
                {
                    new Category { Name = "Tất cả", Color = "#512BD4", DisplayOrder = 0 },
                    new Category { Name = "Yêu thích", Color = "#FF6B6B", DisplayOrder = 1 },
                    new Category { Name = "Mới học", Color = "#4ECDC4", DisplayOrder = 2 },
                    new Category { Name = "Cần ôn lại", Color = "#FFE66D", DisplayOrder = 3 }
                };

                foreach (var category in defaultCategories)
                {
                    await _database!.InsertAsync(category);
                }
            }
            else
            {
                // Ensure existing default categories have DisplayOrder set
                var allCategories = await _database!.Table<Category>().ToListAsync();
                var defaultCategoryNames = new[] { "Tất cả", "Yêu thích", "Mới học", "Cần ôn lại" };

                for (int i = 0; i < defaultCategoryNames.Length; i++)
                {
                    var category = allCategories.FirstOrDefault(c => c.Name == defaultCategoryNames[i]);
                    if (category != null && category.DisplayOrder != i)
                    {
                        category.DisplayOrder = i;
                        await _database!.UpdateAsync(category);
                    }
                }
            }
        }

        // Vocabulary Word operations
        public async Task<List<VocabularyWord>> GetAllWordsAsync()
        {
            await WaitForDatabase();
            return await _database!.Table<VocabularyWord>().OrderByDescending(w => w.CreatedDate).ToListAsync();
        }

        public async Task<List<VocabularyWord>> GetWordsByCategoryAsync(string? category)
        {
            await WaitForDatabase();
            if (string.IsNullOrEmpty(category) || category == "Tất cả")
            {
                return await GetAllWordsAsync();
            }
            else if (category == "Yêu thích")
            {
                return await _database!.Table<VocabularyWord>()
                    .Where(w => w.IsFavorite)
                    .OrderByDescending(w => w.CreatedDate)
                    .ToListAsync();
            }
            else
            {
                // Support both single category and multiple categories (comma-separated)
                // Load all words first, then filter in memory because SQLite.NET cannot translate
                // string concatenation operations (category + ",") to SQL
                var allWords = await _database!.Table<VocabularyWord>()
                    .Where(w => w.Category != null)
                    .ToListAsync();

                return allWords
                    .Where(w => w.Category == category ||
                               w.Category!.Contains(category + ",") ||
                               w.Category.Contains("," + category))
                    .OrderByDescending(w => w.CreatedDate)
                    .ToList();
            }
        }

        public async Task<VocabularyWord?> GetWordByIdAsync(int id)
        {
            await WaitForDatabase();
            return await _database!.Table<VocabularyWord>().Where(w => w.Id == id).FirstOrDefaultAsync();
        }

        public async Task<VocabularyWord?> GetWordByKoreanWordAsync(string koreanWord, int excludeId = 0)
        {
            await WaitForDatabase();
            if (excludeId > 0)
            {
                return await _database!.Table<VocabularyWord>()
                    .Where(w => w.KoreanWord.ToLower().Replace(" ", "") == koreanWord.ToLower().Replace(" ", "") && w.Id != excludeId)
                    .FirstOrDefaultAsync();
            }
            return await _database!.Table<VocabularyWord>()
                .Where(w => w.KoreanWord.ToLower().Replace(" ", "") == koreanWord.ToLower().Replace(" ", ""))
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveWordAsync(VocabularyWord word)
        {
            await WaitForDatabase();
            if (word.Id != 0)
            {
                return await _database!.UpdateAsync(word);
            }
            else
            {
                word.CreatedDate = DateTime.Now;
                return await _database!.InsertAsync(word);
            }
        }

        public async Task<int> DeleteWordAsync(VocabularyWord word)
        {
            await WaitForDatabase();
            return await _database!.DeleteAsync(word);
        }

        public async Task<List<VocabularyWord>> SearchWordsAsync(string searchText)
        {
            await WaitForDatabase();
            return await _database!.Table<VocabularyWord>()
                .Where(w => w.KoreanWord.ToLower().Replace(" ", "").Contains(searchText.ToLower().Replace(" ", "")) ||
                           w.VietnameseMeaning.ToLower().Replace(" ", "").Contains(searchText.ToLower().Replace(" ", "")) ||
                           (w.Pronunciation != null && w.Pronunciation.ToLower().Replace(" ", "").Contains(searchText.ToLower().Replace(" ", ""))))
                .OrderByDescending(w => w.CreatedDate)
                .ToListAsync();
        }

        // Category operations
        public async Task<List<Category>> GetAllCategoriesAsync()
        {
            await WaitForDatabase();
            return await _database!.Table<Category>().OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ToListAsync();
        }

        public async Task UpdateCategoryOrderAsync(List<Category> categories)
        {
            await WaitForDatabase();
            foreach (var category in categories)
            {
                await _database!.UpdateAsync(category);
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            await WaitForDatabase();
            return await _database!.Table<Category>().Where(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task<int> SaveCategoryAsync(Category category)
        {
            await WaitForDatabase();
            if (category.Id != 0)
            {
                return await _database!.UpdateAsync(category);
            }
            else
            {
                category.CreatedDate = DateTime.Now;
                // Set DisplayOrder to max + 1 for new categories
                var maxOrder = await _database!.Table<Category>().OrderByDescending(c => c.DisplayOrder).FirstOrDefaultAsync();
                category.DisplayOrder = maxOrder != null ? maxOrder.DisplayOrder + 1 : 0;
                return await _database!.InsertAsync(category);
            }
        }

        public async Task<int> DeleteCategoryAsync(Category category)
        {
            await WaitForDatabase();
            return await _database!.DeleteAsync(category);
        }

        // Study operations
        public async Task UpdateStudyStatsAsync(int wordId, bool isCorrect)
        {
            await WaitForDatabase();
            var word = await GetWordByIdAsync(wordId);
            if (word != null)
            {
                word.StudyCount++;
                if (isCorrect)
                {
                    word.CorrectCount++;
                }
                word.LastStudiedDate = DateTime.Now;
                await SaveWordAsync(word);
            }
        }

        public async Task<List<VocabularyWord>> GetWordsForStudyAsync(int count = 10)
        {
            await WaitForDatabase();

            // Get all words first
            var allWords = await _database!.Table<VocabularyWord>().ToListAsync();

            if (allWords.Count == 0)
            {
                return new List<VocabularyWord>();
            }

            // Prioritize words that need review (not studied or low correct rate)
            var wordsToReview = allWords
                .Where(w => w.StudyCount == 0 ||
                           w.LastStudiedDate == DateTime.MinValue ||
                           (w.StudyCount > 0 && (double)w.CorrectCount / w.StudyCount < 0.7))
                .OrderBy(w => w.LastStudiedDate == DateTime.MinValue ? 0 : 1)
                .ThenBy(w => w.StudyCount == 0 ? 0 : (double)w.CorrectCount / w.StudyCount)
                .Take(count)
                .ToList();

            // If not enough words to review, add random words
            if (wordsToReview.Count < count)
            {
                var remainingCount = count - wordsToReview.Count;
                var reviewedWordIds = wordsToReview.Select(w => w.Id).ToHashSet();
                var additionalWords = allWords
                    .Where(w => !reviewedWordIds.Contains(w.Id))
                    .OrderByDescending(w => w.CreatedDate)
                    .Take(remainingCount)
                    .ToList();

                wordsToReview.AddRange(additionalWords);
            }

            // Shuffle the words for variety
            var random = new Random();
            return wordsToReview.OrderBy(x => random.Next()).ToList();
        }

        public async Task<List<VocabularyWord>> GetRandomWordsAsync(int count, List<CategorySelectionItem>? categorys = null, string? wordType = null)
        {
            await WaitForDatabase();

            List<VocabularyWord> allWords;

            var databaseWords = await _database!.Table<VocabularyWord>().ToListAsync();
            // Filter by category if specified
            if (categorys != null && categorys.Any(x => x.IsSelected) && categorys.Any(x => x.Category.Name == "Tất cả" && !x.IsSelected))
            {
                if (categorys.Any(x => x.Category.Name == "Yêu thích" && x.IsSelected))
                {
                    if (categorys.Any(x => x.Category.Name != "Yêu thích" && x.IsSelected))
                    {
                        allWords = databaseWords .Where(w => w.IsFavorite || CheckCategoryMatch(w.Category, categorys)).ToList();
                    }
                    else
                    {
                        allWords = databaseWords.Where(w => w.IsFavorite).ToList();
                    }
                }
                else
                {
                    allWords = databaseWords .Where(w => CheckCategoryMatch(w.Category, categorys)).ToList();
                }
            }
            else
            {
                allWords = await _database!.Table<VocabularyWord>().ToListAsync();
            }

            // Filter by word type if specified
            if (!string.IsNullOrEmpty(wordType) && wordType != "Tất cả")
            {
                allWords = allWords.Where(w => w.WordType == wordType).ToList();
            }

            if (allWords.Count == 0)
            {
                return new List<VocabularyWord>();
            }

            // Shuffle and take count
            var random = new Random();
            return allWords.OrderBy(x => random.Next()).Take(count).ToList();
        }

        private bool CheckCategoryMatch(string? categoryName, List<CategorySelectionItem> categorysCheck)
        {
            if(categoryName == null)
            {
                return false;
            }

            foreach (var categoryItem in categorysCheck)
            {
                if (categoryItem.IsSelected)
                {
                    if(categoryName.Contains(categoryItem.Category.Name))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private async Task WaitForDatabase()
        {
            if (!_isInitialized || _database == null)
            {
                await InitializeAsync();
            }
        }
    }
}

