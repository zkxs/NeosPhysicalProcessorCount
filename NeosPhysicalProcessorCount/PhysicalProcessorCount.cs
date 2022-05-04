using FrooxEngine;
using HarmonyLib;
using NeosModLoader;
using System;

namespace PhysicalProcessorCount
{
    public class PhysicalProcessorCount : NeosMod
    {
        public override string Name => "PhysicalProcessorCount";
        public override string Author => "runtime";
        public override string Version => "1.0.0";
        public override string Link => "https://github.com/zkxs/NeosPhysicalProcessorCount";

        private static bool calculated = false;
        private static int? physicalCores = null;
        private static bool warned = false;

        public override void OnEngineInit()
        {

            Harmony harmony = new Harmony("dev.zkxs.NeosPhysicalProcessorCount");
            harmony.PatchAll();
        }

        private static int? PhysicalCores()
        {
            if (!calculated)
            {
                physicalCores = QueryPhysicalCores();
                calculated = true;
                Msg($"Calculated that there are {physicalCores} physical cores");
            }
            return QueryPhysicalCores();
        }

        private static int? QueryPhysicalCores()
        {
            try
            {
                // from https://stackoverflow.com/questions/1542213/how-to-find-the-number-of-cpu-cores-via-net-c
                int coreCount = 0;
                foreach (var item in new System.Management.ManagementObjectSearcher("Select * from Win32_Processor").Get())
                {
                    coreCount += int.Parse(item["NumberOfCores"].ToString());
                }
                
                return coreCount;
            }
            catch (Exception e)
            {
                Error($"Error querying physical core count:\n{e}");
                return null;
            }
        }

        [HarmonyPatch]
        private static class HarmonyPatches
        {
            [HarmonyPostfix]
            [HarmonyPatch(typeof(Engine), nameof(Engine.PhysicalProcessorCount), MethodType.Getter)]
            public static void PhysicalCoresPostfix(ref int? __result)
            {
                if (__result == null)
                {
                    __result = PhysicalCores();
                    Debug($"Somebody toucha my spaghet!");
                }
                else
                {
                    if (!warned)
                    {
                        Warn("Neos has successfully calculated physical core count on its own! This mod may no longer be needed.");
                        warned = true;
                    }
                }
            }
        }
    }
}
