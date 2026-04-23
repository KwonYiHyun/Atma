using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class person_social
    {
        [Key]
        public int person_social_id { get; set; }
        public int person_id { get; set; }
        public int to_person_id { get; set; }
        public string message_text { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
