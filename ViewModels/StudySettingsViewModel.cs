using Korean_Vocabulary_new.Models;
using Korean_Vocabulary_new.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Korean_Vocabulary_new.ViewModels
{
    public class StudySettingsViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<CategorySelectionItem> _categories = new();
        private ObservableCollection<string> _wordTypes = new();
        private Category? _selectedCategoryItem;
        private string _selectedCategory = "Tất cả";
        private string _selectedWordType = string.Empty;
        private string _wordCountText = "10";
        private int _wordCount = 10;
        private bool _isAutoMode = true;
        private bool _isRandomMode = false;
        private bool _isReverseMode = false;
        private bool _isMultipleChoiceMode = false;

        public StudySettingsViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            CancelCommand = new Command(async () => await CancelAsync());
            StartStudyCommand = new Command(async () => await StartStudyAsync(), () => CanStartStudy);

            // Load word types
            var wordTypes = WordTypeHelper.GetWordTypes();
            wordTypes.Insert(0, "Tất cả");
            foreach (var wordType in wordTypes)
            {
                WordTypes.Add(wordType);
            }

            Task.Run(LoadCategoriesAsync);
        }

        public ObservableCollection<CategorySelectionItem> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public ObservableCollection<string> WordTypes
        {
            get => _wordTypes;
            set => SetProperty(ref _wordTypes, value);
        }

        public Category? SelectedCategoryItem
        {
            get => _selectedCategoryItem;
            set
            {
                if (SetProperty(ref _selectedCategoryItem, value))
                {
                    SelectedCategory = value?.Name ?? "Tất cả";
                }
            }
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public string SelectedWordType
        {
            get => _selectedWordType;
            set => SetProperty(ref _selectedWordType, value);
        }

        public string WordCountText
        {
            get => _wordCountText;
            set => SetProperty(ref _wordCountText, value);
        }

        public int WordCount
        {
            get => _wordCount;
            set
            {
                if (SetProperty(ref _wordCount, value))
                {
                    WordCountText = value.ToString();
                    OnPropertyChanged(nameof(CanStartStudy));
                }
            }
        }

        public bool IsAutoMode
        {
            get => _isAutoMode;
            set
            {
                if (SetProperty(ref _isAutoMode, value))
                {
                    if (value)
                    {
                        _isRandomMode = false;
                        OnPropertyChanged(nameof(IsRandomMode));
                    }
                    OnPropertyChanged(nameof(CanStartStudy));
                }
            }
        }

        public bool IsRandomMode
        {
            get => _isRandomMode;
            set
            {
                if (SetProperty(ref _isRandomMode, value))
                {
                    if (value)
                    {
                        _isAutoMode = false;
                        OnPropertyChanged(nameof(IsAutoMode));
                    }
                    OnPropertyChanged(nameof(CanStartStudy));
                }
            }
        }

        public bool IsReverseMode
        {
            get => _isReverseMode;
            set => SetProperty(ref _isReverseMode, value);
        }

        public bool IsMultipleChoiceMode
        {
            get => _isMultipleChoiceMode;
            set => SetProperty(ref _isMultipleChoiceMode, value);
        }

        public bool CanStartStudy => WordCount > 0;

        public void ApplyWordCount()
        {
            if (int.TryParse(WordCountText, out int count) && count > 0)
            {
                _wordCount = count;
                OnPropertyChanged(nameof(WordCount));
                OnPropertyChanged(nameof(CanStartStudy));
                ((Command)StartStudyCommand).ChangeCanExecute();
            }
            else
            {
                _wordCount = 10; // Default
                _wordCountText = "10";
                OnPropertyChanged(nameof(WordCount));
                OnPropertyChanged(nameof(WordCountText));
                OnPropertyChanged(nameof(CanStartStudy));
                ((Command)StartStudyCommand).ChangeCanExecute();
            }
        }

        public ICommand CancelCommand { get; }
        public ICommand StartStudyCommand { get; }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await _databaseService.GetAllCategoriesAsync();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Categories.Clear();
                    // Add "Tất cả" option
                    Categories.Add(new CategorySelectionItem { IsSelected = true, Category = new Category { Name = "Tất cả", Color = "#512BD4" } });
                    foreach (var category in categories.Where(c => c.Name != "Tất cả"))
                    {
                        Categories.Add(new CategorySelectionItem { IsSelected = false, Category = category });
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Lỗi", $"Không thể tải danh mục: {ex.Message}", "OK");
            }
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async Task StartStudyAsync()
        {
            // Apply word count first
            ApplyWordCount();

            List<VocabularyWord> wordsToStudy;

            if (IsAutoMode)
            {
                wordsToStudy = await _databaseService.GetWordsForStudyAsync(WordCount);
            }
            else // IsRandomMode
            {
                wordsToStudy = await _databaseService.GetRandomWordsAsync(WordCount, Categories.ToList(), SelectedWordType);
            }

            if (wordsToStudy.Count == 0)
            {
                await Application.Current!.MainPage!.DisplayAlert("Thông báo", "Không có từ vựng để học", "OK");
                return;
            }

            // Navigate to study page with words and settings
            var wordIds = string.Join(",", wordsToStudy.Select(w => w.Id));
            var reverseModeParam = IsReverseMode ? "true" : "false";
            var multipleChoiceParam = IsMultipleChoiceMode ? "true" : "false";
            await Shell.Current.GoToAsync($"StudyPage?Words={wordIds}&ReverseMode={reverseModeParam}&MultipleChoice={multipleChoiceParam}");
        }
    }
}

