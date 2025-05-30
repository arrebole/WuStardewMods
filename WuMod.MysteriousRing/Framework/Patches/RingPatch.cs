using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using MysteriousRing.Framework.Managers;

namespace MysteriousRing.Framework.Patchs
{
    internal class RingPatch
    {
        private readonly Type _ring = typeof(Ring);
        internal void Apply(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(_ring, nameof(Ring.onEquip), 
                new[] { typeof(Farmer) }), 
                postfix: new HarmonyMethod(GetType(), 
                nameof(OnEquipPostfix))
            );
            harmony.Patch(
                AccessTools.Method(_ring, nameof(Ring.onUnequip), 
                new[] { typeof(Farmer) }), 
                postfix: new HarmonyMethod(GetType(), 
                nameof(OnUnequipPostfix))
            );
            harmony.Patch(
                AccessTools.Method(_ring, nameof(Ring.onNewLocation), 
                new[] { typeof(Farmer), typeof(GameLocation) }), 
                postfix: new HarmonyMethod(GetType(), 
                nameof(OnNewLocationPostfix))
            );
            harmony.Patch(
                AccessTools.Method(_ring, nameof(Ring.onLeaveLocation), 
                new[] { typeof(Farmer), typeof(GameLocation) }), 
                postfix: new HarmonyMethod(GetType(), 
                nameof(OnLeaveLocationPostfix))
            );
        }

        private static void OnEquipPostfix(Ring __instance, Farmer who)
        {
            if (RingManager.IsSummoningRing(__instance))
            {
                RingManager.HandleEquip(who, who.currentLocation, __instance);
            }
        }

        private static void OnUnequipPostfix(Ring __instance, Farmer who)
        {
            if (RingManager.IsSummoningRing(__instance))
            {
                RingManager.HandleUnequip(who, who.currentLocation, __instance);
            }
        }

        private static void OnNewLocationPostfix(Ring __instance, Farmer who, GameLocation environment)
        {
            if (RingManager.IsSummoningRing(__instance))
            {
                RingManager.HandleNewLocation(who, environment, __instance);
            }
        }

        private static void OnLeaveLocationPostfix(Ring __instance, Farmer who, GameLocation environment)
        {
            if (RingManager.IsSummoningRing(__instance))
            {
                RingManager.HandleLeaveLocation(who, environment, __instance);
            }
        }
    }
}