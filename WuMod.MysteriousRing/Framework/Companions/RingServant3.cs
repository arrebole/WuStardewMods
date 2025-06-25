using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MysteriousRing.Framework.Companions
{
    public class RingServant3Factory : RingServantFactory
    {
        public NPC create(Farmer owner)
        {
            // 加载mod中内容贴图
            Texture2D texture = ModEntry.ModHelper.ModContent.Load<Texture2D>(
                $"assets/servant_3.png"
            );

            ServantConfig config = new ServantConfig()
            {
                name = "servant_3",
                owner = owner,
                followDistance = 100,
                viewDistance = 500,
                attackRange = 500,
                attackSpeed = 0.3,
                attackDamage = 2 * owner.CombatLevel,
                attackRemote = true,
                bloodsucking = 0,
                moveSpend = 8,
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
                idleFrames = null,
                attackRightFrames = new List<FarmerSprite.AnimationFrame>{
                    new FarmerSprite.AnimationFrame(8, 50),
                    new FarmerSprite.AnimationFrame(9, 50),
                    new FarmerSprite.AnimationFrame(10, 50),
                    new FarmerSprite.AnimationFrame(11, 50),
                    new FarmerSprite.AnimationFrame(12, 50),
                    new FarmerSprite.AnimationFrame(13, 50),
                    new FarmerSprite.AnimationFrame(14, 50),
                },
                attackLeftFrames = new List<FarmerSprite.AnimationFrame>{
                    new FarmerSprite.AnimationFrame(1, 50),
                    new FarmerSprite.AnimationFrame(2, 50),
                    new FarmerSprite.AnimationFrame(3, 50),
                    new FarmerSprite.AnimationFrame(4, 50),
                    new FarmerSprite.AnimationFrame(5, 50),
                    new FarmerSprite.AnimationFrame(6, 50),
                    new FarmerSprite.AnimationFrame(7, 50),
                },
            };
            return new RingServant3(config);
        }
    }

    public class RingServant3 : RingServant
    {
        internal RingServant3(ServantConfig config) : base(config) { }
    }
}