using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class person_loginbonus
    {
        [Key]
        public int person_loginbonus_id { get; set; }
        public int person_id { get; set; }
        public DateTime? pre_login_date { get; set; }
        public int daily_login_bonus_id { get; set; }
        public int total_login_count { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
