using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using BattleTech;
using Harmony;
using HBS.Logging;
using Newtonsoft.Json;

namespace AugmentedEvents
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [HarmonyPatch(typeof(SimGameState), "OnEventOptionSelected")]
    public static class SimGameState_OnEventOptionSelected_Patch
    {
        public static void Prefix(SimGameEventOption option, SimGameEventTracker tracker)
        {
            foreach (SimGameEventResultSet eventResultSet in option.ResultSets)
            {
                foreach (SimGameEventResult eventResult in eventResultSet.Results)
                {
                    if (eventResult.Stats != null)
                    {
                        for (int i = 0; i < eventResult.Stats.Length; i++)
                        {
                            switch (eventResult.Stats[i].name)
                            {
                                case "Morale":
                                    eventResult.Stats[i].value = (int.Parse(eventResult.Stats[i].value) * ModSettings.MoraleMultiplier).ToString();
                                    break;
                                case "MechTechSkill":
                                    eventResult.Stats[i].value = (int.Parse(eventResult.Stats[i].value) * ModSettings.MechTechSkillMultiplier).ToString();
                                    break;
                                case "MedTechSkill":
                                    eventResult.Stats[i].value = (int.Parse(eventResult.Stats[i].value) * ModSettings.MedTechSkillMultiplier).ToString();
                                    break;
                            }
                        }
                    }
                }
            }
            AugmentedEvents.LastModifiedOption = option;
        }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    [HarmonyPatch(typeof(SimGameState), "OnEventDismissed")]
    public static class SimGameState_OnEventDismissed_Patch
    {
        public static void Postfix(BattleTech.UI.SimGameInterruptManager.EventPopupEntry entry)
        {
            foreach (SimGameEventResultSet eventResultSet in AugmentedEvents.LastModifiedOption.ResultSets)
            {
                foreach (SimGameEventResult eventResult in eventResultSet.Results)
                {
                    if (eventResult.Stats != null)
                    {
                        for (int i = 0; i < eventResult.Stats.Length; i++)
                        {
                            switch (eventResult.Stats[i].name)
                            {
                                case "Morale":
                                    eventResult.Stats[i].value = (int.Parse(eventResult.Stats[i].value) / ModSettings.MoraleMultiplier).ToString();
                                    break;
                                case "MechTechSkill":
                                    eventResult.Stats[i].value = (int.Parse(eventResult.Stats[i].value) / ModSettings.MechTechSkillMultiplier).ToString();
                                    break;
                                case "MedTechSkill":
                                    eventResult.Stats[i].value = (int.Parse(eventResult.Stats[i].value) / ModSettings.MedTechSkillMultiplier).ToString();
                                    break;
                            }
                        }
                    }
                }
            }
        }
    }

    internal static class ModSettings
    {
        public static int MoraleMultiplier = 5;
        public static int MechTechSkillMultiplier = 2;
        public static int MedTechSkillMultiplier = 2;
    }

    public static class AugmentedEvents
    {
        internal static ILog Logger = HBS.Logging.Logger.GetLogger("AugmentedEvents");
        internal static SimGameEventOption LastModifiedOption;

        public static void Init()
        {
            var harmony = HarmonyInstance.Create("io.github.cydonia07.AugmentedEvents");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
