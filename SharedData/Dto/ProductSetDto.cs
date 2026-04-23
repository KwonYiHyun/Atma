using SharedData.Type;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class ProductSetDto
    {
        public int product_set_id;
        public int product_id;
        public ProductInfoDto product_info;
    }

    [Serializable]
    public class ProductSetPrismDto
    {
        public int product_set_prism_id;
        public ProductInfoDto product_info;
    }

    [Serializable]
    public class ProductSetTokenDto
    {
        public int product_set_token_id;
        public int is_package;
        public ObjectDisplayDto package_reward_display;
        public ProductInfoDto product_info;
    }

    [Serializable]
    public class ProductSetPieceDto
    {
        public int product_set_piece_id;
        public ObjectDisplayDto cost_item;
        public ProductInfoDto product_info;
    }

    [Serializable]
    public class ProductInfoDto
    {
        public int buy_upper_limit;
        public int buy_current_count = 0;
        public int price;
        public PriceType price_type;

        public string product_name { get; set; }
        public string product_detail { get; set; }
        public string image { get; set; }
        public ObjectDisplayDto reward_1;
        public ObjectDisplayDto? reward_2;
        public ObjectDisplayDto? reward_3;

        public long remaining_seconds { get; set; }

        public DateTime start_date;
        public DateTime? end_date;
    }
}
