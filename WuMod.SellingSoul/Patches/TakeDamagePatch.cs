using HarmonyLib;
using StardewValley;

namespace WuMod.SellingSoul
{
    [HarmonyPatch(typeof(Farmer), nameof(Farmer.takeDamage))]
    public static class TakeDamagePatch
    {
        public static void Prefix(Farmer __instance, ref int damage)
        {
            if (__instance == null || Game1.isTimePaused) {
                return;
            }

            // 没有装备无法触发效果
            if (!HasDevilDeal(__instance) || __instance.Money <= 0) {
                return;
            }

            // 随机一个抵挡的伤害
            var value = new Random().Next(0, damage);

            // 消耗金币
            var deduct = Math.Max(1, value * 2);
            if (__instance.Money - deduct <= 0) {
                deduct = __instance.Money;
                __instance.Money = 0;
            } else {
                __instance.Money -= deduct;
            }

            // 抵挡伤害
            damage -= value;

            // 显示效果信息
            Game1.addHUDMessage(
                new HUDMessage(
                    $"与魔鬼交易 ${deduct} 抵挡了 {value} 伤害", 
                    2
                )
            );
        }

        /// <summary>
        /// 判断玩家是否装备了契约
        /// </summary>
        public static bool HasDevilDeal(Farmer farmer)
        {
            var rings = new Netcode.NetRef<StardewValley.Objects.Ring>[]{
                farmer.leftRing, 
                farmer.rightRing
            };
            foreach (var ring in rings)
            {
                if (ring != null && ring.Value.Name == "Devil Deal") {
                    return true;
                }
            }
            return false;
        }
    }
}