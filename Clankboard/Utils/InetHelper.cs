using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Clankboard.Utils
{
    public enum DownloadResult
    {
        Success,                // The download has succeeded. The file is now in the file system.
        NoInternet,             // The OS reports no internet connectivity. Happens when not being connected to any network.
        ServerNotReached,       // The webserver could not be reached.
        InvalidData,            // Data is invalid. Happens when media fails to download for example. (Broken file)
        CertificateInvalid,     // HTTPS Certificate of the webserver is invalid.
    }

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

        public static bool IsInternetAvailable()
        {
            return System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();
        }

        public static async Task<DownloadResult> DownloadFile(string downloadLink, string filePath)
        {
            // Check if the OS reports no internet connectivity.
            if (!System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                return DownloadResult.NoInternet;

            try
            {
                // Download file using HttpClient.
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(downloadLink);
                response.EnsureSuccessStatusCode();
                byte[] data = await response.Content.ReadAsByteArrayAsync();
                File.WriteAllBytes(filePath, data);
            }
            catch (System.Net.WebException e)
            {
                if (e.Status == System.Net.WebExceptionStatus.ConnectFailure)
                    return DownloadResult.ServerNotReached;
                else if (e.Status == System.Net.WebExceptionStatus.NameResolutionFailure)
                    return DownloadResult.ServerNotReached;
                else if (e.Status == System.Net.WebExceptionStatus.ProtocolError)
                    return DownloadResult.InvalidData;
                else if (e.Status == System.Net.WebExceptionStatus.TrustFailure)
                    return DownloadResult.CertificateInvalid;
                else
                    return DownloadResult.ServerNotReached;
            }
            return DownloadResult.Success;
        }
    }
}
