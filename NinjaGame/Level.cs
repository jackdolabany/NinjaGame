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
        public List<KillPlayerCell> killPlayerCells;
        public List<GameObject> GameObjects;

        public Level(Player player, TileMap map, Camera camera)
        {
            Player = player;
            Map = map;
            Camera = camera;
            Enemies = new List<Enemy>();
            killPlayerCells = new List<KillPlayerCell>();
            GameObjects = new List<GameObject>();
        }

        public void Update(GameTime gameTime, float elapsed)
        {
            Player.Update(gameTime, elapsed);
            Camera.Position = Player.WorldLocation;
            foreach (var enemy in Enemies)
            {
                enemy.Update(gameTime, elapsed);
            }

            foreach (var gameObject in GameObjects)
            {
                gameObject.Update(gameTime, elapsed);
            }

            // Check collisions
            if (Player.Enabled)
            {
                foreach (var enemy in Enemies)
                {
                    if (enemy.Alive)
                    {
                        Player.CheckEnemyInteractions(enemy);
                    }
                }
            }

            // Check for spikes and other kill player cells
            foreach (var killPlayerCell in killPlayerCells)
            {
                if (killPlayerCell.Enabled)
                {
                    foreach (var enemy in Enemies)
                    {
                        if (enemy.Enabled && enemy.CollisionRectangle.Intersects(killPlayerCell.CollisionRectangle))
                        {
                            enemy.Kill();
                        }
                    }
                    if (Player.Enabled && Player.CollisionRectangle.Intersects(killPlayerCell.CollisionRectangle))
                    {
                        Player.Kill();
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

            foreach (var gameObject in GameObjects)
            {
                gameObject.Draw(spriteBatch);
            }

            foreach (var killPlayerCell in killPlayerCells)
            {
                killPlayerCell.Draw(spriteBatch);
            }
        }
    }
}
