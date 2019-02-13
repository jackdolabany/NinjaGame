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
    public class TempObject : GameObject
    {
        private Texture2D Texture;

        public TempObject(ContentManager content)
        {
            var playerTexture = content.Load<Texture2D>(@"Textures/SimpleSprites");

            this.DisplayComponent = new StaticImageDisplay(Game1.simpleSprites, Helpers.GetTileRect(0, 1));

            Enabled = true;

            // TODO: Whatever
            //cd.DrawDepth = 0.5f;
            this.WorldLocation = new Vector2(100, 100);

            this.IsAbleToMoveOutsideOfWorld = true;
            this.IsAbleToSurviveOutsideOfWorld = true;
            this.IsAffectedByForces = false;

            // Temp
            this.IsAffectedByGravity = false;
            this.RotationsPerSecond = 0.5f;

            this.IsAffectedByPlatforms = false;

            // Temp
            //this.CollisionRectangle = new Rectangle(-6, -12, 12, 12);
            SetCenteredCollisionRectangle(12, 14);
        }

        bool isGrowing = true;

        public override void Update(GameTime gameTime, float elapsed)
        {

            if (isGrowing)
            {
                this.Scale += elapsed;
                if (this.Scale > 5f)
                {
                    isGrowing = false;
                }
            }
            else
            {
                this.Scale -= elapsed;
                if (this.Scale < 0.5f)
                {
                    isGrowing = true;
                }
            }


            base.Update(gameTime, elapsed);
            
        }
    }
}
