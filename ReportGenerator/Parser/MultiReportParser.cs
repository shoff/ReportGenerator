using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Palmmedia.ReportGenerator.Parser.Analysis;

namespace Palmmedia.ReportGenerator.Parser
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Parser that aggregates several parsers.
    /// </summary>
    public class MultiReportParser : ParserBase
    {
        private readonly List<string> parserNames = new List<string>();

        public override string ToString()
        {
            if (this.parserNames.Count == 0)
            {
                return string.Empty;
            }
            if (this.parserNames.Count == 1)
            {
                return this.parserNames[0];
            }
            StringBuilder sb = new StringBuilder(this.GetType().Name);
            sb.Append(" (");

            var groupedParsers = this.parserNames.GroupBy(p => p).OrderBy(pg => pg.Key);

            sb.Append(string.Join(", ",
                groupedParsers.Select(pg => string.Format(CultureInfo.InvariantCulture, "{0}x {1}", pg.Count(), pg.Key))));

            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// Adds the parser.
        /// </summary>
        /// <param name="parser">The parser to add.</param>
        public void AddParser(IParser parser)
        {
            Contract.Requires<ArgumentNullException>(parser != null);

            this.parserNames.Add(parser.ToString());

            this.MergeAssemblies(parser.Assemblies);
        }

        private void MergeAssemblies(IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var existingAssembly = this.Assemblies.FirstOrDefault(a => a.Name == assembly.Name);

                if (existingAssembly != null)
                {
                    existingAssembly.Merge(assembly);
                }
                else
                {
                    this.AddAssembly(assembly);
                }
            }
        }
    }
}
