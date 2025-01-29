using System;

namespace SmartVault.Program.BusinessObjects
{   //simple oauth BO example
    public class OAuthIntegrationBusinessObject
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }

        public OAuthIntegrationBusinessObject()
        {
            AccessToken = string.Empty;
            RefreshToken = string.Empty;
            AccessTokenExpiration = DateTime.MinValue;
        }

        public bool Authenticate(string clientId, string clientSecret, string authorizationCode, string redirectUri)
        {
            // Simulated OAuth authentication (exchange code for tokens)
            AccessToken = "newAccessToken";
            RefreshToken = "newRefreshToken";
            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(60); // 1 hour expiration

            return true;
        }

        public bool RefreshAccessToken(string clientId, string clientSecret)
        {
            AccessToken = "newAccessToken";
            AccessTokenExpiration = DateTime.UtcNow.AddMinutes(60);

            return true;
        }

        public bool IsAccessTokenValid()
        {
            return AccessTokenExpiration > DateTime.UtcNow;
        }
    }
}
