
namespace SwarmOptimization;

public class Pso
{
    private static Random _rand = new Random();
    private Problem Problem{get; set;}
    public Pso(Problem problem)
    {
        this.Problem = problem;
    }
    private Particle[] Init()
    {
        var swarm = new Particle[Parameters.NumberOfParticles];
        for (var i = 0; i < Parameters.NumberOfParticles; i++)
        {
            var p = new Particle();
            for (int x = 0; x < Parameters.SizeOfProblem; x++)
            {
                var xMax = Problem.Domain[x].Max;
                var xMin = Problem.Domain[x].Min;
                p.Position[x] = GenerateRandomNumber(xMin, xMax);
            }
            p.Cost = Problem.FitnessFunction(p.Position);
            p.Velocity = 0;
            p.PersonalBest = p;
            swarm[i] = p;
        }

        return swarm;
    }
    
    private double Limit(double val, double min, double max)
    {
        return Math.Min(Math.Max(val, min), max);
    }

    public Particle OptimizeGbest()
    {
        var swarm = Init();
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

               for (int d = 0; d < Problem.Domain.Length; d++)
               {
                   p.Position[d] += p.Velocity;
                   p.Position[d] = Limit(p.Position[d], Problem.Domain[d].Min, Problem.Domain[d].Max);
               }
               p.Cost = Problem.FitnessFunction(p.Position);
               
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
    
    public Particle OptimizeLbest()
    {
        var swarm = Init();
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

                for (int d = 0; d < Problem.Domain.Length; d++)
                {
                    p.Position[d] += p.Velocity;
                    p.Position[d] = Limit(p.Position[d], Problem.Domain[d].Min, Problem.Domain[d].Max);
                }
                p.Cost = Problem.FitnessFunction(p.Position);
               
                if (p.Cost < p.PersonalBest.Cost)
                {
                    p.PersonalBest = p;
                }
            }
        }

        return swarm.OrderBy(p => p.Cost).First();
    }

    private Particle[] GetNeighbors(Particle[] swarm, Particle particle)
    {
        var x = Array.IndexOf(swarm, particle);
        var indexedArray = swarm.Select((value, index) => new { Index = index, Value = value });
        
        var sortedIndexes = indexedArray.OrderBy(item => Math.Abs(item.Index - x));
        
        var closestIndexes = sortedIndexes.Skip(1).Take(Parameters.SizeOfNeighborhood).Select(item => item.Value).ToArray();

        return closestIndexes;
    }
    private Particle GetBestNeighbor(Particle[] swarm, Particle particle)
    {
        return GetNeighbors(swarm, particle).OrderBy(p => p.Cost).First();
    }
    
    private double GenerateRandomNumber(double min, double max)
    {
        return _rand.NextDouble() * (max - min) + min;
    }
    
}
