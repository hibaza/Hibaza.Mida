using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Providers.Firebase
{
    public interface IFirebaseStorageFactory : IDisposable
    {
        FirebaseStorage GetStorage { get; }
        FirebaseStorageReference GetStorageRef(string business_id);
    }
}
