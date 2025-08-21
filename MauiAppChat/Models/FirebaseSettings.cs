using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiAppChat.Models
{
    public class FirebaseSettings
    {
        #region properties
        public string? ApiKey { get; set; }
        public string? ProjectId { get; set; }
        public string? CredentialsFile { get; set; }
        #endregion
    }
}
