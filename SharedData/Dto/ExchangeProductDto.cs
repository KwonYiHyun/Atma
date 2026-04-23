using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class ExchangeProductDto
    {
        public int exchange_product_id;
        public int buy_upper_limit;
        public int require_amount;
        public int require_item_id;
        public int reward_id;
        public string image;

        public int pay_count;
    }
}
