using Korean_Vocabulary_new.Models;
using Korean_Vocabulary_new.Services;
using System.Windows.Input;

namespace Korean_Vocabulary_new.ViewModels
{
    [QueryProperty(nameof(CategoryId), "Id")]
    public class AddEditCategoryViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private int _categoryId;
        private string _name = string.Empty;
        private string _color = "#512BD4";
        private static Random _rd = new Random();
        private double _redValue = _rd.Next(0, 255);
        private double _greenValue = _rd.Next(0, 255);
        private double _blueValue = _rd.Next(0, 255);

        public AddEditCategoryViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            SaveCommand = new Command(async () => await SaveCategoryAsync(), () => CanSave);
            CancelCommand = new Command(async () => await CancelAsync());

            UpdateColorFromRgb();
            // Initialize RGB values from default color
            UpdateRgbFromColor(_color);
        }

        public int CategoryId
        {
            get => _categoryId;
            set
            {
                SetProperty(ref _categoryId, value);
                if (value > 0)
                {
                    Task.Run(LoadCategoryAsync);
                }
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                SetProperty(ref _name, value);
                OnPropertyChanged(nameof(CanSave));
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }

        public string Color
        {
            get => _color;
            set
            {
                if (SetProperty(ref _color, value))
                {
                    UpdateRgbFromColor(value);
                }
            }
        }

        public double RedValue
        {
            get => _redValue;
            set
            {
                if (SetProperty(ref _redValue, value))
                {
                    UpdateColorFromRgb();
                }
            }
        }

        public double GreenValue
        {
            get => _greenValue;
            set
            {
                if (SetProperty(ref _greenValue, value))
                {
                    UpdateColorFromRgb();
                }
            }
        }

        public double BlueValue
        {
            get => _blueValue;
            set
            {
                if (SetProperty(ref _blueValue, value))
                {
                    UpdateColorFromRgb();
                }
            }
        }

        public bool CanSave => !string.IsNullOrWhiteSpace(Name);

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        private async Task LoadCategoryAsync()
        {
            try
            {
                var category = await _databaseService.GetCategoryByIdAsync(CategoryId);
                if (category != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Name = category.Name;
                        Color = category.Color;
                    });
                }
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Lỗi", $"Không thể tải danh mục: {ex.Message}", "OK");
            }
        }

        private async Task SaveCategoryAsync()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                await Application.Current!.MainPage!.DisplayAlert("Lỗi", "Vui lòng nhập tên danh mục", "OK");
                return;
            }

            try
            {
                Category category;
                if (CategoryId > 0)
                {
                    var existingCategory = await _databaseService.GetCategoryByIdAsync(CategoryId);
                    if (existingCategory == null)
                    {
                        await Application.Current!.MainPage!.DisplayAlert("Lỗi", "Không tìm thấy danh mục", "OK");
                        return;
                    }
                    category = existingCategory;
                }
                else
                {
                    category = new Category();
                }

                category.Name = Name.Trim();
                category.Color = Color;
                var categories = await _databaseService.GetAllCategoriesAsync();
                category.DisplayOrder = categories.Max(c => c.DisplayOrder) + 1;

                await _databaseService.SaveCategoryAsync(category);
                // Navigate back to CategoryListPage (relative navigation)
                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Lỗi", $"Không thể lưu danh mục: {ex.Message}", "OK");
            }
        }

        private async Task CancelAsync()
        {
            // Navigate back to CategoryListPage (relative navigation)
            await Shell.Current.GoToAsync("..");
        }

        private void UpdateRgbFromColor(string colorHex)
        {
            try
            {
                if (string.IsNullOrEmpty(colorHex) || !colorHex.StartsWith("#"))
                    return;

                // Remove # and parse
                var hex = colorHex.TrimStart('#');
                if (hex.Length == 6)
                {
                    var r = Convert.ToInt32(hex.Substring(0, 2), 16);
                    var g = Convert.ToInt32(hex.Substring(2, 2), 16);
                    var b = Convert.ToInt32(hex.Substring(4, 2), 16);

                    _redValue = r;
                    _greenValue = g;
                    _blueValue = b;
                    OnPropertyChanged(nameof(RedValue));
                    OnPropertyChanged(nameof(GreenValue));
                    OnPropertyChanged(nameof(BlueValue));
                }
            }
            catch
            {
                // Ignore parsing errors
            }
        }

        private void UpdateColorFromRgb()
        {
            var r = (int)Math.Round(_redValue);
            var g = (int)Math.Round(_greenValue);
            var b = (int)Math.Round(_blueValue);

            // Clamp values
            r = Math.Max(0, Math.Min(255, r));
            g = Math.Max(0, Math.Min(255, g));
            b = Math.Max(0, Math.Min(255, b));

            var newColor = $"#{r:X2}{g:X2}{b:X2}";
            if (_color != newColor)
            {
                _color = newColor;
                OnPropertyChanged(nameof(Color));
            }
        }
    }
}

