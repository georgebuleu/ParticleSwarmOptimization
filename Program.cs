using System;

namespace SwarmOptimization;
class Program
{
    static void Main(string[] args)
    {
        var problem = new Problem(
            x =>  (1)*(-20 * Math.Exp(-0.2 * Math.Sqrt(0.5 * (x[0] * x[0] + x[1] * x[1])))
                 - Math.Exp(0.5 * (Math.Cos(2 * Math.PI * x[0]) + Math.Cos(2 * Math.PI * x[1])))
                 + Math.E + 20),
            new (double Min, double Max)[] { (-5, 5), (-5, 5) }
        );
        var alpha = 0.1;
        //set parameters
        Parameters.MaxVelocity = alpha * (problem.Domain[0].Max - problem.Domain[0].Min);
        var pso = new Pso(problem);
        var p = pso.OptimizeGbest();
        Console.WriteLine($"Best solution found using GBest: {p.Cost}");
        Console.WriteLine($"Best solution found at: {string.Join(", ", p.Position)}");
        p = pso.OptimizeLbest();
        Console.WriteLine($"Best solution found using LBest: {p.Cost}");
        Console.WriteLine($"Best solution found at: {string.Join(", ", p.Position)}");
    }
}