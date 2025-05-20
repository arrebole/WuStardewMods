using HarmonyLib;
using MysteriousRing.Framework.Managers;
using MysteriousRing.Framework.Patchs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MysteriousRing
{
    public class ModEntry : Mod
    {
        internal static IModHelper ModHelper;
        internal static IMonitor ModLogger;

        public override void Entry(IModHelper helper)
        {
            ModEntry.ModHelper = Helper;
            ModEntry.ModLogger = Monitor;

            // 每天开始
            helper.Events.GameLoop.DayStarted += onDayStarted;
            // 应用补丁
            new RingPatch().Apply(new Harmony(ModManifest.UniqueID));
        }

        public void onDayStarted(object? sender, DayStartedEventArgs e)
        {
            // 切换仆从
            RingManager.ChangeServant(Game1.player);
        }
    }
}