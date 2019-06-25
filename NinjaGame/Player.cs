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

        public Rectangle AttackRectangle;
        //public bool attackRectangleEnabled;

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

            AttackRectangle = Rectangle.Empty;
            if (animations.currentAnimationName == "attack" && attackAnimation.currentFrame >= 2)
            {
                AttackRectangle = new Rectangle(this.CollisionRectangle.Right, this.CollisionRectangle.Y, 10, 15);
                if (flipped)
                {
                    AttackRectangle.X -= (this.collisionRectangle.Width + AttackRectangle.Width);
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
            
            if (OnGround && keyState.IsKeyDown(Keys.Space) && !previousKeyState.IsKeyDown(Keys.Space))
            {
                // Jump
                this.velocity.Y = -400;
                animations.Play("jump");
                return;
            }

            if (keyState.IsKeyDown(Keys.LeftShift) && !previousKeyState.IsKeyDown(Keys.LeftShift))
            {
                animations.Play("attack").FollowedBy("idle");
                return;
            }

            if (animations.currentAnimationName != nextAnimation)
            {
                if (animations.currentAnimationName != "attack") // Don't break the attack
                {
                    animations.Play(nextAnimation);
                }
                
            }

            previousKeyState = keyState;

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
    }
}
