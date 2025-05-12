using Companions;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.GameData.Objects;
using StardewValley.Objects;
using StardewValley.SpecialOrders.Objectives;
using StardewValley.Tools;

namespace MysteriousRing
{
    public interface IJsonAssetsApi
    {
        string GetObjectId(string name);
    }

    public class ModEntry : Mod
    {
        // 召唤物伙伴
        private RingServantCompanion? companion;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.InventoryChanged += OnInventoryChanged;
            // 应用 Patches 目录下的补丁
            // new Harmony(ModManifest.UniqueID).PatchAll();
        }

        private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e) {
            if (!Context.IsWorldReady) return;

            if (PlayerHasRing())
            {
                if (companion == null)
                {
                    companion = RingServantCompanion.build(
                        Helper,
                        Game1.player
                    );
                    Game1.currentLocation.characters.Add(companion);
                    Monitor.Log("Floating weapon summoned!", LogLevel.Info);
                }
            }
            else
            {
                if (companion != null)
                {
                    Game1.currentLocation.characters.Remove(companion);
                    companion = null;
                    Monitor.Log("Floating weapon dismissed.", LogLevel.Info);
                }
            }
        }

        private bool PlayerHasRing()
        {
            var rings = new Netcode.NetRef<StardewValley.Objects.Ring>[]{
                Game1.player.leftRing, 
                Game1.player.rightRing
            };
            foreach (var ring in rings)
            {
                if (ring == null || ring.Value == null) {
                    Monitor.Log($"### null", LogLevel.Info);
                    continue;
                }
                Monitor.Log($"### {ring.Value.Name}", LogLevel.Info);
                if (ring.Value.Name == "Chaos Treasure Ring") {
                    return true;
                }
            }
            return false;
        }
    }
}