using MauiAppChat.ViewModels;
namespace MauiAppChat.Views;

public partial class SigninPage : ContentPage
{
	public SigninPage(SigninPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}