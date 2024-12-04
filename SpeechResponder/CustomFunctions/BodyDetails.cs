﻿using Cottle;
using EddiCore;
using EddiDataDefinitions;
using EddiDataProviderService;
using EddiSpeechResponder.ScriptResolverService;
using JetBrains.Annotations;
using System;
using System.Reflection;

namespace EddiSpeechResponder.CustomFunctions
{
    [UsedImplicitly]
    public class BodyDetails : ICustomFunction
    {
        public string name => "BodyDetails";
        public FunctionCategory Category => FunctionCategory.Details;
        public string description => Properties.CustomFunctions_Untranslated.BodyDetails;
        public Type ReturnType => typeof( Body );
        public IFunction function => Function.CreateNativeMinMax( ( runtime, values, writer ) =>
        {
            StarSystem system;
            if (values.Count == 0)
            {
                system = EDDI.Instance.CurrentStarSystem;
            }
            else if (values.Count == 1 || string.IsNullOrEmpty(values[1].AsString) || (values.Count > 1 && values[1].AsString?.ToLowerInvariant() == EDDI.Instance.CurrentStarSystem?.systemname?.ToLowerInvariant()))
            {
                system = EDDI.Instance.CurrentStarSystem;
            }
            else
            {
                // Named system
                system = StarSystemSqLiteRepository.Instance.GetOrFetchStarSystem(values[1].AsString, true);
            }
            var result = system?.bodies?.Find(v => v.bodyname?.ToLowerInvariant() == values[0].AsString?.ToLowerInvariant());
            return result is null ? Value.EmptyMap : Value.FromReflection( result, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic );
        }, 1, 2);
    }
}
