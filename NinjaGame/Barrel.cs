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
    public class Barrel : GameObject
    {

        private Player _player;
        private IEnumerable<Enemy> _enemies;
        private bool isMoving = false;

        public Barrel(ContentManager content, int x, int y, Player player, IEnumerable<Enemy> enemies)
        {
            var barrelImage = content.Load<Texture2D>(@"Textures\Barrel");
            this.DisplayComponent = new StaticImageDisplay(barrelImage);
            _player = player;
            _enemies = enemies;

           

            this.WorldLocation = new Vector2(x * TileMap.TileSize, y * TileMap.TileSize);
            this.CollisionRectangle = new Rectangle(2, 1, 12, 15);

            SetCenteredCollisionRectangle(12, 15);

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
                            enemy.TakeHit(1, this.velocity);
                            this.Enabled = false;
                        }
                    }

                    // Check for wall collisions
                    //var currentMapSquare = Game1.CurrentMap?.GetMapSquareAtPixel(this.WorldCenter);
                    //if (currentMapSquare == null || !currentMapSquare.Passable)
                    //{
                    //    this.Enabled = false;
                    //}
                    if (OnLeftWall || OnRightWall)
                    {
                        this.Enabled = false;
                    }
                }
            }
        }

    }
}
