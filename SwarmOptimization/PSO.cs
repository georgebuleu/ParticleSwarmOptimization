using System.ComponentModel;

namespace SwarmOptimization;

public class PSO
{
    
    public static Particle[] Init(Problem problem, Parameter parameters)
    {
        var swarm = new Particle[parameters.NumberOfParticles];
        var rand = new Random();
        for (var i = 0; i < parameters.NumberOfParticles; i++)
        {
            var p = new Particle
            {
            Position = Enumerable.Range(0, parameters.SizeOfProblem - 1)
                .Select(x => rand.NextDouble() * (problem.Domain[x].Max - problem.Domain[x].Min) + problem.Domain[x].Min)
                .ToArray(),
            };
            p.Cost = problem.ObjectiveFunction(p.Position);
            p.Velocity = 0;
            p.PersonalBest = p;
            swarm[i] = p;
        }

        return swarm;
    }
    
    public static double Limit(double val, double min, double max)
    {
        return Math.Min(Math.Max(val, min), max);
    }

    public static Particle Optimize(Problem problem, Parameter parameters)
    {
        var swarm = Init(problem, parameters);
        var rand = new Random();
        var socialBest = swarm[
            swarm.Select(p => p.Cost)
            .ToList()
            .IndexOf(swarm.Max(p => p.Cost))];

        for (var i = 0; i < parameters.MaxIterations; i++)
        {
            foreach (var p in swarm)
            {
                // rand * ((1 - 0) + 0)
               var r1 = rand.NextDouble();
               var r2 = rand.NextDouble();
               for (int d = 0; d < parameters.SizeOfProblem; d++)
               {
                   p.Velocity = parameters.W * p.Velocity + parameters.C1 * r1 * (p.PersonalBest.Position[d] - p.Position[d])
                       +parameters.C2*r2*(socialBest.Position[d] - p.Position[d]);
                   p.Velocity = Limit(p.Velocity, -parameters.MaxVelocity, parameters.MaxVelocity);
               }

               for (int d = 0; d < problem.Domain.Length; d++)
               {
                   p.Position[d] += p.Velocity;
                   p.Position[d] = Limit(p.Position[d], problem.Domain[d].Min, problem.Domain[d].Max);
               }
               p.Cost = problem.ObjectiveFunction(p.Position);
               
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
}