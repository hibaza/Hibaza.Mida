using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models.Report
{
    public class ReportDataFilter
    {
        public string date { get; set; }
        public long init { get; set; }
        public long finish { get; set; }
        public long init_utc { get { return (init > 99999999999 ? init / 1000 : init) + 7 * 60 * 60; } }
        public long finish_utc { get { return (finish > 99999999999 ? finish / 1000 : finish) + 7 * 60 * 60; } }
        public string agent { get; set; }
        public string period { get; set; }
        public int page { get; set; } = 0;
        public int limit { get; set; } = 10;
        public string search { get; set; } = "";
    }
}
