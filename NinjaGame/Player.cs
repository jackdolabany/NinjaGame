using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NinjaGame.Platforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaGame
{
    public class Player : GameObject
    {
        AnimationDisplay animations;

        public Rectangle AttackRectangle;

        // Keeps track of the enemies already stabbed for a given knife swing.
        private List<Enemy> stabbedEnemies = new List<Enemy>(5);

        private AnimationStrip attackAnimation;

        public InputManager InputManager { get; private set; }

        private DeadMenu _deadMenu;

        /// <summary>
        /// As longas you hold the jump button down and this timer doesn't run out, you'll continue to soar!
        /// This is so the player can do quick jumps or little ones.
        /// </summary>
        private float JumpButtonIsStillHeldDownTimer;

        private float speed;
        private float jumpSpeed;

        public Player(ContentManager content, InputManager inputManager, DeadMenu deadMenu)
        {
            //animations = new AnimationDisplay();
            //this.DisplayComponent = animations;

            var config = CharacterConfig.GetCharacterConfig(@"Content\Textures\ninja\Player.json");

            this.speed = config.Speed;
            this.jumpSpeed = config.JumpSpeed;

            this.animations = CharacterConfig.GetAnimationDisplay(config, content);
            this.DisplayComponent = animations;

            this.attackAnimation = this.animations.animations["attack"];

            //var idleImage = content.Load<Texture2D>(@"Textures\ninja\idle");
            //var idle = new AnimationStrip(idleImage, 32, "idle");
            //idle.LoopAnimation = true;
            //idle.FrameLength = 0.1f;
            //animations.Add(idle);

            //var walkImage = content.Load<Texture2D>(@"Textures\ninja\walk");
            //var walk = new AnimationStrip(walkImage, 48, "walk");
            //walk.LoopAnimation = true;
            //walk.FrameLength = 0.14f;
            //animations.Add(walk);

            //var jumpImage = content.Load<Texture2D>(@"Textures\ninja\jump");
            //var jump = new AnimationStrip(jumpImage, 36, "jump");
            //jump.FrameLength = 0.1f;
            //animations.Add(jump);

            //var attackImage = content.Load<Texture2D>(@"Textures\ninja\attack");
            //attackAnimation = new AnimationStrip(attackImage, 48, "attack");
            //attackAnimation.Oscillate = true;
            //attackAnimation.FrameLength = 0.05f;
            //animations.Add(attackAnimation);

            Enabled = true;

            // TODO: Whatever
            //cd.DrawDepth = 0.5f;

            this.IsAbleToMoveOutsideOfWorld = true;
            this.IsAbleToSurviveOutsideOfWorld = true;
            this.IsAffectedByForces = false;
            this.isEnemyTileColliding = false;

            this.IsAffectedByGravity = true;

            this.IsAffectedByPlatforms = true;

            this.CollisionRectangle = new Rectangle(-5, -27, 10, 26);

            InputManager = inputManager;
            _deadMenu = deadMenu;

        }

        public override void Update(GameTime gameTime, float elapsed)
        {

            HandleInputs(elapsed);

            if (Velocity.X > 0)
            {
                flipped = false;
            }
            else if (Velocity.X < 0)
            {
                flipped = true;
            }

            base.Update(gameTime, elapsed);

            AttackRectangle = Rectangle.Empty;
            if (animations.currentAnimationName == "attack" && attackAnimation.currentFrame >= 2)
            {
                AttackRectangle = new Rectangle(this.CollisionRectangle.Right, this.CollisionRectangle.Y, 13, 15);
                if (flipped)
                {
                    AttackRectangle.X -= (this.collisionRectangle.Width + AttackRectangle.Width);
                }
            }
        }

        public void CheckEnemyInteractions(Enemy enemy)
        {
            if(enemy.Alive)
            {
                if (AttackRectangle.Intersects(enemy.CollisionRectangle) && !stabbedEnemies.Contains(enemy))
                {
                    stabbedEnemies.Add(enemy);
                    var force = enemy.WorldCenter - this.WorldCenter;
                    force.Normalize();
                    enemy.TakeHit(1, force * 100f);
                }
                else
                {
                    // Check body collisions
                    if (CollisionRectangle.Intersects(enemy.CollisionRectangle))
                    {
                        Kill();
                    }
                }

            }
            
        }

        private void HandleInputs(float elapsed)
        {

            this.velocity.X = 0;

            string nextAnimation = null;
            if (OnGround)
            {
                nextAnimation = "idle";
            }
            else
            {
                nextAnimation = "jump";
            }

            if (InputManager.CurrentAction.left && !InputManager.CurrentAction.right)
            {
                this.velocity.X = -speed;
                if (OnGround)
                {
                    nextAnimation = "walk";
                }
            }
            else if (InputManager.CurrentAction.right && !InputManager.CurrentAction.left)
            {
                this.velocity.X = speed;
                if (OnGround)
                {
                    nextAnimation = "walk";
                }
            }

            if (OnGround)
            {
                PoisonPlatforms.Clear();
            }

            if (InputManager.CurrentAction.jump && !InputManager.PreviousAction.jump && InputManager.CurrentAction.down && OnPlatform)
            {
                // Jump down from platform(s). Find every platform below the player and mark them all as poison.
                var belowPlayerRect = new Rectangle(this.CollisionRectangle.Left, this.CollisionRectangle.Bottom, this.CollisionRectangle.Width, 3);

                foreach (var platform in Game1.Platforms)
                {
                    if (belowPlayerRect.Intersects(platform.CollisionRectangle))
                    {
                        this.PoisonPlatforms.Add(platform);
                    }
                }

            }
            else if (InputManager.CurrentAction.jump && !InputManager.PreviousAction.jump && OnGround)
            {
                // Regular jump.
                this.velocity.Y = -jumpSpeed;
                nextAnimation = "jump";
                SoundManager.PlaySound("cloth1");
                JumpButtonIsStillHeldDownTimer = 0.3f;
            }
            else if (!OnGround && JumpButtonIsStillHeldDownTimer > 0 && InputManager.CurrentAction.jump && InputManager.PreviousAction.jump)
            {
                // Holding down jump to continue a jump.
                this.velocity.Y = -jumpSpeed;
                JumpButtonIsStillHeldDownTimer -= elapsed;
            }
            else
            {
                // Not jumping
                JumpButtonIsStillHeldDownTimer = 0f;
            }

            // Attack.
            if (InputManager.CurrentAction.attack && !InputManager.PreviousAction.attack)
            {
                nextAnimation = "attack";
            }

            if (animations.currentAnimationName != nextAnimation)
            {
                var isAttackPlaying = animations.currentAnimationName == "attack" && !animations.animations["attack"].FinishedPlaying;
                if (!isAttackPlaying) // Don't break the attack
                {
                    animations.Play(nextAnimation);
                    if (nextAnimation == "attack")
                    {
                        SoundManager.PlaySound("woosh1");
                        stabbedEnemies.Clear();
                    }
                }
                
            }
        }

        public void Kill()
        {
            Enabled = false;
            EffectsManager.AddBigBloodEffect(WorldCenter);
            EffectsManager.RisingText("Dead", WorldCenter);
            EffectsManager.EnemyPop(WorldCenter, 10, Color.Red, 50f);
            SoundManager.PlaySound("bookClose");
            MenuManager.AddMenu(_deadMenu);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            // Draw Collision Rectangle in reddish
            if (DrawCollisionRect || Game1.DrawAllCollisisonRects && !AttackRectangle.IsEmpty)
            {
                Color color = Color.Aquamarine * 0.25f;
                spriteBatch.Draw(Game1.simpleSprites, AttackRectangle, Game1.whiteSourceRect, color);
            }

            base.Draw(spriteBatch);
        }

        public bool IsFacingRight()
        {
            return !this.flipped;
        }

        public bool IsFacingLeft()
        {
            return !IsFacingRight();
        }
    }
}
