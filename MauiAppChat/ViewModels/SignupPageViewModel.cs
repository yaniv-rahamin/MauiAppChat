using MauiAppChat.Helpers;
using MauiAppChat.Services;
using MauiAppChat.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiAppChat.ViewModels
{
    public class SignupPageViewModel : BaseViewModel
    {
        #region fields
        private readonly FirebaseService? _firebaseService;
        private readonly IServiceProvider? _serviceProvider;
        private string? _email;
        private string? _password;
        private bool _isPassword = true;
        private string _passwordIcon = FontsApp.OPEN_EYE;
        private bool _isEmailValid;
        private bool _isLoggingIn = false;   
        #endregion

        #region commands
        public ICommand NavigateToLoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand TogglePasswordCommand { get; }
        #endregion

         #region properties
        public string? Email
        {
            get => _email;
            set
            {
                if(SetProperty(ref _email, value))
                {
                    OnPropertyChanged(nameof(IsEmailInvalid));
                }

                UpdateCommandState();
            }
        }
        public bool IsEmailInvalid =>!string.IsNullOrWhiteSpace(Email) && !IsValidEmail(Email);
        public bool IsPasswordInvalid => !string.IsNullOrWhiteSpace(Password) && Password.Length<6;


        public string? Password 
        { 
            get => _password;
            set 
            {
                if (SetProperty(ref _password, value))
                {
                    OnPropertyChanged(nameof(IsPasswordInvalid));
                }

                UpdateCommandState();

            }
         
        }
        public bool IsPassword 
        { 
            get =>_isPassword;
            set
            {
                SetProperty(ref _isPassword, value);

            }
        }

        public string PasswordIcon
        {
            get => _passwordIcon;
            set => SetProperty(ref _passwordIcon, value);
        }
        public bool IsEmailValid
        {
            get => _isEmailValid;
            set => SetProperty(ref _isEmailValid, value);
        }
       
        public bool IsBusy
        {
            get => _isLoggingIn;
            set
            {
                SetProperty(ref _isLoggingIn, value);
                UpdateCommandState();
            }

        }


        #endregion
        #region  costructor
        public SignupPageViewModel(FirebaseService? firebaseService, IServiceProvider? serviceProvider)
        {
            _firebaseService = firebaseService;
            _serviceProvider = serviceProvider;
            RegisterCommand = new Command(async () => await SingupAsync(), canSignup);
            NavigateToLoginCommand = new Command(async () => {
                var signinPage = serviceProvider.GetService<SigninPage>();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new NavigationPage(signinPage);
                });
            });
          
            TogglePasswordCommand = new Command(async () => await TogglePasswordVisibilityAsync()); 

        }
        #endregion

        #region Metodes
        private bool canSignup()
        {
            return IsValidEmail(Email) && !IsPasswordInvalid;
        }

        private async Task SingupAsync()
        {
         
            IsBusy = true;
            UpdateCommandState();

            var result = await _firebaseService.SignUpWithEmailAndPasswordAsync(Email, Password);
            if (!string.IsNullOrEmpty(result))
            {
                await SecureStorage.SetAsync("email", Email);
                await SecureStorage.SetAsync("password", Password);
                await SecureStorage.SetAsync("userID", result);

                var shellViewModel = _serviceProvider.GetService<AppShellViewModel>();

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new AppShell(shellViewModel);
                });

                await Shell.Current.GoToAsync("profileManagerPage?mode=create");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "רישום נכשל", "OK");
            }

            IsBusy = false;
            UpdateCommandState();
        }

        public bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // תבנית Regex פשוטה לבדיקה בסיסית
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }
        private async Task TogglePasswordVisibilityAsync()
        {
            IsPassword = !IsPassword;
            PasswordIcon = IsPassword ? FontsApp.OPEN_EYE : FontsApp.CLOSE_EYE;

        }
        private void UpdateCommandState()
        {
            ((Command)RegisterCommand).ChangeCanExecute();
        }
        #endregion
    }

}
