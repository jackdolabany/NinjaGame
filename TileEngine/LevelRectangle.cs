using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;

namespace TileEngine
{
    /// <summary>
    /// each level is assumed to be some kind of rectangle. This will help us
    /// draw a map for the player.
    /// </summary>
    public class LevelRectangle
    {
        public int LevelNumber { get; set; }
        public Rectangle LevelRect { get; set; }
        public List<LevelTile> Tiles { get; set; }

        public LevelRectangle()
        {
            Tiles = new List<LevelTile>();
        }
    }
}
