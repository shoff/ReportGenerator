namespace Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    [ContractClass(typeof(ClassSearcherContract))]
    public interface IClassSearcher
    {
        /// <summary>
        /// Gets the directory that should be searched for class files.
        /// </summary>
        string Directory { get; }

        /// <summary>
        /// Gets the files the given class is defined in.
        /// </summary>
        /// <param name="className">Name of the class (with full namespace).</param>
        /// <returns>The files the class is defined in.</returns>
        ICollection<string> GetFilesOfClass(string className);
    }

    [Pure]
    [ExcludeFromCodeCoverage]
    [ContractClassFor(typeof(IClassSearcher))]
    public abstract class ClassSearcherContract : IClassSearcher
    {
        public string Directory { get; private set; }

        public ICollection<string> GetFilesOfClass(string className)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(className));
            Contract.Ensures(Contract.Result<ICollection<string>>() != null);

            return new List<string>();
        }
    }
}