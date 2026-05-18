using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    public class NoticeInfoDto
    {
        public int notice_id { get; set; }
        public string notice_link { get; set; }
        public int notice_type { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
    }
}
