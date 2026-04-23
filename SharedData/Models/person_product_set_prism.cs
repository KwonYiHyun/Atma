using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class person_product_set_prism
    {
        [Key]
        public int person_product_set_prism_id { get; set; }
        public int person_id { get; set; }
        public int product_set_prism_id { get; set; }
        public int price { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
