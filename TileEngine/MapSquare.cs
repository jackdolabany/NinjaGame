using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TileEngine
{
    public class MapSquare
    {
        public Tile[] LayerTiles { get; set; }
        public string Door { get; set; }
        public bool Passable { get; set; }

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
            //LoadClasses = new List<string>();
            Door = "";
        }

        public MapSquare()
        {
        }
    }
}
