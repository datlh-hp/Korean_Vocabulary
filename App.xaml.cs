using Korean_Vocabulary_new.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Korean_Vocabulary_new
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Force Light theme - tắt Dark mode để màu sắc không bị đối lập
            UserAppTheme = AppTheme.Light;

            MainPage = new AppShell();
        }

        protected override async void OnStart()
        {
            base.OnStart();
            
            // Initialize database when app starts
            // Use a try-catch to handle any initialization errors gracefully
            try
            {
                // Try to get service from Handler first (works in most cases)
                var dbService = Handler?.MauiContext?.Services?.GetService<DatabaseService>();
                
                // Fallback: try to get from Current if Handler is not available
                if (dbService == null && Current?.Handler?.MauiContext?.Services != null)
                {
                    dbService = Current.Handler.MauiContext.Services.GetService<DatabaseService>();
                }
                
                if (dbService != null)
                {
                    await dbService.InitializeAsync();
                }
            }
            catch (Exception ex)
            {
                // Log error but don't crash the app
                System.Diagnostics.Debug.WriteLine($"Error initializing database in OnStart: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
