﻿using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TileEngine;

namespace NinjaGame
{
    static class EffectsManager
    {
        public const int MAX_PARTICLES = 1000;
        public const int MAX_BLOODS = 1000;
        public const int MAX_TEXT_PARTICLES = 10;

        private static GameObjectCircularBuffer Particles = new GameObjectCircularBuffer(MAX_PARTICLES);
        private static GameObjectCircularBuffer TextParticles = new GameObjectCircularBuffer(MAX_TEXT_PARTICLES);
        private static GameObjectCircularBuffer Bloods = new GameObjectCircularBuffer(MAX_BLOODS);

        static Texture2D SparkTexture;
        static Rectangle WhiteSquareSourceRectangle;

        public static void Initialize(ContentManager content)
        {
            var simpleSprites = content.Load<Texture2D>(@"Textures\SimpleSprites");
            SparkTexture = simpleSprites;
            WhiteSquareSourceRectangle = new Rectangle(2, 2, 2, 2).ToTileRect();
          
            Rectangle collisionRect = new Rectangle(2, 2, 1, 1);

            float drawDepth = TileMap.BLOOD_DRAW_DEPTH; // always above the map?

            //initialize blood
            for (int i = 0; i <= Bloods.Length - 1; i++)
            {
                var blood = new Blood();

                blood.DisplayComponent = new StaticImageDisplay(SparkTexture, WhiteSquareSourceRectangle);
                blood.DisplayComponent.TintColor = Color.Red;

                blood.Enabled = false;
                blood.CollisionRectangle = collisionRect;
                blood.DisplayComponent.DrawDepth = drawDepth;
                Bloods.SetItem(blood, i);
            }

            // initialize particles
            for (int i = 0; i < MAX_PARTICLES; i++)
            {
                var particle = GetEmptyParticle();
                particle.DisplayComponent = new StaticImageDisplay(SparkTexture, WhiteSquareSourceRectangle);
                Particles.SetItem(particle, i);
            }

            // Initialize text particles.
            for (int i = 0; i < MAX_TEXT_PARTICLES; i++)
            {
                var particle = GetEmptyParticle();
                particle.DisplayComponent = new TextDisplay("");
                TextParticles.SetItem(particle, i);
            }
        }

        private static Particle GetEmptyParticle()
        {
            var particle = new Particle(Vector2.Zero, Vector2.Zero, Vector2.Zero, 0f, 0, Color.White, Color.White);
            particle.Enabled = false;
            return particle;
        }

        public static Vector2 RandomDirection(float scale)
        {
            return RandomDirection(scale, scale);
        }

        public static Vector2 RandomDirection(float xScale, float yScale)
        {
            Vector2 direction;
            do
            {
                direction = new Vector2(
                    Game1.Randy.Next(0, 100) - 50,
                    Game1.Randy.Next(0, 100) - 50);
            } while (direction.Length() == 0);
            direction.Normalize();
            direction = new Vector2(direction.X * xScale, direction.Y * yScale);
            return direction;
        }

        static public void ClearEffects()
        {
            Bloods.Disable();
            Particles.Disable();
            TextParticles.Disable();
        }

        static public void Update(GameTime gameTime, float elapsed)
        {
            Particles.Update(gameTime, elapsed);
            TextParticles.Update(gameTime, elapsed);
            Bloods.Update(gameTime, elapsed);
        }

        static public void Draw(SpriteBatch spriteBatch)
        {
            Particles.Draw(spriteBatch);
            TextParticles.Draw(spriteBatch);
            Bloods.Draw(spriteBatch);
        }

        public static void EnemyPop(Vector2 location, int pointCount, Color color, float speed)
        {
            // Add some particles
            for (int x = 0; x < pointCount; x++)
            {
                var pop = (Particle)Particles.GetNextObject();

                pop.Initialize(
                        location,
                        RandomDirection(speed),
                        Vector2.Zero,
                        4000,
                        30,
                        color * 0.8f,
                        Color.White * 0.8f);
                pop.Scale = 0.4f;
                pop.FinalScale = 0.05f;
                pop.SetStaticImage(Game1.simpleSprites, WhiteSquareSourceRectangle);
                //pop.DrawDepth = Game1.CurrentMap.GetObjectDrawDepth(TileMap.DrawObjectGroups.Effects);
            }
        }

