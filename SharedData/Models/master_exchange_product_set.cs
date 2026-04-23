using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_exchange_product_set
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int exchange_product_set_id { get; set; }
        public DateTime sales_start_date { get; set; }
        public DateTime? sales_end_date { get; set; }
        public string title { get; set; }
        public int plate_type { get; set; }
        public string button_image { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
