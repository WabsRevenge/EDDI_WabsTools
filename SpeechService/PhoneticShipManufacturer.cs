﻿using EddiDataDefinitions;

namespace EddiSpeechService
{
    public static partial class Translations
    {
        public static string getPhoneticShipManufacturer(string val2)
        {
            var phoneticManufacturer = ShipManufacturer.SpokenManufacturer(val2);
            if (!string.IsNullOrEmpty( phoneticManufacturer ) )
            {
                return phoneticManufacturer.Trim();
            }
            return val2;
        }
    }
}
