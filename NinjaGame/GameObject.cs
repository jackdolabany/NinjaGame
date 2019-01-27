using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngine;

namespace NinjaGame
{
    public class GameObject
    {
        #region Declarations
        protected Vector2 worldLocation;

        private bool enabled;
        private bool _flipped = false;
        protected bool flipped
        {
            get
            {
                if (initiallyFlipped)
                {
                    return !_flipped;
                }
                else
                {
                    return _flipped;
                }
            }
            set
            {
                if (initiallyFlipped)
                {
                    _flipped = !value;
                }
                else
                {
                    _flipped = value;
                }
            }
        }

        protected bool initiallyFlipped = false;

        public bool Landed;

        // The speed at which you were falling when you landed.
        public float LandingVelocity;

        protected bool onGround;
        protected bool onCeiling;
        protected bool onLeftWall;
        protected bool onRightWall;

        protected Rectangle collisionRectangle;

        /// <summary>
        /// if true, this enemy will be blocked by enemy blocking tiles. This is a way to restrict enemies to walk back
        /// and forth in a given area. This should be false for any enemy that isn't affected by gravity or that can move in the 
        /// y direction (jump) because they can land on the enemy tiles and it will look weird. 
        /// </summary>
        public bool isEnemyTileColliding = true;
        protected bool isTileColliding = true;

        public float RotationsPerSecond = 0;
        public bool IsRotationClockwise;

        public bool DrawCollisionRect = false;
        public bool DrawLocation = false;

        private Point[] pixelsToTest = new Point[20];

        // Encapsulates common display logic and state
        public DisplayComponent DisplayComponent;

        // So old stuff doesnt' break
        public float DrawDepth
        {
            get
            {
                return DisplayComponent.DrawDepth;
            }
            set
            {
                DisplayComponent.DrawDepth = value;
            }
        }

        public float Rotation
        {
            get { return this.DisplayComponent.Rotation; }
            set { this.DisplayComponent.Rotation = value; }
        }

        public Vector2 RotationAndDrawOrigin
        {
            get { return DisplayComponent.RotationAndDrawOrigin; }
            set { DisplayComponent.RotationAndDrawOrigin = value; }
        }

        public virtual float Scale
        {
            get { return this.DisplayComponent.Scale; }
            set { this.DisplayComponent.Scale = value; }
        }

        #endregion

        #region Properties
        public bool Enabled
        {
            get { return enabled; }
            set { enabled = value; }
        }

        /// <summary>
        /// If this is true the gameObject can move outside the bounds of the game world.
        /// If false, the character's position will be clamped in the game world.
        /// </summary>
        public bool IsAbleToMoveOutsideOfWorld;

        /// <summary>
        /// If this is true, the object will be disabled upon leaving the game world.
        /// </summary>
        public bool IsAbleToSurviveOutsideOfWorld;

        /// <summary>
        /// True if the object is colliding with a surface, top, bottom, left, or right
        /// </summary>
        public bool IsTouchingSurface
        {
            get
            {
                return (onCeiling || onGround || onLeftWall || onRightWall);
            }
        }

        public bool OnCeiling { get { return onCeiling; } }
        public bool OnGround { get { return onGround; } }
        public bool OnLeftWall { get { return onLeftWall; } }
        public bool OnRightWall { get { return onRightWall; } }
        public bool OnPlatform { get; private set; }

        /// <summary>
        /// Warning, this fan fuck up updating the gameObject
        /// </summary>
        public void SetToNotTouchingAnything()
        {
            onCeiling = false;
            onGround = false;
            onLeftWall = false;
            onRightWall = false;
        }

        public bool IsAffectedByGravity { get; set; }
        public bool IsAffectedByPlatforms { get; set; }

        public static Vector2 FALL_SPEED = new Vector2(0, 1100);

        protected Vector2 velocity;
        public Vector2 Velocity
        {
            get { return velocity + ForceVelocity; }
            set { velocity = value; }
        }

        public bool IsAffectedByForces = true;

        private Vector2 forceVelocity;
        public Vector2 ForceVelocity
        {
            get
            {
                return forceVelocity;
            }
            set
            {
                if (IsAffectedByForces)
                {
                    forceVelocity = value;
                }
            }
        }

        public Vector2 WorldLocation
        {
            get { return worldLocation; }
            set { worldLocation = value; }
        }

        public Vector2 WorldCenter
        {
            get
            {
                return DisplayComponent.GetWorldCenter(ref worldLocation);
            }
        }

        /// <summary>
        /// This is the rectangle we'll test to see if we should bother drawing the character.
        /// </summary>
        public virtual Rectangle GetDrawRectangle()
        {
            var rect = this.CollisionRectangle;
            return new Rectangle(rect.X - 100, rect.Y - 100, rect.Width + 200, rect.Height + 200);
        }

