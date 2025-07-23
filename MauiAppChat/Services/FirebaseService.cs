using Google.Cloud.Firestore;
using Firebase.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppChat.Services
{
    public class FirebaseService : IFirebaseService
    {
        #region fields
        private readonly FirebaseAuthProvider? _firebaseAuthProvider;   
        private FirestoreDb? _firestoreDb;
        private const string _projectId = "your-project-id";
        private const string _CredentialsFile = "";
        private const string _apiKey = " ";






        public Task<bool> CreateOrUpdateDataAsync<T>(string Collectotion, string DocumentId, T data)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteDataAsync<T>(string Collection, string DocumentId)
        {
            throw new NotImplementedException();
        }

        public Task<List<T>> GetAllDataAsync<T>(string Collection) where T : class
        {
            throw new NotImplementedException();
        }

        public Task ListenToCollectionChangesAsync<T>(string Collection, Action<List<T>> onDataChanged) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<bool> ReadDateAsync<T>(string Collection, string DocumentId) where T : class
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendPasswordResettEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<string?> SignInWithEmailAndPasswordAsync(string email, string password)
        {
            throw new NotImplementedException();
        }

        public Task<string?> SignUpWithEmailAndPasswordAsync(string email, string password)
        {
            throw new NotImplementedException();
        }
    }
}
