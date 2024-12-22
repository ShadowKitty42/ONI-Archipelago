using Archipelago.MultiClient.Net.BounceFeatures.DeathLink;
using Archipelago.MultiClient.Net.MessageLog.Messages;
using KaitoKid.ArchipelagoUtilities.Net.Client;
using KaitoKid.ArchipelagoUtilities.Net.Interfaces;
using Archipelago.MultiClient.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KMod;
using Newtonsoft.Json;

namespace ArchipelagoNotIncluded
{
    public class ArchipelagoNotIncludedClient : ArchipelagoClient
    {

        public override string GameName => "Oxygen Not Included";

        public override string ModName => ModName;

        public override string ModVersion => "test";

        public ArchipelagoNotIncludedClient(ILogger logger, System.Action itemReceivedFunction) : base(logger, new DataPackageCache("oxygen not included"), itemReceivedFunction)
        {
        }

        protected override void InitializeSlotData(string slotName, Dictionary<string, object> slotDataFields)
        {
            throw new NotImplementedException();
        }

        protected override void KillPlayerDeathLink(DeathLink deathlink)
        {
            throw new NotImplementedException();
        }

        protected override void OnMessageReceived(LogMessage message)
        {
            throw new NotImplementedException();
        }
    }
}
