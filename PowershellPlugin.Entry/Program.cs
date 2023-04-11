// See https://aka.ms/new-console-template for more information
using Namotion.Reflection;
using PowershellPlugin.Predictor;

Console.WriteLine("Hello, World!");


var p = new SamplePredictor();


var pr = SamplePredictor._tree.GetPrefixPaths("docker p");
var rs0 = SamplePredictor._tree.GetPrefixPaths("docker compo");
var rs1 = SamplePredictor._tree.GetPrefixPaths("docker log");
var rs2 = SamplePredictor._tree.GetPrefixPaths("kub ");
var rs3 = SamplePredictor._tree.GetPrefixPaths("kubectl ");
var pods = SamplePredictor._tree.GetPrefixPaths("kubectl get p");
var podCompleted = SamplePredictor._tree.GetPrefixPaths("kubectl describe nodes");
 _ = SamplePredictor._tree.GetPrefixPaths("kubectl describe nodes");
