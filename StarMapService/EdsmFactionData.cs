using EddiDataDefinitions;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace EddiStarMapService
{
    public partial class StarMapService
    {
        public List<Faction> GetStarMapFactions(ulong systemAddress )
        {
            if ( systemAddress == 0) { return new List<Faction>(); }
            if (currentGameVersion != null && currentGameVersion < minGameVersion) { return new List<Faction>(); }

            var request = new RestRequest("api-system-v1/factions", Method.POST);
            request.AddParameter( "systemId64", systemAddress );
            var clientResponse = restClient.Execute<JObject>(request);
            if (clientResponse.IsSuccessful)
            {
                Logging.Debug("EDSM responded with " + clientResponse.Content);
                var token = JToken.Parse(clientResponse.Content);
                if (token is JObject response)
                {
                    return ParseStarMapFactionsParallel(response, systemAddress );
                }
            }
            else
            {
                Logging.Debug("EDSM responded with " + clientResponse.ErrorMessage, clientResponse.ErrorException);
            }
            return new List<Faction>();
        }

        public List<Faction> ParseStarMapFactionsParallel(JObject response, ulong systemAddress )
        {
            var Factions = new List<Faction>();
            var systemName = (string)response?[ "name" ];
            var factions = (JArray)response?["factions"];

            if (factions != null)
            {
                Factions = factions
                    .AsParallel()
                    .Select(f => ParseStarMapFaction(f.ToObject<JObject>(), systemName, systemAddress ) )
                    .Where(f => f != null)
                    .ToList();
            }
            return Factions;
        }

        private Faction ParseStarMapFaction(JObject faction, string systemName, ulong systemAddress )
        {
            try
            {
                Logging.Debug($"Parsing EDSM system {systemName} faction", faction);

                if (faction is null) { return null; }
                Faction Faction = new Faction
                {
                    name = (string)faction["name"],
                    EDSMID = (long?)faction["id"],
                    Allegiance = Superpower.FromName((string)faction["allegiance"]) ?? Superpower.None,
                    Government = Government.FromName((string)faction["government"]) ?? Government.None,
                    isplayer = (bool?)faction["isPlayer"],
                    updatedAt = Dates.fromTimestamp((long?)faction["lastUpdate"]) ?? DateTime.MinValue
                };

                Faction.presences.Add(new FactionPresence()
                {
                    systemName = systemName,
                    systemAddress = systemAddress,
                    influence = (decimal?)faction["influence"] * 100, // Convert from a 0-1 range to a percentage
                    FactionState = FactionState.FromName((string)faction["state"]) ?? FactionState.None,
                });

                IDictionary<string, object> factionDetail = faction.ToObject<IDictionary<string, object>>() ?? new Dictionary<string, object>();

                // Active states
                factionDetail.TryGetValue("activeStates", out object activeStatesVal);
                if (activeStatesVal != null)
                {
                    var activeStatesList = (JArray)activeStatesVal;
                    foreach (var activeStateToken in activeStatesList)
                    {
                        var activeState = activeStateToken.ToObject<IDictionary<string, object>>();
                        Faction.presences.FirstOrDefault(p => p.systemAddress == systemAddress )?
                            .ActiveStates.Add(FactionState.FromName(JsonParsing.getString(activeState, "state")) ?? FactionState.None);
                    }
                }

                // Pending states
                factionDetail.TryGetValue("pendingStates", out object pendingStatesVal);
                if (pendingStatesVal != null)
                {
                    var pendingStatesList = ((JArray)pendingStatesVal).ToList();
                    foreach (var pendingStateToken in pendingStatesList)
                    {
                        var pendingState = pendingStateToken.ToObject<IDictionary<string, object>>();
                        FactionTrendingState pTrendingState = new FactionTrendingState(
                            FactionState.FromName(JsonParsing.getString(pendingState, "state")) ?? FactionState.None,
                            JsonParsing.getOptionalInt(pendingState, "trend")
                        );
                        Faction.presences.FirstOrDefault(p => p.systemAddress == systemAddress )?
                            .PendingStates.Add(pTrendingState);
                    }
                }

                // Recovering states
                factionDetail.TryGetValue("recoveringStates", out object recoveringStatesVal);
                if (recoveringStatesVal != null)
                {
                    var recoveringStatesList = (JArray)recoveringStatesVal;
                    foreach (var recoveringStateToken in recoveringStatesList)
                    {
                        var recoveringState = recoveringStateToken.ToObject<IDictionary<string, object>>();
                        FactionTrendingState rTrendingState = new FactionTrendingState(
                            FactionState.FromName(JsonParsing.getString(recoveringState, "state")) ?? FactionState.None,
                            JsonParsing.getOptionalInt(recoveringState, "trend")
                        );
                        Faction.presences.FirstOrDefault(p => p.systemAddress == systemAddress )?
                            .RecoveringStates.Add(rTrendingState);
                    }
                }

                return Faction;
            }
            catch (Exception ex)
            {
                Logging.Error($"Error parsing EDSM system {systemName} faction result.", ex);
            }
            return null;
        }
    }
}
