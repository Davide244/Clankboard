using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard.Utils
{
    static class InetHelper
    {
        public static async Task<bool> ValidateUrlWithHttp(string url) 
        {
            using var client = new HttpClient();

            try 
            {
                var response = client.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
                // Write contents of HEAD to console
                Console.WriteLine(await response.Result.Content.ReadAsStringAsync());
                return response.IsCompletedSuccessfully;
            }
            catch (HttpRequestException e) when (e.InnerException is SocketException)
            {
                return false;
            }
            catch (HttpRequestException e) when (e.StatusCode.HasValue && (int)e.StatusCode > 500)
            {
                return true;
            }
        }
    }
}
