using FrooxEngine;
using HarmonyLib;
using NeosModLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            }
            return QueryPhysicalCores();
        }

        private static int? QueryPhysicalCores()
        {
            try
            {
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
            [HarmonyPatch(typeof(StandaloneSystemInfo), nameof(StandaloneSystemInfo.PhysicalCores), MethodType.Getter)]
            public static void PhysicalCoresPosfix(ref int? __result)
            {
                if (__result == null)
                {
                    __result = PhysicalCores();
                    Debug($"");
                }
                else
                {
                    Warn("");
                }
            }
        }
    }
}
