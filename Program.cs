using System;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

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
        Stopwatch stopwatch = new Stopwatch();
        var alpha = 0.1;
        WriteParametersToCsv(problem.Domain[0]);
        Parameters.MaxVelocity = alpha * (problem.Domain[0].Max - problem.Domain[0].Min);
        var pso = new Pso(problem);
        stopwatch.Start(); 
        var solution = pso.OptimizeGbest();
        stopwatch.Stop();
        var gbestTime = stopwatch.ElapsedMilliseconds;
        solution.WriteToFile();
        Console.WriteLine($"Best solution found using GBest: {solution.BestParticle.Cost}");
        Console.WriteLine($"Best solution found at: {string.Join(", ", solution.BestParticle.Position)}");
        Console.WriteLine($"Time taken: {gbestTime} ms");
        stopwatch.Start();
        solution = pso.OptimizeLbest();
        stopwatch.Stop();
        var lbestTime = stopwatch.ElapsedMilliseconds;
        solution.WriteToFile();
        Console.WriteLine($"Best solution found using LBest: {solution.BestParticle.Cost}");
        Console.WriteLine($"Best solution found at: {string.Join(", ", solution.BestParticle.Position)}");
        Console.WriteLine($"Time taken: {lbestTime} ms");
    }
    
    private static void WriteParametersToCsv((double Min, double Max) domain)
    {
        
        int numberOfParticles = Parameters.NumberOfParticles;
        int dimension = Parameters.Dimension;
        int maxIterations = Parameters.MaxIterations + 1;
        
        string csvFilePath = "C:/Facultate/IA/SwarmOptimizationCLI/parameters.csv";

        try
        {
            using (StreamWriter writer = File.CreateText(csvFilePath))
            {
                // Write the header
                writer.WriteLine("NumberOfParticles,Dimension,MaxIterations,Min,Max");

                // Write the parameter values
                writer.WriteLine($"{numberOfParticles},{dimension},{maxIterations},{domain.Min},{domain.Max}");

                Console.WriteLine("Parameters written to CSV successfully!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    
}