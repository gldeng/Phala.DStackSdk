using System.Text;
using System.Text.Json;
using System.Threading;

namespace Phala.DStackSdk;

public class TappdClient
{
    private readonly HttpClient _client;
    private readonly string _baseUrl;

    public TappdClient(string endpoint = null)
    {
        endpoint = GetEndpoint(endpoint);
        _client = new HttpClient();
        _baseUrl = endpoint.StartsWith("http://") || endpoint.StartsWith("https://") ? endpoint : "http://localhost";
    }

    private string GetEndpoint(string endpoint)
    {
        if (!string.IsNullOrEmpty(endpoint))
        {
            return endpoint;
        }

        var simulatorEndpoint = Environment.GetEnvironmentVariable("DSTACK_SIMULATOR_ENDPOINT");
        if (!string.IsNullOrEmpty(simulatorEndpoint))
        {
            Console.WriteLine($"Using simulator endpoint: {simulatorEndpoint}");
            return simulatorEndpoint;
        }

        return "/var/run/tappd.sock";
    }

    private async Task<Dictionary<string, object>?> SendRpcRequestAsync(string path, Dictionary<string, object> payload)
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30)); // 30 seconds timeout
        var requestContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            var response = await _client.PostAsync($"{_baseUrl}{path}", requestContent, cts.Token);
            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<Dictionary<string, object>>(responseContent);
        }
        catch (TaskCanceledException)
        {
            throw new TimeoutException("Request timed out");
        }
        catch (JsonException)
        {
            throw new InvalidOperationException("Failed to parse response");
        }
    }

    public async Task<DeriveKeyResponse?> DeriveKeyAsync(string? path = null, string? subject = null,
        List<string>? altNames = null)
    {
        var data = new Dictionary<string, object>();
        data["path"] = path ?? string.Empty;
        data["subject"] = subject ?? path ?? string.Empty;
        if (altNames != null)
        {
            data["alt_names"] = altNames;
        }

        var result = await SendRpcRequestAsync("/prpc/Tappd.DeriveKey", data);
        return JsonSerializer.Deserialize<DeriveKeyResponse>(JsonSerializer.Serialize(result));
    }

    public async Task<TdxQuoteResponse?> GetTdxQuoteAsync(string reportData, string hashAlgorithm = "")
    {
        if (reportData == null)
        {
            throw new ArgumentException("report_data cannot be empty");
        }

        if (reportData.Length > 64)
        {
            throw new ArgumentException(
                "Report data is too large, it should at most 64 characters when hash_algorithm is raw.");
        }

        var reportDataBytes = Encoding.UTF8.GetBytes(reportData);
        return await GetTdxQuoteAsync(reportDataBytes, hashAlgorithm);
    }

    public async Task<TdxQuoteResponse?> GetTdxQuoteAsync(byte[] reportData, string hashAlgorithm = "")
    {
        if (reportData == null)
        {
            throw new ArgumentException("report_data cannot be empty");
        }

        if (reportData.Length > 128)
        {
            throw new ArgumentException(
                $"Report data is too large, it should at most 128 bytes when hash_algorithm is raw.");
        }

        var hex = BitConverter.ToString(reportData).Replace("-", "").ToLower();

        if (hashAlgorithm == "raw")
        {
            hex = hex.PadLeft(128, '0');
        }

        var result = await SendRpcRequestAsync("/prpc/Tappd.TdxQuote", new Dictionary<string, object>
        {
            { "report_data", hex },
            { "hash_algorithm", hashAlgorithm }
        });

        return JsonSerializer.Deserialize<TdxQuoteResponse>(JsonSerializer.Serialize(result));
    }
}