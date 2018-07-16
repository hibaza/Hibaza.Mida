
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class BaseModel
    {
        [BsonId]
        public string _id { get; set; }

        public string id
        {
            get;
            set; 
        }
        
    }
}
