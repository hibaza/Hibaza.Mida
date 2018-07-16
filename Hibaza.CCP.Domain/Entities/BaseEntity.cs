using Hibaza.CCP.Core.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public abstract class BaseEntity
    {
        [BsonId]
        public string _id { get; set; }

        public string id {
            get { return _id.ToString(); }
            set { _id = value; }
        }

        public DateTime created_time { get; set; }
        public DateTime? updated_time { get; set; }
        public BaseEntity()
        {
            id = CommonHelper.GenerateNineDigitUniqueNumber();
            created_time = DateTime.UtcNow;
            updated_time = created_time;
        }

    }
}
