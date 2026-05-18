using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_daily_login_bonus_day
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int daily_login_bonus_day_id { get; set; }
        public int daily_login_bonus_id { get; set; }
        public int total_login_count { get; set; }
        public int reward_id_1 { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
        public master_daily_login_bonus daily_login_bonus { get; set; }
    }
}
