using Korean_Vocabulary_new.Models;
using Korean_Vocabulary_new.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Linq;

namespace Korean_Vocabulary_new.ViewModels
{
    public class StudyViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly AudioService _audioService;
        private VocabularyWord? _currentWord;
        private ObservableCollection<VocabularyWord> _studyWords = new();
        private int _currentIndex = 0;
        private bool _showAnswer = false;
        private string _userAnswer = string.Empty;
        private int _correctCount = 0;
        private int _totalCount = 0;
        private bool _isFinished = false;
        private bool _isReverseMode = false;
        private bool _isMultipleChoiceMode = false;
        private string _choice1 = string.Empty;
        private string _choice2 = string.Empty;
        private string _choice3 = string.Empty;
        private string _choice4 = string.Empty;

        private List<int>? _wordIds;

        public StudyViewModel(DatabaseService databaseService, AudioService audioService)
        {
            _databaseService = databaseService;
            _audioService = audioService;
            StartStudyCommand = new Command(async () => await StartStudyAsync());
            ShowAnswerCommand = new Command(() => ShowAnswer = true);
            CheckAnswerCommand = new Command(async () => await CheckAnswerAsync());
            NextWordCommand = new Command(async () => await NextWordAsync());
            FinishStudyCommand = new Command(async () => await FinishStudyAsync());
            SpeakKoreanCommand = new Command(async () => await SpeakKoreanAsync());
            SpeakVietnameseCommand = new Command(async () => await SpeakVietnameseAsync());
            SelectChoiceCommand = new Command<string>(async (param) => await SelectChoiceAsync(param));

            // Delay start to ensure database is ready
            Task.Run(async () =>
            {
                await Task.Delay(500); // Wait for database initialization
                await StartStudyAsync();
            });
        }

        public void SetWordIds(List<int> wordIds)
        {
            _wordIds = wordIds;
            Task.Run(StartStudyAsync);
        }

        public void SetReverseMode(bool isReverseMode)
        {
            if (SetProperty(ref _isReverseMode, isReverseMode))
            {
                OnPropertyChanged(nameof(QuestionText));
                OnPropertyChanged(nameof(AnswerPlaceholder));
                OnPropertyChanged(nameof(CorrectAnswerText));
                UpdateChoices();
            }
        }

        public void SetMultipleChoiceMode(bool isMultipleChoiceMode)
        {
            if (SetProperty(ref _isMultipleChoiceMode, isMultipleChoiceMode))
            {
                UpdateChoices();
            }
        }

        public VocabularyWord? CurrentWord
        {
            get => _currentWord;
            set
            {
                if (SetProperty(ref _currentWord, value))
                {
                    OnPropertyChanged(nameof(QuestionText));
                    OnPropertyChanged(nameof(AnswerPlaceholder));
                    OnPropertyChanged(nameof(CorrectAnswerText));
                    OnPropertyChanged(nameof(ShowPronunciation));
                    UpdateChoices();
                }
            }
        }

