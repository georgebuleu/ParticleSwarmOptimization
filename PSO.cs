
namespace SwarmOptimization;

public class Pso
{
    private static Random rand = new Random();
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
            for (int x = 0; x < Parameters.Dimension; x++)
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
        var globalBest = swarm[
            swarm.Select(p => p.Cost)
            .ToList()
            .IndexOf(swarm.Min(p => p.Cost))];

        for (var i = 0; i < Parameters.MaxIterations; i++)
        {
            foreach (var p in swarm)
            {
               var r1 = rand.NextDouble();
               var r2 = rand.NextDouble();
               for (int d = 0; d < Parameters.Dimension; d++)
               {
                   p.Velocity = Parameters.W * p.Velocity + Parameters.C1 * r1 * (p.PersonalBest.Position[d] - p.Position[d])
                       +Parameters.C2*r2*(globalBest.Position[d] - p.Position[d]);
                   p.Velocity = Limit(p.Velocity, -Parameters.MaxVelocity, Parameters.MaxVelocity);
               }

               for (int d = 0; d < Parameters.Dimension; d++)
               {
                   p.Position[d] += p.Velocity;
                   p.Position[d] = Limit(p.Position[d], Problem.Domain[d].Min, Problem.Domain[d].Max);
               }
               p.Cost = Problem.FitnessFunction(p.Position);
               
               if (p.Cost < p.PersonalBest.Cost)
               {
                   p.PersonalBest = p;
                   if (p.Cost < globalBest.Cost)
                   {
                          globalBest = p;
                   }
               }
            }
        }
        return globalBest;
    }
    
    public Particle OptimizeLbest()
    {
        var swarm = Init();

        for (var i = 0; i < Parameters.MaxIterations; i++)
        {
            foreach (var p in swarm)
            {
                var r1 = rand.NextDouble();
                var r2 = rand.NextDouble();
                var r3 = rand.NextDouble();
                var socialBestNeighbors = GetSocialBestNeighbor(swarm, p); 
                var geoBestNeighbors = GetGeographicalBestNeighbor(swarm, p); 
                
                for (int d = 0; d < Parameters.Dimension; d++)
                {
                        p.Velocity = Parameters.W * p.Velocity + Parameters.C1 * r1 * (p.PersonalBest.Position[d] - p.Position[d])
                                                               +Parameters.C2*r2*(socialBestNeighbors.Position[d] - p.Position[d]) + Parameters.C3*r3*(geoBestNeighbors.Position[d] - p.Position[d]);
                        
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

    private (int Index, Particle Value)[] GetSocialNeighbors(Particle[] swarm, Particle particle)
    {
        var x = Array.IndexOf(swarm, particle);
        var indexedArray = swarm.Select((value, index) => new { Index = index, Value = value });
        
        var sortedIndexes = indexedArray.OrderBy(item => Math.Abs(item.Index - x));
        
        return sortedIndexes
            .Skip(1)
            .Take(Parameters.SizeOfNeighborhood)
            .Select(item => (item.Index, item.Value))
            .ToArray();
    }
    private Particle GetSocialBestNeighbor(Particle[] swarm, Particle particle)
    {
        return GetSocialNeighbors(swarm, particle)
            .OrderBy(item => item.Value.Cost)
            .Select(item => item.Value)
            .First();
    }
    
    private Particle GetGeographicalBestNeighbor(Particle[] swarm, Particle particle)
    {
        return GetGeographicalNeighbors(swarm, particle)
            .OrderBy(item => item.Value.Cost)
            .Select(item => item.Value)
            .First();
    }
    
    private double GenerateRandomNumber(double min, double max)
    {
        return rand.NextDouble() * (max - min) + min;
    }
    
    private (int Index, Particle Value )[] GetGeographicalNeighbors(Particle[] swarm, Particle particle)
    {
        var x = Array.IndexOf(swarm, particle);
        var indexedArray = swarm.Select((value, index) => new { Index = index, Value = value });
        var sortedDistances = indexedArray.OrderBy(item => GetDistance(item.Value, swarm[x]));
        return sortedDistances
            .Where(item => item.Index != x)
            .Take(Parameters.SizeOfNeighborhood)
            .Select(item => (item.Index, item.Value))
            .ToArray();
    } 
    
    private double GetDistance(Particle p1, Particle p2)
    {
        var sum = 0.0;
        for (int i = 0; i < Parameters.Dimension; i++)
        {
            sum += Math.Pow(p1.Position[i] - p2.Position[i], 2);
        }

        return Math.Sqrt(sum);
    }
    
}
