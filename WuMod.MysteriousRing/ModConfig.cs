using StardewValley;

namespace MysteriousRing
{
    public class ServantConfig
    {
        public string name = "servant_1";
        // 所有者
        public Farmer owner;
        // 跟随距离
        public int followDistance = 150;
        // 仆从的视野距离(发现敌人的距离)
        public float viewDistance = 700;
        // 仆从的攻击距离
        public int attackRange = 120;
        // 攻速
        public double attackSpeed = 0.25;
        // 攻击力
        public int attackDamage = 3;
        // 生命吸取
        public float bloodsucking = 0;
        // 移动速度
        public int moveSpend = 8;
        // 移动攻击
        public bool enableMoveAttack = false;
        // 停在头上
        public bool idleOnHead = false;

        // 动画
        public AnimatedSprite animatedSprite;
        // 攻击动画
        public List<FarmerSprite.AnimationFrame> idleFrames;
        public List<FarmerSprite.AnimationFrame> attackRightFrames;
        public List<FarmerSprite.AnimationFrame> attackLeftFrames;
    }
}