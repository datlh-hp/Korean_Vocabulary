using Korean_Vocabulary_new.Models;
using Korean_Vocabulary_new.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Korean_Vocabulary_new.ViewModels
{
    public class VocabularyListViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly TopikDataService _topikDataService;
        private readonly AudioService _audioService;
        private readonly ExportImportService _exportImportService;
        private ObservableCollection<VocabularyWord> _words = new();
        private ObservableCollection<Category> _categories = new();
        private Category? _selectedCategoryItem;
        private string _selectedCategory = "Tất cả";
        private string _searchText = string.Empty;
        private bool _isRefreshing;
        private bool _isImporting;
        private bool _isExporting;
        private bool _isListVisible = true;
        private bool _isCategoryVisible = false;

        public VocabularyListViewModel(DatabaseService databaseService, TopikDataService topikDataService, AudioService audioService, ExportImportService exportImportService)
        {
            _databaseService = databaseService;
            _topikDataService = topikDataService;
            _audioService = audioService;
            _exportImportService = exportImportService;
            LoadWordsCommand = new Command(async () => await LoadWordsAsync());
            LoadCategoriesCommand = new Command(async () => await LoadCategoriesAsync());
            AddWordCommand = new Command(async () => await AddWordAsync());
            EditWordCommand = new Command<VocabularyWord>(async (word) => await EditWordAsync(word));
            DeleteWordCommand = new Command<VocabularyWord>(async (word) => await DeleteWordAsync(word));
            ToggleFavoriteCommand = new Command<VocabularyWord>(async (word) => await ToggleFavoriteAsync(word));
            SearchCommand = new Command(async () => await SearchWordsAsync());
            StudyCommand = new Command(async () => await NavigateToStudyAsync());
            ImportTopikCommand = new Command(async () => await ImportTopikDataAsync(), () => !IsImporting);
            ManageCategoriesCommand = new Command(async () => await NavigateToCategoriesAsync());
            ToggleListVisibilityCommand = new Command(() => IsListVisible = !IsListVisible);
            ToggleCategoryVisibilityCommand = new Command(() => IsCategoryVisible = !IsCategoryVisible);
            SpeakKoreanCommand = new Command<VocabularyWord>(async (word) => await SpeakKoreanAsync(word));
            ExportCommand = new Command(async () => await ExportAsync(), () => !IsExporting);
            ImportCommand = new Command(async () => await ImportAsync(), () => !IsImporting && !IsExporting);

            // Don't load data in constructor - wait for OnAppearing
            // This ensures database is initialized first
        }

        public ObservableCollection<VocabularyWord> Words
        {
            get => _words;
            set => SetProperty(ref _words, value);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    Task.Run(LoadWordsAsync);
                }
            }
        }

        public Category? SelectedCategoryItem
        {
            get => _selectedCategoryItem;
            set
            {
                if (SetProperty(ref _selectedCategoryItem, value) && value != null)
                {
                    SelectedCategory = value.Name;
                }
            }
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public bool IsImporting
        {
            get => _isImporting;
            set
            {
                if (SetProperty(ref _isImporting, value))
                {
                    ((Command)ImportTopikCommand).ChangeCanExecute();
                    ((Command)ImportCommand).ChangeCanExecute();
                }
            }
        }

        public bool IsExporting
        {
            get => _isExporting;
            set
            {
                if (SetProperty(ref _isExporting, value))
                {
                    ((Command)ExportCommand).ChangeCanExecute();
                    ((Command)ImportCommand).ChangeCanExecute();
                }
            }
        }

        public bool IsListVisible
        {
            get => _isListVisible;
            set => SetProperty(ref _isListVisible, value);
        }

        public bool IsCategoryVisible
        {
            get => _isCategoryVisible;
            set => SetProperty(ref _isCategoryVisible, value);
        }

        public ICommand LoadWordsCommand { get; }
        public ICommand LoadCategoriesCommand { get; }
        public ICommand AddWordCommand { get; }
        public ICommand EditWordCommand { get; }
        public ICommand DeleteWordCommand { get; }
        public ICommand ToggleFavoriteCommand { get; }
        
        public DatabaseService GetDatabaseService() => _databaseService;
        public ICommand ImportTopikCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand StudyCommand { get; }
        public ICommand ManageCategoriesCommand { get; }
        public ICommand ToggleListVisibilityCommand { get; }
        public ICommand ToggleCategoryVisibilityCommand { get; }
        public ICommand SpeakKoreanCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand ImportCommand { get; }

        private async Task LoadWordsAsync()
        {
            try
            {
                IsRefreshing = true;
                
                // Ensure database is initialized
                await Task.Delay(100); // Give database time to initialize
                
                List<VocabularyWord> words;

                if (!string.IsNullOrWhiteSpace(SearchText))
                {
                    words = await _databaseService.SearchWordsAsync(SearchText);
                }
                else
                {
                    words = await _databaseService.GetWordsByCategoryAsync(SelectedCategory);
                }

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Words.Clear();
                    foreach (var word in words)
                    {
                        Words.Add(word);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadWordsAsync error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Only show alert if MainPage is available
                if (Application.Current?.MainPage != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert("Lỗi", $"Không thể tải từ vựng: {ex.Message}", "OK");
                    });
                }
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                // Ensure database is initialized
                await Task.Delay(100); // Give database time to initialize
                
                var categories = await _databaseService.GetAllCategoriesAsync();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Categories.Clear();
                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"LoadCategoriesAsync error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Only show alert if MainPage is available
                if (Application.Current?.MainPage != null)
                {
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert("Lỗi", $"Không thể tải danh mục: {ex.Message}", "OK");
                    });
                }
            }
        }

        private async Task AddWordAsync()
        {
            await Shell.Current.GoToAsync("AddEditPage");
        }

        private async Task EditWordAsync(VocabularyWord word)
        {
            if (word == null) return;
            await Shell.Current.GoToAsync($"AddEditPage?Id={word.Id}");
        }

        private async Task DeleteWordAsync(VocabularyWord word)
        {
            if (word == null) return;

            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Xác nhận",
                $"Bạn có chắc muốn xóa từ '{word.KoreanWord}'?",
                "Xóa",
                "Hủy");

            if (confirm)
            {
                await _databaseService.DeleteWordAsync(word);
                await LoadWordsAsync();
            }
        }

        private async Task ToggleFavoriteAsync(VocabularyWord word)
        {
            if (word == null) return;
            word.IsFavorite = !word.IsFavorite;
            await _databaseService.SaveWordAsync(word);
            // Không cần reload toàn bộ, chỉ cần cập nhật item trong collection
            // UI sẽ tự động cập nhật nhờ INotifyPropertyChanged
        }

        private async Task SearchWordsAsync()
        {
            await LoadWordsAsync();
        }

        private async Task NavigateToStudyAsync()
        {
            await Shell.Current.GoToAsync("StudySettingsPage");
        }

        private async Task NavigateToCategoriesAsync()
        {
            await Shell.Current.GoToAsync("CategoryListPage");
        }

        private async Task ImportTopikDataAsync()
        {
            bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                "Nhập dữ liệu TOPIK",
                "Bạn có muốn nhập từ vựng TOPIK 1-4 vào ứng dụng không?",
                "Nhập",
                "Hủy");

            if (!confirm) return;

            try
            {
                IsImporting = true;
                await _topikDataService.SeedTopikVocabularyAsync();
                await LoadCategoriesAsync();
                await LoadWordsAsync();
                
                await Application.Current!.MainPage!.DisplayAlert(
                    "Thành công",
                    "Đã nhập từ vựng TOPIK thành công!",
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Lỗi",
                    $"Không thể nhập dữ liệu: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsImporting = false;
            }
        }

        private async Task SpeakKoreanAsync(VocabularyWord word)
        {
            if (word != null && !string.IsNullOrWhiteSpace(word.KoreanWord))
            {
                await _audioService.SpeakKoreanAsync(word.KoreanWord);
            }
        }

        private async Task ExportAsync()
        {
            try
            {
                IsExporting = true;
                await _exportImportService.ExportAsync();
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Lỗi",
                    $"Không thể export: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsExporting = false;
            }
        }

        private async Task ImportAsync()
        {
            try
            {
                IsImporting = true;
                bool success = await _exportImportService.ImportAsync();

                if (success)
                {
                    // Reload data after import
                    await LoadCategoriesAsync();
                    await LoadWordsAsync();
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert(
                    "Lỗi",
                    $"Không thể import: {ex.Message}",
                    "OK");
            }
            finally
            {
                IsImporting = false;
            }
        }
    }
}

