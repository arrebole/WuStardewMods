using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MysteriousRing.Framework.Utils;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Objects;

namespace MysteriousRing.Framework.Companions
{
    public class RingServant : NPC
    {

        // 所属玩家
        private Farmer owner;
        // 与玩家的跟随距离
        private int followDistance = 120;


        // 仆从的视野距离(发现敌人的距离)
        private readonly float viewDistance = 200;
        // 仆从的攻击距离
        private readonly int attackRange = 50;
        // 仆从的攻击速冻 (冷却时间(毫秒))
        private readonly int AttackCooldownTime = 3000;
        // 剩余攻击冷却时间，下一次攻击剩余时间
        private int attackCooldown = 0;
        // 在攻击动画过程中
        private bool isAttacking = false;
        // 攻击力
        private int attackDamage = 1;

        public static RingServant Create(Farmer owner, String name)
        {
            Texture2D texture = ModEntry.ModHelper.ModContent.Load<Texture2D>(
                "assets/ringServant.png"
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

            return new RingServant(sprite, owner, name);
        }

        private RingServant(AnimatedSprite sprite, Farmer owner, string name) : base(sprite, owner.Tile * 64f, 0, name)
        {
            this.owner = owner;
            base.HideShadow = true;
            base.Scale = 1;
            this.willDestroyObjectsUnderfoot = false;
            base.collidesWithOtherCharacters.Value = false;
            base.Breather = false; // 喘气
            base.displayName = null;
            base.Portrait = null;
            base.speed = 6;
        }

        protected override void initNetFields()
        {
            base.initNetFields();
        }

        public override void ChooseAppearance(LocalizedContentManager content = null)
        {
            return;
        }

        public override bool CanSocialize
        {
            get
            {
                return false;
            }
        }

        public override bool canTalk()
        {
            return false;
        }

        // 核心函数
        // 每帧调用一次 更新数据和状态
        public override void update(GameTime time, GameLocation location)
        {
            // base.update() 会自动增加帧序列
            base.update(time, location);

            // 在动画中不进行操作，继续播放动画
            if (this.Sprite.CurrentAnimation != null)
            {
                return;
            }

            // 计算与玩家的距离, 随机一个范围的跟随
            Vector2 targetPosition = owner.Position + new Vector2(0, -followDistance);
            Vector2 direction = targetPosition - this.Position;

            // 2倍距离范围内优先攻击敌人
            if (direction.Length() < followDistance * 2)
            {
                // 寻找攻击目标
                Monster target = MapUtils.findClosestMonster(
                    Position, 
                    viewDistance, 
                    location
                );
                if (target != null)
                {
                    updateAttack(time, target, location);
                    return;
                }
            }

            // 距离过远跟随玩家
            if (direction.Length() > followDistance)
            {
                direction.Normalize();
                Position += direction * speed;
                return;
            }
        }

        // 攻击目标
        private void updateAttack(GameTime gameTime, Monster target, GameLocation location)
        {
            // 冷却计时
            if (attackCooldown > 0)
            {
                attackCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                return;
            }

            float distanceToTarget = Vector2.Distance(Position, target.Position);
            // 攻击距离足够近 则发起攻击
            if (distanceToTarget <= attackRange)
            {
                // 开始进入战斗模式帧动画
                attackCooldown = AttackCooldownTime;

                // this.Sprite.setCurrentAnimation(new List<FarmerSprite.AnimationFrame>{
                // new FarmerSprite.AnimationFrame(0, 100),
                // new FarmerSprite.AnimationFrame(1, 100),
                // new FarmerSprite.AnimationFrame(2, 100),
                // new FarmerSprite.AnimationFrame(3, 100),
                // });

                // 应用伤害
                target.takeDamage(attackDamage, (int)Position.X, (int)Position.Y, false, 0, owner);

                // 显示伤害数字
                location.debris.Add(
                    new Debris(attackDamage, target.getStandingPosition(), Color.Orange, 1f, target)
                );

                // 播放攻击效果
                Game1.playSound("swordswipe");
            }
            else
            {
                // 向目标移动
                Vector2 direction = Vector2.Normalize(target.Position - Position);
                Position += direction * 2f;
            }
        }
    }
}