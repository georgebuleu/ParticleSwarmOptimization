namespace SwarmOptimization;

public static class Parameters
{
    public static readonly int NumberOfParticles = 20;
    public static readonly int SizeOfProblem = 2;
    public static double MaxVelocity { get; set; }
    public static readonly double W = 100;
    public static readonly double C1 = 2.0;
    public static readonly double C2 = 2.0;
    public static readonly int MaxIterations = 100;
    public static readonly int SizeOfNeighborhood = 10;
}