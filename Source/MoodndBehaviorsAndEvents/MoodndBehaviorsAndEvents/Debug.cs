using System;
using Verse;

namespace MoodndBehaviorsAndEvents
{
    class Debug
    {
        public static readonly string debugPrefix = "MooDnD_LogPrefix";


        public static void MooLog(string printVal, params object[] args)
        {
            Log.Message(debugPrefix.Translate() + String.Format(printVal, args));
        }

        public static void LogIfDebug(string printVal, params object[] args)
        {
            if (MoodndManagerie_Mod.settings.flagDebug)
            {
                Debug.MooLog(printVal, args);
            }
        }

        public static void LogErr(string printVal, params object[] args)
        {
            Log.Error(debugPrefix.Translate() + String.Format(printVal, args));
        }

        public static void LogWarn(string printVal, params object[] args)
        {
            Log.Warning(debugPrefix.Translate() + String.Format(printVal, args));
        }
    }
}
