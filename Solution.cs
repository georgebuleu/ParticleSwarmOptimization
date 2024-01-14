using System.Text.Json;
using Newtonsoft.Json;

namespace SwarmOptimization;

public class Solution
{
    public Particle BestParticle { get; set;}
    private double[][][] Positions { get; set; }
    private string SolutionType { get; set; }
    public Solution(Particle bestParticle, double[][][] positions, string solutionType)
    {
        this.BestParticle = bestParticle;
        this.Positions = positions;
        this.SolutionType = solutionType;
    }
    public void WriteToFile()
    {
        if (BestParticle == null)
        {
            Console.WriteLine("BestParticle is null. Cannot write to file.");
            return;
        }

        string fileName = $"C:/Facultate/IA/SwarmOptimizationCLI/{SolutionType}.json";

        try
        {
            // Create an anonymous object for the final structure
            var jsonData = new
            {
                BestParticle = BestParticle,
                Positions = Positions
            };

            // Serialize the entire object to JSON
            string jsonContent = JsonConvert.SerializeObject(jsonData, Formatting.Indented);

            // Write the JSON content to the file
            File.WriteAllText(fileName, jsonContent);

            Console.WriteLine($"Solution data written to file: {fileName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

}