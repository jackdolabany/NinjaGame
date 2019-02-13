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
    public class TempTextObject : GameObject
    {
        private Texture2D Texture;

        public TempTextObject(ContentManager content)
        {
            var playerTexture = content.Load<Texture2D>(@"Textures/SimpleSprites");

            var cd = new TextDisplay("Awesome!");
            this.DisplayComponent = cd;

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
            this.Scale = 0.4f;
            this.IsAffectedByPlatforms = false;
            

        }
        bool isGrowing = true;
        public override void Update(GameTime gameTime, float elapsed)
        {
            if (isGrowing)
            {
                this.Scale += elapsed;
                if (this.Scale > 0.4f)
                {
                    isGrowing = false;
                }
            }
            else
            {
                this.Scale -= elapsed;
                if (this.Scale < 0.1f)
                {
                    isGrowing = true;
                }
            }


            base.Update(gameTime, elapsed);
            
        }
    }
}
