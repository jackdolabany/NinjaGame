using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
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

        public Player(ContentManager content)
        {
            playerTexture = content.Load<Texture2D>(@"Textures/Tiles");

            var cd = new StaticImageDisplay(playerTexture, new Rectangle(0, Game1.TileSize, Game1.TileSize, Game1.TileSize));
            this.DisplayComponent = cd;
            Enabled = true;

            // TODO: Whatever
            //cd.DrawDepth = 0.5f;
            this.WorldLocation = new Vector2(0, 0);

            this.IsAbleToMoveOutsideOfWorld = true;
            this.IsAbleToSurviveOutsideOfWorld = true;
            this.IsAffectedByForces = false;
            this.IsAffectedByGravity = false;
            this.IsAffectedByPlatforms = false;
            this.CollisionRectangle = new Rectangle(0, 0, 16, 16);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
