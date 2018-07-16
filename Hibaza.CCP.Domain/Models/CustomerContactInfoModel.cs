using Hibaza.CCP.Domain.Entities;
using Hibaza.CCP.Domain.Models.Facebook;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hibaza.CCP.Domain.Models
{
    public class CustomerContactInfoModel : BaseModel
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string zipcode { get; set; }
        public bool blocked { get; set; }
        public DateTime birthdate { get; set; }
        public string sex { get; set; }
        public int age { get; set; } = 0;
        public int weight { get; set; } = 0;
        public int height { get; set; } = 0;
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string avatar { get; set; }
        public string business_id { get; set; }
        public DateTime created_time { get; set; } = DateTime.Now;
        public DateTime? updated_time { get; set; } = DateTime.Now;
        public List<string> phone_list { get; set; }
        public string real_name { get; set; } = "";
        

        public CustomerContactInfoModel() { }
        public CustomerContactInfoModel(Customer customer)
        {
            if (customer == null)
                return;
            this.id = customer.id;
            this.name = string.IsNullOrWhiteSpace(customer.name) ? (customer.first_name + " " + customer.last_name) : customer.name;
            this.phone = customer.phone ?? "";
            this.email = customer.email ?? "";
            this.address = customer.address ?? "";
            this.city = customer.city ?? "";
            this.zipcode = customer.zipcode ?? "";
            this.blocked = customer.blocked;
            this.sex = customer.sex ?? "";
            this.phone_list = (customer.phone_list == null ? new List<string>():customer.phone_list);
            this.age = customer.age;
            this.weight = customer.weight;
            this.height = customer.height;
            this.real_name = customer.real_name;
            // this.birthdate = (customer.birthdate == null || customer.birthdate == DateTime.MinValue) ? "" : ((DateTime)customer.birthdate).ToString("dd/MM/yyyy");
        }

        public CustomerContactInfoModel(CustomerContactInfoModel customer)
        {
            if (customer == null)
                return;
            this.id = customer.id;
            this.name = string.IsNullOrWhiteSpace(customer.name)? (customer.first_name + " " + customer.last_name) : customer.name;
            this.phone = customer.phone ?? "";
            this.email = customer.email ?? "";
            this.address = customer.address ?? "";
            this.city = customer.city ?? "";
            this.zipcode = customer.zipcode ?? "";
            this.blocked = customer.blocked;
            this.sex = customer.sex ?? "";
            this.phone_list = customer.phone_list;
           // this.birthdate = ( customer.birthdate== null || customer.birthdate== "0001-01-01T00:00:00Z") ? "" :Convert.ToDateTime(customer.birthdate).ToString("dd-MM-yyyy");
            this.age = customer.age;
            this.weight = customer.weight;
            this.height = customer.height;
            this.first_name = customer.first_name ?? "";
            this.last_name = customer.last_name ?? "";
            this.real_name = customer.real_name;
        }
    }
}
