using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_gacha_lot
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int gacha_lot_id { get; set; }
        public int gacha_lot_group_id { get; set; }
        public int gacha_character_id { get; set; }
        public int character_level { get; set; }
        public int character_plus_lot_group { get; set; }
        public int weight { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
        public master_gacha_lot_group gacha_lot_group { set; get; }
        public master_character character { get; set; }
    }
}
