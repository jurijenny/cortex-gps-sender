using ei8.Cortex.Gps.Sender.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ei8.Cortex.Gps.Sender.Services
{
    public interface ILocationService
    {
        event EventHandler<LocationModel> LocationChanged;

        event EventHandler<string> StatusChanged;

        void Initialize();

        void Stop();
    }
}
