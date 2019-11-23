using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace WebDekAPI.Auth
{
    public class TokenAuthOption
    {
        public static string Audience { get; } = "MMISLab";
        public static string Issuer { get; } = "VedKaf";
        public const int LIFETIME = 50;

        const string KEY = "646a9ebf-9cb9-4622-b2e4-72a6f4cfce95"; 
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }

        public static SigningCredentials SigningCredentials { get; } = new SigningCredentials(GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256);
      

    }

    public class RSAKeyHelper
    {
        public static RSAParameters GenerateKey()
        {

            using (var key = new RSACryptoServiceProvider(2048))
            {
                return key.ExportParameters(true);
            }
        }
    }
    //public static RsaSecurityKey Key { get; } = new RsaSecurityKey(RSAKeyHelper.GenerateKey());
    //public static SigningCredentials SigningCredentials { get; } = new SigningCredentials(Key, SecurityAlgorithms.RsaSha256Signature);
    //public static TimeSpan ExpiresSpan { get; } = TimeSpan.FromMinutes(50);
    //public static string TokenType { get; } = "Bearer";
}