        public static void AddFadeOut(GameObject gameObject, int ticks = 20)
        {
            var particle = (Particle)Particles.GetNextObject();
            particle.Initialize(gameObject.WorldLocation, Vector2.Zero, Vector2.Zero, 0f, ticks, gameObject.DisplayComponent.TintColor, Color.Transparent);

            // Only works for static images, would be useful for other types as well
            var dc = gameObject.DisplayComponent as StaticImageDisplay;
            particle.SetStaticImage(dc.Texture, dc.Source);
            particle.Rotation = gameObject.Rotation;
            particle.Scale = gameObject.Scale;
            particle.FinalScale = 0.5f;
            //particle.DrawDepth = Game1.CurrentMap.GetObjectDrawDepth(TileMap.DrawObjectGroups.Effects);
            particle.RotationAndDrawOrigin = gameObject.RotationAndDrawOrigin;
        }

        public static void AddSparksEffect(Vector2 location, Vector2 impactVelocity, Color? color = null, float scale = 1f)
        {

            if (!color.HasValue)
            {
                color = Color.Yellow;
            }

            int particleCount = Game1.Randy.Next(10, 20);
            for (int x = 0; x < particleCount; x++)
            {
                var spark = (Particle)Particles.GetNextObject();

                // Add some whiteness to some particles
                var sparkColor = Color.Lerp(color.Value, Color.White, Game1.Randy.NextFloat() / 2);

                spark.Initialize(
                    location - (impactVelocity / 100),
                    RandomDirection((float)Game1.Randy.Next(10, 10 + (int)(10 * scale))),
                    Vector2.Zero,
                    100f,
                    70,
                    sparkColor,
                    Color.Transparent);

                spark.SetStaticImage(SparkTexture, WhiteSquareSourceRectangle);
                spark.Scale = scale;
                spark.Rotation = Game1.Randy.NextFloat() * MathHelper.TwoPi;
                //spark.DrawDepth = Game1.CurrentMap.GetObjectDrawDepth(TileMap.DrawObjectGroups.Effects);
            }
        }

        private static void addBloodEffects(int min, int max, Vector2 location, bool isUpOnly, float velocityMultiplier)
        {

            int particleCount = Game1.Randy.Next(min, max);

            for (int x = 0; x < particleCount; x++)
            {
                Vector2 velocity;
                if (isUpOnly)
                {
                    // make the spray more up than left and right.
                    velocity = RandomDirection((float)Game1.Randy.Next(50, 200), (float)Game1.Randy.Next(100, 300));
                    if (velocity.Y > 0)
                    {
                        velocity.Y = -velocity.Y;
                    }
                }
                else
                {
                    //a nice, even spread of blood.
                    velocity = RandomDirection((float)Game1.Randy.Next(100, 300));
                }

                velocity *= velocityMultiplier;
                var blood = (Blood)Bloods.GetNextObject();
                blood.Initialize(true, location, velocity);
            }
        }

        public static void AddBloodEffect(Vector2 location, bool isUpOnly = false, float velocityMultiplier = 1f)
        {
            addBloodEffects(10, 20, location, isUpOnly, velocityMultiplier);
        }

        public static void AddBigBloodEffect(Vector2 location, bool isUpOnly = false, float velocityMultiplier = 1f)
        {
            addBloodEffects(50, 100, location, isUpOnly, velocityMultiplier);
        }

        public static void AddHugeBloodEffect(Vector2 location, bool isUpOnly = false, float velocityMultiplier = 1f)
        {
            addBloodEffects(200, 300, location, isUpOnly, velocityMultiplier);
        }

        public static void AddBricks(Vector2 location, int pieceCount, Texture2D image, bool forceUp = false, float scale = 1f, Color color = default(Color))
        {
            AddBricks(location, pieceCount, image, image.BoundingRectangle(), forceUp, scale, color);
        }

