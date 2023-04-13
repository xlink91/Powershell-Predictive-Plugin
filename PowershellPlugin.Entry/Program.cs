// See https://aka.ms/new-console-template for more information
using Namotion.Reflection;
using PowershellPlugin.Predictor;
using System.Management.Automation.Subsystem.Prediction;

ShowPredictions("docker p");
ShowPredictions("docker compo");
ShowPredictions("docker log");
ShowPredictions("kub ");
ShowPredictions("kubectl ");
ShowPredictions("kubectl get p");
ShowPredictions("kubectl describe nodes");
ShowPredictions("kubectl describe nodes");
ShowPredictions("kubectl describe nodes | f");
ShowPredictions("kubectl describe nodes | find");
ShowPredictions("kubectl describe nodes | findstr | c");
ShowPredictions("kubectl describe nodes | findstr | compose | grep | concat | t");



static void ShowPredictions(string input)
{
    var context = PredictionContext.Create(input);
    var predictor = new SamplePredictor();
    var suggestions = predictor.GetSuggestion(null, context, default);
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.WriteLine(input);
    Console.ForegroundColor = ConsoleColor.DarkGray;
    foreach (var entry in suggestions.SuggestionEntries ?? new())
    {
        Console.WriteLine(entry.SuggestionText);
    }
}
