using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

// Modelled off the CompEnchantedStuff from RoM
// https://github.com/TorannD/TMagic/blob/f0895d1d296bf54184d5153ddbb307ac42b776dd/Source/TMagic/TMagic/Enchantment/CompEnchantedStuff.cs
namespace MoodndBehaviorsAndEvents
{
    public class Comp_BuffedStuff : ThingComp
    {
        public CompProperties_BuffedStuff Props
        {
            get
            {
                return (CompProperties_BuffedStuff)this.props;
            }
        }
    }
}
