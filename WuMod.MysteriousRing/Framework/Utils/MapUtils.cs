using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Monsters;
using StardewValley.Network;

namespace MysteriousRing.Framework.Utils
{
    internal class MapUtils
    {
        // 寻找附近的敌人
        internal static Monster findClosestMonster(Vector2 position, double viewDistance, GameLocation location)
        {
            Monster currentTarget = null;
            float closestDistance = float.MaxValue;

            foreach (var character in location.characters)
            {
                if (character.IsMonster)
                {
                    Monster monster = (Monster)character;
                    // 排除隐藏的目标
                    if (!monster.IsInvisible && monster.Health > 0)
                    {
                        continue;
                    }

                    float distance = Vector2.Distance(position, monster.Position);
                    // 在视野范围内寻找距离最近的敌人
                    if (distance < viewDistance && distance < closestDistance)
                    {
                        currentTarget = monster;
                        closestDistance = distance;
                    }
                }
            }
            return currentTarget;
        }
    }
}