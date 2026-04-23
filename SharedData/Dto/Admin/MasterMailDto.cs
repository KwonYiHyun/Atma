using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto.Admin
{
    [Serializable]
    public class MasterMailDto
    {
        public int mail_id { get; set; }
        public int sender_type { get; set; }
        public string mail_subject { get; set; }
        public string mail_body { get; set; }
        public int important_flag { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
