using System.Security.Cryptography;
using AElf;
using AElf.Cryptography;
using AElf.Types;
using Microsoft.AspNetCore.Mvc;
using Phala.DStackSdk;

namespace ExampleApp;

[ApiController]
[Route("api")]
public class ApiController : ControllerBase
{
    [HttpGet("derivekey")]
    public async Task<IActionResult> DeriveKey()
    {
        var randomNumString = new Random().NextDouble().ToString();

        // Call the deriveKey function and pass in the root of trust to derive a key
        var randomDeriveKey = await _client.DeriveKeyAsync("/", randomNumString);
        var keyPair = CryptoHelper.FromPrivateKey(Sha256(randomDeriveKey.ToBytes()));
        return Ok(new
        {
            Account = Address.FromPublicKey(keyPair.PublicKey).ToBase58(),
            PrivateKey = keyPair.PrivateKey.ToHex()
        });
    }

    private readonly TappdClient _client;

    public ApiController()
    {
        var endpoint = Environment.GetEnvironmentVariable("DSTACK_SIMULATOR_ENDPOINT") ?? "http://localhost:8090";
        _client = new TappdClient(endpoint);
    }

    [HttpGet("tdxquote")]
    public async Task<IActionResult> TdxQuote()
    {
        var randomNumString = new Random().NextDouble().ToString();
        var getRemoteAttestation = await _client.GetTdxQuoteAsync(randomNumString);
        return Ok(new { getRemoteAttestation });
    }

    #region Private Methods

    private byte[] Sha256(byte[] input)
    {
        using var sha256 = SHA256.Create();
        return sha256.ComputeHash(input);
    }

    #endregion
}