
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace MysteriousRing.Framework.Companions
{
    internal class ServantCreator
    {

        internal RingServant create(Farmer owner)
        {
            int num = new Random().Next(1, 3);
            switch (num)
            {
                case 1:
                    return this.Create1(owner);
                case 2:
                    return this.Create2(owner);
                default:
                    return this.Create1(owner);
            }
        }

        private RingServant Create1(Farmer owner)
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
                moveSpend = 6,
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

        private RingServant Create2(Farmer owner)
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
                attackRange = 80,
                attackSpeed = 0.6,
                attackDamage = 1 * owner.CombatLevel,
                moveSpend = 8,
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
}