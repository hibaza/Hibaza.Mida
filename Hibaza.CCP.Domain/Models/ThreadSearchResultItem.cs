using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class ThreadSearchResultItem
    {
        public int Weight { get; set; }
        public Thread Thread { get; set; }
    }
}
