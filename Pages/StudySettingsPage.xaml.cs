using Korean_Vocabulary_new.ViewModels;

namespace Korean_Vocabulary_new.Pages
{
    public partial class StudySettingsPage : ContentPage
    {
        private StudySettingsViewModel? _viewModel;

        public StudySettingsPage(StudySettingsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        private void OnApplyWordCount(object? sender, EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.ApplyWordCount();
            }
        }
    }
}