        /// <summary>
        /// Lazy load helpers
        /// </summary>
        public virtual Rectangle CollisionRectangle
        {
            get
            {
                return getCollisionRectangleForPosition(ref worldLocation);
            }
            set { collisionRectangle = value; }
        }

        public Vector2 CollisionCenter
        {
            get
            {
                if (collisionRectangle.IsEmpty)
                {
                    return this.WorldCenter;
                }
                else
                {
                    return CollisionRectangle.Center.ToVector();
                }
            }
        }

        protected Rectangle getCollisionRectangleForPosition(ref Vector2 position)
        {
            if (DisplayComponent == null)
            {
                return new Rectangle(
                  (int)(position.X) + collisionRectangle.X,
                  (int)(position.Y) + collisionRectangle.Y,
                  collisionRectangle.Width,
                  collisionRectangle.Height);
            }
            else
            {
                return new Rectangle(
                  (int)(position.X - DisplayComponent.RotationAndDrawOrigin.X) + collisionRectangle.X,
                  (int)(position.Y - DisplayComponent.RotationAndDrawOrigin.Y) + collisionRectangle.Y,
                  collisionRectangle.Width,
                  collisionRectangle.Height);
            }
        }

        #endregion 

        public GameObject()
        {
            IsAbleToMoveOutsideOfWorld = true;
            IsAbleToSurviveOutsideOfWorld = false;
            IsAffectedByGravity = false;
            IsAffectedByPlatforms = true;
        }

        #region Public Methods

        public virtual void Flip()
        {
            flipped = !flipped;
        }

        public void RotateTo(Vector2 direction)
        {
            Rotation = (float)Math.Atan2(direction.Y, direction.X);
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!enabled)
                return;

            Vector2 previousLocation = this.worldLocation;

            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (RotationsPerSecond > 0)
            {
                var rotation = (IsRotationClockwise ? 1 : -1) * elapsed * RotationsPerSecond;
                Rotation += rotation;
            }

            onCeiling = false;
            OnPlatform = false;

            if (IsAffectedByGravity)
            {
                velocity += FALL_SPEED * elapsed;
            }

            Vector2 moveAmount = Velocity * elapsed;

            //var previousUpdatePlatForm = PlatformThatThisIsOn;

            //if (isTileColliding)
            //{
            //    moveAmount = horizontalCollisionTest(moveAmount);
            //    moveAmount = verticalCollisionTest(moveAmount);

            //    // They are on a platform that they were already on, move them with the platform
            //    if (PlatformThatThisIsOn != null && PlatformThatThisIsOn == previousUpdatePlatForm)
            //    {
            //        moveAmount += PlatformThatThisIsOn.Delta;
            //        moveAmount = horizontalCollisionTest(moveAmount);
            //    }
            //}

            Vector2 newPosition = worldLocation + moveAmount;

            if (!IsAbleToMoveOutsideOfWorld)
            {
                //only clamp the x position in the world. If they fall out of the world, they are toast.
                if (CollisionRectangle.Left < 0)
                {
                    newPosition.X -= CollisionRectangle.Left;
                    velocity.X = 0;
                    onLeftWall = true;
                }
                else if (CollisionRectangle.Right > Camera.WorldRectangle.Width)
                {
                    newPosition.X -= (CollisionRectangle.Right - Camera.WorldRectangle.Width);
                    velocity.X = 0;
                    onRightWall = true;
                }
            }

            if (!IsAbleToSurviveOutsideOfWorld)
            {
                if (!this.CollisionRectangle.Intersects(Camera.WorldRectangle))
                {
                    Enabled = false;
                }
            }

            AdjustPositionBeforeDraw(ref newPosition, ref previousLocation);

            worldLocation = newPosition;

            DisplayComponent.Update(elapsed, this.worldLocation, this._flipped);
        }

        public virtual void AdjustPositionBeforeDraw(ref Vector2 newPosition, ref Vector2 previousLocation) { }

        public void SetupDraw(float elapsed)
        {
            this.DisplayComponent.Update(elapsed, this.worldLocation, this.flipped);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (enabled)
            {
                this.DisplayComponent.Draw(spriteBatch);
            }

            // Draw Collision Rectangle
            if (DrawCollisionRect || Game1.DrawAllCollisisonRects && !collisionRectangle.IsEmpty)
            {
                Color color = Color.Red * 0.25f;
                spriteBatch.Draw(Game1.simpleSprites, CollisionRectangle, Game1.whiteSourceRect, color);
            }

            // Draw a square at the GameObjects location
            if (DrawLocation || Game1.DrawAllCollisisonRects)
            {
                var rectSize = 8;
                var location = WorldLocation;
                spriteBatch.Draw(Game1.simpleSprites, new Rectangle(-rectSize / 2 + (int)location.X, -rectSize / 2 + (int)location.Y, rectSize / 2, rectSize / 2), Game1.whiteSourceRect, Color.Green);
            }
        }

        #endregion

    }
}
