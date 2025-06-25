using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MysteriousRing.Framework.Companions
{
    public class RingServant2Factory : RingServantFactory
    {
        public NPC create(Farmer owner)
        {
            // 加载mod中内容贴图
            Texture2D texture = ModEntry.ModHelper.ModContent.Load<Texture2D>(
                $"assets/servant_2.png"
            );

            ServantConfig config = new ServantConfig()
            {
                name = "servant_2",
                owner = owner,
                followDistance = 100,
                viewDistance = 500,
                attackRange = 50,
                attackSpeed = 0.6,
                attackDamage = 1 * owner.CombatLevel,
                bloodsucking = 0.1F,
                moveSpend = 8,
                idleOnHead = true,
                enableMoveAttack = true,
                animatedSprite = new AnimatedSprite(
                    textureName: "",   // 留空（因为直接使用Texture2D）
                    currentFrame: 0,   // 固定0帧
                    spriteWidth: texture.Width / 7,  // 图片宽度=单帧宽度
                    spriteHeight: texture.Height / 2 // 图片高度=单帧高度
                )
                {
                    spriteTexture = texture,
                    loop = false,
                },
                idleFrames = new List<FarmerSprite.AnimationFrame>
                {
                    new FarmerSprite.AnimationFrame(0, 100),
                    new FarmerSprite.AnimationFrame(1, 100),
                    new FarmerSprite.AnimationFrame(2, 100),
                    new FarmerSprite.AnimationFrame(3, 100),
                    new FarmerSprite.AnimationFrame(4, 100),
                },
                attackRightFrames = new List<FarmerSprite.AnimationFrame>{
                    new FarmerSprite.AnimationFrame(7, 100),
                    new FarmerSprite.AnimationFrame(8, 100),
                    new FarmerSprite.AnimationFrame(9, 100),
                    new FarmerSprite.AnimationFrame(10, 100),
                    new FarmerSprite.AnimationFrame(11, 100),
                },
                attackLeftFrames = new List<FarmerSprite.AnimationFrame>{
                    new FarmerSprite.AnimationFrame(7, 100),
                    new FarmerSprite.AnimationFrame(8, 100),
                    new FarmerSprite.AnimationFrame(9, 100),
                    new FarmerSprite.AnimationFrame(10, 100),
                    new FarmerSprite.AnimationFrame(11, 100),
                }
            };
            return new RingServant2(config);
        }
    }

    public class RingServant2 : RingServant
    {
        internal RingServant2(ServantConfig config) : base(config) { }
    }
}