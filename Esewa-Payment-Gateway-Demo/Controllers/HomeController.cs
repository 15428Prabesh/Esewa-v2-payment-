using Esewa_Payment_Gateway;
using Esewa_Payment_Gateway_Demo.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Esewa_Payment_Gateway_Demo.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private static string secretKey = "EPAYTEST";
		private static bool sandBoxMode = true;
        private string data = "eyJ0cmFuc2FjdGlvbl9jb2RlIjoiMDAwN1pLSCIsInN0YXR1cyI6IkNPTVBMRVRFIiwidG90YWxfYW1vdW50IjoiMTAwLjAiLCJ0cmFuc2FjdGlvbl91dWlkIjoiZGM5YzE2MDEtZTU0MS00ZmRiLWIyODUtYTc3OTU1MzY4NmY2IiwicHJvZHVjdF9jb2RlIjoiRVBBWVRFU1QiLCJzaWduZWRfZmllbGRfbmFtZXMiOiJ0cmFuc2FjdGlvbl9jb2RlLHN0YXR1cyx0b3RhbF9hbW91bnQsdHJhbnNhY3Rpb25fdXVpZCxwcm9kdWN0X2NvZGUsc2lnbmVkX2ZpZWxkX25hbWVzIiwic2lnbmF0dXJlIjoiSnZYTDNFQ3EyWDMxK294VUR4UDZJVGFhQVQ3M1Jwa2puWDRCMWFQOUk1QT0ifQ==";
			

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}
        public async Task<IActionResult> Index()
		{
			PaymentApi.Version = "v2";
			if (!string.IsNullOrWhiteSpace(data))
			{
				await VerifyPayment(data);
			}
			return View();
		}
		public async Task<ActionResult> PayWithEsewa()
		{
			var productCode = "EPAYTEST";
			var totalAmount = "100";
			var transactionUuid = Guid.NewGuid().ToString();
			var secretKey = "8gBm/:&EnhH.1/q";
            var signature_sha = GenerateSignature(totalAmount, transactionUuid, productCode , secretKey);
            EsewaPayment EsewaPayment = new EsewaPayment(secretKey, sandBoxMode);
			var EsewaRequest = new
			{
				amount = "100",
                failure_url = "https://google.com",
                product_service_charge = "0",
                product_delivery_charge = "0",
                product_code = productCode,
                signature = signature_sha,                
                signed_field_names = "total_amount,transaction_uuid,product_code",
                success_url = "https://esewa.com.np",
                tax_amount = "0",
                total_amount = totalAmount,
                transaction_uuid = transactionUuid,
            };			
			try
			{
				var response = await EsewaPayment.ProcessPayment<dynamic>(secretKey, sandBoxMode, EsewaRequest);
				if (!string.IsNullOrEmpty(response))
				{
					return Redirect(response);
				}
				else
				{
					ViewBag.Message = "Payment processing failed.";
					return View();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during payment processing.");
				ViewBag.Message = "An error occurred while processing the payment.";
				return View();
			}
			return View();
		}
		public static string GenerateSignature(string totalAmount, string transactionUuid, string productCode, string secretkey)
		{
			var data = $"total_amount={totalAmount},transaction_uuid={transactionUuid},product_code={productCode}";
			byte[] keyBytes = Encoding.UTF8.GetBytes(secretkey);
			byte[] dataBytes = Encoding.UTF8.GetBytes(data);
			using (var hmac = new HMACSHA256(keyBytes))			
			{
                byte[] hashBytes = hmac.ComputeHash(dataBytes);
                return Convert.ToBase64String(hashBytes);
            }
		}
		private async Task<ActionResult> VerifyPayment(string data)
		{
			var secretKey = "8gBm/:&EnhH.1/q";
			EsewaPayment esewaPayment = new EsewaPayment(secretKey,sandBoxMode);
			var esewaResponse = esewaPayment.DecodeEsewaResponse(data);
			if (esewaResponse.status == "COMPLETE")
			{
					return View("Success");
			}
            else
            {
                ViewBag.Message = "Payment was not completed.";
                return View("Failure");
            }
        }
		public IActionResult SuccessWithEsewa() { 
			return View(SuccessWithEsewa);
		}
		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}