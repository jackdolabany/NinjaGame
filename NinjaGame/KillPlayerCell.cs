using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace NinjaGame
{
    /// <summary>
    /// Represents a tile that kills the player. Maybe spikes or a bottomless pit or something.
    /// </summary>
    public class KillPlayerCell : GameObject
    {

        private IEnumerable<Enemy> _enemies;

        public KillPlayerCell(ContentManager content, int cellX, int cellY, Player player, Camera camera, KillPlayer killPlayer, IEnumerable<Enemy> enemies)
            : base()
        {

            this._enemies = enemies;

            this.Enabled = true;

            this.DisplayComponent = new NoDisplay();

            this.WorldLocation = new Vector2(TileMap.TileSize * cellX, TileMap.TileSize * cellY);

            SetCenteredCollisionRectangle(16, 26);

            switch(killPlayer)
            {
                case KillPlayer.Full:
                    this.collisionRectangle = new Rectangle(0, 0, TileMap.TileSize, TileMap.TileSize);
                    break;
                case KillPlayer.Top:
                    this.collisionRectangle = new Rectangle(0, 0, TileMap.TileSize, TileMap.TileSize / 2);
                    break;
                case KillPlayer.Bottom:
                    this.collisionRectangle = new Rectangle(0, TileMap.TileSize / 2, TileMap.TileSize, TileMap.TileSize / 2);
                    break;
                case KillPlayer.Left:
                    this.collisionRectangle = new Rectangle(0, 0, TileMap.TileSize / 2, TileMap.TileSize);
                    break;
                case KillPlayer.Right:
                    this.collisionRectangle = new Rectangle(TileMap.TileSize / 2, 0, TileMap.TileSize / 2, TileMap.TileSize);
                    break;
                default:
                    throw new NotImplementedException("Unsupported KillPlayer value.");
            }

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

    }
}
