using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto.Admin
{
    [Serializable]
    public class MasterProductSetDto
    {
        public int product_set_id { get; set; }
        public int product_id { get; set; }
        public int show_order { get; set; }
        public int buy_upper_limit { get; set; }
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
