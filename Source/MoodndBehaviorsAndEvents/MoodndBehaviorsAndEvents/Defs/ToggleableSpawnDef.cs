using System;
using System.Collections.Generic;
using Verse.Sound;
using Verse;
using RimWorld;
using UnityEngine;

// Spawn control code copied from https://github.com/juanosarg/AlphaAnimals/blob/master/1.3/Source/AlphaBehavioursAndEvents/AlphaBehavioursAndEvents/Def/ToggleableSpawnDef.cs 
// Feels a little weird copying stuff wholesale... but I don't think there's really any other way to do settings.
namespace MoodndBehaviorsAndEvents
{
    public class ToggleableSpawnDef : Def
    {

        //ToggleableSpawnDef is a simple custom def that allows you to input a list of defNames for use
        //in mod options so people can choose which animals don't spawn

        //A list of pawnkind defNames
        public List<string> toggleablePawns;
    }
}
