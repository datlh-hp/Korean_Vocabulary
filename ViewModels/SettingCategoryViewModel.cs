using Korean_Vocabulary_new.Services;
using System.Windows.Input;

namespace Korean_Vocabulary_new.ViewModels;

public class SettingCategoryViewModel : ContentPage
{

    private readonly DatabaseService _databaseService;
    public SettingCategoryViewModel(DatabaseService databaseService)
    {
        _databaseService = databaseService;

        SetManyHideCommand = new Command<string>(async (categoryText) => await SetManyHideAsync(categoryText));
        ResetHideCommand = new Command(async () => await ResetHideAsync());

    }


    public ICommand SetManyHideCommand { get; }
    public ICommand ResetHideCommand { get; }

    private async Task SetManyHideAsync(string categoryText)
    {

        try
        {
            if (categoryText == null || string.IsNullOrWhiteSpace(categoryText))
            {
                return;
            }
            var dataRaw = await _databaseService.GetAllCategoriesAsync();
            var data = dataRaw.FindAll(x => x.Name.Contains(categoryText));
            foreach (var item in data)
            {
                item.Hide = true;
            }
            await _databaseService.UpdateManyCategoryAsync(data);

            await Application.Current!.MainPage!.DisplayAlert("Infor", $"Updated", "OK");
        }
        catch (Exception ex)
        {

            await Application.Current!.MainPage!.DisplayAlert("Error", $"Message: {ex.Message}", "OK");
        }

    }

    private async Task ResetHideAsync()
    {
        try
        {
            var dataRaw = await _databaseService.GetAllCategoriesAsync();
            foreach (var item in dataRaw)
            {
                item.Hide = false;
            }
            await _databaseService.UpdateManyCategoryAsync(dataRaw);

            await Application.Current!.MainPage!.DisplayAlert("Infor", $"Updated", "OK");
        }
        catch (Exception ex)
        {

            await Application.Current!.MainPage!.DisplayAlert("Error", $"Message: {ex.Message}", "OK");
        }
    }
}