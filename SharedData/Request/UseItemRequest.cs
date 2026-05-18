using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Request
{
    [Serializable]
    public class UseItemRequest
    {
        public UseItemRequest(int id, int count)
        {
            item_id = id;
            amount = count;
        }
        public int item_id;
        public int amount;
    }

    [Serializable]
    public class GiveItemRequest
    {
        public GiveItemRequest(int personId, int itemId, int count)
        {
            person_id = personId;
            item_id = itemId;
            amount = count;
        }
        public int person_id;
        public int item_id;
        public int amount;
    }
}
