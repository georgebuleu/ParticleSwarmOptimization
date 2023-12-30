using System.Collections;
using System.ComponentModel;

namespace SwarmOptimization;

public class PSO
{
    private static Particle[] Init(Problem problem)
    {
        var swarm = new Particle[Parameters.NumberOfParticles];
        var rand = new Random();
        for (var i = 0; i < Parameters.NumberOfParticles; i++)
        {
            var p = new Particle
            {
            Position = Enumerable.Range(0, Parameters.SizeOfProblem - 1)
                .Select(x => rand.NextDouble() * (problem.Domain[x].Max - problem.Domain[x].Min) + problem.Domain[x].Min)
                .ToArray(),
            };
            p.Cost = problem.FitnessFunction(p.Position);
            p.Velocity = 0;
            p.PersonalBest = p;
            swarm[i] = p;
        }

        return swarm;
    }
    
    private static double Limit(double val, double min, double max)
    {
        return Math.Min(Math.Max(val, min), max);
    }

    public static Particle OptimizeGbest(Problem problem)
    {
        var swarm = Init(problem);
        var rand = new Random();
        var socialBest = swarm[
            swarm.Select(p => p.Cost)
            .ToList()
            .IndexOf(swarm.Max(p => p.Cost))];

        for (var i = 0; i < Parameters.MaxIterations; i++)
        {
            foreach (var p in swarm)
            {
               var r1 = rand.NextDouble();
               var r2 = rand.NextDouble();
               for (int d = 0; d < Parameters.SizeOfProblem; d++)
               {
                   p.Velocity = Parameters.W * p.Velocity + Parameters.C1 * r1 * (p.PersonalBest.Position[d] - p.Position[d])
                       +Parameters.C2*r2*(socialBest.Position[d] - p.Position[d]);
                   p.Velocity = Limit(p.Velocity, -Parameters.MaxVelocity, Parameters.MaxVelocity);
               }

               for (int d = 0; d < problem.Domain.Length; d++)
               {
                   p.Position[d] += p.Velocity;
                   p.Position[d] = Limit(p.Position[d], problem.Domain[d].Min, problem.Domain[d].Max);
               }
               p.Cost = problem.FitnessFunction(p.Position);
               
               if (p.Cost < p.PersonalBest.Cost)
               {
                   p.PersonalBest = p;
                   if (p.Cost < socialBest.Cost)
                   {
                          socialBest = p;
                   }
               }
            }
        }
        return socialBest;
    }
    
    public static Particle OptimizeLbest(Problem problem)
    {
        var swarm = Init(problem);
        var rand = new Random();

        for (var i = 0; i < Parameters.MaxIterations; i++)
        {
            foreach (var p in swarm)
            {
                var r1 = rand.NextDouble();
                var r2 = rand.NextDouble();
                var socialBest = GetBestNeighbor(swarm, p);
                for (int d = 0; d < Parameters.SizeOfProblem; d++)
                {
                    p.Velocity = Parameters.W * p.Velocity + Parameters.C1 * r1 * (p.PersonalBest.Position[d] - p.Position[d])
                                                           +Parameters.C2*r2*(socialBest.Position[d] - p.Position[d]);
                    p.Velocity = Limit(p.Velocity, -Parameters.MaxVelocity, Parameters.MaxVelocity);
                }

                for (int d = 0; d < problem.Domain.Length; d++)
                {
                    p.Position[d] += p.Velocity;
                    p.Position[d] = Limit(p.Position[d], problem.Domain[d].Min, problem.Domain[d].Max);
                }
                p.Cost = problem.FitnessFunction(p.Position);
               
                if (p.Cost < p.PersonalBest.Cost)
                {
                    p.PersonalBest = p;
                }
            }
        }

        return swarm.OrderBy(p => p.Cost).First();
    }

    private static Particle[] GetNeighbors(Particle[] swarm, Particle particle)
    {
        var x = Array.IndexOf(swarm, particle);
        var indexedArray = swarm.Select((value, index) => new { Index = index, Value = value });
        
        var sortedIndexes = indexedArray.OrderBy(item => Math.Abs(item.Index - x));
        
        var closestIndexes = sortedIndexes.Skip(1).Take(Parameters.SizeOfNeighborhood).Select(item => item.Value).ToArray();

        return closestIndexes;
    }
    private static Particle GetBestNeighbor(Particle[] swarm, Particle particle)
    {
        return GetNeighbors(swarm, particle).OrderBy(p => p.Cost).First();
    }
    
}
