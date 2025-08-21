using MauiAppChat.ViewModels;
namespace MauiAppChat.Views;

public partial class ProfileManagerPage : ContentPage
{
	public ProfileManagerPage(ProfileManagerPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}