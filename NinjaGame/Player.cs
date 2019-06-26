using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NinjaGame
{
    public class Player : GameObject
    {
        KeyboardState previousKeyState;

        AnimationDisplay animations;

        private Rectangle attackRectangle;
        private List<Enemy> punchedEnemies = new List<Enemy>(5);

        private AnimationStrip attackAnimation;

        public Player(ContentManager content)
        {
            animations = new AnimationDisplay();
            this.DisplayComponent = animations;

            var idleImage = content.Load<Texture2D>(@"Textures\ninja\idle");
            var idle = new AnimationStrip(idleImage, 32, "idle");
            idle.LoopAnimation = true;
            idle.FrameLength = 0.1f;
            animations.Add(idle);

            var walkImage = content.Load<Texture2D>(@"Textures\ninja\walk");
            var walk = new AnimationStrip(walkImage, 32, "walk");
            walk.LoopAnimation = true;
            walk.FrameLength = 0.1f;
            animations.Add(walk);

            var jumpImage = content.Load<Texture2D>(@"Textures\ninja\jump");
            var jump = new AnimationStrip(jumpImage, 36, "jump");
            jump.FrameLength = 0.1f;
            animations.Add(jump);

            var attackImage = content.Load<Texture2D>(@"Textures\ninja\attack");
            attackAnimation = new AnimationStrip(attackImage, 32, "attack");
            attackAnimation.Oscillate = true;
            attackAnimation.FrameLength = 0.05f;
            animations.Add(attackAnimation);

            Enabled = true;

            // TODO: Whatever
            //cd.DrawDepth = 0.5f;
            //this.WorldLocation = new Vector2(100, 100);

            this.IsAbleToMoveOutsideOfWorld = true;
            this.IsAbleToSurviveOutsideOfWorld = true;
            this.IsAffectedByForces = false;
            this.isEnemyTileColliding = false;

            // Temp
            this.IsAffectedByGravity = true;
            //this.RotationsPerSecond = 0.5f;

            this.IsAffectedByPlatforms = false;

            SetCenteredCollisionRectangle(16, 26);

            //AttackRectangle = new Rectangle(10, 5, 10, 10);

            //this.Scale = 3;

            //animations.Play("idle");
        }

        bool isGrowing = true;

        public override void Update(GameTime gameTime, float elapsed)
        {

            //if (isGrowing)
            //{
            //    this.Scale += elapsed;
            //    if (this.Scale > 2f)
            //    {
            //        isGrowing = false;
            //    }
            //}
            //else
            //{
            //    this.Scale -= elapsed;
            //    if (this.Scale < 0.5f)
            //    {
            //        isGrowing = true;
            //    }
            //}

            HandleInputs();

            if (Velocity.X > 0)
            {
                flipped = false;
            }
            else if (Velocity.X < 0)
            {
                flipped = true;
            }

            base.Update(gameTime, elapsed);

            attackRectangle = Rectangle.Empty;
            if (animations.currentAnimationName == "attack" && attackAnimation.currentFrame >= 2)
            {
                attackRectangle = new Rectangle(this.CollisionRectangle.Right, this.CollisionRectangle.Y, 10, 15);
                if (flipped)
                {
                    attackRectangle.X -= (this.collisionRectangle.Width + attackRectangle.Width);
                }
            }
        }

        public void CheckEnemyInteractions(Enemy enemy)
        {
            if(enemy.Alive)
            {
                if (attackRectangle.Intersects(enemy.CollisionRectangle) && !punchedEnemies.Contains(enemy))
                {
                    punchedEnemies.Add(enemy);
                    var force = enemy.WorldCenter - this.WorldCenter;
                    force.Normalize();
                    enemy.TakeHit(1, force * 100f);
                }
                else
                {
                    // Check body collisions
                    if (CollisionRectangle.Intersects(enemy.CollisionRectangle))
                    {
                        // Enemy collided with the player. Kill the player
                        Enabled = false;
                        EffectsManager.AddBigBloodEffect(WorldCenter);
                        EffectsManager.RisingText("Dead", WorldCenter);
                        EffectsManager.EnemyPop(WorldCenter, 10, Color.Red, 50f);
                        SoundManager.PlaySound("bookClose");
                    }
                }

            }
            
        }

        private void HandleInputs()
        {

            var keyState = Keyboard.GetState();

            const float speed = 140f;

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

            if (keyState.IsKeyDown(Keys.Left) && !keyState.IsKeyDown(Keys.Right))
            {
                this.velocity.X = -speed;
                if (OnGround)
                {
                    nextAnimation = "walk";
                }
            }
            if (keyState.IsKeyDown(Keys.Right) && !keyState.IsKeyDown(Keys.Left))
            {
                this.velocity.X = speed;
                if (OnGround)
                {
                    nextAnimation = "walk";
                }
            }
            
            // Jump.
            if (OnGround && keyState.IsKeyDown(Keys.Space) && !previousKeyState.IsKeyDown(Keys.Space))
            {
                this.velocity.Y = -400;
                nextAnimation = "jump";
                SoundManager.PlaySound("cloth1");
            }

            // Attack.
            if (keyState.IsKeyDown(Keys.LeftShift) && !previousKeyState.IsKeyDown(Keys.LeftShift))
            {
                nextAnimation = "attack";
                SoundManager.PlaySound("woosh1");
            }

            if (animations.currentAnimationName != nextAnimation)
            {
                var isAttackPlaying = animations.currentAnimationName == "attack" && !animations.animations["attack"].FinishedPlaying;
                if (!isAttackPlaying) // Don't break the attack
                {
                    animations.Play(nextAnimation);
                    if (nextAnimation == "attack")
                    {
                        punchedEnemies.Clear();
                    }
                }
                
            }

            previousKeyState = keyState;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {

            // Draw Collision Rectangle in reddish
            if (DrawCollisionRect || Game1.DrawAllCollisisonRects && !attackRectangle.IsEmpty)
            {
                Color color = Color.Aquamarine * 0.25f;
                spriteBatch.Draw(Game1.simpleSprites, attackRectangle, Game1.whiteSourceRect, color);
            }

            base.Draw(spriteBatch);
        }
    }
}
