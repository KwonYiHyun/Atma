using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Models
{
    public class master_gift
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int gift_id { get; set; }
        public string gift_subject { get; set; }
        public string gift_description { get; set; }
        public DateTime start_date { get; set; }
        public DateTime? end_date { get; set; }
        public DateTime insert_date { get; set; }
        public DateTime update_date { get; set; }
    }
}
