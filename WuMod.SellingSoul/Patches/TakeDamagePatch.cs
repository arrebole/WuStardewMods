using HarmonyLib;
using StardewValley;

namespace WuMod.SellingSoul
{
    [HarmonyPatch(typeof(Farmer), nameof(Farmer.takeDamage))]
    public static class TakeDamagePatch
    {
        public static void Prefix(ref int damage, Farmer farmer)
        {
            if (farmer == null) {
                return;
            }

            // 没有装备 或是没有足够的钱触发效果
            if (!HasDevilDeal(farmer) || farmer.Money < damage) {
                return;
            }
            
            // 随机一个抵挡的伤害
            int value = new Random().Next(1, damage);

            // 消耗金币
            farmer.Money -= value;
            // 抵挡伤害
            damage -= value;

            // 显示效果信息
            Game1.addHUDMessage(
                new HUDMessage($"与恶魔交易抵挡了 {value} 伤害", 2)
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
                if (ring != null && ring.Name == "Devil Deal") {
                    return true;
                }
            }
            return false;
        }
    }
}