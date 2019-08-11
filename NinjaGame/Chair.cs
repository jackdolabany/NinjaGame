using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TileEngine;

namespace NinjaGame
{
    public class Chair : GameObject
    {

        private Player _player;
        private IEnumerable<Enemy> _enemies;
        private bool isMoving = false;

        public Chair(ContentManager content, int x, int y, Player player, IEnumerable<Enemy> enemies)
        {
            var ChairImage = content.Load<Texture2D>(@"Textures\Chair");
            this.DisplayComponent = new StaticImageDisplay(ChairImage);
            _player = player;
            _enemies = enemies;
            
            this.WorldLocation = new Vector2(x * TileMap.TileSize, y * TileMap.TileSize);

            SetCenteredCollisionRectangle(12, 20);

            this.Enabled = true;
            this.IsAffectedByGravity = true;
            this.isEnemyTileColliding = false;
        }

        public override void Update(GameTime gameTime, float elapsed)
        {
            base.Update(gameTime, elapsed);

            if (Enabled)
            {
                if (!isMoving)
                {
                    if (_player.AttackRectangle.Intersects(this.CollisionRectangle))
                    {
                        isMoving = true;
                        this.velocity = new Vector2(300, 0);
                        if (_player.IsFacingLeft())
                        {
                            this.velocity *= -1;
                        }
                    }
                }
                else
                {
                    // Check for enemy collisions
                    foreach (var enemy in _enemies)
                    {
                        if (enemy.Enabled && this.CollisionRectangle.Intersects(enemy.CollisionRectangle))
                        {
                            enemy.ForceVelocity += this.velocity;
                            this.Enabled = false;
                        }
                    }

                    if (OnLeftWall || OnRightWall)
                    {
                        this.Enabled = false;
                    }
                }
            }
        }

    }
}
