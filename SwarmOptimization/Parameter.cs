namespace SwarmOptimization;

public class Parameter
{
    public int NumberOfParticles { get; set; }
    public int SizeOfProblem { get; set; }
    public double MaxVelocity { get; set; }
    public double W { get; set; }
    public double C1 { get; set; }
    public double C2 { get; set; }
    public int MaxIterations { get; set; }
}