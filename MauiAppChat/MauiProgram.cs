using MauiAppChat.Models;
using MauiAppChat.Services;
using MauiAppChat.ViewModels;
using MauiAppChat.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace MauiAppChat
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
                    fonts.AddFont("MaterialIcons-Regular.ttf", "MaterialIconsRegular");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            using var configStream = FileSystem.OpenAppPackageFileAsync("appsettings.local.json").GetAwaiter().GetResult();
            var config = new ConfigurationBuilder()
                .AddJsonStream(configStream)
                .Build();
            // מוסיפים IConfiguration ל-DI
            builder.Services.AddSingleton<IConfiguration>(config);

            // ---- רישום Views, ViewModels, Services, Models ----
            builder.RegisterView()
                   .RegisterViewModel()
                   .Registerservice()
                   .Registermodel();

            return builder.Build();
        }

        public static MauiAppBuilder RegisterView(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<SigninPage>();
            builder.Services.AddSingleton<SignupPage>();
            builder.Services.AddSingleton<RecoveryPasswordPage>();
            builder.Services.AddSingleton<ProfileManagerPage>();
            builder.Services.AddSingleton<UserListPage>();
            builder.Services.AddTransient<ChatPage>();
            builder.Services.AddTransient<ProfilePage>();
            return builder;
        }

        public static MauiAppBuilder RegisterViewModel(this MauiAppBuilder builder)
        {
            builder.Services.AddTransient<BaseViewModel>();
            builder.Services.AddSingleton<SigninPageViewModel>();
            builder.Services.AddSingleton<SignupPageViewModel>();
            builder.Services.AddSingleton<RecoveryPasswordPageViewModel>();
            builder.Services.AddSingleton<ProfileManagerPageViewModel>();
            builder.Services.AddSingleton<UserListPageViewModel>();
            builder.Services.AddTransient<ChatPageViewModel>();
            builder.Services.AddTransient<ProfilePageViewModel>();
            builder.Services.AddSingleton<AppShellViewModel>();
            return builder;
        }

        public static MauiAppBuilder Registerservice(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<FirebaseService>();
            return builder;
        }

        public static MauiAppBuilder Registermodel(this MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<UserProfile>();
            return builder;
        }
    }
}
