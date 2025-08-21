using MauiAppChat.ViewModels;	
namespace MauiAppChat.Views;

public partial class SignupPage : ContentPage
{
	public SignupPage(SignupPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;	
    }
}