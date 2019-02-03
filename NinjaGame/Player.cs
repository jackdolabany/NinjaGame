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
        private Texture2D playerTexture;

        KeyboardState previousKeyState;

        public Player(ContentManager content)
        {
            playerTexture = content.Load<Texture2D>(@"Textures/Tiles");

            var cd = new StaticImageDisplay(playerTexture, new Rectangle(0, Game1.TileSize, Game1.TileSize, Game1.TileSize));
            this.DisplayComponent = cd;
            Enabled = true;

            // TODO: Whatever
            //cd.DrawDepth = 0.5f;
            this.WorldLocation = new Vector2(100, 100);

            this.IsAbleToMoveOutsideOfWorld = true;
            this.IsAbleToSurviveOutsideOfWorld = true;
            this.IsAffectedByForces = false;
            this.IsAffectedByGravity = false;
            this.IsAffectedByPlatforms = false;
            this.CollisionRectangle = new Rectangle(0, 0, 16, 16);

        }

        public override void Update(GameTime gameTime, float elapsed)
        {
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

            base.Update(gameTime, elapsed);

            IsAffectedByGravity = true;

            previousKeyState = keyState;
        }
    }
}
