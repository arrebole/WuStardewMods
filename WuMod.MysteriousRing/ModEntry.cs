using HarmonyLib;
using MysteriousRing.Patchs;
using StardewModdingAPI;

namespace MysteriousRing
{
    public interface IJsonAssetsApi
    {
        string GetObjectId(string name);
    }

    public class ModEntry : Mod
    {
        internal static IModHelper ModHelper;
        internal static IMonitor ModLogger;

        public override void Entry(IModHelper helper)
        {
            ModEntry.ModHelper = Helper;
            ModEntry.ModLogger = Monitor;

            // 应用补丁
            new RingPatch().Apply(new Harmony(ModManifest.UniqueID));
        }
    }
}