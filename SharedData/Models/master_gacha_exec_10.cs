using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_gacha_exec_10
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int gacha_exec_10_id { get; set; }
        public int gacha_lot_group_id_1 { get; set; }
        public int gacha_lot_count_1 { get; set; }
        public int gacha_lot_group_id_2 { get; set; }
        public int gacha_lot_count_2 { get; set; }
        public int gacha_lot_group_id_3 { get; set; }
        public int gacha_lot_count_3 { get; set; }
        public int gacha_consume_value { get; set; }
        public int gacha_point { get; set; }
        public int gacha_bonus_reward_group_lot_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
        public ICollection<master_gacha> gachas { get; set; }
    }
}
