// See https://aka.ms/new-console-template for more information
using Namotion.Reflection;
using PowershellPlugin.Predictor;
using System.Management.Automation.Subsystem.Prediction;


var test = new PredictionTests();

test.ShowPredictions("docker p | ");
test.ShowPredictions("docker p");
test.ShowPredictions("docker compo");
test.ShowPredictions("docker log");
test.ShowPredictions("kub ");
test.ShowPredictions("kubectl ");
test.ShowPredictions("kubectl get p");
test.ShowPredictions("kubectl describe nodes");
test.ShowPredictions("kubectl describe nodes");
test.ShowPredictions("kubectl describe nodes |");
test.ShowPredictions("kubectl describe nodes | f");
test.ShowPredictions("kubectl describe nodes | find");
test.ShowPredictions("kubectl describe nodes | findstr | c");
test.ShowPredictions("kubectl describe nodes | findstr | compose | grep | concat | t");

internal class PredictionTests
{
    SamplePredictor predictor = new SamplePredictor();
    public void ShowPredictions(string input)
    {
        var context = PredictionContext.Create(input);
        var suggestions = predictor.GetSuggestion(null, context, default);
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine(input);
        Console.ForegroundColor = ConsoleColor.DarkGray;
        foreach (var entry in suggestions.SuggestionEntries ?? new())
        {
            Console.WriteLine(entry.SuggestionText);
        }
    }
}
