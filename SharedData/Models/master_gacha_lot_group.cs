using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_gacha_lot_group
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int gacha_lot_group_id { get; set; }
        public int show_order { get; set; }
        public string gacha_lot_group_name { get; set; }
        public string gacha_lot_group_description { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
        public ICollection<master_gacha> gachas {  get; set; }
        public ICollection<master_gacha_lot> gacha_lots { get; set; }
    }
}
