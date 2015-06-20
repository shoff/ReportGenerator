namespace Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    [ContractClass(typeof(ClassSearcherFactoryContract))]
    public interface IClassSearcherFactory
    {
        /// <summary>
        /// Creates the class searcher.
        /// </summary>
        /// <param name="directory">The directory that should be searched for class files.</param>
        /// <returns>The class searcher.</returns>
        IClassSearcher CreateClassSearcher(string directory);

        /// <summary>
        /// Creates the class searcher.
        /// </summary>
        /// <param name="directories">The directories that should be searched for class files.</param>
        /// <returns>The class searcher.</returns>
        IClassSearcher CreateClassSearcher(params string[] directories);
    }

    [Pure]
    [ExcludeFromCodeCoverage]
    [ContractClassFor(typeof(IClassSearcherFactory))]
    public abstract class ClassSearcherFactoryContract : IClassSearcherFactory
    {
        public IClassSearcher CreateClassSearcher(string directory)
        {
            Contract.Ensures(Contract.Result<IClassSearcher>() != null);
            return default(IClassSearcher);
        }

        public IClassSearcher CreateClassSearcher(params string[] directories)
        {
            Contract.Requires<ArgumentNullException>(directories != null);
            Contract.Ensures(Contract.Result<IClassSearcher>() != null);
            return default(IClassSearcher);
        }
    }
}