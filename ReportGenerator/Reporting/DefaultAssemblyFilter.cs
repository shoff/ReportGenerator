﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Palmmedia.ReportGenerator.Reporting
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Default implementation of <see cref="IAssemblyFilter"/>.
    /// An assembly is included if at least one include filter matches their name.
    /// The assembly is excluded if at least one exclude filter matches its name.
    /// Exclusion filters take precedence over inclusion filters. Wildcards are allowed in filters.
    /// </summary>
    public class DefaultAssemblyFilter : IAssemblyFilter
    {
        /// <summary>
        /// The include filters.
        /// </summary>
        private readonly IEnumerable<string> includeFilters;

        /// <summary>
        /// The exclude filters.
        /// </summary>
        private readonly IEnumerable<string> excludeFilters;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultAssemblyFilter"/> class.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public DefaultAssemblyFilter(ICollection<string> filters)
        {
            Contract.Requires<ArgumentNullException>(filters != null);

            this.excludeFilters = filters
                .Where(f => f.StartsWith("-", StringComparison.OrdinalIgnoreCase))
                .Select(CreateFilterRegex);

            this.includeFilters = filters
                .Where(f => f.StartsWith("+", StringComparison.OrdinalIgnoreCase))
                .Select(CreateFilterRegex);

            if (!this.includeFilters.Any())
            {
                this.includeFilters = Enumerable.Repeat(CreateFilterRegex("+*"), 1);
            }
        }

        /// <summary>
        /// Determines whether the given assembly should be included in the report.
        /// </summary>
        /// <param name="assemblyName">Name of the assembly.</param>
        /// <returns>
        ///   <c>true</c> if assembly should be included in the report; otherwise, <c>false</c>.
        /// </returns>
        public bool IsAssemblyIncludedInReport(string assemblyName)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrEmpty(assemblyName));

            if (this.excludeFilters.Any(f => Regex.IsMatch(assemblyName, f)))
            {
                return false;
            }
            return this.includeFilters.Any(f => Regex.IsMatch(assemblyName, f));
        }

        /// <summary>
        /// Converts the given filter to a corresponding regular expression.
        /// Special characters are escaped. Wildcards '*' are converted to '.*'.
        /// </summary>
        /// <param name="filter">The filter.</param>
        /// <returns>The regular expression.</returns>
        private static string CreateFilterRegex(string filter)
        {
            filter = filter.Substring(1);
            filter = filter.Replace("*", "$$$*");
            filter = Regex.Escape(filter);
            filter = filter.Replace(@"\$\$\$\*", ".*");

            return string.Format(CultureInfo.InvariantCulture, "^{0}$", filter);
        }
    }
}
