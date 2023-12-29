namespace SwarmOptimization;

public class Problem
{
    public Func<double[], double> ObjectiveFunction { get;}
    public (double Min, double Max)[]Domain { get; }
    
    public Problem (Func<double[], double> objectiveFunction, (double Min, double Max)[] domain)
    {
        ObjectiveFunction = objectiveFunction;
        Domain = domain;
    }
}