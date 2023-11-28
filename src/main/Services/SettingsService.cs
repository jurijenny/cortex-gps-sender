using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.Gps.Sender.Services
{
    public class SettingsService : ISettingsService
    {
        public string ClientSecret { get; set; }
        public string InstantiatesGpsNeuronId { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
    }
}
