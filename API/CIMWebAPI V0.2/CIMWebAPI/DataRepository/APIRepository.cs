using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;

namespace CIMWebAPI.DataRepository
{
    public class APIRepository
    {
        public async Task<string> BufferReload(int ServiceID, string URI)
        {
            try
            {
                using(var Client = new HttpClient())
                {
                    string username = "cimtest";
                    string password = "083B@b123";
                    Client.BaseAddress = new Uri(URI);
                    Client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue(
                            "Basic", Convert.ToBase64String(
                                System.Text.ASCIIEncoding.ASCII.GetBytes(
                                   $"{username}:{password}")));
                    var responseAuth = Client.GetAsync("/api/v1/token?expiration=10").GetAwaiter().GetResult();
                    var contents = await responseAuth.Content.ReadAsStringAsync();
                    return contents;
                    //var response = await Client.PutAsync("api/v1/services/outbound/" + ServiceID.ToString() + "/reloadbuffers", content);
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
