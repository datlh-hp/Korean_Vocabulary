using Microsoft.Extensions.Logging;
using Korean_Vocabulary_new.Services;
using Korean_Vocabulary_new.ViewModels;
using Korean_Vocabulary_new.Pages;
using Korean_Vocabulary_new.Converters;

namespace Korean_Vocabulary_new
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            // Register Services
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<TopikDataService>();
            builder.Services.AddSingleton<TranslationService>();
            builder.Services.AddSingleton<AudioService>();
            builder.Services.AddSingleton<ExportImportService>();

            // Register ViewModels
            builder.Services.AddTransient<VocabularyListViewModel>();
            builder.Services.AddTransient<AddEditViewModel>();
            builder.Services.AddTransient<StudyViewModel>();
            builder.Services.AddTransient<StudySettingsViewModel>();
            builder.Services.AddTransient<CategoryListViewModel>();
            builder.Services.AddTransient<AddEditCategoryViewModel>();

            // Register Pages
            builder.Services.AddTransient<VocabularyListPage>();
            builder.Services.AddTransient<AddEditPage>();
            builder.Services.AddTransient<StudyPage>();
            builder.Services.AddTransient<StudySettingsPage>();
            builder.Services.AddTransient<CategoryListPage>();
            builder.Services.AddTransient<AddEditCategoryPage>();

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
