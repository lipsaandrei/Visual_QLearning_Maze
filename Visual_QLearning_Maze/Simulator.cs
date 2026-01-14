using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Visual_QLearning_Maze
{
    public class Simulator
    {
        public MazeEnvironment Env;
        public QLearningAgent Agent;

        public int MaxStepsPerEpisode = 500;

        public Simulator(MazeEnvironment env, QLearningAgent agent)
        {
            Env = env;
            Agent = agent;
        }

        public double RunEpisode()
        {
            Env.Reset();
            double totalReward = 0.0;

            for(int step = 0; step < MaxStepsPerEpisode; ++step)
            {
                int state = Env.GetState();
                int action = Agent.ChooseAction(state);
                double reward = Env.Step(action, out int nextState);
                Agent.Update(state, action, reward, nextState);
                totalReward += reward;
                if (Env.IsGoal())
                    break;
            }

            return totalReward;
        }

        public void Train(int episodes)
        {
            for(int i = 0; i < episodes; ++i)
            {
                RunEpisode();
            }
        }
    }
}
