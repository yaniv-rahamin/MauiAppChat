using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppChat.Models
{
    [FirestoreData]
    public class UserProfile
    {
        #region fields
        private DateTime _dateOfBirth;
        #endregion

        #region properties
        [FirestoreProperty]
        public string? Name { get; set; }
        [FirestoreProperty]
        public string? Email { get; set; }
        [FirestoreProperty]
        public string? ProfilePictureUrl { get; set; }
        [FirestoreProperty]
        public string? StatusMessage { get; set; }

        [FirestoreProperty]
        public DateTime DateOfBirth
        {
            get => _dateOfBirth;
            set => _dateOfBirth = value.Kind == DateTimeKind.Unspecified ? DateTime.SpecifyKind(value, DateTimeKind.Utc) : value;

        }
        #endregion
        

    }
}
