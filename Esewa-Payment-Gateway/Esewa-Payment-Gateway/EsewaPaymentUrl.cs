using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Esewa_Payment_Gateway
{
	internal class EsewaPaymentUrl
	{
		public const string SandBoxUrl = "https://rc-epay.esewa.com.np";
		public const string Url = "https://epay.esewa.com.np";
		public const string InitializationEndpoint = "/api/epay/main/v2/form";
		public const string VerificationEndpoint = "/api/epay/transaction/status";
	}
	public class PaymentApi
	{
		public static string Version = "v2";
	}
	enum EsewaPaymentUrlType
	{
		Initialization = 0,
		Verification
	}
}
