using EddiDataDefinitions;
using System;
using System.Collections.Generic;

namespace EddiStarMapService
{
    public interface IEdsmService
    {
        void EnqueueEvent(IDictionary<string, object> eventObject);
        void sendStarMapComment(string systemName, string comment);
        void SetEdsmCredentials();
        bool EdsmCredentialsSet();
        Dictionary<string, string> getStarMapComments();
        List<Body> GetStarMapBodies(ulong systemAddress);
        List<StarMapResponseLogEntry> getStarMapLog(DateTime? since = null, ulong[] systemAddresses = null);
        List<string> getIgnoredEvents();
        List<Faction> GetStarMapFactions(ulong systemAddress);
        List<Station> GetStarMapStations(ulong systemAddress);
        StarSystem GetStarMapSystem(string system, bool showCoordinates = true);
        StarSystem GetStarMapSystem (ulong systemAddress, bool showCoordinates = true);
        List<StarSystem> GetStarMapSystems(string[] systems, bool showCoordinates = true);
        List<StarSystem> GetStarMapSystems(ulong[] systemAddresses, bool showCoordinates = true);
        List<string> GetTypeAheadStarSystems(string partialSystemName);
        List<Dictionary<string, object>> GetStarMapSystemsSphere(string starSystem, int minRadiusLy = 0, int maxRadiusLy = 100, bool showEdsmId = true, bool showCoordinates = true, bool showPrimaryStar = true, bool showPermit = true);
        List<StarSystem> GetStarMapSystemsCube(string starSystem, int cubeLy = 200, bool showEdsmId = true, bool showCoordinates = true, bool showPrimaryStar = true, bool showPermit = true);
        Traffic GetStarMapHostility(string systemName, long? edsmId = null);
        Traffic GetStarMapTraffic(string systemName, long? edsmId = null);
        Traffic GetStarMapDeaths(string systemName, long? edsmId = null);
        void StartJournalSync();
        void StopJournalSync();
    }
}
