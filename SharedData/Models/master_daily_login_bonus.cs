using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_daily_login_bonus
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int daily_login_bonus_id { get; set; }
        public string title_text { get; set; }
        public string back_image { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
        public ICollection<master_daily_login_bonus_day> daily_login_bonus_days { get; set; }
    }
}
