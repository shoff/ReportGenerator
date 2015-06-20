

namespace Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Searches several directories for class files.
    /// </summary>
    public class MultiDirectoryClassSearcher : ClassSearcher
    {
        private readonly List<IClassSearcher> classSearchers;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiDirectoryClassSearcher"/> class.
        /// </summary>
        /// <param name="classSearchers">
        /// The <see cref="ClassSearcher">ClassSearchers</see>.
        /// </param>
        public MultiDirectoryClassSearcher(IEnumerable<IClassSearcher> classSearchers)
        {
            this.classSearchers = new List<IClassSearcher>(classSearchers);
        }

        /// <summary>
        /// Gets the files the given class is defined in.
        /// </summary>
        /// <param name="className">
        /// Name of the class (with full namespace).
        /// </param>
        /// <returns>
        /// The files the class is defined in.
        /// </returns>
        public override ICollection<string> GetFilesOfClass(string className)
        {
            var filesOfClass = this.classSearchers.SelectMany(c => c.GetFilesOfClass(className));
            var filesOfClassArray = filesOfClass as string[] ?? filesOfClass.ToArray();
            if (filesOfClassArray.Any())
            {
                return new List<string>(filesOfClassArray.Distinct());
            }

            return new List<string>();
        }
    }
}