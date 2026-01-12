using Korean_Vocabulary_new.Pages;

namespace Korean_Vocabulary_new
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register routes (only register routes that are NOT in AppShell.xaml)
            // VocabularyListPage is already registered in AppShell.xaml, so don't register it here
            Routing.RegisterRoute(nameof(AddEditPage), typeof(AddEditPage));
            Routing.RegisterRoute(nameof(StudyPage), typeof(StudyPage));
            Routing.RegisterRoute(nameof(StudySettingsPage), typeof(StudySettingsPage));
            Routing.RegisterRoute(nameof(CategoryListPage), typeof(CategoryListPage));
            Routing.RegisterRoute(nameof(AddEditCategoryPage), typeof(AddEditCategoryPage));
        }
    }
}
