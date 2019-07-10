using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TileEngine;

namespace NinjaGame
{
    /// <summary>
    /// Represents a tile that kills the player. Maybe spikes or a bottomless pit or something.
    /// </summary>
    public class KillPlayerCell : Enemy
    {

        public KillPlayerCell(ContentManager content, int cellX, int cellY, Player player, Camera camera, KillPlayer killPlayer)
            : base(content, cellX, cellY, player, camera)
        {
            this.DisplayComponent = new NoDisplay();
         
            
            isEnemyTileColliding = false;
            Attack = 1;
            Health = 3;
            IsAffectedByGravity = false;

            IsAbleToMoveOutsideOfWorld = true;
            IsAffectedByForces = false;
            IsAffectedByPlatforms = false;
            IsCustomPlayerColliding = false;
            IsAbleToSurviveOutsideOfWorld = true;

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
                default:
                    throw new NotImplementedException("Unsupported KillPlayer value.");
            }

        }

        public override void Update(GameTime gameTime, float elapsed)
        {

        }

    }
}
