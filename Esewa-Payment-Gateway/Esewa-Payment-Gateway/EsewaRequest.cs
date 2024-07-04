using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esewa_Payment_Gateway
{
	internal class EsewaRequest
	{
        public string Amount { get; set; }
        public string product_code { get; set; }
        public string total_amount { get; set; }
        public string transaction_uuid { get; set; }
        public string success_url { get; set; }
        public string signature { get; set; }
    }
}
