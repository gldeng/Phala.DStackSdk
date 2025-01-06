using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Phala.DStackSdk;

public class TdxQuoteResponse
{
    public TdxQuoteResponse(string quote, string eventLog)
    {
        Quote = quote;
        EventLog = eventLog;
    }

    [JsonPropertyName("quote")] public string Quote { get; set; }

    [JsonPropertyName("event_log")] public string EventLog { get; set; }

    public Dictionary<int, string> ReplayRtmrs()
    {
        var parsedEventLog = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(EventLog);
        var rtmrs = new Dictionary<int, string>();

        for (var idx = 0; idx < 4; idx++)
        {
            var history = new List<string>();
            foreach (var eventItem in parsedEventLog)
            {
                if (eventItem.TryGetValue("imr", out var imr) && int.Parse(imr) == idx)
                {
                    history.Add(eventItem["digest"]);
                }
            }

            rtmrs[idx] = ReplayRtmr(history);
        }

        return rtmrs;
    }

    private string ReplayRtmr(List<string> history)
    {
        // ReSharper disable once InconsistentNaming
        const string INIT_MR =
            "000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
        if (history.Count == 0)
        {
            return INIT_MR;
        }

        var mr = Enumerable.Repeat((byte)0, 48).ToArray();
        foreach (var content in history)
        {
            var contentBytes = Enumerable.Repeat((byte)0, 48).ToArray();
            var contentFromHex = Convert.FromHexString(content);
            Array.Copy(contentFromHex, contentBytes, Math.Min(contentFromHex.Length, 48));
            using var sha384 = SHA384.Create();
            mr = sha384.ComputeHash(mr.Concat(contentBytes).ToArray());
        }

        return BitConverter.ToString(mr).Replace("-", "").ToLower();
    }
}