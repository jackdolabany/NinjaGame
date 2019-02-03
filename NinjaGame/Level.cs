﻿using Microsoft.Xna.Framework;
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

        public Level(Player player, TileMap map, Camera camera)
        {
            Player = player;
            Map = map;
            Camera = camera;
        }

        public void Update(GameTime gameTime, float elapsed)
        {
            Player.Update(gameTime, elapsed);
            Camera.Position = Player.WorldLocation;
        }

        public void Draw(SpriteBatch spriteBatch, Rectangle scaledViewPort)
        {
            Player.Draw(spriteBatch);
            Map.Draw(spriteBatch, scaledViewPort);
        }
    }
}