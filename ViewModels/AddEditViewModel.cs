using Korean_Vocabulary_new.Models;
using Korean_Vocabulary_new.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Korean_Vocabulary_new.ViewModels
{
    public class AddEditViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly TranslationService _translationService;
        private readonly AudioService _audioService;
        private VocabularyWord _word = new();
        private ObservableCollection<CategorySelectionItem> _categorySelections = new();
        private ObservableCollection<string> _wordTypes = new();
        private string _selectedWordType = string.Empty;
        private bool _isAutoFilling = false;
        private string _lastAutoFilledKoreanWord = string.Empty;
        private bool _isTranslatingExample = false;
        private string _lastTranslatedExampleSentence = string.Empty;

        public AddEditViewModel(DatabaseService databaseService, TranslationService translationService, AudioService audioService)
        {
            _databaseService = databaseService;
            _translationService = translationService;
            _audioService = audioService;
            SaveCommand = new Command(async () => await SaveWordAsync(), () => IsValid);
            CancelCommand = new Command(async () => await CancelAsync());
            LoadCategoriesCommand = new Command(async () => await LoadCategoriesAsync());
            AutoFillCommand = new Command(async () => await AutoFillAsync());
            TranslateExampleCommand = new Command(async () => await TranslateExampleAsync(), () => !IsTranslatingExample && !string.IsNullOrWhiteSpace(Word.ExampleSentence));
            SpeakKoreanCommand = new Command(async () => await SpeakKoreanAsync());
            SpeakVietnameseCommand = new Command(async () => await SpeakVietnameseAsync());

            // Subscribe to property changes of the default word
            _word.PropertyChanged += OnWordPropertyChanged;

            // Load word types
            var wordTypes = WordTypeHelper.GetWordTypes();
            foreach (var wordType in wordTypes)
            {
                WordTypes.Add(wordType);
            }

            Task.Run(LoadCategoriesAsync);
        }

        public VocabularyWord Word
        {
            get => _word;
            set
            {
                if (_word != null)
                {
                    _word.PropertyChanged -= OnWordPropertyChanged;
                }

                SetProperty(ref _word, value);

                if (value != null)
                {
                    value.PropertyChanged += OnWordPropertyChanged;
                }

                if (value != null)
                {
                    SelectedWordType = value.WordType ?? string.Empty;
                    
                    // Load selected categories from word
                    UpdateCategorySelectionsFromWord(value.Category);
                    
                    // Cập nhật IsValid và trạng thái nút Lưu sau khi set Word
                    OnPropertyChanged(nameof(IsValid));
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        ((Command)SaveCommand).ChangeCanExecute();
                    });
                }
            }
        }

        public ObservableCollection<CategorySelectionItem> CategorySelections
        {
            get => _categorySelections;
            set => SetProperty(ref _categorySelections, value);
        }

        public ObservableCollection<string> WordTypes
        {
            get => _wordTypes;
            set => SetProperty(ref _wordTypes, value);
        }

        public string SelectedWordType
        {
            get => _selectedWordType;
            set
            {
                SetProperty(ref _selectedWordType, value);
                if (Word != null)
                {
                    Word.WordType = value;
                }
            }
        }

        private int _wordId;
        public int WordId
        {
            get => _wordId;
            set
            {
                if (SetProperty(ref _wordId, value))
                {
                    Task.Run(async () => await LoadWordAsync(value));
                }
            }
        }

        public bool IsValid => !string.IsNullOrWhiteSpace(Word.KoreanWord) && 
                              !string.IsNullOrWhiteSpace(Word.VietnameseMeaning);

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand LoadCategoriesCommand { get; }
        public ICommand AutoFillCommand { get; }
        public ICommand TranslateExampleCommand { get; }
        public ICommand SpeakKoreanCommand { get; }
        public ICommand SpeakVietnameseCommand { get; }

        public bool IsAutoFilling
        {
            get => _isAutoFilling;
            set
            {
                if (SetProperty(ref _isAutoFilling, value))
                {
                    ((Command)AutoFillCommand).ChangeCanExecute();
                }
            }
        }

        public bool IsTranslatingExample
        {
            get => _isTranslatingExample;
            set
            {
                if (SetProperty(ref _isTranslatingExample, value))
                {
                    ((Command)TranslateExampleCommand).ChangeCanExecute();
                }
            }
        }

        private void OnWordPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(VocabularyWord.KoreanWord) ||
                e.PropertyName == nameof(VocabularyWord.VietnameseMeaning))
            {
                OnPropertyChanged(nameof(IsValid));
                // Cập nhật trạng thái nút Lưu
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    ((Command)SaveCommand).ChangeCanExecute();
                });
            }
        }

        private async Task LoadWordAsync(int id)
        {
            if (id == 0)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Word = new VocabularyWord();
                    // Khi tạo từ mới, nút Lưu sẽ bị disable cho đến khi nhập đủ thông tin
                    OnPropertyChanged(nameof(IsValid));
                    ((Command)SaveCommand).ChangeCanExecute();
                });
                return;
            }

            var word = await _databaseService.GetWordByIdAsync(id);
            if (word != null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Word = word;
                    SelectedWordType = word.WordType ?? string.Empty;
                    
                    // Update category selections from word
                    UpdateCategorySelectionsFromWord(word.Category);
                    
                    // Cập nhật IsValid và trạng thái nút Lưu sau khi load từ
                    OnPropertyChanged(nameof(IsValid));
                    ((Command)SaveCommand).ChangeCanExecute();
                });
            }
        }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                var categories = await _databaseService.GetAllCategoriesAsync();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    CategorySelections.Clear();
                    foreach (var category in categories)
                    {
                        // Skip "Tất cả" category as it's not meant to be selected
                        if (category.Name == "Tất cả" || category.Name == "Yêu thích")
                            continue;
                            
                        CategorySelections.Add(new CategorySelectionItem
                        {
                            Category = category,
                            IsSelected = false
                        });
                    }
                    
                    // Update selections if word already has categories
                    if (Word != null && !string.IsNullOrEmpty(Word.Category))
                    {
                        UpdateCategorySelectionsFromWord(Word.Category);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Lỗi", $"Không thể tải danh mục: {ex.Message}", "OK");
            }
        }

        private void UpdateCategorySelectionsFromWord(string? categoryString)
        {
            if (string.IsNullOrEmpty(categoryString))
            {
                // Clear all selections
                foreach (var selection in CategorySelections)
                {
                    selection.IsSelected = false;
                }
                return;
            }

            // Parse comma-separated categories
            var selectedCategoryNames = categoryString.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.Trim())
                .ToList();

            // Update selections
            foreach (var selection in CategorySelections)
            {
                selection.IsSelected = selectedCategoryNames.Contains(selection.Category.Name);
            }
        }

        private async Task SaveWordAsync()
        {
            if (!IsValid)
            {
                await Application.Current!.MainPage!.DisplayAlert("Lỗi", "Vui lòng nhập từ tiếng Hàn và nghĩa tiếng Việt", "OK");
                return;
            }

            try
            {
                // Kiểm tra từ vựng trùng (chỉ khi thêm mới, không kiểm tra khi sửa cùng một từ)
                if (Word.Id == 0)
                {
                    var existingWord = await _databaseService.GetWordByKoreanWordAsync(Word.KoreanWord.Trim());
                    if (existingWord != null)
                    {
                        bool continueSave = await Application.Current!.MainPage!.DisplayAlert(
                            "Cảnh báo",
                            $"Từ vựng '{Word.KoreanWord}' đã tồn tại trong danh sách.\n\nNghĩa hiện tại: {existingWord.VietnameseMeaning}\n\nBạn có muốn tiếp tục lưu không?",
                            "Tiếp tục",
                            "Hủy");
                        
                        if (!continueSave)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    // Khi sửa, kiểm tra xem có từ khác trùng không (trừ chính từ đang sửa)
                    var existingWord = await _databaseService.GetWordByKoreanWordAsync(Word.KoreanWord.Trim(), Word.Id);
                    if (existingWord != null)
                    {
                        bool continueSave = await Application.Current!.MainPage!.DisplayAlert(
                            "Cảnh báo",
                            $"Từ vựng '{Word.KoreanWord}' đã tồn tại trong danh sách.\n\nNghĩa hiện tại: {existingWord.VietnameseMeaning}\n\nBạn có muốn tiếp tục lưu không?",
                            "Tiếp tục",
                            "Hủy");
                        
                        if (!continueSave)
                        {
                            return;
                        }
                    }
                }

                // Save selected categories as comma-separated string
                var selectedCategories = CategorySelections
                    .Where(cs => cs.IsSelected)
                    .Select(cs => cs.Category.Name)
                    .ToList();
                Word.Category = selectedCategories.Any() ? string.Join(",", selectedCategories) : null;
                Word.WordType = SelectedWordType;
                await _databaseService.SaveWordAsync(Word);
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Lỗi", $"Không thể lưu từ vựng: {ex.Message}", "OK");
            }
        }

        private async Task CancelAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        public async Task AutoFillAsync()
        {
            if (string.IsNullOrWhiteSpace(Word.KoreanWord))
                return;

            // Tránh tự động điền lại nếu từ đã được điền rồi
            if (Word.KoreanWord == _lastAutoFilledKoreanWord && 
                !string.IsNullOrWhiteSpace(Word.VietnameseMeaning) && 
                !string.IsNullOrWhiteSpace(Word.Pronunciation))
            {
                return;
            }

            IsAutoFilling = true;
            _lastAutoFilledKoreanWord = Word.KoreanWord;

            try
            {
                // Tự động điền phát âm (Romanization) - luôn có thể làm được
                var romanization = _translationService.ConvertToRomanization(Word.KoreanWord);
                System.Diagnostics.Debug.WriteLine($"Romanization result for '{Word.KoreanWord}': '{romanization}'");
                
                if (!string.IsNullOrWhiteSpace(romanization))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Word.Pronunciation = romanization;
                        OnPropertyChanged(nameof(Word));
                    });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Romanization is empty for '{Word.KoreanWord}'");
                }

                // Tự động phát hiện và chọn loại từ
                var detectedWordType = _translationService.DetectWordType(Word.KoreanWord);
                if (!string.IsNullOrWhiteSpace(detectedWordType))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        // Chỉ tự động chọn nếu chưa có loại từ hoặc đang thêm từ mới
                        if (Word.Id == 0 || string.IsNullOrWhiteSpace(Word.WordType))
                        {
                            SelectedWordType = detectedWordType;
                            Word.WordType = detectedWordType;
                            System.Diagnostics.Debug.WriteLine($"Auto-detected word type for '{Word.KoreanWord}': '{detectedWordType}'");
                        }
                    });
                }

                // Tự động dịch nghĩa tiếng Việt - cần internet
                var translation = await _translationService.TranslateKoreanToVietnameseAsync(Word.KoreanWord);
                if (!string.IsNullOrWhiteSpace(translation))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Word.VietnameseMeaning = translation;
                    });
                }
                else
                {
                    // Nếu không dịch được, chỉ hiển thị thông báo nhẹ
                    // Không bắt buộc phải có nghĩa từ API
                    System.Diagnostics.Debug.WriteLine($"Translation is empty for '{Word.KoreanWord}'");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"AutoFill error: {ex.Message}");
                // Không hiển thị lỗi cho người dùng, chỉ log
            }
            finally
            {
                IsAutoFilling = false;
            }
        }

        /// <summary>
        /// Được gọi khi người dùng nhập từ tiếng Hàn (từ XAML)
        /// </summary>
        public async Task OnKoreanWordChangedAsync()
        {
            // Chỉ tự động điền nếu:
            // 1. Từ tiếng Hàn không rỗng
            // 2. Đang thêm từ mới (Id == 0) hoặc nghĩa/phát âm còn trống
            // 3. Không đang trong quá trình điền
            if (!string.IsNullOrWhiteSpace(Word.KoreanWord) && 
                (Word.Id == 0 || string.IsNullOrWhiteSpace(Word.VietnameseMeaning) || string.IsNullOrWhiteSpace(Word.Pronunciation)) &&
                !IsAutoFilling)
            {
                // Đợi một chút để người dùng gõ xong
                await Task.Delay(500);
                
                // Kiểm tra lại xem từ có thay đổi không và vẫn còn trống
                if (!string.IsNullOrWhiteSpace(Word.KoreanWord) &&
                    (string.IsNullOrWhiteSpace(Word.VietnameseMeaning) || string.IsNullOrWhiteSpace(Word.Pronunciation)))
                {
                    await AutoFillAsync();
                }
            }
        }

        /// <summary>
        /// Dịch câu ví dụ từ tiếng Hàn sang tiếng Việt
        /// </summary>
        public async Task TranslateExampleAsync()
        {
            if (string.IsNullOrWhiteSpace(Word.ExampleSentence))
                return;

            // Tránh dịch lại nếu câu đã được dịch rồi
            if (Word.ExampleSentence == _lastTranslatedExampleSentence && 
                !string.IsNullOrWhiteSpace(Word.ExampleTranslation))
            {
                return;
            }

            IsTranslatingExample = true;
            _lastTranslatedExampleSentence = Word.ExampleSentence;

            try
            {
                // Dịch câu ví dụ từ tiếng Hàn sang tiếng Việt
                var translation = await _translationService.TranslateKoreanToVietnameseAsync(Word.ExampleSentence);
                if (!string.IsNullOrWhiteSpace(translation))
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Word.ExampleTranslation = translation;
                    });
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Translation is empty for example sentence: '{Word.ExampleSentence}'");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"TranslateExampleAsync error: {ex.Message}");
                // Không hiển thị lỗi cho người dùng, chỉ log
            }
            finally
            {
                IsTranslatingExample = false;
            }
        }

        /// <summary>
        /// Được gọi khi người dùng nhập câu ví dụ (từ XAML)
        /// </summary>
        public async Task OnExampleSentenceChangedAsync()
        {
            // Chỉ tự động dịch nếu:
            // 1. Câu ví dụ không rỗng
            // 2. Bản dịch còn trống
            // 3. Không đang trong quá trình dịch
            if (!string.IsNullOrWhiteSpace(Word.ExampleSentence) && 
                string.IsNullOrWhiteSpace(Word.ExampleTranslation) &&
                !IsTranslatingExample)
            {
                // Đợi một chút để người dùng gõ xong
                await Task.Delay(1000);
                
                // Kiểm tra lại xem câu có thay đổi không và bản dịch vẫn còn trống
                if (!string.IsNullOrWhiteSpace(Word.ExampleSentence) &&
                    string.IsNullOrWhiteSpace(Word.ExampleTranslation))
                {
                    await TranslateExampleAsync();
                }
            }
        }

        private async Task SpeakKoreanAsync()
        {
            if (!string.IsNullOrWhiteSpace(Word.KoreanWord))
            {
                await _audioService.SpeakKoreanAsync(Word.KoreanWord);
            }
        }

        private async Task SpeakVietnameseAsync()
        {
            if (!string.IsNullOrWhiteSpace(Word.VietnameseMeaning))
            {
                await _audioService.SpeakVietnameseAsync(Word.VietnameseMeaning);
            }
        }
    }
}

