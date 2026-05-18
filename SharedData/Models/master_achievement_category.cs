using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_achievement_category
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int achievement_category_id { get; set; }
        public int show_order { get; set; }
        public string category_title { get; set; }
        public int flag { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
