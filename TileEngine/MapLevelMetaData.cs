using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TileEngine
{
    /// <summary>
    /// Some metadata about a level that's been seen or otherwise.
    /// </summary>
    public class MapLevelMetaData
    {
        public int LevelNumber { get; set; }
        const int TilesPerBlock = 12;

        /// <summary>
        /// Did this level ever have a coin?
        /// </summary>
        public bool HasCoin { get; set; }

        public List<KioskLocation> Kiosks { get; set; }
        
        public List<DoorLocation> Doors { get; set; }

        public MapEnvironment Environment { get; set; }

        public MapLevelMetaData()
        {
            Kiosks = new List<KioskLocation>();
            Doors = new List<DoorLocation>();
        }

        public MapLevelMetaData(int levelNumber, TileMap map)
        {
            LevelNumber = levelNumber;
            Environment = map.MapEnvironment;
            HasCoin = false;
            Kiosks = new List<KioskLocation>();

            var doorsInThisLevel = new List<DoorLocation>();

            int right = (map.MapCells.Length - 1) / TilesPerBlock;
            int bottom = (map.MapCells[0].Length - 1) / TilesPerBlock;

            for (int x = 0; x < map.MapCells.Length; x++)
            {
                for (int y = 0; y < map.MapCells[x].Length; y++)
                {

                    // Check doors on left
                    if (x == 0)
                    {
                        if (map.MapCells[x][y].Passable)
                        {
                            FindOrAddDoor(doorsInThisLevel, map, x, y, DoorDirection.Left);
                        }
                    }

                    // Check doors on right
                    if (x == map.MapCells.Length - 1)
                    {
                        if (map.MapCells[x][y].Passable)
                        {
                            FindOrAddDoor(doorsInThisLevel, map, x, y, DoorDirection.Right);
                        }
                    }
                    
                    // Check doors on top
                    if (y == 0)
                    {
                        if (map.MapCells[x][y].Passable)
                        {
                            FindOrAddDoor(doorsInThisLevel, map, x, y, DoorDirection.Up);
                        }
                    }

                    // Check doors on bottom
                    if (y == map.MapCells[x].Length - 1)
                    {
                        if (map.MapCells[x][y].Passable)
                        {
                            FindOrAddDoor(doorsInThisLevel, map, x, y, DoorDirection.Down);
                        }
                    }

                    // Search for Kiosks and coins
                    foreach (var layer in map.MapCells[x][y].LayerTiles)
                    {
                        if (layer.LoadClass != null)
                        {
                            var className = layer.LoadClass.ToLower();

                            if(className.StartsWith("kiosk."))
                            {
                                var kiosk = new KioskLocation();
                                kiosk.KioskType = GetKioskTypeFromLoadClass(layer.LoadClass);
                                kiosk.X = x;
                                kiosk.Y = y;
                                Kiosks.Add(kiosk);
                            }
                            else if (className == "gameobject.coin")
                            {
                                HasCoin = true;
                            }

                        }
                    }
                } // y
            } // x

            Doors = doorsInThisLevel;
        }

        public static void FindOrAddDoor(List<DoorLocation> doors, TileEngine.TileMap map, int mapX, int mapY, DoorDirection direction)
        {
            int doorX = mapX / TilesPerBlock;
            int doorY = mapY / TilesPerBlock;

            var matchedDoors = doors.Where(d => d.X == doorX && d.Y == doorY && d.DoorDirection == direction);

            DoorLocation door;
            if(matchedDoors.Any())
            {
                // there should only be one
                door = matchedDoors.Single();
            }
            else
            {
                // Add a new door for this one
                door = new DoorLocation();
                door.X = doorX;
                door.Y = doorY;
                door.DoorDirection = direction;
                doors.Add(door);
            }
        }

        public KioskType GetKioskTypeFromLoadClass(string loadClass)
        {
            if (loadClass.EndsWith("Info")) return KioskType.Info;
            if (loadClass.EndsWith("Save")) return KioskType.Save;
            if (loadClass.EndsWith("Power")) return KioskType.Health;
            if (loadClass.EndsWith("Transport")) return KioskType.Teleport;
            throw new Exception("Unhandled Kiosk type");
        }

        public Color Color
        {
            get
            {
                switch ((MapEnvironment)Environment)
                {
                    case MapEnvironment.SpaceShip:
                        return new Color(0.1f, 0.1f, 0.1f);
                    case MapEnvironment.GreenAlien:
                        return Color.ForestGreen;
                    case MapEnvironment.GreenBrick:
                        return Color.DarkGreen;
                    case MapEnvironment.Mountain:
                        return Color.Blue;
                    case MapEnvironment.PurpleAlien:
                        return Color.Purple;
                    case MapEnvironment.RedRock:
                        return Color.Red;
                    case MapEnvironment.RedCaves:
                        return Color.DarkGoldenrod;
                    default:
                        throw new Exception("Unhandled environment");
                }
            }
        }

    }

    public enum KioskType: int
    {
        Info, Save, Health, Teleport
    }

    /// <summary>
    /// Has the location of a kiosk suitable for displaying on the map.
    /// </summary>
    //[Serializable]
    public struct KioskLocation
    {
        public KioskType KioskType;
        public int X;
        public int Y;
    }

    public enum DoorDirection : int
    {
        Up, Down, Left, Right
    }

    //[Serializable]
    public class DoorLocation
    {
        public int X;
        public int Y;
        public DoorDirection DoorDirection;
    }
}
