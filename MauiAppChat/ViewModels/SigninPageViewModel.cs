using MauiAppChat.Services;
using MauiAppChat.Helpers;
using MauiAppChat.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using MauiAppChat.Models;

namespace MauiAppChat.ViewModels
{
    public class SigninPageViewModel :BaseViewModel
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
        public ICommand LoginCommand { get; }
        public ICommand NavigateToSignupPage { get; }
        public ICommand NavigateToRecoveryPasswordPage { get; }
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
        public SigninPageViewModel(FirebaseService? firebaseService, IServiceProvider? serviceProvider)
        {
            _firebaseService = firebaseService;
            _serviceProvider = serviceProvider;

            LoginCommand = new Command(async () => await LoginAsync() ,canLoging);

            NavigateToSignupPage = new Command (() =>{
                var signupPage = serviceProvider.GetService<SignupPage>();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new NavigationPage(signupPage);
                });
            });

            NavigateToRecoveryPasswordPage = new Command (() =>
            {
                var recoveryPasswordPage = serviceProvider.GetService<RecoveryPasswordPage>();
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage = new NavigationPage(recoveryPasswordPage);
                });
            });
            TogglePasswordCommand =new Command(async () => await TogglePasswordVisibilityAsync());  
            
        }       
        #endregion

        #region Metodes
        private bool canLoging()
        {
            return IsValidEmail(Email) && !IsPasswordInvalid;
        }

        private async Task LoginAsync()
        {
          
            IsBusy = true;
            UpdateCommandState();

            var userId = await _firebaseService.SignInWithEmailAndPasswordAsync(Email, Password);
            if (!string.IsNullOrEmpty(userId))
            {
                // שמירת פרטי התחברות ל-SecureStorage
                await SecureStorage.SetAsync("email", Email);
                await SecureStorage.SetAsync("password", Password);
                await SecureStorage.SetAsync("userID", userId);

                try
                {
                    App.user = await _firebaseService.ReadDateAsync<UserProfile>("users", userId);

                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Error", " והתחבר בחזרה אם אתה מעוניין לעשות שינויי בנתוני המשתמש", "OK");
                }
                // מגדירים את הדף הראשי של האפליציה 
                var shellViewModel = App.Current?.Handler?.MauiContext?.Services?.GetService<AppShellViewModel>();
                Application.Current.MainPage = new AppShell(shellViewModel);

                // מבצעים ניווט ל־bookListPage
                await Shell.Current.GoToAsync("//chatPage");

            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Login failed", "OK");
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
            ((Command)LoginCommand).ChangeCanExecute();
        }
        #endregion





    }
}
