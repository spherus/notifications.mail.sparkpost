using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Spherus.Notifications.Mail.SparkPost
{
    class SparkPostClient
    {
        protected internal async Task<ServiceResponse<HttpResponseMessage>> SendNotification(TransmissionContent transmission)
        {
            ServiceResponse<HttpResponseMessage> result = new ServiceResponse<HttpResponseMessage>();

            try
            {

                var apiKeyItem = transmission.ApiCredentials.Any(t => t.Key == "ApiKey");
                if (!apiKeyItem)
                {
                    result.ObjectResult = null;
                    result.Status = new ServiceStatus { Code = 2, Message = "SparkPostApiKeyNotFound" };

                    return result;
                }

                using (var httpClient = new HttpClient())
                {
                    transmission.Headers.Clear();
                    transmission.Headers.Add("Content-Type", "application/json");
                    httpClient.DefaultRequestHeaders.Add("Authorization", transmission.ApiCredentials.FirstOrDefault(t => t.Key == "ApiKey").Value.ToString());
                    var response = await httpClient.PostAsync((Uri)transmission.ApiCredentials.FirstOrDefault(t => t.Key == "URI").Value, transmission);
                    if (!response.IsSuccessStatusCode)
                    {
                        result.ObjectResult = null;
                        result.Status = new ServiceStatus { Code = 3, Message = await response.Content.ReadAsStringAsync() };

                        return result;
                    }

                    result.ObjectResult = response;
                    result.Status = new ServiceStatus { Code = 0, Message = "Success" };

                    return result;
                }
            }
            catch
            {
                result.ObjectResult = null;
                result.Status = new ServiceStatus { Code = 1, Message = "ServiceError" };

                return result;
            }
        }
    }
}
