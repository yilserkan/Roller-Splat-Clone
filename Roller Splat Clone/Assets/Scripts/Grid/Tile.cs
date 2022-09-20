using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public enum Neighbors
    {
        Up,
        Down,
        Left,
        Right
    }
    
    public class Tile
    {
        public Vector2Int Coordinates;
        public bool IsBlocked;
        public Dictionary<Neighbors,Tile> Neigbors;

        public Tile(Vector2Int coordinates)
        {
            Coordinates = coordinates;
            IsBlocked = false;
            Neigbors = new Dictionary<Neighbors, Tile>()
            {
                { Neighbors.Up, null },
                { Neighbors.Down, null },
                { Neighbors.Left, null },
                { Neighbors.Right, null }
            };
        }
    }
}