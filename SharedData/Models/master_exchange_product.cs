using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_exchange_product
    {
        [Key]
        public int exchange_product_id { get; set; }
        public int exchange_product_set_id { get; set; }
        public int buy_upper_limit { get; set; }
        public int require_amount { get; set; }
        public int require_item_id { get; set; }
        public int reward_id { get; set; }
        public string image { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
