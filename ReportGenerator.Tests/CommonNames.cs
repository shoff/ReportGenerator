namespace ReportGenerator.Tests
{
    using System;

    internal static class CommonNames
    {
        internal const string TestNamespace = "ReportGenerator.Tests.TestFiles.Project.";

        internal static string TestFilesRoot
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "\\TestFiles\\"; }
        }

        internal static string ReportDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "\\TestFiles\\Reports\\"; }
        }

        internal static string CodeDirectory
        {
            get { return AppDomain.CurrentDomain.BaseDirectory + "\\TestFiles\\Project\\"; }
        }
    }
}
