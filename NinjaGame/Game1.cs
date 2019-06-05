using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using TileEngine;

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
        public const float MIN_DRAW_INCREMENT = 0.0000005f;
        public static bool DrawAllCollisisonRects = false;

        public static Texture2D simpleSprites;
        public static Texture2D background;

        public static Rectangle whiteSourceRect = new Rectangle(1, 1, 1, 1).ToTileRect();

        public const int TileSize = TileEngine.TileMap.TileSize;

        private static RenderTarget2D gameRenderTarget;

        private static Player player;

        private static SceneManager sceneManager;

        private static Level currentLevel;

        public static TileMap CurrentMap
        {
            get
            {
                return currentLevel.Map;
            }
        }

        public static bool IS_DEBUG = true;

        public static Camera Camera;
        private static KeyboardState previousKeyState;

        public static SpriteFont Font;

        private TempTextObject tempText;
        private TempObject tempObject;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = GAME_X_RESOLUTION * 3;
            graphics.PreferredBackBufferHeight = GAME_Y_RESOLUTION * 3;
            Window.AllowUserResizing = true;
            Window.Title = "Ninja Game";
            //Window.

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            sceneManager = new SceneManager();
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

            simpleSprites = Content.Load<Texture2D>(@"Textures\SimpleSprites");
            background = Content.Load<Texture2D>(@"Textures\Background");

            var rpgSystem = Content.Load<SpriteFont>(@"Fonts\RPGSystem");
            Font = rpgSystem;

            player = new Player(Content);

            gameRenderTarget = new RenderTarget2D(GraphicsDevice, GAME_X_RESOLUTION, GAME_Y_RESOLUTION, false, SurfaceFormat.Color, DepthFormat.None);

            Camera = new Camera();

            // Load map and adjust Camera
            currentLevel = sceneManager.LoadLevel("TestLevel2", Content, player, Camera);
            
            Camera.Map = currentLevel.Map;

            // Basic Camera Setup
            Camera.Position = Vector2.Zero;
            Camera.Zoom = Camera.DEFAULT_ZOOM;
            Camera.ViewPortWidth = Game1.GAME_X_RESOLUTION;
            Camera.ViewPortHeight = Game1.GAME_Y_RESOLUTION;

            // Testing rotating text
            tempText = new TempTextObject(Content);
            tempText.WorldLocation = new Vector2(170, 170);
            tempObject = new TempObject(Content);
            tempObject.WorldLocation = new Vector2(210, 100);

            EffectsManager.Initialize(Content);

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

            var elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;

            currentLevel.Update(gameTime, elapsed);
            EffectsManager.Update(gameTime, elapsed);
            
            if (Game1.IS_DEBUG)
            {
                KeyboardState keyState = Keyboard.GetState();
                if (keyState.IsKeyDown(Keys.I))
                {
                    Camera.Zoom += 0.4f * elapsed;
                }
                else if (keyState.IsKeyDown(Keys.O))
                {
                    Camera.Zoom -= 0.4f * elapsed;
                }
                else if (keyState.IsKeyDown(Keys.R))
                {
                    Camera.Zoom = Camera.DEFAULT_ZOOM;
                }

                if (keyState.IsKeyDown(Keys.D) && !previousKeyState.IsKeyDown(Keys.D))
                {
                    Game1.DrawAllCollisisonRects = !Game1.DrawAllCollisisonRects;
                }
                previousKeyState = keyState;
            }

            tempText.Update(gameTime, elapsed);
            tempObject.Update(gameTime, elapsed);

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
 
            
            // We'll draw everything to gameRenderTarget, including the white render target.
            GraphicsDevice.SetRenderTarget(gameRenderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the background.
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointWrap, null, null);
            spriteBatch.Draw(background, Vector2.Zero, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.End();

            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;

            spriteBatch.Begin(SpriteSortMode.BackToFront,
                  BlendState.AlphaBlend,
                  SamplerState.PointClamp,
                  null,
                  null,
                  null,
                  cameraTransformation);

            currentLevel.Draw(spriteBatch, Camera.ScaledViewPort);

            EffectsManager.Draw(spriteBatch);

            tempText.Draw(spriteBatch);
            tempObject.Draw(spriteBatch);


            spriteBatch.End();

            // Switch back to drawing onto the back buffer. This is the default space in memory, the size is determined by the ClientWindow. 
            // When the present call is made, the backbuffer will show up as the new screen.
            GraphicsDevice.SetRenderTarget(null);

            // XNA draws a bright purple color to the backbuffer by default when we switch to it. Lame! Let's clear it out.
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Draw the gameRenderTarget with everything in it to the back buffer. We'll reuse spritebatch and just stretch it to fit.
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            // We need to stretch the image to fit the screen size. 
            spriteBatch.Draw(gameRenderTarget, new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);

            spriteBatch.End();


            base.Draw(gameTime);
        }
    }
}
