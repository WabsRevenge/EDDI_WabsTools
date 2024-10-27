using EddiDataDefinitions;
using System;
using System.Runtime.Caching;

namespace EddiDataProviderService
{
    public class StarSystemCache
    {
        private readonly CacheItemPolicy cacheItemPolicy = new CacheItemPolicy();
        private readonly ObjectCache starSystemCache = new MemoryCache( "StarSystemCache" );

        // Store deserialized star systems in short term memory for this amount of time.
        // Storage time is reset whenever the cached value is accessed.
        public StarSystemCache ( int expirationSeconds )
        {
            cacheItemPolicy.SlidingExpiration = TimeSpan.FromSeconds( expirationSeconds );
        }

        public void Add ( StarSystem starSystem )
        {
            starSystemCache.Add( starSystem.systemAddress.ToString(), starSystem, cacheItemPolicy );
        }

        public bool Contains ( ulong systemAddress )
        {
            return starSystemCache.Contains( systemAddress.ToString() );
        }

        public StarSystem Get ( ulong systemAddress )
        {
            return starSystemCache.Get( systemAddress.ToString() ) as StarSystem;
        }

        public void Remove ( ulong systemAddress )
        {
            starSystemCache.Remove( systemAddress.ToString() );
        }
    }
}
