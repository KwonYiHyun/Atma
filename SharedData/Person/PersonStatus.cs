using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class PersonStatus
{
    public int display_person_id { get; set; }
    public string person_hash { get; set; }
    public string email { get; set; }
    public string person_name { get; set; }
    public int level { get; set; }
    public int exp { get; set; }
    public int token { get; set; }
    public int gift { get; set; }
    public int manual { get; set; }
    public int film { get; set; }
    public int prism { get; set; }
    public int leader_person_character_id { get; set; }
    public string introduce { get; set; }
    public DateTime insert_date { get; set; }
}