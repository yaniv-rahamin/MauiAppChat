using MauiAppChat.Models;
using MauiAppChat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MauiAppChat.ViewModels
{
    public class ProfileManagerPageViewModel : BaseViewModel ,IQueryAttributable
    {
        #region fields
        private readonly FirebaseService firebaseService;
        private string fullName;
        private ImageSource profileImage;
        private string savedImagePath;
        private bool isBusy;
        private string mode;
        private DateTime birthDate;        
        private string statusMessage;
        #endregion

        #region commands
        public ICommand PickImageCommand { get; }
        public ICommand TakePhotoCommand { get; }
        public ICommand SaveCommand { get; }
        #endregion

        #region constructor
        public ProfileManagerPageViewModel(FirebaseService firebaseService)
        {
            this.firebaseService = firebaseService;
            Title = "Profile Manager";
            PickImageCommand = new Command(async () => await PickImageAsync());
            TakePhotoCommand = new Command(async () => await TakePhotoAsync());
            SaveCommand = new Command(async () => await SaveProfileAsync());
            mode = "Edit";
            birthDate = DateTime.Today;
        }
        #endregion

        #region properties
        public string FullName
        {
            get => fullName;
            set 
            {
                SetProperty(ref fullName, value);
                UpdateCommandState();
            }


        }

        public ImageSource ProfileImage
        {
            get => profileImage;
            set
            {
                SetProperty(ref profileImage, value);
                UpdateCommandState();
            }

            }

        public string Mode
        {
            get => mode;
            set
            {
                SetProperty(ref mode, value);
                UpdateCommandState();
            }

        }

        public bool IsBusy
        {
            get => isBusy;
            set => SetProperty(ref isBusy, value);

        }
        public DateTime BirthDate
        {
            get => birthDate;
            set => SetProperty(ref birthDate, value);
        }

        public string StatusMessage
        {
            get => statusMessage;
            set => SetProperty(ref statusMessage, value);
        }

        public string Title { get; set; }
        #endregion

        #region methods

        private void UpdateCommandState()
        {
            ((Command)SaveCommand).ChangeCanExecute();
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey("mode"))
                Mode = query["mode"].ToString();

            if (Mode == "edit" && App.user != null)
            {
                Title = "עדכון פרופיל";
                FullName = App.user.Name;
                StatusMessage = App.user.StatusMessage;
                BirthDate = App.user.DateOfBirth == default ? DateTime.Today : App.user.DateOfBirth;
                ProfileImage = string.IsNullOrEmpty(App.user.ProfilePictureUrl)
                    ? "default_profile.png"
                    : ImageSource.FromFile(App.user.ProfilePictureUrl);
                savedImagePath = App.user.ProfilePictureUrl;
            }
            else
            {
                Title = "צור פרופיל";
                FullName = string.Empty;
                ProfileImage = null;
                savedImagePath = null;
                BirthDate = DateTime.Today ;
            }

            OnPropertyChanged(nameof(Title));
            UpdateCommandState();
        }

        private bool CanExecuteSave()
        {
            if (Mode == "edit")
                return true;

            return !string.IsNullOrWhiteSpace(FullName) && ProfileImage != null;
        }

        private async Task PickImageAsync()
        {
            try
            {
                IsBusy = true;

#if ANDROID
        // באנדרואיד 12 ומטה יש צורך בהרשאה
        if (DeviceInfo.Version.Major < 13)
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();
                if (status != PermissionStatus.Granted)
                {
                    await Shell.Current.DisplayAlert("שגיאה", "אין הרשאה לגשת לקבצים", "אישור");
                    return;
                }
            }
        }
         
#endif

                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "בחר תמונה",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    savedImagePath = await SaveImageToLocalAsync(stream, result.FileName);
                    ProfileImage = ImageSource.FromFile(savedImagePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Pick error: " + ex.Message);
                await Shell.Current.DisplayAlert("שגיאה", "אירעה שגיאה בבחירת קובץ", "אישור");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task TakePhotoAsync()
        {
            try
            {
                IsBusy = true;

                var cameraStatus = await Permissions.CheckStatusAsync<Permissions.Camera>();
                if (cameraStatus != PermissionStatus.Granted)
                {
                    cameraStatus = await Permissions.RequestAsync<Permissions.Camera>();
                }

                if (cameraStatus != PermissionStatus.Granted)
                {
                    await Shell.Current.DisplayAlert("שגיאה", "אין הרשאה למצלמה", "אישור");
                    return;
                }

                if (MediaPicker.Default.IsCaptureSupported)
                {
                    var photo = await MediaPicker.Default.CapturePhotoAsync();
                    if (photo != null)
                    {
                        using var stream = await photo.OpenReadAsync();
                        savedImagePath = await SaveImageToLocalAsync(stream, photo.FileName);
                        ProfileImage = ImageSource.FromFile(savedImagePath);
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("שגיאה", "התקן לא תומך בצילום", "אישור");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Camera error: " + ex.Message);
                await Shell.Current.DisplayAlert("שגיאה", "אירעה שגיאה בשימוש במצלמה", "אישור");
            }
            finally
            {
                IsBusy = false;
            }
        }
        private async Task<string> SaveImageToLocalAsync(Stream stream, string fileName)
        {
            var filePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            using var fileStream = File.OpenWrite(filePath);
            await stream.CopyToAsync(fileStream);
            return filePath;
        }
        private async Task SaveProfileAsync()
        {
            if (string.IsNullOrWhiteSpace(FullName))
            {
                await Shell.Current.DisplayAlert("שגיאה", "אנא הזן שם", "אישור");
                return;
            }

            IsBusy = true;

            var userId = await SecureStorage.GetAsync("userID");
            var email = await SecureStorage.GetAsync("email");

            if (App.user == null)
                App.user = new UserProfile();

            App.user.Name = FullName;
            App.user.Email = email;
            App.user.StatusMessage = StatusMessage;
            App.user.DateOfBirth = BirthDate;

            if (!string.IsNullOrEmpty(savedImagePath))
                App.user.ProfilePictureUrl = savedImagePath;

            bool success = await firebaseService.CreateOrUpdateDataAsync("users", userId, App.user);

            IsBusy = false;

            if (success)
            {
                await Shell.Current.DisplayAlert("הצלחה", "הפרופיל נשמר", "אישור");
                if (Mode == "edit")
                    await Shell.Current.GoToAsync("///profilePage");
                else
                    await Shell.Current.GoToAsync("///chatPage");
            }
            else
            {
                await Shell.Current.DisplayAlert("שגיאה", "אירעה שגיאה בשמירה", "אישור");
            }
        }


        #endregion

    }

}
