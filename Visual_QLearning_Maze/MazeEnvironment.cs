using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Visual_QLearning_Maze
{
    public class MazeEnvironment
    {
        public int Width { get; }
        public int Height { get; }

        // 0 = liber, 1 = perete, 2 = start, 3 = goal
        private int[,] maze;

        public int StartX = 0;
        public int StartY = 0;

        public int AgentX { get; private set; }
        public int AgentY { get; private set; }

        public MazeEnvironment(int[,] maze)
        {
            this.maze = maze;
            Height = maze.GetLength(0);
            Width = maze.GetLength(1);

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (maze[y, x] == 2)
                    {
                        StartX = x;
                        StartY = y;
                        break;
                    }
                }
            }

            Reset();
        }

        public void Reset()
        {
            AgentX = StartX;
            AgentY = StartY;
        }

        public int GetState()
        {
            return AgentY * Width + AgentX;
        }

        public bool IsGoal()
        {
            return maze[AgentY, AgentX] == 3;
        }

        public double Step(int action, out int nextState)
        {
            int newX = AgentX;
            int newY = AgentY;

            switch (action)
            {
                case 0: newY--; break; // sus
                case 1: newX++; break; // dreapta
                case 2: newY++; break; // jos
                case 3: newX--; break; // stanga
            }

            // verficare coliziune
            if (newX < 0 || newX >= Width || newY < 0 || newY >= Height || maze[newY, newX] == 1)
            {
                nextState = GetState();
                return -10; // penalizare
            }

            // mutare agent
            AgentX = newX;
            AgentY = newY;

            if (maze[newY, newX] == 3)
            {
                nextState = GetState();
                return 100; // recompensa pentru atingerea scopului
            }

            nextState = GetState();
            return -1; // pas normal
        }
    }
}
