using Korean_Vocabulary_new.ViewModels;

namespace Korean_Vocabulary_new.Pages
{
    [QueryProperty(nameof(WordId), "Id")]
    public partial class AddEditPage : ContentPage
    {
        private AddEditViewModel? _viewModel;
        private string? _wordIdString;

        public string? WordId
        {
            set
            {
                _wordIdString = value;
                if (_viewModel != null && int.TryParse(value, out int id))
                {
                    _viewModel.WordId = id;
                }
            }
        }

        public AddEditPage(AddEditViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
            
            // Load word if ID was set before viewModel was assigned
            if (!string.IsNullOrEmpty(_wordIdString) && int.TryParse(_wordIdString, out int id))
            {
                _viewModel.WordId = id;
            }
        }

        private void OnDifficultyChanged(object? sender, ValueChangedEventArgs e)
        {
            if (DifficultyLabel != null && sender is Slider slider)
            {
                DifficultyLabel.Text = $"Độ khó: {(int)slider.Value}";
            }
        }

        private void OnKoreanWordTextChanged(object? sender, TextChangedEventArgs e)
        {
            // Event handler for Korean word text changed
            // The binding will handle the property update automatically
        }

        private void OnExampleSentenceTextChanged(object? sender, TextChangedEventArgs e)
        {
            // Event handler for example sentence text changed
            // The binding will handle the property update automatically
        }
    }
}

