using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto.Admin
{
    [Serializable]
    public class MasterRewardDto
    {
        public int reward_id { get; set; }
        public int object_type { get; set; }
        public int object_value { get; set; }
        public int object_amount { get; set; }
        public string? additional_param { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
