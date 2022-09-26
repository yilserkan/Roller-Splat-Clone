using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
public class Directions
{
    public Direction GetRandomDirection(Direction currentDirection,ref bool isStartTileInitialized)
    {
        if (!isStartTileInitialized)
        {
            int directionCount = Enum.GetNames(typeof(Direction)).Length;
            int randomDir = Random.Range(0, directionCount);
            Direction randomDirection = (Direction)randomDir;
            isStartTileInitialized = true;
            return randomDirection;
        }
            
        if (currentDirection == Direction.Down || currentDirection == Direction.Up)
        {
            return GetRandomDirFromOtherAxis(Direction.Up);
        }
       
        return GetRandomDirFromOtherAxis(Direction.Left);
    }
    
    public Direction GetRandomDirFromOtherAxis(Direction dir)
    {
        Direction nb;
        if (dir == Direction.Up || dir == Direction.Down)
        {     
            int ran = Random.Range(2, 4);
            nb = (Direction)ran;
        }
        else 
        {
            int ran = Random.Range(0, 2);
            nb = (Direction)ran;
        }

        return nb;
    }
    
    public Direction FindOppositeDirection(Direction dir)
    {
        Direction nb = Direction.Down;
        switch (dir)
        {
            case Direction.Up:
                nb = Direction.Down;
                break;
            case Direction.Down:
                nb =Direction.Up;
                break;
            case Direction.Left:
                nb = Direction.Right;
                break;
            case Direction.Right:
                nb = Direction.Left;
                break;
        }

        return nb;
    }

}
