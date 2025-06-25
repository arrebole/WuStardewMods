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
using StardewValley.Projectiles;

namespace MysteriousRing.Framework.Companions
{
    public class RingServant : NPC
    {

        // 所属玩家
        protected Farmer owner;
        // 与玩家跟随状态下的跟随距离
        protected int followDistance;

        // 仆从的视野距离(发现敌人的距离)
        protected readonly float viewDistance;
        // 仆从的攻击距离
        protected readonly int attackRange;
        // 仆从的攻击速冻 (冷却时间(毫秒))
        protected readonly int AttackCooldownTime;
        // 剩余攻击冷却时间，下一次攻击剩余时间
        protected int attackCooldown = 0;
        // 攻击力
        protected int attackDamage;
        // 是否停在头上
        protected bool idleOnHead;
        // 停留在头上剩余时间
        protected int idleOnHeadTime;
        //  状态上下浮动
        protected int idleOffsetY = 0;
        // 移动攻击
        protected bool enableMoveAttack;
        // 吸取生命
        protected float bloodsucking;
        // 远程攻击
        protected bool attackRemote;
        // 空闲
        protected List<FarmerSprite.AnimationFrame> idleFrames;
        // 右攻击动画
        protected List<FarmerSprite.AnimationFrame> attackRightFrames;
        // 左攻击动画
        protected List<FarmerSprite.AnimationFrame> attackLeftFrames;

        internal RingServant(ServantConfig config) : base(config.animatedSprite, config.owner.Tile * 64f, 0, config.name)
        {
            this.owner = config.owner;
            this.viewDistance = config.viewDistance;
            this.bloodsucking = config.bloodsucking;
            this.attackDamage = config.attackDamage;
            this.attackRange = config.attackRange;
            this.attackRemote = config.attackRemote;
            this.enableMoveAttack = config.enableMoveAttack;
            this.idleOnHead = config.idleOnHead;
            this.followDistance = config.followDistance;
            this.idleFrames = config.idleFrames;
            this.attackRightFrames = config.attackRightFrames;
            this.attackLeftFrames = config.attackLeftFrames;
            this.AttackCooldownTime = (int)(1000 / config.attackSpeed);
            this.speed = config.moveSpend;
            this.Position = owner.Position + new Vector2(100f, 100f);
            this.addedSpeed = 0;
            this.HideShadow = true;
            this.Scale = 1;
            this.Breather = false; // 喘气
            this.displayName = null;
            this.Portrait = null;
            this.willDestroyObjectsUnderfoot = false;
            this.drawOnTop = true;
            // 与其他角色发生碰撞
            this.collidesWithOtherCharacters.Value = false;
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

            // 计算是否需要跟随主人
            // 不需要跟随则进入空闲状态
            Vector2 nextPosition = getNextIdlePosition();
            if (nextPosition != Position)
            {
                Position = nextPosition;
            }
            else
            {
                idle();
            }
        }

        protected virtual Vector2 getNextIdlePosition()
        {
            Vector2 nextPosition = Position;

            // 计算与玩家的距离, 如果距离过远 则进行计算跟随的位置
            float distance = Vector2.Distance(owner.Position, Position);
            if (distance > followDistance)
            {
                // 计算方向距离
                Vector2 direction = owner.Position - Position;
                // 向量长度变为1
                direction.Normalize();
                // 下一个移动位置
                nextPosition = Position + direction * (speed + addedSpeed);
            }
            else if (idleOnHead)
            {
                // 停在头上
                Vector2 target = owner.Position + new Vector2(-owner.Sprite.SpriteWidth * 2, -owner.Sprite.SpriteHeight * 3);
                if (Vector2.Distance(target, Position) > 10)
                {
                    Vector2 direction = target - Position;
                    direction.Normalize();
                    nextPosition = Position + direction * 2;
                }
            }

            // 保持原地
            return nextPosition;
        }

        // 空闲状态
        protected virtual void idle()
        {
            // 如果有空闲动画，则播放空闲动画
            if (idleFrames != null && Sprite.CurrentAnimation == null)
            {
                Sprite.setCurrentAnimation(idleFrames);
                return;
            }

            // 上下浮动漂浮
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

        // 攻击目标
        private void updateAttack(GameTime gameTime, Monster target, GameLocation location)
        {
            // 冷却计时
            if (attackCooldown > 0 || Sprite.CurrentAnimation != null)
            {
                attackCooldown -= gameTime.ElapsedGameTime.Milliseconds;

                // 没有启动移动攻击
                if (!enableMoveAttack)
                {
                    return;
                }
            }

            // 计算与目标的距离
            float distanceToTarget = Vector2.Distance(Position, target.Position);

            // 攻击距离足够近 则发起攻击
            bool canAttack = distanceToTarget <= attackRange - 20
                && attackCooldown <= 0
                && Sprite.CurrentAnimation == null;

            if (canAttack)
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
                // 如果是远程攻击 只取一个目标
                if (attackRemote)
                {
                    monsters = monsters.Take(1).ToList();
                }

                foreach (var monster in monsters)
                {

                    int damageNum = attackDamage;
                    if (attackRemote)
                    {
                        // 计算火球位置
                        Vector2 velocity = target.Position - Position - new Vector2(0, 20f);
                        velocity.Normalize();
                        // 远程攻击弹药
                        BasicProjectile fireball = new BasicProjectile(
                            damageToFarmer: damageNum,
                            spriteIndex: 6,
                            bouncesTillDestruct: 0,
                            tailLength: 3,
                            rotationVelocity: 5f,
                            xVelocity: velocity.X * 10f,
                            yVelocity: velocity.Y * 10f,
                            startingPosition: Position,
                            damagesMonsters: true, // 是否伤害怪物
                            firingSound: "fireball", // 发射音效
                            explode: true, // 投射物忽略物体碰撞
                            firer: owner
                        );
                        Game1.currentLocation.projectiles.Add(fireball);
                    }
                    else
                    {
                        // 应用伤害
                        damageNum = monster.takeDamage(
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
                        // 播放攻击声效
                        Game1.playSound("swordswipe");
                    }


                    // 是否有吸血效果
                    if (bloodsucking > 0 && owner.health < owner.maxHealth)
                    {
                        int healAmount = Math.Max((int)(damageNum * bloodsucking), 1);
                        owner.health = Math.Min(owner.health + healAmount, owner.maxHealth);

                        // 显示治疗效果
                        Game1.player.currentLocation.debris.Add(new Debris(
                            healAmount,
                            owner.getStandingPosition(),
                            Color.LimeGreen,
                            1f,
                            owner
                        ));

                        // 播放治疗音效
                        Game1.playSound("healSound");
                    }
                }
            }
            else
            {
                // 向目标移动
                Vector2 direction = Vector2.Normalize(target.Position - Position);
                Position += direction * (speed + addedSpeed);
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
            return Rectangle.Empty;
        }

        // 可以通过所有动作地块
        public override bool canPassThroughActionTiles()
        {
            return true;
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