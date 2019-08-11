using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace NinjaGame.Platforms
{
    public class StaticPlatform : Platform
    {
        public StaticPlatform(ContentManager content, int cellX, int cellY)
            : base(content, cellX, cellY)
        {
            this.DisplayComponent = new StaticImageDisplay(content.Load<Texture2D>(@"Textures/Platforms"), new Rectangle(0, 0, 16, 5));
            SetCenteredCollisionRectangle(16, 5);
        }
    }
}
