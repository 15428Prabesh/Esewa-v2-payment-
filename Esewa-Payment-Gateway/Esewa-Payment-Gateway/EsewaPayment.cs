using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Linq;

namespace Esewa_Payment_Gateway
{
    public class EsewaPayment
    {
        public string ContentType { get; set; } = "application/json";
        private readonly string _secretKey;
        private readonly bool _isSandBox;

        public EsewaPayment(string secretKey, bool isSandBox)
        {
            _secretKey = secretKey;
            _isSandBox = isSandBox;
        }

        private string GetEsewaPaymentUrl(EsewaPaymentUrlType urlType)
        {            string baseUrl = _isSandBox ? EsewaPaymentUrl.SandBoxUrl : EsewaPaymentUrl.Url;
            string version = PaymentApi.Version;
            switch (urlType)
            {
                case EsewaPaymentUrlType.Initialization:
                    return $"{baseUrl}{EsewaPaymentUrl.InitializationEndpoint}";
                case EsewaPaymentUrlType.Verification:
                    return $"{baseUrl}{EsewaPaymentUrl.VerificationEndpoint}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(urlType), urlType, null);
            }
        }
        public async Task<string> ProcessPayment<T>(string secretKey, bool sandBox, object content)
        {
            try
            {
                using var httpClient = new HttpClient();                
                string apiUrl = GetEsewaPaymentUrl(EsewaPaymentUrlType.Initialization);
				var contentDictionary = content.GetType().GetProperties().ToDictionary(prop => prop.Name, prop => prop.GetValue(content)?.ToString());
				HttpContent multipartContent = new FormUrlEncodedContent(contentDictionary);
                HttpResponseMessage response = await httpClient.PostAsync(apiUrl, multipartContent);
                string result =  response.RequestMessage.RequestUri.ToString();
				response.EnsureSuccessStatusCode();
				return result;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to process payment: {ex.Message}", ex);
            }
        }
        public EsewaResponse DecodeEsewaResponse(string encodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(encodedData);
            var jsonString = Encoding.UTF8.GetString(base64EncodedBytes);
            return JsonConvert.DeserializeObject<EsewaResponse>(jsonString);
        }
        //public async Task<string> VerifyPayment(string secretKey,bool sandboxmode,object content)
        //{
        //    try
        //    {
        //        using var httpClient = new HttpClient();
        //        string verificationUrl = GetEsewaPaymentUrl(EsewaPaymentUrlType.Verification);
        //        EsewaResponse esewaResponse = new EsewaResponse();
        //        using (var client = new HttpClient())
        //        {
        //            var parameters = new Dictionary<string, string>
        //            {
        //                { "amt", esewaResponse.total_amount },
        //                { "refId", esewaResponse.transaction_code },
        //                { "pid", esewaResponse.product_code },
        //                { "scd", "your_merchant_code" }
        //            };

        //            var response = await client.PostAsync(verificationUrl, new FormUrlEncodedContent(parameters));
        //            var responseContent = await response.Content.ReadAsStringAsync();
        //            return responseContent;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception($"Failed to process payment: {ex.Message}", ex);
        //    }
            
        //}
    }
}
