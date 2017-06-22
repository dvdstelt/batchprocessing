using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NServiceBus;

namespace Shared
{
    public static class CommonConfiguration
    {
        public static void ApplyCommonConfiguration(this EndpointConfiguration endpointConfiguration)
        {
            endpointConfiguration.UseSerialization<JsonSerializer>();
            endpointConfiguration.UseTransport<LearningTransport>();
            endpointConfiguration.UsePersistence<LearningPersistence>();

            // Conventions
            var conventions = endpointConfiguration.Conventions();
            conventions.DefiningCommandsAs(c => c.Namespace != null && c.Namespace.EndsWith("Commands"));
            conventions.DefiningEventsAs(c => c.Namespace != null && c.Namespace.EndsWith("Events"));
            conventions.DefiningMessagesAs(c => c.Namespace != null && c.Namespace.EndsWith("Messages"));
        }
    }
}
