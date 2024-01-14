namespace SwarmOptimization;

public class Problem
{
    public  Func<double[], double> FitnessFunction { get; set; }
    public (double Min, double Max)[]Domain { get; set; }
    
    public Problem (Func<double[], double> fitnessFunction, (double Min, double Max)[] domain)
    {
        FitnessFunction = fitnessFunction;
        Domain = domain;
    }
}