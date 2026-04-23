using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_weekly_mission
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int weekly_mission_id { get; set; }
        public int show_order { get; set; }
        public int category_id { get; set; }
        public string title { get; set; }
        public int judge_type { get; set; }
        public int terms_param_1 { get; set; }
        public int terms_param_2 { get; set; }
        public int judge_param { get; set; }
        public string description { get; set; }
        public int reward_id_1 { get; set; }
        public int reward_id_2 { get; set; }
        public int reward_id_3 { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
