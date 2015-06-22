namespace ReportGenerator.Tests.TestHelpers
{
    using System.Collections.Generic;
    using System.Linq;
    using Palmmedia.ReportGenerator.Parser.Analysis;

    public class FileAnalysisCreator
    {
        internal static FileAnalysis GetFileAnalysis(ICollection<Assembly> assemblies, string className, string fileName)
        {
            var classes = assemblies.Single(a => a.Name == "ReportGenerator.Tests").Classes;
            var files = classes.Single(c => c.Name == className).Files;
            var path = files.Single(f => f.Path == fileName);
            var analysis = path.AnalyzeFile();
            return analysis;
        } 
    }
}