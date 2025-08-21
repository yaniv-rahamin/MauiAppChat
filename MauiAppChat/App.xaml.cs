using MauiAppChat.Models;
using MauiAppChat.Views;
namespace MauiAppChat
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public static UserProfile user;
      
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var signinPage = _serviceProvider.GetRequiredService<SigninPage>();
            return new Window(signinPage);
        }
    }
}