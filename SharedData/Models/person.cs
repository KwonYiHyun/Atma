using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class person
    {
        [Key]
        public int person_id { get; set; }
        public int display_person_id { get; set; }
        public string login_provider { get; set; }
        public string person_hash { get; set; }
        public string email { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
