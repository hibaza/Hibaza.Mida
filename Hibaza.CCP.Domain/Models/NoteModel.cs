using Hibaza.CCP.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hibaza.CCP.Domain.Models
{
    public class NoteModel : BaseModel
    {
        public string type { get; set; }
        public string customer_id { get; set; }
        public string text { get; set; }
        public bool featured { get; set; }
        public string sender_id { get; set; }
        public string sender_name { get; set; }
        public string sender_avatar { get; set; }
        public string created_time { get; set; }
        public string thread_id { get; set; }
        public NoteModel() { }
        public NoteModel(Note note)
        {
            id = note.id;
            created_time = note.created_time.ToLocalTime().ToString("hh:mm dd/MM/yyyy");
            sender_id = note.sender_id;
            thread_id = note.thread_id;
            sender_name = string.IsNullOrWhiteSpace(note.sender_name) ? "?" : note.sender_name;
            sender_avatar = string.IsNullOrWhiteSpace(note.sender_avatar) ? "/avatars/agent.png" : note.sender_avatar;
            text = note.text;
            featured = note.featured;
            type = note.type;
            switch (this.type)
            {
                case "customer":
                    customer_id = note.customer_id;
                    break;
            }
        }
    }
}
