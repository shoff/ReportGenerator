namespace Palmmedia.ReportGenerator.Reporting
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Interface to filter assemblies based on their name during report generation.
    /// This can be used to include only a subset of all assemblies in the report.
    /// </summary>
    [ContractClass(typeof(AssemblyFilterContract))]
    public interface IAssemblyFilter
    {
        /// <summary>
        /// Determines whether the given assembly should be included in the report.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>
        ///   <c>true</c> if assembly should be included in the report; otherwise, <c>false</c>.
        /// </returns>
        bool IsAssemblyIncludedInReport(string assemblyName);
    }

    [ExcludeFromCodeCoverage]
    [ContractClassFor(typeof(IAssemblyFilter))]
    public abstract class AssemblyFilterContract : IAssemblyFilter
    {
        public bool IsAssemblyIncludedInReport(string assemblyName)
        {
            Contract.Requires<ArgumentNullException>(assemblyName != null);
            return true;
        }
    }
}
