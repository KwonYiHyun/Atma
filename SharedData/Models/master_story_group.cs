using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_story_group
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int story_group_id { get; set; }
        public int show_order { get; set; }
        public string group_name { get; set; }
        public int story_type { get; set; }
        public string main_character { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
