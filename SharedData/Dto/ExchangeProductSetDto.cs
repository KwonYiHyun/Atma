using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Dto
{
    [Serializable]
    public class ExchangeProductSetDto
    {
        public int exchange_product_set_id;
        public DateTime sales_start_date;
        public DateTime? sales_end_date;
        public string title;
        public int plate_type;
        public string button_image;
    }
}
