using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Clankboard.Utils;

public enum DownloadResult
{
        Success,                // The download has succeeded. The file is now in the file system.
        NoInternet,             // The OS reports no internet connectivity. Happens when not being connected to any network.
        ServerNotReached,       // The webserver could not be reached.
        InvalidData,            // Data is invalid. Happens when media fails to download for example. (Broken file)
        CertificateInvalid,     // HTTPS Certificate of the webserver is invalid.
}

internal static class InetHelper
{
    private static readonly HttpClient _client = new();

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

    //public static async Task<byte[]> DownloadFileBytesAsync(string uri)
    //{
    //    if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri _))
    //    {
    //        throw new InvalidOperationException("URI is invalid.");
    //    }

    //    return await _client.GetByteArrayAsync(uri);
    //}

    public static bool IsInternetAvailable()
    {
        return NetworkInterface.GetIsNetworkAvailable();
    }

    public static async Task<DownloadResult> DownloadFile(string downloadLink, string filePath)
    {
        // Check if the OS reports no internet connectivity.
        if (!NetworkInterface.GetIsNetworkAvailable())
            return DownloadResult.NoInternet;

        try
        {
            // Download file using HttpClient.
            var client = new HttpClient();
            var response = await client.GetAsync(downloadLink);
            response.EnsureSuccessStatusCode();
            var data = await response.Content.ReadAsByteArrayAsync();
            File.WriteAllBytes(filePath, data);
        }
        catch (WebException e)
        {
            if (e.Status == WebExceptionStatus.ConnectFailure)
                return DownloadResult.ServerNotReached;
            if (e.Status == WebExceptionStatus.NameResolutionFailure)
                return DownloadResult.ServerNotReached;
            if (e.Status == WebExceptionStatus.ProtocolError)
                return DownloadResult.InvalidData;
            if (e.Status == WebExceptionStatus.TrustFailure)
                return DownloadResult.CertificateInvalid;
            return DownloadResult.ServerNotReached;
        }

        return DownloadResult.Success;
    }
}