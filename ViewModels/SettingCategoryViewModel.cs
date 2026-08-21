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
        TrungCheckedCommand = new Command(async () => await TrungCheckedAsync());

    }


    public ICommand SetManyHideCommand { get; }
    public ICommand ResetHideCommand { get; }
    public ICommand TrungCheckedCommand { get; }

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

    private async Task TrungCheckedAsync()
    {
        try
        {
            var dataCate = await _databaseService.GetAllCategoriesAsync();
            if(!dataCate.Any(x=> x.Name.Equals("trung")))
            {
                await _databaseService.SaveCategoryAsync(new Models.Category { Name = "trung"});
            }
           
            var dataWord = await _databaseService.GetAllWordsAsync();

            List<int> ids = new List<int>();
            int i = 0;
            while (true)
            {
                var koreanWord = dataWord[i].KoreanWord;
                dataWord.RemoveAt(i); ;
                var temp = dataWord.FindAll(x => x.KoreanWord.Trim().Equals(koreanWord.Trim()));
                if (temp.Any())
                {
                   foreach (var item in temp)
                    {
                        await _databaseService.DeleteWordAsync(item);
                    }
                    
                }
                if(dataWord.Count == 0)
                {
                    break;
                }
            }


            await Application.Current!.MainPage!.DisplayAlert("Infor", $"Updated", "OK");
        }
        catch (Exception ex)
        {

            await Application.Current!.MainPage!.DisplayAlert("Error", $"Message: {ex.Message}", "OK");
        }
    }


}