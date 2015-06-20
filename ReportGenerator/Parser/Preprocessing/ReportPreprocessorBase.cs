﻿namespace Palmmedia.ReportGenerator.Parser.Preprocessing
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;
    using System.Xml.Linq;
    using Palmmedia.ReportGenerator.Parser.Preprocessing.CodeAnalysis;
    using Palmmedia.ReportGenerator.Parser.Preprocessing.FileSearch;

    /// <summary>
    ///   Base class for report preprocessing.
    /// </summary>
    public abstract class ReportPreprocessorBase : IReportPreprocessorBase
    {

        private readonly IClassSearcherFactory classSearcherFactory;
        private readonly IClassSearcher globalClassSearcher;
        private int currentFileId = int.MaxValue;

        /// <summary>
        ///   Initializes a new instance of the <see cref="ReportPreprocessorBase" /> class.
        /// </summary>
        /// <param name="report">The report.</param>
        /// <param name="classSearcherFactory">The class searcher factory.</param>
        /// <param name="globalClassSearcher">The global class searcher.</param>
        protected ReportPreprocessorBase(
            XContainer report, 
            IClassSearcherFactory classSearcherFactory, 
            IClassSearcher globalClassSearcher)
        {
            Contract.Requires<ArgumentNullException>(report != null);
            Contract.Requires<ArgumentNullException>(classSearcherFactory != null);
            Contract.Requires<ArgumentNullException>(globalClassSearcher != null);

            this.Report = report;
            this.classSearcherFactory = classSearcherFactory;
            this.globalClassSearcher = globalClassSearcher;
        }

        /// <summary>
        /// Gets the report file as XContainer.
        /// </summary>
        protected XContainer Report { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="ClassSearcher" /> instance which is instantiated during preprocessing depending on the
        ///source directories used in the report.
        /// </summary>
        protected IClassSearcher ClassSearcher { get; set; }

        /// <summary>
        ///   Executes the preprocessing of the report.
        /// </summary>
        public abstract void Execute();

        /// <summary>
        ///   Adds a new source code file to the report.
        /// </summary>
        /// <param name="filesContainer">The files container.</param>
        /// <param name="fileId">The file id.</param>
        /// <param name="file">The file path.</param>
        protected abstract void AddNewFile(XContainer filesContainer, string fileId, string file);

        /// <summary>
        ///   Searches the given source element (e.g. property) and updates the report if element can be found in source code
        ///   files.
        /// </summary>
        /// <param name="sourceElement">The source element.</param>
        /// <param name="filenameByFileIdDictionary">Dictionary containing all files used in the report by their corresponding id.</param>
        /// <param name="fileIdsOfClass">The file ids of class.</param>
        /// <param name="reportElement">The report element.</param>
        /// <param name="updateReportElement">Action that updates the report element.</param>
        /// <param name="filesContainer">The files container.</param>
        /// <returns><c>true</c> if source element has been found.</returns>
        protected bool SearchElement(
            SourceElement sourceElement, 
            Dictionary<string, string> filenameByFileIdDictionary, 
            IEnumerable<string> fileIdsOfClass, 
            XContainer reportElement, 
            Action<XContainer, SourceElementPosition, string> updateReportElement, 
            XContainer filesContainer)
        {
            var classFields = fileIdsOfClass;
            // TODO why is this func here?

            Func<bool> searchSourceElement = () =>
            {
                foreach (var fileId in classFields)
                {
                    var elementPosition = SourceCodeAnalyzer.FindSourceElement(filenameByFileIdDictionary[fileId], sourceElement);

                    if (elementPosition != null)
                    {
                        updateReportElement(reportElement, elementPosition, fileId);
                        return true;
                    }
                }

                return false;
            };

            // Search files from module first
            if (!searchSourceElement())
            {
                // Property has not been found in classes of module, now search the common directory
                if (this.ClassSearcher == null)
                {
                    this.ClassSearcher =
                        this.classSearcherFactory.CreateClassSearcher(
                            CommonDirectorySearcher.GetCommonDirectory(filenameByFileIdDictionary.Values));
                }

                fileIdsOfClass = this.TryToFindFileIdsOfClass(
                    this.ClassSearcher, 
                    sourceElement.Classname, 
                    filenameByFileIdDictionary, 
                    filesContainer);

                // Property has not been found in common directory, now search the global directory
                if (!searchSourceElement())
                {
                    fileIdsOfClass = this.TryToFindFileIdsOfClass(
                        this.globalClassSearcher, 
                        sourceElement.Classname, 
                        filenameByFileIdDictionary, 
                        filesContainer);
                    return searchSourceElement();
                }
            }

            return true;
        }

        /// <summary>
        ///   Tries to find file ids of class.
        /// </summary>
        /// <param name="classSearcher">The class searcher.</param>
        /// <param name="className">Name of the class.</param>
        /// <param name="filenameByFileIdDictionary">Dictionary containing all files used in the report by their corresponding id.</param>
        /// <param name="filesContainer">The files container.</param>
        /// <returns>The ids of the files the class is defined in.</returns>
        private IEnumerable<string> TryToFindFileIdsOfClass(
            IClassSearcher classSearcher, 
            string className, 
            Dictionary<string, string> filenameByFileIdDictionary, 
            XContainer filesContainer)
        {
            var files = classSearcher.GetFilesOfClass(className.Replace("/", string.Empty));

            var fileIds = new List<string>();
            foreach (var file in files)
            {
                var existingFileId = filenameByFileIdDictionary.Where(kv => kv.Value == file).Select(kv => kv.Key).FirstOrDefault();
                if (existingFileId != null)
                {
                    fileIds.Add(existingFileId);
                }
                else
                {
                    // Update dictionary
                    var newFileId = this.currentFileId.ToString(CultureInfo.InvariantCulture);
                    filenameByFileIdDictionary.Add(newFileId, file);
                    fileIds.Add(newFileId);

                    // Update report
                    this.AddNewFile(filesContainer, newFileId, file);

                    this.currentFileId--;
                }
            }

            return fileIds;
        }
    }
}