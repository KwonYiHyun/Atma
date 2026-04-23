using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class PersonItem
{
    public int person_item_id { get; set; }
    public int item_id { get; set; }
    public int amount { get; set; }
}