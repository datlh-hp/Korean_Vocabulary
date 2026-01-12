using Korean_Vocabulary_new.Models;
using Korean_Vocabulary_new.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Korean_Vocabulary_new.ViewModels
{
    public class CategoryListViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private ObservableCollection<Category> _categories = new();
        private bool _isRefreshing;

        public CategoryListViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            LoadCategoriesCommand = new Command(async () => await LoadCategoriesAsync());
            AddCategoryCommand = new Command(async () => await AddCategoryAsync());
            EditCategoryCommand = new Command<Category>(async (category) => await EditCategoryAsync(category));
            DeleteCategoryCommand = new Command<Category>(async (category) => await DeleteCategoryAsync(category));
            MoveUpCommand = new Command<Category>(async (category) => await MoveCategoryUpAsync(category));
            MoveDownCommand = new Command<Category>(async (category) => await MoveCategoryDownAsync(category));

            Task.Run(LoadCategoriesAsync);
        }

        public ObservableCollection<Category> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
        }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetProperty(ref _isRefreshing, value);
        }

        public ICommand LoadCategoriesCommand { get; }
        public ICommand AddCategoryCommand { get; }
        public ICommand EditCategoryCommand { get; }
        public ICommand DeleteCategoryCommand { get; }
        public ICommand MoveUpCommand { get; }
        public ICommand MoveDownCommand { get; }

        private async Task LoadCategoriesAsync()
        {
            try
            {
                IsRefreshing = true;
                var categories = await _databaseService.GetAllCategoriesAsync();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Categories.Clear();
                    foreach (var category in categories)
                    {
                        Categories.Add(category);
                    }
                });
            }
            catch (Exception ex)
            {
                await Application.Current!.MainPage!.DisplayAlert("Lỗi", $"Không thể tải danh mục: {ex.Message}", "OK");
            }
            finally
            {
                IsRefreshing = false;
            }
        }

        private async Task AddCategoryAsync()
        {
            await Shell.Current.GoToAsync("AddEditCategoryPage");
        }

        private async Task EditCategoryAsync(Category category)
        {
            if (category == null) return;
            await Shell.Current.GoToAsync($"AddEditCategoryPage?Id={category.Id}");
        }

        private async Task DeleteCategoryAsync(Category category)
        {
            if (category == null) return;

            // Không cho phép xóa các danh mục mặc định
            var defaultCategories = new[] { "Tất cả", "Yêu thích", "Mới học", "Cần ôn lại" };
            if (defaultCategories.Contains(category.Name))
            {
                await Application.Current!.MainPage!.DisplayAlert("Thông báo", "Không thể xóa danh mục mặc định này", "OK");
                return;
            }

            // Kiểm tra xem có từ vựng nào đang sử dụng danh mục này không
            var words = await _databaseService.GetWordsByCategoryAsync(category.Name);
            if (words.Count > 0)
            {
                bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                    "Xác nhận",
                    $"Danh mục '{category.Name}' đang có {words.Count} từ vựng. Xóa danh mục này sẽ chuyển các từ vựng về danh mục 'Tất cả'. Bạn có chắc muốn xóa?",
                    "Xóa",
                    "Hủy");

                if (!confirm) return;

                // Chuyển các từ vựng về danh mục "Tất cả"
                foreach (var word in words)
                {
                    word.Category = "Tất cả";
                    await _databaseService.SaveWordAsync(word);
                }
            }
            else
            {
                bool confirm = await Application.Current!.MainPage!.DisplayAlert(
                    "Xác nhận",
                    $"Bạn có chắc muốn xóa danh mục '{category.Name}'?",
                    "Xóa",
                    "Hủy");

                if (!confirm) return;
            }

            await _databaseService.DeleteCategoryAsync(category);
            await LoadCategoriesAsync();
        }

        private async Task MoveCategoryUpAsync(Category category)
        {
            if (category == null) return;
            try
            {
                var currentDisplayOrder = Categories.FirstOrDefault(x => x.DisplayOrder == category.DisplayOrder)!.DisplayOrder;
                if (currentDisplayOrder <= Categories.Min(x => x.DisplayOrder)) return; // Already at top

                var previousCategory = Categories.FirstOrDefault(x => x.DisplayOrder == currentDisplayOrder - 1);

                // Swap DisplayOrder
                var tempOrder = category.DisplayOrder;
                category.DisplayOrder = previousCategory.DisplayOrder;
                previousCategory.DisplayOrder = tempOrder;

                // Update in database
                await _databaseService.UpdateCategoryOrderAsync(new List<Category> { category, previousCategory });

                // Reload to reflect changes
                await LoadCategoriesAsync();
            }
            catch
            {
                var tempList = Categories.OrderBy(c => c.DisplayOrder).ToList();
                for (int i = 0; i < Categories.Count; i++)
                {
                    tempList[i].DisplayOrder = i;
                }
                // Update in database
                await _databaseService.UpdateCategoryOrderAsync(tempList);
                // Reload to reflect changes
                await LoadCategoriesAsync();
            }


        }

        private async Task MoveCategoryDownAsync(Category category)
        {
            if (category == null) return;
            try
            {

                var currentDisplayOrder = Categories.FirstOrDefault(x => x.DisplayOrder == category.DisplayOrder)!.DisplayOrder;
                if (currentDisplayOrder >= Categories.Max(x => x.DisplayOrder)) return; // Already at bottom

                var nextCategory = Categories.FirstOrDefault(x => x.DisplayOrder == currentDisplayOrder + 1);

                // Swap DisplayOrder
                var tempOrder = category.DisplayOrder;
                category.DisplayOrder = nextCategory.DisplayOrder;
                nextCategory.DisplayOrder = tempOrder;

                // Update in database
                await _databaseService.UpdateCategoryOrderAsync(new List<Category> { category, nextCategory });

                // Reload to reflect changes
                await LoadCategoriesAsync();

            }
            catch
            {
                var tempList = Categories.OrderBy(c => c.DisplayOrder).ToList();
                for (int i = 0; i < Categories.Count; i++)
                {
                    tempList[i].DisplayOrder = i;
                }
                // Update in database
                await _databaseService.UpdateCategoryOrderAsync(tempList);
                // Reload to reflect changes
                await LoadCategoriesAsync();
            }
        }
    }
}


