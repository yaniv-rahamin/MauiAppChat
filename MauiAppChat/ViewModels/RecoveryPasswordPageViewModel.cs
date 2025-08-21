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
    public class RecoveryPasswordPageViewModel :BaseViewModel
    {
        #region fields
        private readonly FirebaseService? _firebaseService;
        private readonly IServiceProvider? _serviceProvider;
        private string? _email;
        #endregion

        #region commands
        public ICommand SendRecoveryEmailCommand { get; }
        public ICommand NavigateToSigninPageCommand { get; }
        #endregion

        #region properties
        public string? Email
        {
            get => _email;
            set
            {
                if (SetProperty(ref _email, value))
                {
                    OnPropertyChanged(nameof(IsEmailInvalid));
                }

                ((Command)SendRecoveryEmailCommand).ChangeCanExecute();
            }
        }

        public bool IsEmailInvalid => !string.IsNullOrWhiteSpace(Email) && !IsValidEmail(Email);

        #endregion

        #region constructor
        public RecoveryPasswordPageViewModel(FirebaseService firebaseService, IServiceProvider serviceProvider)
        {
            _firebaseService = firebaseService;
            _serviceProvider = serviceProvider;

            SendRecoveryEmailCommand = new Command(async () => await SendRecoveryEmailAsync(), CanSendRecoveryEmail);

            NavigateToSigninPageCommand = new Command(() => NavigateToSigninPage());

        }
        #endregion

        #region methods


        private void NavigateToSigninPage()
        {
            var signinPage = _serviceProvider.GetService<SigninPage>();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                Application.Current.MainPage = new NavigationPage(signinPage);
            });
        }




        private bool CanSendRecoveryEmail()
        {
            return  !string.IsNullOrWhiteSpace(Email) && IsValidEmail(Email) ;
        } 

        private async Task SendRecoveryEmailAsync()
        {
            if (CanSendRecoveryEmail())
            {
                await _firebaseService.SendPasswordResettEmailAsync(Email);
                NavigateToSigninPage();
            }
        }

        private bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        #endregion
    }
}
