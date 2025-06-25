

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MysteriousRing.Framework.Companions
{

    public class RingServant1Factory : RingServantFactory
    {
        public NPC create(Farmer owner)
        {
            // 加载mod中内容贴图
            Texture2D texture = ModEntry.ModHelper.ModContent.Load<Texture2D>(
                $"assets/servant_1.png"
            );

            ServantConfig config = new ServantConfig()
            {
                name = "servant_1",
                owner = owner,
                followDistance = 150,
                viewDistance = 700,
                attackRange = 120,
                attackSpeed = 0.25,
                attackDamage = 3 * owner.CombatLevel,
                bloodsucking = 0,
                moveSpend = 6,
                idleOnHead = false,
                enableMoveAttack = false,
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
                attackRightFrames = new List<FarmerSprite.AnimationFrame>{
                    new FarmerSprite.AnimationFrame(0, 50),
                    new FarmerSprite.AnimationFrame(1, 150),
                    new FarmerSprite.AnimationFrame(2, 80),
                    new FarmerSprite.AnimationFrame(3, 80),
                    new FarmerSprite.AnimationFrame(4, 80),
                    new FarmerSprite.AnimationFrame(5, 80),
                    new FarmerSprite.AnimationFrame(6, 150),
                },
                attackLeftFrames = new List<FarmerSprite.AnimationFrame>{
                    new FarmerSprite.AnimationFrame(7, 50),
                    new FarmerSprite.AnimationFrame(8, 150),
                    new FarmerSprite.AnimationFrame(9, 80),
                    new FarmerSprite.AnimationFrame(10, 80),
                    new FarmerSprite.AnimationFrame(11, 80),
                    new FarmerSprite.AnimationFrame(12, 80),
                    new FarmerSprite.AnimationFrame(13, 150),
                }
            };
            return new RingServant1(config);
        }
    }

    public class RingServant1 : RingServant
    {
        internal RingServant1(ServantConfig config) : base(config) { }

    }
}