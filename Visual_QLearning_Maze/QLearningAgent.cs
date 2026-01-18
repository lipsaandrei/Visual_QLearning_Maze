using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_QLearning_Maze
{
    public class QLearningAgent
    {
        public int NumStates { get; }
        public int NumActions { get; }

        public double[,] Q;   // Q[state, action]

        private Random rnd = new Random();

        public double Alpha;   // learning rate
        public double Gamma;   // discount factor
        public double Epsilon; // exploration rate

        public QLearningAgent(int numStates, int numActions)
        {
            NumStates = numStates;
            NumActions = numActions;
            Q = new double[numStates, numActions];
        }

        // alege o actiune folosind ε-greedy
        public int ChooseAction(int state)
        {
            if (rnd.NextDouble() < Epsilon)
            {
                // explorare
                return rnd.Next(NumActions);
            }

            // exploatare
            double max = Q[state, 0];
            int best = 0;

            for (int a = 1; a < NumActions; a++)
            {
                if (Q[state, a] > max)
                {
                    max = Q[state, a];
                    best = a;
                }
            }

            return best;
        }

        // update Q-learning
        public void Update(int state, int action, double reward, int nextState)
        {
            double maxNext = Q[nextState, 0];
            for (int a = 1; a < NumActions; a++)
            {
                if (Q[nextState, a] > maxNext)
                    maxNext = Q[nextState, a];
            }

            // formula de update
            Q[state, action] =
                Q[state, action] +
                Alpha * (reward + Gamma * maxNext - Q[state, action]);
        }
    }
}
