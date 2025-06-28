using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;

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
                viewDistance = 600,
                followDistance = 140,
                attackRange = 600,
                attackSpeed = 0.4,
                attackDamage = 3 * owner.CombatLevel,
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
                    new FarmerSprite.AnimationFrame(7, 80),
                    new FarmerSprite.AnimationFrame(8, 80),
                    new FarmerSprite.AnimationFrame(9, 80),
                    new FarmerSprite.AnimationFrame(10, 80),
                    new FarmerSprite.AnimationFrame(11, 80),
                    new FarmerSprite.AnimationFrame(12, 80),
                    new FarmerSprite.AnimationFrame(13, 80),
                },
                attackLeftFrames = new List<FarmerSprite.AnimationFrame>{
                    new FarmerSprite.AnimationFrame(0, 80),
                    new FarmerSprite.AnimationFrame(1, 80),
                    new FarmerSprite.AnimationFrame(2, 80),
                    new FarmerSprite.AnimationFrame(3, 80),
                    new FarmerSprite.AnimationFrame(4, 80),
                    new FarmerSprite.AnimationFrame(5, 80),
                    new FarmerSprite.AnimationFrame(6, 80),
                },
            };
            return new RingServant3(config);
        }
    }

    public class RingServant3 : RingServant
    {
        internal RingServant3(ServantConfig config) : base(config) { }

        protected override void updateAttack(GameTime gameTime, Monster target, GameLocation location)
        {
            // 冷却计时
            if (attackCooldown > 0 || Sprite.CurrentAnimation != null)
            {
                attackCooldown -= gameTime.ElapsedGameTime.Milliseconds;

                // 没有启动移动攻击
                return;
            }

            // 计算与目标的距离
            float distanceToTarget = Vector2.Distance(Position, target.Position);

            // 攻击距离足够近 则发起攻击
            bool canAttack = distanceToTarget <= attackRange - 20
                && attackCooldown <= 0
                && Sprite.CurrentAnimation == null;

            // 向目标移动
            if (!canAttack)
            {
                if (Vector2.Distance(owner.Position, target.Position) < viewDistance)
                {
                    Vector2 direction = Vector2.Normalize(target.Position - Position - new Vector2(8, 8));
                    Position += direction * (speed + addedSpeed);
                }
                return;
            }

            // 开始进入战斗模式帧动画
            attackCooldown = AttackCooldownTime;

            // 判断攻击方向 播放动画
            if (target.Position.X < Position.X)
            {
                // 怪物在玩家左侧
                Sprite.setCurrentAnimation(attackLeftFrames);
            }
            else
            {
                // 怪物在玩家右侧
                Sprite.setCurrentAnimation(attackRightFrames);
            }

            int damageNum = attackDamage;
            Game1.delayedActions.Add(new DelayedAction(500, () => // 延迟 500ms 发射
            {
                // 计算火球位置
                Vector2 startingPosition = Position + new Vector2(0, -Sprite.getHeight() * 3);
                Vector2 velocity = target.Position - startingPosition;
                velocity.Normalize();

                // 远程攻击弹药
                BasicProjectile fireball = new BasicProjectile(
                    damageToFarmer: damageNum,
                    spriteIndex: 10,
                    bouncesTillDestruct: 0,
                    tailLength: 10,
                    rotationVelocity: 5f,
                    xVelocity: velocity.X * 10f,
                    yVelocity: velocity.Y * 10f,
                    startingPosition: startingPosition,
                    damagesMonsters: true, // 是否伤害怪物
                    firingSound: "fireball", // 发射音效
                    explode: true, // 投射物忽略物体碰撞
                    firer: owner
                );
                Game1.currentLocation.projectiles.Add(fireball);
            }));
        }

    }
}