
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
                //print the position of the particle and it's number
                //Console.WriteLine($"Particle {i} position: {p.Position[x]}");
                
            }
            p.Cost = Problem.FitnessFunction(p.Position);
            p.Velocity = new double[] { 0.0, 0.0 };
            p.PersonalBest = p;
            swarm[i] = p;
        }
        return swarm;
    }
    
    private double Limit(double val, double min, double max)
    {
        return val < min ? min : (val > max ? max : val);
    }

    public Solution OptimizeGbest()
    {
        var swarm = Init();
        var positions = InitPositionsArray(swarm);
        var globalBest = swarm[
            swarm.Select(p => p.Cost)
            .ToList()
            .IndexOf(swarm.Min(p => p.Cost))];

        for (var i = 0; i < Parameters.MaxIterations; i++)
        {
            for (var j = 0; j < Parameters.NumberOfParticles; j++)
            { 
                var r1 = GenerateRandomR();
                var r2 = GenerateRandomR();
               for (int d = 0; d < Parameters.Dimension; d++)
               {
                   
                   swarm[j].Velocity[d] = Parameters.W * swarm[j].Velocity[d] + Parameters.C1 * r1 * (swarm[j].PersonalBest.Position[d] - swarm[j].Position[d])
                       +Parameters.C2*r2*(globalBest.Position[d] - swarm[j].Position[d]);
                   swarm[j].Velocity[d] = Limit(swarm[j].Velocity[d], -Parameters.MaxVelocity, Parameters.MaxVelocity);
               }

               for (int d = 0; d < Parameters.Dimension; d++)
               {
                   
                   swarm[j].Position[d] += swarm[j].Velocity[d];
                   swarm[j].Position[d] = Limit(swarm[j].Position[d], Problem.Domain[d].Min, Problem.Domain[d].Max);
                   positions[i+1][j][d] = swarm[j].Position[d];
               }
               swarm[j].Cost = Problem.FitnessFunction(swarm[j].Position);
               
               if (swarm[j].Cost < swarm[j].PersonalBest.Cost)
               {
                   swarm[j].PersonalBest = swarm[j];
                   if (swarm[j].Cost < globalBest.Cost)
                   {
                          globalBest = swarm[j];
                   }
               }
            }
        }
         
        return new Solution(globalBest, positions, SolutionType.GBEST.ToString());
    }
    
    public Solution OptimizeLbest()
    {
        var swarm = Init();
        var positions = InitPositionsArray(swarm);

        for (var i = 0; i < Parameters.MaxIterations; i++)
        {
            for (int j = 0; j < Parameters.NumberOfParticles; j++)
            {
                var r1 = rand.NextDouble();
                var r2 = rand.NextDouble();
                var r3 = rand.NextDouble();
                var socialBestNeighbors = GetSocialBestNeighbor(swarm, swarm[j]); 
                var geoBestNeighbors = GetGeographicalBestNeighbor(swarm, swarm[j]); 
                
                for (int d = 0; d < Parameters.Dimension; d++)
                {
                        swarm[j].Velocity[d] = Parameters.W * swarm[j].Velocity[d] + Parameters.C1 * r1 * (swarm[j].PersonalBest.Position[d] - swarm[j].Position[d])
                                                               +Parameters.C2*r2*(socialBestNeighbors.Position[d] - swarm[j].Position[d]) + Parameters.C3*r3*(geoBestNeighbors.Position[d] - swarm[j].Position[d]);
                    swarm[j].Velocity[d] = Limit(swarm[j].Velocity[d], -Parameters.MaxVelocity, Parameters.MaxVelocity);
                }

                for (int d = 0; d < Problem.Domain.Length; d++)
                {
                    swarm[j].Position[d] += swarm[j].Velocity[d];
                    swarm[j].Position[d] = Limit(swarm[j].Position[d], Problem.Domain[d].Min, Problem.Domain[d].Max);
                    positions[i+1][j][d] = swarm[j].Position[d];
                }
                swarm[j].Cost = Problem.FitnessFunction(swarm[j].Position);
               
                if (swarm[j].Cost < swarm[j].PersonalBest.Cost)
                {
                    swarm[j].PersonalBest = swarm[j];
                }
            }
        }
        var bestParticle = swarm.OrderBy(p => p.Cost).First();

        return new Solution(bestParticle, positions, SolutionType.LBEST.ToString());
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

    private double GenerateRandomR()
    {
        var r = rand.NextDouble();
        
        while (r == 0.0)
        {
            r = rand.NextDouble();
        }

        return r;
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

    private double[][][] InitPositionsArray(Particle[] swarm)
    {
        var positions = new double[Parameters.MaxIterations + 1][][];
        
        
        for (int i = 0; i < Parameters.MaxIterations + 1; i++)
        { 
            positions[i] = new double[Parameters.NumberOfParticles][];
                for (int j = 0; j < Parameters.NumberOfParticles; j++)
                {
                    positions[i][j] = new double[Parameters.Dimension];
                    if (i == 0)
                    {
                        for (int d = 0; d < Parameters.Dimension; d++)
                        {
                            positions[i][j][d] = swarm[j].Position[d];
                        }
                    }
                }
        }
        return positions;
    }
}
