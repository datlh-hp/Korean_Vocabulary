using Korean_Vocabulary_new.ViewModels;
using Microsoft.Maui.Graphics;

namespace Korean_Vocabulary_new.Pages
{
    public partial class AddEditCategoryPage : ContentPage
    {
        private AddEditCategoryViewModel? _viewModel;

        public AddEditCategoryPage(AddEditCategoryViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        private void OnColorChanged(object? sender, ValueChangedEventArgs e)
        {
            if (_viewModel == null || ColorPreview == null) return;

            // Update color preview
            var r = (int)Math.Round(_viewModel.RedValue);
            var g = (int)Math.Round(_viewModel.GreenValue);
            var b = (int)Math.Round(_viewModel.BlueValue);

            // Clamp values
            r = Math.Max(0, Math.Min(255, r));
            g = Math.Max(0, Math.Min(255, g));
            b = Math.Max(0, Math.Min(255, b));

            // Update preview color
            ColorPreview.Color = Color.FromRgb(r, g, b);
        }
    }
}

