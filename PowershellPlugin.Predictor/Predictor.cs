using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Subsystem;
using System.Management.Automation.Subsystem.Prediction;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PowershellPlugin.Predictor
{
     public class SamplePredictor : ICommandPredictor
     {
        private static PreffixTree _tree;
        private static PreffixTree _afterPipe;
        private readonly Guid _guid;
        private readonly static object _objLock = new object();

        public SamplePredictor()
        {
            _tree = new PreffixTree();
            _afterPipe = new PreffixTree();
            string environmentVariable = Environment.GetEnvironmentVariable("PLUGIN_POWERSHELL_PREDICTIVE");
            if (environmentVariable == null)
            {
              throw new ArgumentException("Environment variable PLUGIN_POWERSHELL_PREDICTIVE with path value not set.");
            }
            _tree.Load(Path.Combine(environmentVariable, "Text"), "*.txt");
            _afterPipe.Load(Path.Combine(environmentVariable, "Text"), "*.pipe");
        }

        internal SamplePredictor(string guid) 
            : this()
        {
            _guid = new Guid(guid);
        }

        /// <summary>
        /// Gets the unique identifier for a subsystem implementation.
        /// </summary>
        public Guid Id => _guid;

        /// <summary>
        /// Gets the name of a subsystem implementation.
        /// </summary>
        public string Name => "Predictor";

        /// <summary>
        /// Gets the description of a subsystem implementation.
        /// </summary>
        public string Description => "A sample predictor";

        /// <summary>
        /// Get the predictive suggestions. It indicates the start of a suggestion rendering session.
        /// </summary>
        /// <param name="client">Represents the client that initiates the call.</param>
        /// <param name="context">The <see cref="PredictionContext"/> object to be used for prediction.</param>
        /// <param name="cancellationToken">The cancellation token to cancel the prediction.</param>
        /// <returns>An instance of <see cref="SuggestionPackage"/>.</returns>
        public SuggestionPackage GetSuggestion(PredictionClient client, PredictionContext context, CancellationToken cancellationToken)
        {
            string input = context.InputAst.Extent.Text;
            string pipePattern = @".+(?:\s+\|\s+([^\s^\|]+))+\s*";
            if (Regex.IsMatch(input, pipePattern))
            {
                var match = Regex.Match(input, pipePattern, RegexOptions.RightToLeft);
                Group afterPipeGroup = match.Groups[1];
                var afterPipeText = afterPipeGroup.Captures[0].Value;
                var index = afterPipeGroup.Captures[0].Index;
                var rs = _afterPipe.GetPrefixPaths(afterPipeText)
                                                       .Select(x => new PredictiveSuggestion(string.Concat(input.AsSpan(0, index), x)))
                                                       .ToList();
                return rs.Any() ? new SuggestionPackage(rs) : default;
            }
            var r = _tree.GetPrefixPaths(input)
                                              .Select(x => new PredictiveSuggestion(x))
                                              .ToList();
            return r.Any() ? new SuggestionPackage(r) : default;
        }

        #region "interface methods for processing feedback"

        /// <summary>
        /// Gets a value indicating whether the predictor accepts a specific kind of feedback.
        /// </summary>
        /// <param name="client">Represents the client that initiates the call.</param>
        /// <param name="feedback">A specific type of feedback.</param>
        /// <returns>True or false, to indicate whether the specific feedback is accepted.</returns>
        public bool CanAcceptFeedback(PredictionClient client, PredictorFeedbackKind feedback) => false;

        /// <summary>
        /// One or more suggestions provided by the predictor were displayed to the user.
        /// </summary>
        /// <param name="client">Represents the client that initiates the call.</param>
        /// <param name="session">The mini-session where the displayed suggestions came from.</param>
        /// <param name="countOrIndex">
        /// When the value is greater than 0, it's the number of displayed suggestions from the list
        /// returned in <paramref name="session"/>, starting from the index 0. When the value is
        /// less than or equal to 0, it means a single suggestion from the list got displayed, and
        /// the index is the absolute value.
        /// </param>
        public void OnSuggestionDisplayed(PredictionClient client, uint session, int countOrIndex) { }

        /// <summary>
        /// The suggestion provided by the predictor was accepted.
        /// </summary>
        /// <param name="client">Represents the client that initiates the call.</param>
        /// <param name="session">Represents the mini-session where the accepted suggestion came from.</param>
        /// <param name="acceptedSuggestion">The accepted suggestion text.</param>
        public void OnSuggestionAccepted(PredictionClient client, uint session, string acceptedSuggestion) { }

        /// <summary>
        /// A command line was accepted to execute.
        /// The predictor can start processing early as needed with the latest history.
        /// </summary>
        /// <param name="client">Represents the client that initiates the call.</param>
        /// <param name="history">History command lines provided as references for prediction.</param>
        public void OnCommandLineAccepted(PredictionClient client, IReadOnlyList<string> history) { }

        /// <summary>
        /// A command line was done execution.
        /// </summary>
        /// <param name="client">Represents the client that initiates the call.</param>
        /// <param name="commandLine">The last accepted command line.</param>
        /// <param name="success">Shows whether the execution was successful.</param>
        public void OnCommandLineExecuted(PredictionClient client, string commandLine, bool success) { }

        #endregion;
    }

    /// <summary>
    /// Register the predictor on module loading and unregister it on module un-loading.
    /// </summary>
    public class Init : IModuleAssemblyInitializer, IModuleAssemblyCleanup
    {
        private const string Identifier = "843b51d0-55c8-4c1a-8116-f0728d419306";

        /// <summary>
        /// Gets called when assembly is loaded.
        /// </summary>
        public void OnImport()
        {
            var predictor = new SamplePredictor(Identifier);
            SubsystemManager.RegisterSubsystem(SubsystemKind.CommandPredictor, predictor);
        }

        /// <summary>
        /// Gets called when the binary module is unloaded.
        /// </summary>
        public void OnRemove(PSModuleInfo psModuleInfo)
        {
            SubsystemManager.UnregisterSubsystem(SubsystemKind.CommandPredictor, new Guid(Identifier));
        }
    }
}
