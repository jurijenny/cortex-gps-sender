using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.Gps.Sender.Services
{
    public interface ITokenProviderService
    {
        string XsrfToken { get; set; }
        string AccessToken { get; set; }
        string RefreshToken { get; set; }
        DateTimeOffset ExpiresAt { get; set; }
    }
}
