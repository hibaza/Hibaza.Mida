using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Entities
{
    public class Task : BaseEntity
    {
        public string LastMessageId { get; set; }
        public string Type { get; set; } //message, comment, call, email
        public string ConversationId { get; set; }
    }
}
