using IdentityModel.OidcClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.Gps.Sender.Services
{
    public class OidcClientService : IOidcClientService
    {
        private readonly IUrlService urlService;
        private OidcClient oidcClient;
        private readonly WebAuthenticatorBrowser webAuthenticatorBrowser;

        public OidcClientService(IUrlService urlService, WebAuthenticatorBrowser webAuthenticatorBrowser)
        {
            this.urlService = urlService;
            this.webAuthenticatorBrowser = webAuthenticatorBrowser;
        }

        public OidcClient GetOidcClient()
        {
            if (this.oidcClient == null)
            {
                this.oidcClient = new OidcClient(new OidcClientOptions
                {
                    Authority = this.urlService.Authority,
                    // TODO: Use proper values
                    ClientId = "mauimauefood.appclient",
                    Scope = "openid profile avatarapi",
                    RedirectUri = "mauimauefoodclient://",
                    PostLogoutRedirectUri = "mauimauefoodclient://",
                    // TODO: transfer to login page "advanced"
                    ClientSecret = "SuperSecretPassword",
                    HttpClientFactory = options => GetInsecureHttpClient(),
                    Browser = this.webAuthenticatorBrowser
                });
            }

            return this.oidcClient;
        }

        public static HttpClient GetInsecureHttpClient()
        {
            // #if ANDROID
            // TODO: var handler = new CustomAndroidMessageHandler();
            // #else
            var handler = new HttpClientHandler
            {
                // #endif
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
            };
            HttpClient client = new HttpClient(handler);
            return client;
        }
    }
}
