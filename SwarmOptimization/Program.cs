namespace SwarmOptimization;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new Form1());
        var problem = new Problem(
            x => Math.Pow(x[0], 2) + Math.Pow(x[1], 2),
            new (double Min, double Max)[] { (-10, 10), (-10, 10) }
        );
        var alpha = 0.1;
        //set parameters
        Parameters.MaxVelocity = alpha * (problem.Domain[0].Max - problem.Domain[0].Min);
        var pso = new Pso(problem);
        var p = pso.OptimizeGbest();
        Console.WriteLine($"Best solution found: {p.Cost}");
        Console.WriteLine($"Best solution found at: {string.Join(", ", p.Position)}");
        p = pso.OptimizeLbest();
        Console.WriteLine($"Best solution found: {p.Cost}");
        Console.WriteLine($"Best solution found at: {string.Join(", ", p.Position)}");
        
    }    
}