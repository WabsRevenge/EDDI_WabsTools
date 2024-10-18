using System;
using System.Linq;

namespace EddiDataDefinitions
{
    /// <summary>
    /// Squadron Ranks
    /// </summary>
    public class Power : ResourceBasedLocalizedEDName<Power>
    {
        static Power()
        {
            resourceManager = Properties.Powers.ResourceManager;
            resourceManager.IgnoreCase = false;
        }

        public static readonly Power None = new Power ("None", Superpower.None, null);
        public static readonly Power ALavignyDuval = new Power("ALavignyDuval", Superpower.Empire, "Kamadhenu");
        public static readonly Power AislingDuval = new Power("AislingDuval", Superpower.Empire, "Cubeo");
        public static readonly Power ArchonDelaine = new Power("ArchonDelaine", Superpower.Independent, "Harma");
        public static readonly Power DentonPatreus = new Power("DentonPatreus", Superpower.Empire, "Eotienses");
        public static readonly Power EdmundMahon = new Power("EdmundMahon", Superpower.Alliance, "Gateway"); 
        public static readonly Power FeliciaWinters = new Power("FeliciaWinters", Superpower.Federation, "Rhea");
        public static readonly Power LiYongRui = new Power("LiYongRui", Superpower.Independent, "Lembava");
        public static readonly Power PranavAntal = new Power("PranavAntal", Superpower.Independent, "Polevnic");
        public static readonly Power YuriGrom = new Power("YuriGrom", Superpower.Alliance, "Clayakarma");
        public static readonly Power ZeminaTorval = new Power("ZeminaTorval", Superpower.Empire, "Synteini");
        public static readonly Power NakatoKaine = new Power("NakatoKaine", Superpower.Alliance, "Tionisla");
        public static readonly Power JeromeArcher = new Power("JeromeArcher", Superpower.Federation, "Nanomam");

        [Obsolete]
        public static readonly Power ZacharyHudson = new Power("ZacharyHudson", Superpower.Federation, "Nanomam");

        public Superpower Allegiance { get; private set; }
        public string headquarters { get; private set; }

        // dummy used to ensure that the static constructor has run
        public Power() : this("", Superpower.None, "None")
        { }

        private Power(string edname, Superpower allegiance, string headquarters) : base(edname, edname)
        {
            this.Allegiance = allegiance;
            this.headquarters = headquarters;
        }

        public new static Power FromEDName(string edName)
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
