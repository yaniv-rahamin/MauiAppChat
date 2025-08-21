using MauiAppChat.ViewModels;
namespace MauiAppChat.Views;

public partial class RecoveryPasswordPage : ContentPage
{
	public RecoveryPasswordPage(RecoveryPasswordPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;	
    }
}