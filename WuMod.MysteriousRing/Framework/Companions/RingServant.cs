using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Objects;

namespace MysteriousRing.Framework.Companions
{
    public class RingServant : NPC
    {

        private Farmer owner;
        private int attackCooldown = 0;
        private int followDistance = 100;
        private readonly int attackRange = 192;
        // 攻击冷却时间(毫秒)
        private readonly int AttackCooldownTime = 1000;
        private bool isAttacking;
        private int attackFrame;
        private float attackRotation;
        private readonly int speed = 6;

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

        public override void update(GameTime time, GameLocation location)
        {
            base.update(time, location);

            // 寻找攻击目标
            Monster target = this.findNewTarget(location);
            if (target != null)
            {
                updateAttack(time, target, location);
                return;
            }

            // 计算与玩家的距离, 随机一个范围的跟随
            Vector2 targetPosition = owner.Position + new Vector2(0, -followDistance);
            Vector2 direction = targetPosition - Position;

            // 距离过远
            double diff = 0.7 + new Random().NextDouble() * (1.2 - 0.7);
            if (direction.Length() > followDistance * diff)
            {
                direction.Normalize();
                Position += direction * speed;
            }
            // else if (direction.Length() < followDistance * diff)
            // {
            //     direction.Normalize();
            //     Position -= direction * speed;
            // }
        }

        // 寻找附近的攻击目标
        private Monster findNewTarget(GameLocation location)
        {
            // 寻找附近的敌人
            Monster currentTarget = null;
            float closestDistance = float.MaxValue;

            foreach (var character in location.characters)
            {
                if (character.IsMonster)
                {
                    float distance = Vector2.Distance(Position, character.Position);
                    if (distance < attackRange && distance < closestDistance)
                    {
                        currentTarget = (Monster)character;
                        closestDistance = distance;
                    }
                }
            }
            return currentTarget;
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
            if (distanceToTarget <= attackRange)
            {
                // 开始进入战斗模式
                if (!isAttacking)
                {
                    isAttacking = true;
                    attackFrame = 0;
                    attackRotation = (float)(Game1.random.NextDouble() * Math.PI * 2);
                }
                if (isAttacking) // 攻击动画到特定帧时造成伤害
                {
                    this.dealDamage(target, location);
                    attackCooldown = AttackCooldownTime;
                    isAttacking = false;
                }
            }
            else
            {
                // 向目标移动
                Vector2 direction = Vector2.Normalize(target.Position - Position);
                Position += direction * 2f;
            }
        }

        private void dealDamage(Monster target, GameLocation location)
        {
            int damage = 1;

            // 应用伤害
            target.takeDamage(
                damage,
                (int)Position.X,
                (int)Position.Y,
                false,
                0,
                owner
            );

            // 播放攻击效果
            Game1.playSound("swordswipe");

            // 显示伤害数字
            location.debris.Add(new Debris(
                damage,
                target.getStandingPosition(),
                Color.Orange,
                1f,
                target
            ));
        }
    }
}