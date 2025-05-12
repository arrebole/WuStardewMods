

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;

namespace Companions
{
    public class RingServantCompanion : NPC
    {
        private Farmer owner;
        private int attackCooldown = 0;
        private readonly int followDistance = 100;
        private readonly int attackRange = 192;

        public static RingServantCompanion build(IModHelper helper, Farmer owner) {
            Texture2D texture = helper.ModContent.Load<Texture2D>(
                "assets/ringServantCompanion.png"
            );
            // 创建AnimatedSprite（单帧）
            var sprite = new AnimatedSprite(
                textureName: "",       // 留空（因为直接使用Texture2D）
                currentFrame: 0,       // 固定0帧
                spriteWidth: texture.Width,  // 图片宽度=单帧宽度
                spriteHeight: texture.Height // 图片高度=单帧高度
            ) {
                spriteTexture = texture // 直接赋值纹理
            };

            return new RingServantCompanion(sprite, owner, "Ring Servant Companion");
        }

        private RingServantCompanion(AnimatedSprite sprite, Farmer owner, string name)
            : base(
                sprite, 
                owner.Tile * 64f,
                null,
                2,
                name,
                false, 
                sprite.Texture
            )
        {
            this.owner = owner;
            this.HideShadow = true;
            this.speed = 3;
            this.willDestroyObjectsUnderfoot = false;
            this.collidesWithOtherCharacters.Value = false;
        }

        public override void update(GameTime time, GameLocation location)
        {
            base.update(time, location);

            // 跟随玩家
            // 计算与玩家的距离
            Vector2 targetPosition = owner.Position + new Vector2(0, -followDistance);
            Vector2 direction = targetPosition - Position;

            if (direction.Length() > followDistance * 0.8f)
            {
                direction.Normalize();
                Position += direction * speed;
            }
        }
    }
}