using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;

namespace MysteriousRing.Framework.Companions
{
    public class RingServant4Factory : RingServantFactory
    {
        public NPC create(Farmer owner)
        {
            // 加载mod中内容贴图
            Texture2D texture = ModEntry.ModHelper.ModContent.Load<Texture2D>(
                $"assets/servant_4.png"
            );

            ServantConfig config = new ServantConfig()
            {
                name = "servant_4",
                owner = owner,
                followDistance = 140,
                viewDistance = 800,
                attackRange = 800,
                attackSpeed = 0.4,
                attackDamage = 2 * owner.CombatLevel,
                attackRemote = true,
                bloodsucking = 0,
                moveSpend = 8,
                idleOnHead = true,
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
                    new FarmerSprite.AnimationFrame(0, 100),
                    new FarmerSprite.AnimationFrame(1, 100),
                    new FarmerSprite.AnimationFrame(2, 100),
                    new FarmerSprite.AnimationFrame(3, 100),
                    new FarmerSprite.AnimationFrame(4, 100),
                    new FarmerSprite.AnimationFrame(5, 100),
                    new FarmerSprite.AnimationFrame(6, 100),
                },
                attackLeftFrames = new List<FarmerSprite.AnimationFrame>{
                    new FarmerSprite.AnimationFrame(0, 100),
                    new FarmerSprite.AnimationFrame(1, 100),
                    new FarmerSprite.AnimationFrame(2, 100),
                    new FarmerSprite.AnimationFrame(3, 100),
                    new FarmerSprite.AnimationFrame(4, 100),
                    new FarmerSprite.AnimationFrame(5, 100),
                    new FarmerSprite.AnimationFrame(6, 100),
                },
            };
            return new RingServant4(config);
        }
    }

    public class RingServant4 : RingServant
    {
        internal RingServant4(ServantConfig config) : base(config) { }

        protected override void updateAttack(GameTime gameTime, Monster target, GameLocation location)
        {
            // 冷却计时
            if (attackCooldown > 0 || Sprite.CurrentAnimation != null)
            {
                attackCooldown -= gameTime.ElapsedGameTime.Milliseconds;
                return;
            }

            // 计算与目标的距离
            float distanceToTarget = Vector2.Distance(Position, target.Position);

            // 攻击距离足够近 则发起攻击
            bool canAttack = distanceToTarget <= attackRange - 20
                && attackCooldown <= 0
                && Sprite.CurrentAnimation == null;

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
            Sprite.setCurrentAnimation(attackLeftFrames);


            int damageNum = attackDamage;

            float startAngle = 0f;
            for (int i = 0; i < 8; i++)
            {
                float angle = startAngle + (float)(i * Math.PI / 4);
                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 5f;

                // 延迟发射，形成螺旋效果
                int delay = i * 5; // 每颗火球间隔 5 帧
                Game1.delayedActions.Add(new DelayedAction(delay, () =>
                {
                    BasicProjectile fireball = new BasicProjectile(
                        damageToFarmer: damageNum,
                        startingPosition: Position + new Vector2(Sprite.getWidth(), -Sprite.getHeight() * 2),
                        spriteIndex: 0,
                        bouncesTillDestruct: 0,
                        tailLength: 3,
                        rotationVelocity: 5,
                        xVelocity: velocity.X,
                        yVelocity: velocity.Y,
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
}