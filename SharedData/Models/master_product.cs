using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int product_id { get; set; }
        public string product_name { get; set; }
        public int prism { get; set; }
        public int price { get; set; }
        public int product_type { get; set; }
        public string product_detail { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
