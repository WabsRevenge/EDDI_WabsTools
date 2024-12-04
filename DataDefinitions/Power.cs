﻿using JetBrains.Annotations;
using System;
using System.Linq;

namespace EddiDataDefinitions
{
    public class Power : ResourceBasedLocalizedEDName<Power>
    {
        static Power()
        {
            resourceManager = Properties.Powers.ResourceManager;
            resourceManager.IgnoreCase = false;
        }

        public static readonly Power None = new Power ("None", Superpower.None, null, 0 );
        public static readonly Power ALavignyDuval = new Power("ALavignyDuval", Superpower.Empire, "Kamadhenu", 5031856116434 );
        public static readonly Power AislingDuval = new Power("AislingDuval", Superpower.Empire, "Cubeo", 2415692581235 );
        public static readonly Power ArchonDelaine = new Power("ArchonDelaine", Superpower.Independent, "Harma", 4481764758234 );
        public static readonly Power DentonPatreus = new Power("DentonPatreus", Superpower.Empire, "Eotienses", 13865630573993 );
        public static readonly Power EdmundMahon = new Power("EdmundMahon", Superpower.Alliance, "Gateway", 2832631665362 ); 
        public static readonly Power FeliciaWinters = new Power("FeliciaWinters", Superpower.Federation, "Rhea", 1694121331043 );
        public static readonly Power LiYongRui = new Power("LiYongRui", Superpower.Independent, "Lembava", 3824408316259 );
        public static readonly Power PranavAntal = new Power("PranavAntal", Superpower.Independent, "Polevnic", 11664996705689 );
        public static readonly Power YuriGrom = new Power("YuriGrom", Superpower.Alliance, "Clayakarma", 6680922297058 );
        public static readonly Power ZeminaTorval = new Power("ZeminaTorval", Superpower.Empire, "Synteini", 5856355619546 );
        public static readonly Power NakatoKaine = new Power("NakatoKaine", Superpower.Alliance, "Tionisla", 5031789105890 );
        public static readonly Power JeromeArcher = new Power("JeromeArcher", Superpower.Federation, "Nanomam", 4752121268587 );

        [Obsolete("Replaced by Jerome Archer")]
        public static readonly Power ZacharyHudson = new Power("ZacharyHudson", Superpower.Federation, "Nanomam", 4752121268587 );

        [PublicAPI("The power's superpower allegiance")]
        public Superpower Allegiance { get; private set; }

        [PublicAPI("The power's star system headquarters")]
        public string headquarters { get; private set; }

        [PublicAPI( "The power's star system headquarters system address" )]
        public ulong hqSystemAddress { get; private set; }

        // dummy used to ensure that the static constructor has run
        public Power() : this("", Superpower.None, "None", 0 )
        { }

        private Power ( string edname, Superpower allegiance, string HQSystemName, ulong HQSystemAddress ) : base(edname, edname)
        {
            this.Allegiance = allegiance;
            this.headquarters = HQSystemName;
            this.hqSystemAddress = HQSystemAddress;
        }

        public static new Power FromEDName(string edName)
        {
            if (edName == null)
            {
                return null;
            }

            string tidiedName = edName.ToLowerInvariant().Replace(" ", "").Replace(".", "").Replace("-", "");
            return AllOfThem.FirstOrDefault(v => v.edname.ToLowerInvariant() == tidiedName);
        }
    }
}
