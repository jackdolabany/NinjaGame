using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
    public static class Camera
    {
        private static Vector2 position = Vector2.Zero;
        private static Vector2 viewPortSize = Vector2.Zero;
        public static float Zoom; // Camera Zoom
        public static Matrix Transform; // Matrix Transform
        public static float Rotation; // Camera Rotation
        public static TileMap Map { get; set; }
        public static bool IsMaxed;
        public static float Velocity { get; set; }

        public const float DEFAULT_ZOOM = 1.35f;

        static Camera()
        {
            Velocity = 30f;
        }

        public static Vector2 ParallaxScale = new Vector2(0.75f, 0.75f);

        public static void UpdateTransformation(GraphicsDevice graphicsDevice)
        {
            var translationMatrix = Matrix.CreateTranslation(new Vector3(-position.X, -position.Y, 0));
            var rotationMatrix = Matrix.CreateRotationZ(Rotation);
            var scaleMatrix = Matrix.CreateScale(new Vector3(Zoom, Zoom, 1f));
            var originMatrix = Matrix.CreateTranslation(new Vector3(viewPortSize.X / 2f, viewPortSize.Y / 2f, 0));

            Transform = translationMatrix * rotationMatrix * scaleMatrix * originMatrix;
        }

        public static Matrix GetParallaxScrollingBackgroundTransformation(GraphicsDevice graphicsDevice)
        {
            var translationMatrix = Matrix.CreateTranslation(new Vector3(-position, 0));
            var rotationMatrix = Matrix.CreateRotationZ(Rotation);
            var scaleMatrix = Matrix.CreateScale(new Vector3(ParallaxScale * Zoom, 1f));
            var originMatrix = Matrix.CreateTranslation(new Vector3(viewPortSize.X / 2f, viewPortSize.Y / 2f, 0));

            return translationMatrix * rotationMatrix * scaleMatrix * originMatrix;
        }

        public static void FuckEverybodyHardCodeSetPosition(Vector2 position)
        {
            Camera.position = position;
        }

        public static Vector2 Position
        {
            get { return position; }
            set
            {
                float x = 0;
                float y = 0;
                
                var mapWidth = Map.MapWidth * TileMap.TileSize;
                if (mapWidth < ViewWidth)
                {
                    x = mapWidth / 2;
                }
                else
                {
                    x = MathHelper.Clamp(value.X,
                        WorldRectangle.X + (ViewWidth / 2),
                        WorldRectangle.Width - (ViewWidth / 2));
                }
                var mapHeight = Map.MapHeight * TileMap.TileSize;
                if (mapHeight < ViewHeight)
                {
                    y = mapHeight / 2;
                }
                else
                {
                    y = MathHelper.Clamp(value.Y,
                        WorldRectangle.Y + (ViewHeight / 2),
                        WorldRectangle.Height - (ViewHeight / 2));
                }

                position = new Vector2(x, y);
            }
        }

        private static Rectangle _worldRectangle = new Rectangle(0, 0, 0, 0);
        public static Rectangle WorldRectangle
        {
            get
            {
                return _worldRectangle;
            }
            set 
            { 
                _worldRectangle = value; 
            }
        }

        public static int ViewWidth
        {
            get
            {
                return (int)(viewPortSize.X / Zoom);
            }
        }

        public static int ViewHeight
        {
            get
            {
                return (int)(viewPortSize.Y / Zoom);
            }
        }

        public static int ViewPortWidth
        {
            get { return (int)viewPortSize.X; }
            set { viewPortSize.X = value; }
        }

        public static int ViewPortHeight
        {
            get { return (int)viewPortSize.Y; }
            set { viewPortSize.Y = value; }
        }

        public static Rectangle ViewPort
        {
            get
            {
                return new Rectangle(
                    (int)Position.X, (int)Position.Y,
                    ViewPortWidth, ViewPortHeight);
            }
        }

        public static Rectangle ScaledViewPort
        {
            get
            {
                int width = ViewWidth;
                int height = ViewHeight;
                return new Rectangle(
                    (int)(Position.X - width / 2f), (int)(Position.Y - height / 2f),
                    width, height);
            }
        }

        public static Rectangle ParallaxScaledViewPort
        {
            get
            {
                var svp = viewPortSize / Zoom / ParallaxScale;
                return new Rectangle(
                    (int)(Position.X - svp.X / 2f),
                    (int)(Position.Y - svp.Y / 2),
                    (int)svp.X,
                    (int)svp.Y);
            }
        }

        #region Public Methods

        public static bool IsObjectVisible(Rectangle bounds)
        {
            return ScaledViewPort.Intersects(bounds);
        }

        public static Vector2 GetRelativeScreenPosition(Vector2 worldPosition)
        {
            return Vector2.Transform(worldPosition, Camera.Transform);
        }

        public static bool IsPointVisible(Vector2 point)
        {
            return ScaledViewPort.Contains((int)point.X, (int)point.Y);
        }

        #endregion

    }
}
