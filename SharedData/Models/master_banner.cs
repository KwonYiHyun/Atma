using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_banner
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int banner_id { get; set; }
        public int show_order { get; set; }
        public int show_place_type { get; set; }
        public string banner_image { get; set; }
        public int banner_action_type { get; set; }
        public string banner_action_param { get; set; }
        public int limited_type { get; set; }
        public int limited_param { get; set; }
        public int os_type { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
