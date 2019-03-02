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

        public Player(ContentManager content)
        {
            animations = new AnimationDisplay();
            this.DisplayComponent = animations;

            var idle = new AnimationStrip(
                    content.Load<Texture2D>(@"Textures\idle"),
                    32,
                    "idle");
            idle.LoopAnimation = true;
            idle.FrameLength = 0.1f;
            animations.Add(idle);

            Enabled = true;

            // TODO: Whatever
            //cd.DrawDepth = 0.5f;
            //this.WorldLocation = new Vector2(100, 100);

            this.IsAbleToMoveOutsideOfWorld = true;
            this.IsAbleToSurviveOutsideOfWorld = true;
            this.IsAffectedByForces = false;

            // Temp
            this.IsAffectedByGravity = true;
            //this.RotationsPerSecond = 0.5f;

            this.IsAffectedByPlatforms = false;

            SetCenteredCollisionRectangle(16, 26);

            //this.Scale = 3;

            animations.Play("idle");
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


            // Movement
            KeyboardState keyState;

            keyState = Keyboard.GetState();

            const float speed = 140f;

            this.velocity.X = 0;

            if (keyState.IsKeyDown(Keys.Left) && !keyState.IsKeyDown(Keys.Right))
            {
                this.velocity.X = -speed;
            }
            if (keyState.IsKeyDown(Keys.Right) && !keyState.IsKeyDown(Keys.Left))
            {
                this.velocity.X = speed;
            }
            
            if (OnGround && keyState.IsKeyDown(Keys.Space) && !previousKeyState.IsKeyDown(Keys.Space))
            {
                // Jump
                this.velocity.Y = -400;
            }

            if (Velocity.X > 0)
            {
                flipped = false;
            }
            else if (Velocity.X < 0)
            {
                flipped = true;
            }

            base.Update(gameTime, elapsed);

            previousKeyState = keyState;
        }
    }
}
