using Korean_Vocabulary_new.ViewModels;

namespace Korean_Vocabulary_new.Pages
{
    [QueryProperty(nameof(WordIds), "Words")]
    [QueryProperty(nameof(ReverseMode), "ReverseMode")]
    [QueryProperty(nameof(MultipleChoice), "MultipleChoice")]
    public partial class StudyPage : ContentPage
    {
        private StudyViewModel? _viewModel;
        private string? _wordIdsString;
        private string? _reverseModeString;
        private string? _multipleChoiceString;

        public string? WordIds
        {
            set
            {
                _wordIdsString = value;
                if (_viewModel != null && !string.IsNullOrEmpty(value))
                {
                    var ids = value.Split(',').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToList();
                    _viewModel.SetWordIds(ids);
                }
            }
        }

        public string? ReverseMode
        {
            set
            {
                _reverseModeString = value;
                if (_viewModel != null && !string.IsNullOrEmpty(value))
                {
                    bool isReverseMode = value.Equals("true", StringComparison.OrdinalIgnoreCase);
                    _viewModel.SetReverseMode(isReverseMode);
                }
            }
        }

        public string? MultipleChoice
        {
            set
            {
                _multipleChoiceString = value;
                if (_viewModel != null && !string.IsNullOrEmpty(value))
                {
                    bool isMultipleChoice = value.Equals("true", StringComparison.OrdinalIgnoreCase);
                    _viewModel.SetMultipleChoiceMode(isMultipleChoice);
                }
            }
        }

        public StudyPage(StudyViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            // Set reverse mode and multiple choice mode FIRST, before loading words
            // This ensures the mode is set before CurrentWord is assigned
            if (!string.IsNullOrEmpty(_reverseModeString))
            {
                bool isReverseMode = _reverseModeString.Equals("true", StringComparison.OrdinalIgnoreCase);
                _viewModel.SetReverseMode(isReverseMode);
            }

            if (!string.IsNullOrEmpty(_multipleChoiceString))
            {
                bool isMultipleChoice = _multipleChoiceString.Equals("true", StringComparison.OrdinalIgnoreCase);
                _viewModel.SetMultipleChoiceMode(isMultipleChoice);
            }

            // Load words LAST, so CurrentWord is set after modes are configured
            if (!string.IsNullOrEmpty(_wordIdsString))
            {
                var ids = _wordIdsString.Split(',').Where(s => int.TryParse(s, out _)).Select(int.Parse).ToList();
                _viewModel.SetWordIds(ids);
            }
        }
    }
}

