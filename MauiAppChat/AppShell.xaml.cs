using MauiAppChat.Views;
using MauiAppChat.ViewModels;
namespace MauiAppChat
{
    public partial class AppShell : Shell
    {
        public AppShell(AppShellViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;

            Routing.RegisterRoute("chatPage", typeof(ChatPage));
            Routing.RegisterRoute("profileManagerPage", typeof(ProfileManagerPage));
        }
    }
}
