using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace NinjaGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public const int GAME_X_RESOLUTION = 432;
        public const int GAME_Y_RESOLUTION = 240;

        public static Random Randy = new Random();
        public static bool DrawAllCollisisonRects = false;

        public static Texture2D simpleSprites;
        public static Rectangle whiteSourceRect = new Rectangle(1, 1, 1, 1).ToTileRect();

        public const int TileSize = TileEngine.TileMap.TileSize;

        RenderTarget2D gameRenderTarget;

        private Player player;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = GAME_X_RESOLUTION * 3;
            graphics.PreferredBackBufferHeight = GAME_Y_RESOLUTION * 3;
            Window.AllowUserResizing = true;
            Window.Title = "Ninja Game";
            //Window.

            Content.RootDirectory = "Content";

            Camera.Position = Vector2.Zero;
            Camera.Zoom = Camera.DEFAULT_ZOOM;
            Camera.ViewPortWidth = Game1.GAME_X_RESOLUTION;
            Camera.ViewPortHeight = Game1.GAME_Y_RESOLUTION;

            DrawAllCollisisonRects = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            simpleSprites = Content.Load<Texture2D>(@"Textures\Tiles");
            player = new Player(Content);
            gameRenderTarget = new RenderTarget2D(GraphicsDevice, GAME_X_RESOLUTION, GAME_Y_RESOLUTION, false, SurfaceFormat.Color, DepthFormat.None);

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            if (gameRenderTarget != null)
            {
                gameRenderTarget.Dispose();
                gameRenderTarget = null;
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            

            Camera.UpdateTransformation(GraphicsDevice);
            var cameraTransformation = Camera.Transform;

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                  BlendState.AlphaBlend,
                  SamplerState.LinearClamp,
                  null,
                  null,
                  null,
                  cameraTransformation);

            player.Draw(spriteBatch);

            // We'll draw everything to gameRenderTarget, including the white render target.
            GraphicsDevice.SetRenderTarget(gameRenderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.End();

            // Switch back to drawing onto the back buffer. This is the default space in memory, the size is determined by the ClientWindow. 
            // When the present call is made, the backbuffer will show up as the new screen.
            GraphicsDevice.SetRenderTarget(null);

            // XNA draws a bright purple color to the backbuffer by default when we switch to it. Lame! Let's clear it out.
            GraphicsDevice.Clear(Color.Black);

            // Draw the gameRenderTarget with everything in it to the back buffer. We'll reuse spritebatch and just stretch it to fit.
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // We need to stretch the image to fit the screen size. 
            spriteBatch.Draw(gameRenderTarget, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
