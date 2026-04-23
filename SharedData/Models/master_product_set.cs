using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_product_set
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int product_set_id { get; set; }
        public int product_id { get; set; }
        public int show_order { get; set; }
        public int buy_upper_limit { get; set; }
        //public int plate_type { get; set; }
        public string image { get; set; }
        public int reward_id_1 { get; set; }
        public int? reward_id_2 { get; set; }
        public int? reward_id_3 { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
