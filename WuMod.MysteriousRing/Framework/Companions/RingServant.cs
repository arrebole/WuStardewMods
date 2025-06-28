using Microsoft.Xna.Framework;
using MysteriousRing.Framework.Utils;
using StardewValley;
using StardewValley.Monsters;

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
            if (target != null && distanceToOwner() < viewDistance)
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

        private float distanceToOwner()
        {
            return Vector2.Distance(owner.Position, Position);
        }

        protected virtual Vector2 getNextIdlePosition()
        {
            Vector2 nextPosition = Position;

            // 计算与玩家的距离, 如果距离过远 则进行计算跟随的位置
            if (distanceToOwner() > followDistance)
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

        // 攻击目标, 在子类实现
        protected virtual void updateAttack(GameTime gameTime, Monster target, GameLocation location)
        {
            return;
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