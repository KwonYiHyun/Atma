using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_gacha
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int gacha_id { get; set; }
        public string gacha_name { get; set; }
        public int show_order { get; set; }
        public int gacha_type { get; set; }
        public int gacha_lot_group_id { get; set; }
        public int gacha_consume_value { get; set; }
        public int gacha_coupon_item_id { get; set; }
        public string gacha_image { get; set; }
        public string gacha_detail_image { get; set; }
        public string gacha_description { get; set; }
        public int gacha_point { get; set; }
        public int gacha_bonus_reward_group_lot_id { get; set; }
        public int? gacha_exec_10_id { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
        public master_gacha_lot_group gacha_lot_group { get; set; }
        public master_gacha_exec_10 gacha_exec_10 { get; set; }
    }
}
