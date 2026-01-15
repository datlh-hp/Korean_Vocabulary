using Korean_Vocabulary_new.Models;
using Korean_Vocabulary_new.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace Korean_Vocabulary_new.Pages
{
    public partial class VocabularyListPage : ContentPage
    {
        private VocabularyListViewModel? _viewModel;

        public VocabularyListPage(VocabularyListViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            _viewModel = viewModel;

        }

        private async void OnWordTapped(object? sender, EventArgs e)
        {
            if (sender is BindableObject bindable && bindable.BindingContext is VocabularyWord word)
            {
                await Shell.Current.GoToAsync($"AddEditPage?Id={word.Id}");
            }
        }

        private async void OnWordSelected(object? sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is VocabularyWord word)
            {
                await Shell.Current.GoToAsync($"AddEditPage?Id={word.Id}");
            }
        }

        private async void OnStarButtonLoaded(object? sender, EventArgs e)
        {
            if (sender is ImageButton imageButton && imageButton.CommandParameter is VocabularyWord word && _viewModel != null)
            {
                // Set icon ban đầu dựa trên trạng thái IsFavorite từ database
                var databaseService = _viewModel.GetDatabaseService();
                if (databaseService != null)
                {
                    var wordFromDb = await databaseService.GetWordByIdAsync(word.Id);
                    if (wordFromDb != null)
                    {
                        if (wordFromDb.IsFavorite)
                        {
                            imageButton.Source = ImageSource.FromFile("star_86960.png");
                        }
                        else
                        {
                            imageButton.Source = ImageSource.FromFile("star_black_icons_com_68483.png");
                        }

                        // Cập nhật trạng thái trong word object
                        word.IsFavorite = wordFromDb.IsFavorite;
                    }
                }
            }
        }

        private async void OnFavoriteClicked(object? sender, EventArgs e)
        {
            if (sender is ImageButton imageButton && imageButton.CommandParameter is VocabularyWord word && _viewModel != null)
            {
                // Toggle favorite trong database
                if (_viewModel.ToggleFavoriteCommand.CanExecute(word))
                {
                    _viewModel.ToggleFavoriteCommand.Execute(word);
                }

                // Đợi một chút để database được cập nhật
                await Task.Delay(100);

                // Reload từ database để lấy trạng thái mới
                var databaseService = _viewModel.GetDatabaseService();
                if (databaseService != null)
                {
                    var updatedWord = await databaseService.GetWordByIdAsync(word.Id);
                    if (updatedWord != null)
                    {
                        // Cập nhật icon dựa trên trạng thái IsFavorite từ database
                        if (updatedWord.IsFavorite)
                        {
                            imageButton.Source = ImageSource.FromFile("star_86960.png");
                        }
                        else
                        {
                            imageButton.Source = ImageSource.FromFile("star_black_icons_com_68483.png");
                        }

                        // Cập nhật trạng thái trong word object
                        word.IsFavorite = updatedWord.IsFavorite;
                    }
                }
            }
        }

        private async void OnEditClicked(object? sender, EventArgs e)
        {
            if (sender is ImageButton imageButton && imageButton.CommandParameter is VocabularyWord word && _viewModel != null)
            {
                if (_viewModel.EditWordCommand.CanExecute(word))
                {
                    _viewModel.EditWordCommand.Execute(word);
                }
            }
        }

        private async void OnDeleteClicked(object? sender, EventArgs e)
        {
            if (sender is ImageButton imageButton && imageButton.CommandParameter is VocabularyWord word && _viewModel != null)
            {
                if (_viewModel.DeleteWordCommand.CanExecute(word))
                {
                    _viewModel.DeleteWordCommand.Execute(word);
                }
            }
        }


        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            if (_viewModel != null)
            {
                try
                {
                    // Load data with delay to ensure database is ready
                    await Task.Delay(200);
                    
                    if (_viewModel.LoadCategoriesCommand.CanExecute(null))
                    {
                        _viewModel.LoadCategoriesCommand.Execute(null);
                    }
                    
                    await Task.Delay(100);
                    
                    if (_viewModel.LoadWordsCommand.CanExecute(null))
                    {
                        _viewModel.LoadWordsCommand.Execute(null);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"OnAppearing error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                }
            }
        }

        private void SearchBarControl_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is SearchBar searchBar && _viewModel != null)
            {                
                if (_viewModel.SearchCommand.CanExecute(null))
                {
                    _viewModel.SearchCommand.Execute(null);
                }
            }
        }

        private async void OnSpeakKoreanClicked(object? sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is VocabularyWord word && _viewModel != null)
            {
                if (_viewModel.SpeakKoreanCommand.CanExecute(word))
                {
                    _viewModel.SpeakKoreanCommand.Execute(word);
                }
            }
        }

    }
}

