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
        private int followDistance;

        // 仆从的视野距离(发现敌人的距离)
        private readonly float viewDistance;
        // 仆从的攻击距离
        private readonly int attackRange;
        // 仆从的攻击速冻 (冷却时间(毫秒))
        private readonly int AttackCooldownTime;
        // 剩余攻击冷却时间，下一次攻击剩余时间
        private int attackCooldown = 0;
        // 攻击力
        private int attackDamage;
        //  状态上下浮动
        private int idleOffsetY = 0;
        // 空闲
        private List<FarmerSprite.AnimationFrame> idleFrames;
        // 右攻击动画
        private List<FarmerSprite.AnimationFrame> attackRightFrames;
        // 左攻击动画
        private List<FarmerSprite.AnimationFrame> attackLeftFrames;

        internal RingServant(ServantConfig config) : base(config.animatedSprite, config.owner.Tile * 64f, 0, config.name)
        {
            this.owner = config.owner;
            this.viewDistance = config.viewDistance;
            this.attackDamage = config.attackDamage;
            this.attackRange = config.attackRange;
            this.followDistance = config.followDistance;
            this.attackRightFrames = config.attackRightFrames;
            this.attackLeftFrames = config.attackLeftFrames;
            this.AttackCooldownTime = (int)(1000 / config.attackSpeed);
            base.speed = config.moveSpend;

            base.HideShadow = true;
            base.Scale = 1;
            base.Breather = false; // 喘气
            base.displayName = null;
            base.Portrait = null;
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
                idle();
            }
        }

        public virtual void idle()
        {
            if (idleOffsetY > 25)
            {
                Position += new Vector2(0, -0.2f);
            }
            else
            {
                Position += new Vector2(0, 0.2f);
            }
            idleOffsetY = (idleOffsetY + 1) % 50;
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
            if (distanceToTarget <= attackRange - 20)
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

                // 查询目标范围内的所有怪物 攻击产生群体伤害
                List<Monster> monsters = MapUtils.findRangeMonsters(
                    Position,
                    attackRange,
                    location
                );

                foreach (var monster in monsters)
                {
                    // 应用伤害
                    int damageNum = monster.takeDamage(
                        attackDamage,
                        (int)monster.Position.X,
                        (int)monster.Position.Y,
                        false,
                        0,
                        owner
                    );
                    // 显示伤害数字
                    location.debris.Add(
                        new Debris(damageNum, monster.getStandingPosition(), Color.Orange, 1f, monster)
                    );
                }

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