using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.Gps.Sender.Services
{
    public interface ISettingsService
    {
        string ClientSecret { get; set; }

        string InstantiatesGpsNeuronId { get; set; }
    }
}
