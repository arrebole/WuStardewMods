

using MysteriousRing.Framework.Companions;
using StardewValley;
using StardewValley.Objects;

namespace MysteriousRing.Framework.Managers
{
    internal static class RingManager
    {
        // 支持召唤的戒指名
        internal static List<string> rings = new List<string> {
            "Chaos Treasure Ring"
        };

        // 召唤物伙伴
        private static RingServant? ringServant;

        // 是否装备了召唤戒指
        internal static bool IsSummoningRing(Ring ring)
        {
            if (ring != null && rings.Any(r => r == ring.Name))
            {
                return true;
            }
            return false;
        }

        internal static bool ChangeServant(Farmer who)
        {
            if (ringServant != null && who.currentLocation.characters.Contains(ringServant))
            {
                // 移除旧随从
                who.currentLocation.characters.Remove(ringServant);
            }

            // 创建新随从
            ringServant = new ServantCreator().create(who);
            // 加入地图
            who.currentLocation.characters.Add(ringServant);
            return true;
        }

        internal static void HandleEquip(Farmer who, GameLocation location, Ring ring)
        {
            if (ringServant == null)
            {
                ringServant = new ServantCreator().create(who);
            }
            if (!location.characters.Contains(ringServant))
            {
                location.characters.Add(ringServant);
            }
        }

        internal static void HandleUnequip(Farmer who, GameLocation location, Ring ring)
        {
            if (ringServant != null && location.characters.Contains(ringServant))
            {
                location.characters.Remove(ringServant);
            }
        }

        internal static void HandleNewLocation(Farmer who, GameLocation location, Ring ring)
        {
            HandleEquip(who, location, ring);
        }

        internal static void HandleLeaveLocation(Farmer who, GameLocation location, Ring ring)
        {
            HandleUnequip(who, location, ring);
        }
    }
}