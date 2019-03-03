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
    /// <summary>
    /// Represents a level in the game with a map, and GameObjects, and any state variables as the game goes on. 
    /// </summary>
    public class Level
    {

        public Player Player;
        public TileMap Map;
        public Camera Camera;
        public List<Enemy> Enemies;

        public Level(Player player, TileMap map, Camera camera)
        {
            Player = player;
            Map = map;
            Camera = camera;
            Enemies = new List<Enemy>();
        }

        public void Update(GameTime gameTime, float elapsed)
        {
            Player.Update(gameTime, elapsed);
            Camera.Position = Player.WorldLocation;
            foreach (var enemy in Enemies)
            {
                enemy.Update(gameTime, elapsed);
            }

            // Check collisions
            if (Player.Enabled)
            {
                foreach (var enemy in Enemies)
                {
                    if (enemy.Alive && Player.CollisionRectangle.Intersects(enemy.CollisionRectangle))
                    {

                        if (Math.Abs(Player.CollisionRectangle.Bottom - enemy.CollisionRectangle.Top) <= 6 && Player.CollisionRectangle.Right > enemy.CollisionRectangle.Left && Player.CollisionRectangle.Left < enemy.CollisionRectangle.Right)
                        {
                            // kill the enemy
                            enemy.Kill();
                        }
                        else
                        {
                            // kill the player
                            Player.Enabled = false;
                            EffectsManager.AddBigBloodEffect(Player.WorldCenter);
                            EffectsManager.RisingText("Dead", Player.WorldCenter);
                            EffectsManager.EnemyPop(Player.WorldCenter, 10, Color.Red, 50f);
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle scaledViewPort)
        {
            Player.Draw(spriteBatch);
            Map.Draw(spriteBatch, scaledViewPort);
            foreach (var enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
        }
    }
}
