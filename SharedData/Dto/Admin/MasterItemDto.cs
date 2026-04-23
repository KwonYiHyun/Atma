using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto.Admin
{
    [Serializable]
    public class MasterItemDto
    {
        public int item_id { get; set; }
        public int show_order { get; set; }
        public string item_name { get; set; }
        public int item_type { get; set; }
        public int tab_type { get; set; }
        public int? item_param_1 { get; set; }
        public int? item_param_2 { get; set; }
        public string item_description { get; set; }
        public string item_image_name { get; set; }
        public DateTime? expire_date { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
