﻿using EddiDataDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace EddiEvents
{
    [PublicAPI]
    public class ShipRebootedEvent : Event
    {
        public const string NAME = "Ship rebooted";
        public const string DESCRIPTION = "Triggered when you run reboot/repair on your ship";
        public static ShipRebootedEvent SAMPLE = new ShipRebootedEvent(DateTime.UtcNow, new List<string>()) { Modules = new List<Module> { Module.FromEDName("modularcargobaydoor"), Module.FromEDName("int_powerplant_size2_class5"), Module.FromEDName("int_engine_size7_class2"), Module.FromEDName("hpt_plasmapointdefence_turret_tiny") } };

        [PublicAPI("The localized module names that have been repaired")]
        public List<string> modules => Modules?.Select(m => m.localizedName).ToList();

        [PublicAPI("The invariant module names that have been repaired")]
        public List<string> modules_invariant => Modules?.Select(m => m.invariantName).ToList();

        // Not intended to be user facing

        public List<string> compartments { get; private set; }

        public List<Module> Modules { get; set; } = new List<Module>(); // Set via the Ship Monitor, referencing the current ship

        public ShipRebootedEvent ( DateTime timestamp, List<string> compartments ) : base( timestamp, NAME )
        {
            this.compartments = compartments;
        }
    }
}