        public ObservableCollection<VocabularyWord> StudyWords
        {
            get => _studyWords;
            set => SetProperty(ref _studyWords, value);
        }

        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                if (SetProperty(ref _currentIndex, value))
                {
                    OnPropertyChanged(nameof(ProgressText));
                }
            }
        }

        public bool ShowAnswer
        {
            get => _showAnswer;
            set => SetProperty(ref _showAnswer, value);
        }

        public string UserAnswer
        {
            get => _userAnswer;
            set => SetProperty(ref _userAnswer, value);
        }

        public int CorrectCount
        {
            get => _correctCount;
            set
            {
                if (SetProperty(ref _correctCount, value))
                {
                    OnPropertyChanged(nameof(ScoreText));
                }
            }
        }

        public int TotalCount
        {
            get => _totalCount;
            set
            {
                if (SetProperty(ref _totalCount, value))
                {
                    OnPropertyChanged(nameof(ProgressText));
                    OnPropertyChanged(nameof(ScoreText));
                }
            }
        }

        public bool IsFinished
        {
            get => _isFinished;
            set => SetProperty(ref _isFinished, value);
        }

        public string ProgressText
        {
            get
            {
                if (TotalCount == 0)
                    return "Chưa có từ vựng";
                return $"{CurrentIndex + 1} / {TotalCount}";
            }
        }
        
        public string ScoreText
        {
            get
            {
                if (TotalCount == 0)
                    return "Chưa có từ vựng để học";
                return $"Đúng: {CorrectCount}/{TotalCount}";
            }
        }

        public bool IsReverseMode
        {
            get => _isReverseMode;
            set => SetReverseMode(value);
        }

        public bool IsMultipleChoiceMode
        {
            get => _isMultipleChoiceMode;
            set => SetMultipleChoiceMode(value);
        }

        public string QuestionText
        {
            get
            {
                if (CurrentWord == null) return string.Empty;
                return IsReverseMode ? CurrentWord.VietnameseMeaning : CurrentWord.KoreanWord;
            }
        }

        public string AnswerPlaceholder
        {
            get
            {
                return IsReverseMode ? "Nhập từ tiếng Hàn..." : "Nhập nghĩa tiếng Việt...";
            }
        }

        public bool ShowPronunciation
        {
            get => !IsReverseMode && CurrentWord != null && !string.IsNullOrWhiteSpace(CurrentWord.Pronunciation);
        }

        public string CorrectAnswerText
        {
            get
            {
                if (CurrentWord == null) return string.Empty;
                return IsReverseMode ? CurrentWord.KoreanWord : CurrentWord.VietnameseMeaning;
            }
        }

        public string Choice1
        {
            get => _choice1;
            set => SetProperty(ref _choice1, value);
        }

        public string Choice2
        {
            get => _choice2;
            set => SetProperty(ref _choice2, value);
        }

        public string Choice3
        {
            get => _choice3;
            set => SetProperty(ref _choice3, value);
        }

        public string Choice4
        {
            get => _choice4;
            set => SetProperty(ref _choice4, value);
        }

        public ICommand StartStudyCommand { get; }
        public ICommand ShowAnswerCommand { get; }
        public ICommand CheckAnswerCommand { get; }
        public ICommand NextWordCommand { get; }
        public ICommand FinishStudyCommand { get; }
        public ICommand SpeakKoreanCommand { get; }
        public ICommand SpeakVietnameseCommand { get; }
        public ICommand SelectChoiceCommand { get; }

        private async Task StartStudyAsync()
        {
            try
            {
                // Wait a bit to ensure database is ready
                await Task.Delay(100);
                
                List<VocabularyWord> words;
                
                // If word IDs are provided, use them
                if (_wordIds != null && _wordIds.Count > 0)
                {
                    words = new List<VocabularyWord>();
                    foreach (var id in _wordIds)
                    {
                        var word = await _databaseService.GetWordByIdAsync(id);
                        if (word != null)
                        {
                            words.Add(word);
                        }
                    }
                }
                else
                {
                    words = await _databaseService.GetWordsForStudyAsync(10);
                }
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    StudyWords.Clear();
                    foreach (var word in words)
                    {
                        StudyWords.Add(word);
                    }
                    
                    TotalCount = StudyWords.Count;
                    CurrentIndex = 0;
                    CorrectCount = 0;
                    IsFinished = false;
                    ShowAnswer = false;
                    UserAnswer = string.Empty;

                    if (StudyWords.Count > 0)
                    {
                        CurrentWord = StudyWords[0];
                        UpdateChoices();
                    }
                    else
                    {
                        // No words available
                        IsFinished = true;
                        CurrentWord = null;
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Lỗi", $"Không thể tải từ vựng: {ex.Message}", "OK");
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    IsFinished = true;
                    CurrentWord = null;
                    TotalCount = 0;
                });
            }
        }

        private async Task CheckAnswerAsync()
        {
            if (CurrentWord == null) return;

            if (string.IsNullOrWhiteSpace(UserAnswer))
            {
                await Application.Current!.MainPage!.DisplayAlert("Thông báo", "Vui lòng nhập đáp án", "OK");
                return;
            }

            string correctAnswer = IsReverseMode ? CurrentWord.KoreanWord : CurrentWord.VietnameseMeaning;
            bool isCorrect = UserAnswer.Trim().Equals(correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase);
            
            await _databaseService.UpdateStudyStatsAsync(CurrentWord.Id, isCorrect);

            if (isCorrect)
            {
                CorrectCount++;
                await Application.Current!.MainPage!.DisplayAlert("Đúng rồi! 👍", "Bạn trả lời đúng!", "OK");
            }
            else
            {
                await Application.Current!.MainPage!.DisplayAlert("Sai rồi 😔", $"Đáp án đúng: {correctAnswer}", "OK");
            }

            ShowAnswer = true;
        }

        private async Task NextWordAsync()
        {
            if (CurrentIndex < StudyWords.Count - 1)
            {
                CurrentIndex++;
                CurrentWord = StudyWords[CurrentIndex];
                ShowAnswer = false;
                UserAnswer = string.Empty;
                UpdateChoices();
            }
            else
            {
                IsFinished = true;
            }
        }

        private async Task FinishStudyAsync()
        {
            await Shell.Current.GoToAsync("..");
        }

        private async Task SpeakKoreanAsync()
        {
            if (CurrentWord != null && !string.IsNullOrWhiteSpace(CurrentWord.KoreanWord))
            {
                await _audioService.SpeakKoreanAsync(CurrentWord.KoreanWord);
            }
        }

        private async Task SpeakVietnameseAsync()
        {
            if (CurrentWord != null && !string.IsNullOrWhiteSpace(CurrentWord.VietnameseMeaning))
            {
                await _audioService.SpeakVietnameseAsync(CurrentWord.VietnameseMeaning);
            }
        }

        private async Task SelectChoiceAsync(string choiceIndex)
        {
            if (CurrentWord == null) return;

            if (!int.TryParse(choiceIndex, out int index) || index < 0 || index > 3)
                return;

            string selectedChoice = index switch
            {
                0 => Choice1,
                1 => Choice2,
                2 => Choice3,
                3 => Choice4,
                _ => string.Empty
            };

            if (string.IsNullOrWhiteSpace(selectedChoice))
                return;

            string correctAnswer = IsReverseMode ? CurrentWord.KoreanWord : CurrentWord.VietnameseMeaning;
            bool isCorrect = selectedChoice.Trim().Equals(correctAnswer.Trim(), StringComparison.OrdinalIgnoreCase);

            await _databaseService.UpdateStudyStatsAsync(CurrentWord.Id, isCorrect);

            if (isCorrect)
            {
                CorrectCount++;
                await Application.Current!.MainPage!.DisplayAlert("Đúng rồi! 👍", "Bạn trả lời đúng!", "OK");
            }
            else
            {
                await Application.Current!.MainPage!.DisplayAlert("Sai rồi 😔", $"Đáp án đúng: {correctAnswer}", "OK");
            }

            ShowAnswer = true;
        }

        private async void UpdateChoices()
        {
            if (CurrentWord == null || !IsMultipleChoiceMode)
            {
                Choice1 = string.Empty;
                Choice2 = string.Empty;
                Choice3 = string.Empty;
                Choice4 = string.Empty;
                return;
            }

            // Get random words for multiple choice options
            var allWords = await _databaseService.GetRandomWordsAsync(10, "Tất cả", "Tất cả");
            var correctAnswer = IsReverseMode ? CurrentWord.KoreanWord : CurrentWord.VietnameseMeaning;
            
            // Filter out the correct answer and current word
            var wrongAnswers = allWords
                .Where(w => w.Id != CurrentWord.Id)
                .Select(w => IsReverseMode ? w.KoreanWord : w.VietnameseMeaning)
                .Where(a => !string.IsNullOrWhiteSpace(a) && !a.Equals(correctAnswer, StringComparison.OrdinalIgnoreCase))
                .Distinct()
                .Take(3)
                .ToList();

            // Create list of choices
            var choices = new List<string> { correctAnswer };
            choices.AddRange(wrongAnswers);

            // Shuffle choices
            var random = new Random();
            choices = choices.OrderBy(x => random.Next()).ToList();

            // Ensure we have 4 choices (pad with empty if needed)
            while (choices.Count < 4)
            {
                choices.Add(string.Empty);
            }

            MainThread.BeginInvokeOnMainThread(() =>
            {
                Choice1 = choices[0];
                Choice2 = choices[1];
                Choice3 = choices[2];
                Choice4 = choices[3];
            });
        }
    }
}

