using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MysteriousRing.Framework.Utils;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Pathfinding;

namespace MysteriousRing.Framework.Companions
{
    public class RingServant : NPC
    {

        // 所属玩家
        private Farmer owner;
        // 与玩家跟随状态下的跟随距离
        private int followDistance = 150;

        // 仆从的视野距离(发现敌人的距离)
        private readonly float viewDistance = 500;
        // 仆从的攻击距离
        private readonly int attackRange = 80;
        // 仆从的攻击速冻 (冷却时间(毫秒))
        private readonly int AttackCooldownTime = 2000;
        // 剩余攻击冷却时间，下一次攻击剩余时间
        private int attackCooldown = 0;
        // 攻击力
        private int attackDamage = 3;
        //  状态上下浮动
        private int idleOffsetY = 0;

        private List<FarmerSprite.AnimationFrame> attackRightFrames;
        private List<FarmerSprite.AnimationFrame> attackLeftFrames;

        public static RingServant Create(Farmer owner, String name)
        {
            // 加载mod中内容贴图
            Texture2D texture = ModEntry.ModHelper.ModContent.Load<Texture2D>(
                "assets/servant_1.png"
            );
            // 创建AnimatedSprite（单帧）
            AnimatedSprite sprite = new AnimatedSprite(
                textureName: "",   // 留空（因为直接使用Texture2D）
                currentFrame: 0,   // 固定0帧
                spriteWidth: texture.Width / 7,  // 图片宽度=单帧宽度
                spriteHeight: texture.Height / 2 // 图片高度=单帧高度
            )
            {
                // 使用mod中的贴图纹理
                spriteTexture = texture,
                loop = false,
            };

            RingServant servant = new RingServant(sprite, owner, name);

            // 设置动画
            servant.attackRightFrames = new List<FarmerSprite.AnimationFrame>{
                new FarmerSprite.AnimationFrame(0, 100),
                new FarmerSprite.AnimationFrame(1, 150),
                new FarmerSprite.AnimationFrame(2, 100),
                new FarmerSprite.AnimationFrame(3, 100),
                new FarmerSprite.AnimationFrame(4, 100),
                new FarmerSprite.AnimationFrame(5, 100),
                new FarmerSprite.AnimationFrame(6, 150),
            };
            servant.attackLeftFrames = new List<FarmerSprite.AnimationFrame>{
                new FarmerSprite.AnimationFrame(7, 100),
                new FarmerSprite.AnimationFrame(8, 150),
                new FarmerSprite.AnimationFrame(9, 100),
                new FarmerSprite.AnimationFrame(10, 100),
                new FarmerSprite.AnimationFrame(11, 100),
                new FarmerSprite.AnimationFrame(12, 100),
                new FarmerSprite.AnimationFrame(13, 150),
            };
            return servant;
        }

        private RingServant(AnimatedSprite sprite, Farmer owner, string name) : base(sprite, owner.Tile * 64f, 0, name)
        {
            this.owner = owner;
            base.HideShadow = true;
            base.Scale = 1;
            base.Breather = false; // 喘气
            base.displayName = null;
            base.Portrait = null;
            base.speed = 6;
            base.willDestroyObjectsUnderfoot = false;
            base.collidesWithOtherCharacters.Value = false;
        }

        protected override void initNetFields()
        {
            base.initNetFields();
        }

        // 静止状态hook
        public override void Halt()
        {
            base.Halt();
        }

        // 核心函数
        // 每帧调用一次 更新数据和状态
        public override void update(GameTime time, GameLocation location)
        {
            // base.update() 会自动增加帧序列
            base.update(time, location);

            // 在动画中不进行操作，继续播放动画
            if (Sprite.CurrentAnimation != null)
            {
                return;
            }

            // 以主人为中心 寻找视野范围内的敌人
            Monster target = MapUtils.findClosestMonster(
                owner.Position,
                viewDistance,
                location
            );
            // 如果存在敌人 并且与主人距离小于视野距离 则进行攻击
            if (target != null)
            {
                updateAttack(time, target, location);
                return;
            }
            updateFollow(time, location);
        }

        // 跟随
        private void updateFollow(GameTime gameTime, GameLocation location)
        {
            // 计算与玩家的距离, 进行跟随
            float distance = Vector2.Distance(owner.Position, Position);
            if (distance > followDistance)
            {
                // 计算方向并归一化
                Vector2 direction = owner.Position - Position;
                direction.Normalize();
                // 移动仆从
                Position += direction * speed;
                return;
            }
            else
            {
                if (idleOffsetY > 15)
                {
                    Position += new Vector2(0, -0.5f);
                }
                else
                {
                    Position += new Vector2(0, 0.5f);
                }
                idleOffsetY = (idleOffsetY + 1) % 30;
            }
        }

        public bool IsStationary()
        {
            // 同时检测X轴和Y轴速度
            return this.xVelocity == 0 && this.yVelocity == 0;
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

            // 计算与目标的距离
            float distanceToTarget = Vector2.Distance(Position, target.Position);

            // 攻击距离足够近 则发起攻击
            if (distanceToTarget <= attackRange)
            {
                // 开始进入战斗模式帧动画
                attackCooldown = AttackCooldownTime;

                // 判断攻击方向 播放动画
                if (target.Position.X < Position.X)
                {
                    if (attackLeftFrames != null)
                    {
                        // 怪物在玩家左侧
                        Sprite.setCurrentAnimation(attackLeftFrames);
                    }
                }
                else
                {
                    if (attackRightFrames != null)
                    {
                        // 怪物在玩家右侧
                        Sprite.setCurrentAnimation(attackRightFrames);
                    }
                }

                // 应用伤害
                target.takeDamage(
                    attackDamage,
                    (int)target.Position.X,
                    (int)target.Position.Y,
                    false,
                    0,
                    owner
                );

                // 显示伤害数字
                location.debris.Add(
                    new Debris(attackDamage, target.getStandingPosition(), Color.Orange, 1f, target)
                );

                // 播放攻击声效
                Game1.playSound("swordswipe");
            }
            else
            {
                // 向目标移动
                Vector2 direction = Vector2.Normalize(target.Position - Position);
                Position += direction * speed;
            }
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

        public override Rectangle GetBoundingBox()
        {
            return new Rectangle((int)Position.X, (int)Position.Y, 0, 0);
        }

        public override bool isColliding(GameLocation l, Vector2 tile)
        {
            return false;
        }

        public override bool collideWith(StardewValley.Object o)
        {
            return false;

        }

        public override bool shouldCollideWithBuildingLayer(GameLocation location)
        {
            return false;
        }
    }
}