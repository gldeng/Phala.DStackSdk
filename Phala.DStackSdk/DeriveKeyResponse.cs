using System.Text.Json.Serialization;

namespace Phala.DStackSdk;

using System;
using System.Collections.Generic;
using System.Linq;

public class DeriveKeyResponse
{
    public DeriveKeyResponse(string key, List<string> certificateChain)
    {
        Key = key;
        CertificateChain = certificateChain;
    }

    [JsonPropertyName("key")]
    public string Key { get; set; }

    [JsonPropertyName("certificate_chain")]
    public List<string> CertificateChain { get; set; }

    public byte[] ToBytes(int? maxLength = null)
    {
        var content = Key.Replace("-----BEGIN PRIVATE KEY-----", "")
            .Replace("-----END PRIVATE KEY-----", "")
            .Replace("\n", "");
        var binaryDer = Convert.FromBase64String(content);
        return binaryDer.Take(maxLength ?? binaryDer.Length).ToArray();
    }
}