using Firebase.Auth;
using Google.Api;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using MauiAppChat.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MauiAppChat.Services
{
    public class FirebaseService : IFirebaseService
    {
        #region fields
        private readonly FirebaseAuthProvider? _firebaseAuthProvider;   
        private FirestoreDb? _firestoreDb;
        private readonly string? _projectId;
        private readonly string? _credentialsFile ;
        private readonly string? _apiKey;
        private List<FirestoreChangeListener> listeners = new List<FirestoreChangeListener>();
        private readonly IConfiguration _configuration;
        #endregion

        #region costructor
        //public FirebaseService(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //    var firebaseConfigSection = _configuration.GetSection("Firebase");
        //    var settings = new FirebaseSettings
        //    {
        //        ApiKey = firebaseConfigSection["ApiKey"],
        //        ProjectId = firebaseConfigSection["ProjectId"],
        //        CredentialsFile = firebaseConfigSection["CredentialsFile"]
        //    };

        //    _apiKey = settings.ApiKey;
        //    _projectId = settings.ProjectId;
        //    _credentialsFile = settings.CredentialsFile;

        //    _firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
        //}
        public FirebaseService()
        {
            // טוען את ההגדרות מה־JSON
            var settingsTask = LoadFirebaseSettingsAsync();
            settingsTask.Wait(); // כאן מחכים לסיום הטעינה (בבנאי אסינכרוני לא אפשרי)
            var settings = settingsTask.Result;

            _apiKey = settings.ApiKey;
            _projectId = settings.ProjectId;
            _credentialsFile = settings.CredentialsFile;

            // יוצר את FirebaseAuthProvider
            _firebaseAuthProvider = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
        }
        #endregion

        #region load json settings  
        //private FirebaseSettings LoadFirebaseSettings()
        //{
        //    var appsettingsPath = Path.Combine(FileSystem.AppDataDirectory, "appsettings.local.json");
        //    var json = File.ReadAllText(appsettingsPath);
        //    var root = JsonSerializer.Deserialize<Dictionary<string, FirebaseSettings>>(json);
        //    return root["Firebase"];
        //}
        #endregion

        #region setup firestore method
        //private async Task SetupFirestore()
        //{
        //    if (_firestoreDb == null)
        //    {
        //        var stream = await FileSystem.OpenAppPackageFileAsync(_credentialsFile);
        //        using var reader = new StreamReader(stream);
        //        var jsonCredentials = await reader.ReadToEndAsync();

        //        _firestoreDb = new FirestoreDbBuilder
        //        {
        //            ProjectId = _projectId,
        //            JsonCredentials = jsonCredentials
        //        }.Build();
        //    }
        //}
        private async Task<FirebaseSettings> LoadFirebaseSettingsAsync()
        {
            // שם הקובץ בפרויקט MAUI עם Build Action = Content
            var fileName = "appsettings.local.json";

            using var stream = await FileSystem.OpenAppPackageFileAsync(fileName);
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync();

            using var document = JsonDocument.Parse(json);
            var firebaseSection = document.RootElement.GetProperty("Firebase");

            var settings = new FirebaseSettings
            {
                ApiKey = firebaseSection.GetProperty("ApiKey").GetString()!,
                ProjectId = firebaseSection.GetProperty("ProjectId").GetString()!,
                CredentialsFile = firebaseSection.GetProperty("CredentialsFile").GetString()!
            };

            return settings;
        }
        private async Task SetupFirestore()
        {
            if (_firestoreDb != null)
                return;

            using var stream = await FileSystem.OpenAppPackageFileAsync(_credentialsFile);
            using var reader = new StreamReader(stream);
            var jsonCredentials = await reader.ReadToEndAsync();

            _firestoreDb = new FirestoreDbBuilder
            {
                ProjectId = _projectId,
                JsonCredentials = jsonCredentials
            }.Build();
        }
        #endregion

        #region Firebase authentication methods    
        public async Task<bool> SendPasswordResettEmailAsync(string email)
        {
            try
            {
                await _firebaseAuthProvider.SendPasswordResetEmailAsync(email);
                Console.WriteLine($"Password reset email sent to: {email}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending password reset email: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            try
            {
                var auth = await _firebaseAuthProvider.SignInWithEmailAndPasswordAsync(email, password);                            
                return auth.User.LocalId;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error logging in: " + ex.Message);
                return null;
            }
        }

        public async Task<string?> SignUpWithEmailAndPasswordAsync(string email, string password)
        {
            try
            {
                var auth = await _firebaseAuthProvider.CreateUserWithEmailAndPasswordAsync(email, password);
                return auth.User.LocalId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error registering user: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region firestore CRUD(Create, Read, Update, Delete) Methods
        public async Task<bool> CreateOrUpdateDataAsync<T>(string collection, string documentId, T data)
        {
            try
            {
                await SetupFirestore();
                var document = _firestoreDb.Collection(collection).Document(documentId);
                await document.SetAsync(data);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DOTNET] Error adding data: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner: {ex.InnerException.Message}");
                return false;
                //Console.WriteLine($"Error adding data: {ex.Message}");
                //return false;
            }
        }

       
        public async Task<bool> deleteDataAsync<T>(string collection, string documentId)
        {
            try
            {
                await SetupFirestore();
                var document = _firestoreDb.Collection(collection).Document(documentId);
                await document.DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting data: {ex.Message}");
                return false;
            }
        }

        public async Task<List<T>> GetAllDataAsync<T>(string collection) where T : class
        {
            try
            {
                await SetupFirestore();
                var snapshot = await _firestoreDb.Collection(collection).GetSnapshotAsync();
                return snapshot.Documents.Select(doc => doc.ConvertTo<T>()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting documents: {ex.Message}");
                return new List<T>();
            }
        }

        public async Task<T> ReadDateAsync<T>(string collection, string documentId) where T : class
        {
            try
            {
                await SetupFirestore();
                var document = await _firestoreDb.Collection(collection).Document(documentId).GetSnapshotAsync();
                return document.Exists ? document.ConvertTo<T>() : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading data: {ex.Message}");
                return null;
            }
        }
        #endregion

        #region Listening Methods
        public async Task ListenToCollectionChangesAsync<T>(string collectionName, Action<List<T>> onDataChanged) where T : class
        {
            await SetupFirestore();
            CollectionReference collection = _firestoreDb.Collection(collectionName);

            FirestoreChangeListener listener = collection.Listen(snapshot =>
            {
                List<T> documents = new List<T>();
                foreach (var doc in snapshot.Documents)
                {
                    T data = doc.ConvertTo<T>();
                    documents.Add(data);
                }

                // קורא לפונקציה שמעדכנת את ה-ViewModel
                onDataChanged(documents);
            });

            listeners.Add(listener);
        }
        #endregion



    }
}
