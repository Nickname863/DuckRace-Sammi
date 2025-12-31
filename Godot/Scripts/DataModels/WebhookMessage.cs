using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DuckRace.Scripts.DataModels
{
    // Just a little Datamodel to turn into a JSON
    public class WebhookMessage
    {
        // Setting a default value here
        public string trigger { get; set; } = "duckRaceWinner";
        public DuckRaceInfo customObjectData { get; set; }
    }

}