        /// <summary>
        /// This method adds "bricks". it needs an animation strip and considers each frame a brick. It then creates an effect
        /// as if the brick was smashed. Or it could be the parts of an enemy exploding. 
        /// </summary>
        /// <param name="isTileProcessed">Pass in true if the tilesheet that we are bricking from has been procssessed with a 1px border around each tile</param>
        public static void AddBricks(Vector2 location, int pieceCount, Texture2D image, Rectangle imageSourceRect, bool forceUp = false, float scale = 1f, Color color = default(Color), bool isTileProcessed = true, float drawDepth = 0, int brickSourceWidth = TileMap.TileSize)
        {

            var effectDepth = drawDepth > 0 ? drawDepth : Game1.CurrentMap.GetObjectDrawDepth(TileMap.DrawObjectGroups.Effects);
            for (int i = 0; i <= pieceCount - 1; i++)
            {
                int tileOffset = Game1.Randy.Next(0, (imageSourceRect.Width / brickSourceWidth));

                int x = tileOffset * brickSourceWidth;
                if (isTileProcessed)
                {
                    // padding represents the 2px border between 48 by 48 tiles on a processed tile sheet.
                    x += tileOffset * 2;
                }

                var textureSourceRect = new Rectangle(x + imageSourceRect.X, imageSourceRect.Y, brickSourceWidth, imageSourceRect.Height);
                var singleBrickDrawDepth = effectDepth + (i * Game1.MIN_DRAW_INCREMENT);
                AddBrickPiece(location, image, textureSourceRect, singleBrickDrawDepth, forceUp, scale, float.MaxValue, false, float.MaxValue, color);
            }
        }

        public static void AddBrickPiece(Vector2 location, Texture2D image, Rectangle imageSourceRect, float drawDepth, bool forceUp = false, float scale = 1f, float rotation = float.MaxValue, bool flipped = false, float rotationsPerSecond = float.MaxValue, Color color = default(Color))
        {
            var piece = (Particle)Particles.GetNextObject();

            var maxDirection = (float)Game1.Randy.Next(200, 300);
            Vector2 direction = EffectsManager.RandomDirection(maxDirection);

            if (forceUp && direction.Y > 0)
            {
                direction.Y = -direction.Y;
            }
            else
            {
                // we're not going to force them all up, but it looks better if we influence them upwards.
                direction.Y -= maxDirection * 0.25f;
            }

            if (color == default(Color))
            {
                color = Color.White;
            }

            // Mess with the direction/velocity a bit so everything doesn't pan out in a perfect circle.
            var variance = (float)Game1.Randy.NextDouble();
            var speed = 0.75f + (0.5f * variance);
            var velocity = direction * speed;

            piece.Initialize(location,
                velocity,
                new Vector2(0, 6),
                10000,
                600,
                color,
                color);

            if (flipped)
            {
                piece.Flip();
            }
            piece.Scale = scale;
            if (rotationsPerSecond != float.MaxValue)
            {
                piece.RotationsPerSecond = rotationsPerSecond;
            }
            else
            {
                piece.RotationsPerSecond = (float)Game1.Randy.NextDouble() * 2 * MathHelper.TwoPi;
            }

            if (rotation != float.MaxValue)
            {
                piece.Rotation = rotation;
            }
            else
            {
                piece.Rotation = (float)Game1.Randy.NextDouble() * MathHelper.TwoPi;
            }

            piece.IsRotationClockwise = Game1.Randy.GetBool();
            piece.SetStaticImage(image, imageSourceRect);

            piece.DrawDepth = drawDepth;
        }

        public static void RisingText(string text, Vector2 location)
        {
            var textParticle = (Particle)TextParticles.GetNextObject();
            textParticle.DisplayComponent = new TextDisplay(text);

            textParticle.Initialize(location,
                new Vector2(0, -50),
                Vector2.Zero,
                10000,
                100,
                Color.White,
                Color.White * 0.3f);

            textParticle.InitialScale = 0.4f;
            textParticle.FinalScale = 0.8f;

            //textParticle.DrawDepth = TileMap.EFFECTS_DRAW_DEPTH;
        }
    }
}
