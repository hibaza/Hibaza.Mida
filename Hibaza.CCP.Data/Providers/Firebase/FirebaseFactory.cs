using Firebase.Auth;
using Firebase.Database;
using Firebase.Database.Query;
using Hibaza.CCP.Core;
using Hibaza.CCP.Data.Infrastructure;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Providers.Firebase
{


    public class FirebaseFactory : IFirebaseFactory
    {
        private readonly AppSettings _appSettings;
        public FirebaseFactory(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        private async Task<string> LoginAsync()
        {
            // manage oauth login to Google / Facebook etc.
            // call FirebaseAuthentication.net library to get the Firebase Token
            // return the token

            var authProvider = new FirebaseAuthProvider(new FirebaseConfig(_appSettings.FirebaseDB.APIKey));
            //var facebookAccessToken = "<login with facebook and get oauth access token>";
            //var auth = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Facebook, facebookAccessToken);
            var auth = await authProvider.SignInWithEmailAndPasswordAsync("bazavn88@gmail.com", "Vaza@d4a2");//Vaza@d4a2
            return auth.FirebaseToken;
        }

        public FirebaseClient GetConnection
        {
            get
            {
                var firebase = new FirebaseClient(_appSettings.FirebaseDB.DatabaseURL,
                  new FirebaseOptions
                  {
                      AuthTokenAsyncFactory = () => LoginAsync()
                  });
                //var firebase = new FirebaseClient(_appSettings.FirebaseDB.DatabaseURL);
                return firebase;
            }
        }

        public ChildQuery GetConnectionRef(string rootId)
        {
            rootId = rootId ?? "";
            var firebase = this.GetConnection;
            return firebase.Child(rootId);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ConnectionFactory() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
