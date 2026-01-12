using Korean_Vocabulary_new.Models;
using Korean_Vocabulary_new.ViewModels;

namespace Korean_Vocabulary_new.Pages
{
    public partial class CategoryListPage : ContentPage
    {
        private CategoryListViewModel? _viewModel;

        public CategoryListPage(CategoryListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (_viewModel != null && _viewModel.LoadCategoriesCommand.CanExecute(null))
            {
                _viewModel.LoadCategoriesCommand.Execute(null);
            }
        }

        private void OnCategoryTapped(object? sender, EventArgs e)
        {
            if (sender is BindableObject bindable && bindable.BindingContext is Category category && _viewModel != null)
            {
                if (_viewModel.EditCategoryCommand.CanExecute(category))
                {
                    _viewModel.EditCategoryCommand.Execute(category);
                }
            }
        }

        private void OnEditClicked(object? sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is Category category && _viewModel != null)
            {
                if (_viewModel.EditCategoryCommand.CanExecute(category))
                {
                    _viewModel.EditCategoryCommand.Execute(category);
                }
            }
        }

        private void OnDeleteClicked(object? sender, EventArgs e)
        {
            if (sender is ImageButton button && button.CommandParameter is Category category && _viewModel != null)
            {
                if (_viewModel.DeleteCategoryCommand.CanExecute(category))
                {
                    _viewModel.DeleteCategoryCommand.Execute(category);
                }
            }
        }

        private void OnMoveUpClicked(object? sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Category category && _viewModel != null)
            {
                if (_viewModel.MoveUpCommand.CanExecute(category))
                {
                    _viewModel.MoveUpCommand.Execute(category);
                }
            }
        }

        private void OnMoveDownClicked(object? sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is Category category && _viewModel != null)
            {
                if (_viewModel.MoveDownCommand.CanExecute(category))
                {
                    _viewModel.MoveDownCommand.Execute(category);
                }
            }
        }
    }
}


