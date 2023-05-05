using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.Gps.Sender.Services
{
    public class UrlService : IUrlService
    {
        public string AvatarUrl { get; set; }

        public string Authority 
        { 
            get
            {
                var uri = new Uri(this.AvatarUrl);
                return $"{uri.Scheme}://login.{uri.Host}";
            }
        }
    }
}
