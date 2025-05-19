

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

        internal static void HandleEquip(Farmer who, GameLocation location, Ring ring)
        {
            if (ringServant == null && !location.characters.Contains(ringServant))
            {
                ringServant = RingServant.Create(
                    who
                );
                location.characters.Add(ringServant);
            }
        }

        internal static void HandleUnequip(Farmer who, GameLocation location, Ring ring)
        {
            if (ringServant != null && location.characters.Contains(ringServant))
            {
                location.characters.Remove(ringServant);
                ringServant = null;
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