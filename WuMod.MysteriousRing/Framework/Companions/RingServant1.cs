

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MysteriousRing.Framework.Utils;
using StardewValley;
using StardewValley.Monsters;

namespace MysteriousRing.Framework.Companions
{

    public class RingServant1Factory : RingServantFactory
    {
        public NPC create(Farmer owner)
        {
            // 加载mod中内容贴图
            Texture2D texture = ModEntry.ModHelper.ModContent.Load<Texture2D>(
                $"assets/servant_1.png"
            );

            ServantConfig config = new ServantConfig()
            {
                name = "servant_1",
                owner = owner,
                viewDistance = 400,
                followDistance = 200,
                attackRange = 150,
                attackSpeed = 0.4,
                attackDamage = 3 * owner.CombatLevel,
                bloodsucking = 0,
                moveSpend = 6,
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
                attackRightFrames = new List<FarmerSprite.AnimationFrame>{
                    new FarmerSprite.AnimationFrame(0, 50),
                    new FarmerSprite.AnimationFrame(1, 150),
                    new FarmerSprite.AnimationFrame(2, 80),
                    new FarmerSprite.AnimationFrame(3, 80),
                    new FarmerSprite.AnimationFrame(4, 80),
                    new FarmerSprite.AnimationFrame(5, 80),
                    new FarmerSprite.AnimationFrame(6, 150),
                },
                attackLeftFrames = new List<FarmerSprite.AnimationFrame>{
                    new FarmerSprite.AnimationFrame(7, 50),
                    new FarmerSprite.AnimationFrame(8, 150),
                    new FarmerSprite.AnimationFrame(9, 80),
                    new FarmerSprite.AnimationFrame(10, 80),
                    new FarmerSprite.AnimationFrame(11, 80),
                    new FarmerSprite.AnimationFrame(12, 80),
                    new FarmerSprite.AnimationFrame(13, 150),
                }
            };
            return new RingServant1(config);
        }
    }

    public class RingServant1 : RingServant
    {
        internal RingServant1(ServantConfig config) : base(config) { }


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
                Sprite.setCurrentAnimation(attackLeftFrames);
            }
            else
            {
                // 怪物在玩家右侧
                Sprite.setCurrentAnimation(attackRightFrames);
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
                // 播放攻击声效
                Game1.playSound("swordswipe");
            }
        }

    }
}