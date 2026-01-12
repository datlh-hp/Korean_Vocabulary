# ProGuard rules for Korean Vocabulary App
# These rules prevent ProGuard from obfuscating or removing code needed for the app to work

# Keep SQLite classes
-keep class sqlite3.** { *; }
-keep class SQLite.** { *; }
-keep class SQLitePCLRaw.** { *; }
-keep class * extends SQLite.** { *; }

# Keep all models with SQLite attributes
-keep class Korean_Vocabulary_new.Models.** { *; }
-keepclassmembers class Korean_Vocabulary_new.Models.** {
    *;
}

# Keep all services
-keep class Korean_Vocabulary_new.Services.** { *; }
-keepclassmembers class Korean_Vocabulary_new.Services.** {
    *;
}

# Keep ViewModels
-keep class Korean_Vocabulary_new.ViewModels.** { *; }
-keepclassmembers class Korean_Vocabulary_new.ViewModels.** {
    *;
}

# Keep Pages
-keep class Korean_Vocabulary_new.Pages.** { *; }
-keepclassmembers class Korean_Vocabulary_new.Pages.** {
    *;
}

# Keep MAUI types
-keep class Microsoft.Maui.** { *; }
-keep class Microsoft.Maui.Controls.** { *; }
-keep class Microsoft.Maui.Essentials.** { *; }

# Keep dependency injection
-keep class Microsoft.Extensions.DependencyInjection.** { *; }

# Keep INotifyPropertyChanged implementations
-keep class * implements System.ComponentModel.INotifyPropertyChanged {
    *;
}

# Keep attributes
-keepattributes *Annotation*
-keepattributes *Attribute*
-keepattributes Signature
-keepattributes Exceptions
-keepattributes InnerClasses
-keepattributes EnclosingMethod

# Keep native methods
-keepclasseswithmembernames class * {
    native <methods>;
}

# Keep setters in Properties
-keepclassmembers class * {
    void set*(***);
    *** get*();
}

# Keep constructors
-keepclassmembers class * {
    <init>();
}

# Keep enums
-keepclassmembers enum * {
    public static **[] values();
    public static ** valueOf(java.lang.String);
}

# Don't warn about missing classes
-dontwarn sqlite3.**
-dontwarn SQLite.**
-dontwarn SQLitePCLRaw.**

