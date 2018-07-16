using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Data.Providers.Firebase
{
    public interface IFirebaseFactory : IDisposable
    {
        FirebaseClient GetConnection { get; }
        ChildQuery GetConnectionRef(string business_id);
    }
}
