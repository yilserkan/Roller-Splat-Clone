using System;

namespace LevelSystem
{
   [Serializable]
   public class Level
   {
      public int Height;
      public int Width;
      public int Cycles;
      public int Seed;

      public Level(int height, int width, int cycles, int seed)
      {
         Height = height;
         Width = width;
         Cycles = cycles;
         Seed = seed;
      }
   }
}