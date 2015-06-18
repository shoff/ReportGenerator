namespace ReportGenerator.Tests.TestFiles.Project
{
    using System;

    [CoverageExclude]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
    public class CoverageExcludeAttribute : Attribute
    {
    }
}
