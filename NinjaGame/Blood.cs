using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace NinjaGame
{
    class Blood : GameObject
    {
        public bool IsStuckToSurface = false;

        // Wait some amount of frames before sticking. This is because sometimes blood appears in a wall and then just
        //creates a weird dark spot.
        int frameCount;
        private const int framesBeforeICanBeStuck = 5;

        MapSquare currentMapSquare;

        public void SetupTextures(ref AnimationStrip anim)
        {
            DisplayComponent = new StaticImageDisplay(anim.Texture, new Rectangle(0, 0, anim.FrameWidth, anim.FrameHeight));
        }

        public Blood()
        {
            this.isTileColliding = false;
            this.isEnemyTileColliding = false;
            this.IsAffectedByGravity = true;
            this.IsAffectedByPlatforms = false;
            this.IsAbleToSurviveOutsideOfWorld = false;
            this.IsAbleToMoveOutsideOfWorld = true;
            this.DrawCollisionRect = false;
            this.Initialize(false, Vector2.Zero, Vector2.Zero);
        }

        public void Initialize(bool enabled, Vector2 location, Vector2 velocity)
        {
            
            this.IsStuckToSurface = false;
            this.WorldLocation = location;
            this.Velocity = velocity;
            this.LoadCurrentMapSquare();
            this.Enabled = enabled;
            
            // hack because LoadCurrentMapSquare could disable the blood and so it's draw object could be stale and it will
            // never get a chance to update it.
            if (this.Enabled && !this.currentMapSquare.Passable)
            {
                this.SetupDraw(new GameTime(), 0.001f);
            }
        }

        public override void Update(GameTime gameTime, float elapsed)
        {

            if (!Enabled) return;

            if (!currentMapSquare.Passable && frameCount >= framesBeforeICanBeStuck) return;

            if (frameCount < framesBeforeICanBeStuck)
            {
                frameCount++;
            }

            if (IsStuckToSurface)
            {
                // Blood was stuck to a surface that no longer exists. Give it some kind of random velocity so chunks
                // of it don't look weird all falling in a straight line.
                this.velocity = EffectsManager.RandomDirection(30);
                if (this.velocity.Y > 0)
                {
                    this.velocity.Y = -this.velocity.Y;
                }
                IsStuckToSurface = false;
            }

            base.Update(gameTime, elapsed);

            if (frameCount >= framesBeforeICanBeStuck)
            {
                LoadCurrentMapSquare();
            }


            IsStuckToSurface = currentMapSquare != null && !currentMapSquare.Passable;

        }

        public void LoadCurrentMapSquare()
        {
            currentMapSquare = Game1.CurrentMap.GetMapSquareAtPixel(this.worldLocation);
            this.Enabled = currentMapSquare != null;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Enabled && Game1.Camera.IsObjectVisible(this.CollisionRectangle))
            {
                base.Draw(spriteBatch);
            }
        }

    }
}
