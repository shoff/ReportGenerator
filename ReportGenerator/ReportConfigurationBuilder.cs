﻿

namespace Palmmedia.ReportGenerator
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Text.RegularExpressions;
    using Palmmedia.ReportGenerator.Reporting;

    /// <summary>
    /// Builder for <see cref="ReportConfiguration"/>.
    /// Creates instances of <see cref="ReportConfiguration"/> based on command line parameters.
    /// </summary>
    public class ReportConfigurationBuilder
    {
        /// <summary>
        /// The report builder factory.
        /// </summary>
        private readonly IReportBuilderFactory reportBuilderFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportConfigurationBuilder"/> class.
        /// </summary>
        /// <param name="reportBuilderFactory">
        /// The report builder factory.
        /// </param>
        public ReportConfigurationBuilder(
            IReportBuilderFactory reportBuilderFactory)
        {
            Contract.Requires<ArgumentNullException>(reportBuilderFactory != null);

            this.reportBuilderFactory = reportBuilderFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReportConfiguration"/> class.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        /// <returns>
        /// The report configuration.
        /// </returns>
        public ReportConfiguration Create(string[] args)
        {
            Contract.Requires<ArgumentNullException>(args != null);

            if (args.Length > 0 && Regex.IsMatch(args[0], "-\\w{2,}:"))
            {
                return this.CreateBasedOnNamedArguments(args);
            }

            return this.CreateBasedOnLegacyArguments(args);
        }

        /// <summary>
        /// Shows the help of the program.
        /// </summary>
        // TODO convert to form
        public void ShowHelp()
        {
            // var availableReportTypes = this.reportBuilderFactory.GetAvailableReportTypes();

            // Console.WriteLine();
            // Console.WriteLine(typeof(ReportConfigurationBuilder).Assembly.GetName().Name + " "
            // + typeof(ReportConfigurationBuilder).Assembly.GetName().Version);

            // AssemblyCopyrightAttribute assemblyCopyrightAttribute = typeof(ReportConfigurationBuilder).Assembly
            // .GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false)
            // .Cast<AssemblyCopyrightAttribute>()
            // .FirstOrDefault();

            // if (assemblyCopyrightAttribute != null)
            // {
            // Console.WriteLine(assemblyCopyrightAttribute.Copyright);
            // }

            // Console.WriteLine();
            // Console.WriteLine(Help.Parameters);
            // Console.WriteLine("    " + Help.Parameters1);
            // Console.WriteLine("    " + Help.Parameters2);
            // Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "    " + Help.Parameters3, string.Join("|", availableReportTypes.Take(3).Union(new[] { "..." }))));
            // Console.WriteLine("    " + Help.Parameters4);
            // Console.WriteLine("    " + Help.Parameters5);
            // Console.WriteLine("    " + Help.Parameters6);
            // Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "    " + Help.Parameters7, string.Join("|", Enum.GetNames(typeof(VerbosityLevel)))));

            // Console.WriteLine();
            // Console.WriteLine(Help.Explanations);
            // Console.WriteLine("    " + Help.Explanations1);
            // Console.WriteLine("    " + Help.Explanations2);
            // Console.WriteLine("    " + Help.Explanations3);
            // Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "    " + Help.ReportTypeValues, string.Join(", ", availableReportTypes)));
            // Console.WriteLine("    " + Help.Explanations4);
            // Console.WriteLine("    " + Help.Explanations5);
            // Console.WriteLine("    " + Help.Explanations6);
            // Console.WriteLine("    " + Help.Explanations7);
            // Console.WriteLine("    " + Help.Explanations8);
            // Console.WriteLine("    " + Help.Explanations9);
            // Console.WriteLine("    " + Help.Explanations10);
            // Console.WriteLine(string.Format(CultureInfo.InvariantCulture, "    " + Help.VerbosityValues, string.Join(", ", Enum.GetNames(typeof(VerbosityLevel)))));

            // Console.WriteLine();
            // Console.WriteLine(Help.DefaultValues);
            // Console.WriteLine("   -reporttypes:Html");
            // Console.WriteLine("   -filters:+*");
            // Console.WriteLine("   -verbosity:" + VerbosityLevel.Verbose);

            // Console.WriteLine();
            // Console.WriteLine(Help.Examples);
            // Console.WriteLine("   \"-reports:coverage.xml\" \"-targetdir:C:\\report\"");
            // Console.WriteLine("   \"-reports:target\\*\\*.xml\" \"-targetdir:C:\\report\" -reporttypes:Latex;HtmlSummary");
            // Console.WriteLine("   \"-reports:coverage1.xml;coverage2.xml\" \"-targetdir:report\"");
            // Console.WriteLine("   \"-reports:coverage.xml\" \"-targetdir:C:\\report\" -reporttypes:Latex \"-sourcedirs:C:\\MyProject\"");
            // Console.WriteLine("   \"-reports:coverage.xml\" \"-targetdir:C:\\report\" \"-sourcedirs:C:\\MyProject1;C:\\MyProject2\" \"-filters:+Included;-Excluded.*\"");
        }

        /// <summary>
        /// Initializes a <see cref="ReportConfiguration"/> instance based on "named" command line parameters.
        /// E.g. -reports:test.xml (Key: "reports", Value: "test.xml")
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        /// <returns>
        /// The report configuration.
        /// </returns>
        private ReportConfiguration CreateBasedOnNamedArguments(string[] args)
        {
            var namedArguments = new Dictionary<string, string>();

            foreach (var arg in args)
            {
                var match = Regex.Match(arg, "-(?<key>\\w{2,}):(?<value>.+)");

                if (match.Success)
                {
                    namedArguments[match.Groups["key"].Value.ToUpperInvariant()] = match.Groups["value"].Value;
                }
            }

            var reportFilePatterns = new string[] { };
            string targetDirectory = string.Empty;
            string historyDirectory = null;
            var reportTypes = new string[] { };
            var sourceDirectories = new string[] { };
            var filters = new string[] { };
            string verbosityLevel = null;

            string value = null;

            if (namedArguments.TryGetValue("REPORTS", out value))
            {
                reportFilePatterns = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (namedArguments.TryGetValue("TARGETDIR", out value))
            {
                targetDirectory = value;
            }

            if (namedArguments.TryGetValue("HISTORYDIR", out value))
            {
                historyDirectory = value;
            }

            if (namedArguments.TryGetValue("REPORTTYPES", out value))
            {
                reportTypes = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }
            else if (namedArguments.TryGetValue("REPORTTYPE", out value))
            {
                reportTypes = new[] { value };
            }

            if (namedArguments.TryGetValue("SOURCEDIRS", out value))
            {
                sourceDirectories = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (namedArguments.TryGetValue("FILTERS", out value))
            {
                filters = value.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (namedArguments.TryGetValue("VERBOSITY", out value))
            {
                verbosityLevel = value;
            }

            return new ReportConfiguration(this.reportBuilderFactory, reportFilePatterns, targetDirectory, 
                historyDirectory, reportTypes, sourceDirectories, filters, verbosityLevel);
        }

        /// <summary>
        /// Initializes a <see cref="ReportConfiguration"/> instance based on "legacy" command line parameters.
        /// Only the parameters of ReportGenerator 1.2.7.0 are applied to provide legacy support.
        /// </summary>
        /// <param name="args">
        /// The command line arguments.
        /// </param>
        /// <returns>
        /// The report configuration.
        /// </returns>
        private ReportConfiguration CreateBasedOnLegacyArguments(string[] args)
        {
            var reportFilePatterns = new string[] { };
            string targetDirectory = string.Empty;
            string[] reportTypes = { };
            var sourceDirectories = new string[] { };
            var filters = new string[] { };
            string verbosityLevel = null;

            if (args.Length > 0)
            {
                reportFilePatterns = args[0].Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            }

            if (args.Length > 1)
            {
                targetDirectory = args[1];
            }

            if (args.Length > 2)
            {
                reportTypes = new[] { args[2] };
            }

            return new ReportConfiguration(this.reportBuilderFactory, reportFilePatterns, targetDirectory, 
                null, reportTypes, sourceDirectories, filters, verbosityLevel);
        }
    }
}
