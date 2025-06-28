using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;

namespace MysteriousRing.Framework.Companions
{
    public class RingServant2Factory : RingServantFactory
    {
        public NPC create(Farmer owner)
        {
            // 加载mod中内容贴图
            Texture2D texture = ModEntry.ModHelper.ModContent.Load<Texture2D>(
                $"assets/servant_2.png"
            );

            ServantConfig config = new ServantConfig()
            {
                name = "servant_2",
                owner = owner,
                followDistance = 150,
                viewDistance = 300,
                attackRange = 80,
                attackSpeed = 0.6,
                attackDamage = 1 * owner.CombatLevel,
                bloodsucking = 0.1F,
                moveSpend = 8,
                idleOnHead = true,
                enableMoveAttack = true,
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
                idleFrames = new List<FarmerSprite.AnimationFrame>
                {
                    new FarmerSprite.AnimationFrame(0, 100),
                    new FarmerSprite.AnimationFrame(1, 100),
                    new FarmerSprite.AnimationFrame(2, 100),
                    new FarmerSprite.AnimationFrame(3, 100),
                    new FarmerSprite.AnimationFrame(4, 100),
                },
                attackRightFrames = new List<FarmerSprite.AnimationFrame>{
                    new FarmerSprite.AnimationFrame(7, 100),
                    new FarmerSprite.AnimationFrame(8, 100),
                    new FarmerSprite.AnimationFrame(9, 100),
                    new FarmerSprite.AnimationFrame(10, 100),
                    new FarmerSprite.AnimationFrame(11, 100),
                },
                attackLeftFrames = new List<FarmerSprite.AnimationFrame>{
                    new FarmerSprite.AnimationFrame(7, 100),
                    new FarmerSprite.AnimationFrame(8, 100),
                    new FarmerSprite.AnimationFrame(9, 100),
                    new FarmerSprite.AnimationFrame(10, 100),
                    new FarmerSprite.AnimationFrame(11, 100),
                }
            };
            return new RingServant2(config);
        }
    }

    public class RingServant2 : RingServant
    {
        internal RingServant2(ServantConfig config) : base(config) { }


        protected override void updateAttack(GameTime gameTime, Monster target, GameLocation location)
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
            // 应用伤害
            damageNum = target.takeDamage(
                attackDamage,
                (int)target.Position.X,
                (int)target.Position.Y,
                false,
                0,
                owner
            );

            // 显示伤害数字
            location.debris.Add(
                new Debris(damageNum, target.getStandingPosition(), Color.Orange, 1f, target)
            );

            // 播放攻击声效
            Game1.playSound("swordswipe");

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
}