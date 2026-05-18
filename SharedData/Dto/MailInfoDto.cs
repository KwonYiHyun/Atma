using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class MailInfoDto
    {
        public int person_mail_id;
        public string title;
        public string description;
        public bool is_receive;
        public DateTime insert_date;
        public DateTime update_date;
        public DateTime current_date;
        public DateTime? expire_date;

        public ObjectDisplayDto reward;
    }
}
