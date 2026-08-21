
using Korean_Vocabulary_new.Services;
using Korean_Vocabulary_new.ViewModels;

namespace Korean_Vocabulary_new.Pages;

public partial class SettingCategoryPage : ContentPage
{
    private SettingCategoryViewModel? _viewModel;
    public SettingCategoryPage(SettingCategoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        if (category != null)
        {
            if (_viewModel.SetManyHideCommand.CanExecute(category.Text))
            {
                _viewModel.SetManyHideCommand.Execute(category.Text);
            }
        }
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
        if (_viewModel.ResetHideCommand.CanExecute(null))
        {
            _viewModel.ResetHideCommand.Execute(null);
        }
    }

    private void Button_Clicked_2(object sender, EventArgs e)
    {
        if (_viewModel.TrungCheckedCommand.CanExecute(null))
        {
            _viewModel.TrungCheckedCommand.Execute(null);
        }
    }
}