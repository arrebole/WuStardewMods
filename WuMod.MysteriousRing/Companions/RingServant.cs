

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;

namespace MysteriousRing.Companions
{
    public class RingServant : NPC
    {
        private Farmer owner;
        private int attackCooldown = 0;
        private int followDistance = 100;
        private readonly int attackRange = 192;

        private readonly int speed = 6;

        public static RingServant build(Farmer owner, Ring ring)
        {
            Texture2D texture = ModEntry.ModHelper.ModContent.Load<Texture2D>(
                "assets/ringServantCompanion.png"
            );
            // 创建AnimatedSprite（单帧）
            var sprite = new AnimatedSprite(
                textureName: "",       // 留空（因为直接使用Texture2D）
                currentFrame: 0,       // 固定0帧
                spriteWidth: texture.Width,  // 图片宽度=单帧宽度
                spriteHeight: texture.Height // 图片高度=单帧高度
            )
            {
                spriteTexture = texture // 直接赋值纹理
            };

            return new RingServant(sprite, owner, "Ring Servant Companion");
        }

        private RingServant(AnimatedSprite sprite, Farmer owner, string name)
            : base(
                sprite,
                owner.Tile * 64f,
                0,
                "CustomCompanion"
            )
        {
            this.owner = owner;
            this.HideShadow = true;
            this.willDestroyObjectsUnderfoot = false;
            this.collidesWithOtherCharacters.Value = false;
            this.SimpleNonVillagerNPC = true;
            this.Portrait = null;
        }

        public override bool CanSocialize
        {
            get
            {
                return false;
            }
        }

        public override bool checkAction(Farmer who, GameLocation l)
        {
            ModEntry.ModLogger.Log("checkAction");
            return false;
        }

        public override void update(GameTime time, GameLocation location)
        {
            base.update(time, location);

            // 计算与玩家的距离, 随机一个范围的跟随激励
            Vector2 targetPosition = owner.Position + new Vector2(0, -followDistance);
            Vector2 direction = targetPosition - Position;

            // 距离过远
            double diff = 0.7 + new Random().NextDouble() * (1.2 - 0.7);
            if (direction.Length() > followDistance * diff)
            {
                direction.Normalize();
                Position += direction * speed;
            } else if (direction.Length() > followDistance * diff)
            {
                direction.Normalize();
                Position -= direction * speed;
            }
        }
    }
}