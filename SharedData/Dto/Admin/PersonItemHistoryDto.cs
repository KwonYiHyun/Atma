using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto.Admin
{
    [Serializable]
    public class PersonItemHistoryDto
    {
        public int person_item_history_id { get; set; }
        public int person_id { get; set; }
        public int item_id { get; set; }
        public int amount { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
