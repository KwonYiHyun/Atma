using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_mail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
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
