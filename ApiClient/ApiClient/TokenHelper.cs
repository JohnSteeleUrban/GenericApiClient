using JWT.Algorithms;
using JWT.Builder;

using System;

namespace ApiClient
{
    public class TokenHelper
    {
        public string GenerateToken(string clientId, string secret)
        {
            //do it.
            var token = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(secret)
                .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(20).ToUnixTimeSeconds())
                .AddClaim("iss", clientId)
                .Encode();

            return token;
        }
    }
}
