using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_gacha_bonus_reward_group
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int gacha_bonus_reward_group_id { get; set; }
        public int gacha_bonus_reward_group_lot_id { get; set; }
        public int reward_id_1 { get; set; }
        public int reward_id_2 { get; set; }
        public int reward_id_3 { get; set; }
        public int weight { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
