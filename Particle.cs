using Newtonsoft.Json;

namespace SwarmOptimization;

public class Particle
{
    //particle class
    public double[] Position { get; set; }
    public double[] Velocity { get; set; }
    public double Cost { get; set; }
    [JsonIgnore]
    public Particle PersonalBest { get; set; }
    public Particle()
    {
        Position = new double[Parameters.Dimension];
    }
    
}