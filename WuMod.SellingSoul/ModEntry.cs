using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.GameData.Objects;
using StardewValley.Objects;
using StardewValley.SpecialOrders.Objectives;
using StardewValley.Tools;

namespace WuMod.SellingSoul
{
    public interface IJsonAssetsApi
    {
        string GetObjectId(string name);
    }

    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            // 应用 Patches 目录下的补丁
            new Harmony(ModManifest.UniqueID).PatchAll();
        }
    }
}