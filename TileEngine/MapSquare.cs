using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
    /// <summary>
    /// Represents all of the tiles in a grid location in the map. 
    /// </summary>
    public class MapSquare
    {
        public Tile[] LayerTiles { get; set; }
        public bool Passable { get; set; }

        /// <summary>
        /// Tiles that should kill the player instantly. 
        /// </summary>
        public KillPlayer KillPlayer { get; set; }

        /// <summary>
        /// Only for the WorldMap. Each square represents part of a level.
        /// </summary>
        public int LevelNumber { get; set; }

        private bool _enemyPassable = true;
        public bool EnemyPassable
        {
            get
            {
                return Passable && _enemyPassable;
            }
            set
            {
                _enemyPassable = value;
            }
        }

        private bool _platformPassable = true;
        public bool PlatformPassable
        {
            get
            {
                return Passable && _platformPassable;
            }
            set
            {
                _platformPassable = value;
            }
        }

        public MapSquare(int depth, bool passable)
        {
            LayerTiles = new Tile[depth];
            Passable = passable;
        }

        public MapSquare()
        {
        }
    }
}
