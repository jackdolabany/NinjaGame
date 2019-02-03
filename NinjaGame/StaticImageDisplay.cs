using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace NinjaGame
{
    public class StaticImageDisplay : DisplayComponent
    {

        public DrawObject DrawObject;

        public Rectangle Source
        {
            get
            {
                return DrawObject.SourceRectangle;
            }
            set
            {
                DrawObject.SourceRectangle = value;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return DrawObject.Texture;
            }
            set
            {
                DrawObject.Texture = value;
            }
        }

        public StaticImageDisplay(Texture2D texture, Rectangle textureSourceRectangle)
            : base()
        {
            this.DrawObject = new DrawObject()
            {
                Texture = texture,
                SourceRectangle = textureSourceRectangle
            };
        }

        public StaticImageDisplay(Texture2D texture)
            : base()
        {
            this.DrawObject = new DrawObject()
            {
                Texture = texture,
                SourceRectangle = texture.BoundingRectangle()
            };
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (DrawObject.Texture != null)
            {
                spriteBatch.Draw(
                    DrawObject.Texture,
                    new Vector2(DrawObject.Position.X, DrawObject.Position.Y),
                    DrawObject.SourceRectangle,
                    this.TintColor,
                    this.Rotation,
                    this.RotationAndDrawOrigin,
                    this.Scale,
                    DrawObject.Effect,
                    this.DrawDepth);
            }
        }

        public override void Update(GameTime gameTime, float elapsed, Vector2 position, bool flipped)
        {
            base.Update(gameTime, elapsed, position, flipped);

            DrawObject.Position = position;

            SpriteEffects effect = SpriteEffects.None;
            if (flipped)
            {
                effect = SpriteEffects.FlipHorizontally;
            }
            DrawObject.Effect = effect;
        }

        public override Vector2 GetWorldCenter(ref Vector2 worldLocation)
        {
            return new Vector2(
              worldLocation.X + ((float)this.DrawObject.SourceRectangle.Width / 2f),
              worldLocation.Y + ((float)this.DrawObject.SourceRectangle.Height / 2f));
        }
    }
}
