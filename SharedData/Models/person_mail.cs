using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class person_mail
    {
        [Key]
        public int person_mail_id { get; set; }
        public int person_id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public int reward_id_1 { get; set; }
        public int reward_id_1_amount { get; set; }
        public int? reward_id_2 { get; set; }
        public int? reward_id_2_amount { get; set; }
        public int? reward_id_3 { get; set; }
        public int? reward_id_3_amount { get; set; }
        public bool is_receive { get; set; }
        public DateTime? expire_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
