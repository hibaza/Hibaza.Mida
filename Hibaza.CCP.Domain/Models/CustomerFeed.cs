using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class CustomerFeed
    {
        public Paging Paging { get; set; }

        public IEnumerable<CustomerModel> Data { get; set; }
    }
}
