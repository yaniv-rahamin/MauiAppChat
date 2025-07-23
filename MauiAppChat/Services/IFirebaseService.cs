using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppChat.Services
{
    public interface IFirebaseService
    {
        #region Authentication Methods
        Task<string?> SignInWithEmailAndPasswordAsync(string email, string password);
        Task<string?> SignUpWithEmailAndPasswordAsync(string email, string password);
        Task <bool> SendPasswordResettEmailAsync(string email);
        #endregion

        #region firestore CRUD(Create, Read, Update, Delete) Methods
        Task<bool> CreateOrUpdateDataAsync<T>(string collection, string documentId , T data);
        Task<T> ReadDateAsync<T>(string collection, string documentId) where T : class;
        Task<bool> deleteDataAsync<T>(string collection, string documentId);
        Task<List<T>> GetAllDataAsync<T>(string cllection) where T : class;
        #endregion

        #region Listening Methods   
        Task ListenToCollectionChangesAsync<T>(string collection, Action<List<T>> onDataChanged) where T : class;
        #endregion

    }
}
