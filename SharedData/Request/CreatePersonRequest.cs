using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Request
{
    [Serializable]
    public class CreatePersonRequest
    {
        public int person_id { get; set; }
        public int display_person_id { get; set; }
        public string person_hash { get; set; }
        public string email { get; set; }
    }
}
