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
        Task<bool> CreateOrUpdateDataAsync<T>(string Collectotion ,string DocumentId , T data);
        Task<bool> ReadDateAsync<T>(string Collection, string DocumentId) where T : class;
        Task<bool> deleteDataAsync<T>(string Collection, string DocumentId);
        Task<List<T>> GetAllDataAsync<T>(string Collection) where T : class;
        #endregion

        #region Listening Methods   
        Task ListenToCollectionChangesAsync<T>(string Collection, Action<List<T>> onDataChanged) where T : class;
        #endregion

    }
}
