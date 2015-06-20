using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.NRefactory.CSharp;

namespace Palmmedia.ReportGenerator.Parser.Preprocessing.CodeAnalysis
{
    using System.Diagnostics.Contracts;
    using System.Security;
    using log4net;

    /// <summary>
    /// Helper class to determine the begin and end line number of source code elements within a source code file.
    /// </summary>
    public static class SourceCodeAnalyzer
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(SourceCodeAnalyzer));

        /// <summary>The name of the last source code file that has successfully been parsed.</summary>
        private static string lastFilename;

        /// <summary>The <see cref="AstNode"/> of the last source code file that has successfully been parsed.</summary>
        private static AstNode lastNode;

        /// <summary>Gets all classes in the given file.</summary>
        /// <param name="filename">The filename.</param>
        /// <returns>All classes (with full namespace).</returns>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="UnauthorizedAccessException">Condition.</exception>
        /// <exception cref="IOException">Condition.</exception>
        public static ICollection<string> GetClassesInFile(string filename)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(filename));

            AstNode parentNode = GetParentNode(filename);

            if (parentNode == null)
            {
                return new string[] { };
            }
            var collection = FindClasses(new[] { parentNode }).Select(GetFullClassName).Distinct();

            if (collection.Any())
            {
                return new List<string>(collection);
            }
            return new List<string>();
        }

        /// <summary>
        /// Searches the given source code file for a source element matching the given <see cref="SourceElement"/>.
        /// If the source element can be found, a <see cref="SourceElementPosition"/> containing the start and end line numbers is returned.
        /// Otherwise <c>null</c> is returned.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="sourceElement">The source element.</param>
        /// <returns>A <see cref="SourceElementPosition"/> or <c>null</c> if source element can not be found.</returns>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="IOException">Condition.</exception>
        /// <exception cref="UnauthorizedAccessException">Condition.</exception>
        public static SourceElementPosition FindSourceElement(string filename, SourceElement sourceElement)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(filename));
            Contract.Requires<ArgumentNullException>(sourceElement != null);

            AstNode parentNode = GetParentNode(filename);

            if (parentNode == null)
            {
                return null;
            }

            var matchingClasses = FindClasses(new[] { parentNode }).Where(c => GetFullClassName(c) == sourceElement.Classname);
            return FindSourceElement(matchingClasses, sourceElement);
        }

        /// <summary>Searches the given <see cref="ICSharpCode.NRefactory.PatternMatching.INode">INodes</see> recursively for classes.</summary>
        /// <param name="nodes">The nodes.</param>
        /// <returns>The type declarations corresponding to all classes.</returns>
        internal static ICollection<TypeDeclaration> FindClasses(IEnumerable<AstNode> nodes)
        {
            var result = new List<TypeDeclaration>();

            foreach (var node in nodes)
            {
                var typeDeclaration  = node as TypeDeclaration;
                if (typeDeclaration != null)
                {
                    result.Add(typeDeclaration);
                }

                if (typeDeclaration != null || node is NamespaceDeclaration || node is SyntaxTree)
                {
                    result.AddRange(FindClasses(node.Children));
                }
            }

            return result;
        }

        /// <summary>Searches the given <see cref="ICSharpCode.NRefactory.PatternMatching.INode">INodes</see> recursively for the given <see cref="SourceElement"/>.</summary>
        /// <param name="nodes">The nodes.</param>
        /// <param name="sourceElement">The source element.</param>
        /// <returns>A <see cref="SourceElementPosition"/> or <c>null</c> if source element can not be found.</returns>
        internal static SourceElementPosition FindSourceElement(IEnumerable<AstNode> nodes, SourceElement sourceElement)
        {
            Contract.Requires<ArgumentNullException>(nodes != null);
            Contract.Requires<ArgumentNullException>(sourceElement != null);
            var astNodes = nodes as AstNode[] ?? nodes.ToArray();
            
            foreach (var node in astNodes)
            {
                var sourceElementPosition = sourceElement.GetSourceElementPosition(node);
                if (sourceElementPosition != null)
                {
                    return sourceElementPosition;
                }
            }

            foreach (var node in astNodes)
            {
                var sourceElementPosition = FindSourceElement(node.Children, sourceElement);
                if (sourceElementPosition != null)
                {
                    return sourceElementPosition;
                }
            }

            return null;
        }

        /// <summary>Gets the topmost <see cref="ICSharpCode.NRefactory.PatternMatching.INode"/> in the given source file.</summary>
        /// <param name="filename">The filename.</param>
        /// <returns>The topmost <see cref="ICSharpCode.NRefactory.PatternMatching.INode"/> in the given source file.</returns>
        /// <exception cref="SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="IOException">Condition.</exception>
        /// <exception cref="UnauthorizedAccessException">Condition.</exception>
        internal static AstNode GetParentNode(string filename)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(filename));

            if (filename.Equals(lastFilename))
            {
                return lastNode;
            }

            try
            {
                using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var parser = new CSharpParser();

                    var syntaxTree = parser.Parse(fs, filename);

                    if (parser.HasErrors || syntaxTree == null)
                    {
                        return null;
                    }

                    // Cache the node
                    lastFilename = filename;
                    lastNode = syntaxTree;

                    return lastNode;
                }
            }
            catch (IOException ioe)
            {
                logger.Error(ioe.Message, ioe);
                throw;
            }
            catch (UnauthorizedAccessException uae)
            {
                logger.Error(uae.Message, uae);
                throw;
            }
        }

        /// <summary>
        /// Gets the full name of the class.
        /// </summary>
        /// <param name="typeDeclaration">The type declaration.</param>
        /// <returns>The full name of the class.</returns>
        internal static string GetFullClassName(TypeDeclaration typeDeclaration)
        {
            Contract.Requires<ArgumentNullException>(typeDeclaration != null);
            Contract.Ensures(Contract.Result<string>() != null);

            string result = typeDeclaration.Name;
            AstNode current = typeDeclaration;

            while (current.Parent != null)
            {
                current = current.Parent;

                var parentTypeDeclaration = current as TypeDeclaration;
                var parentNamespaceDeclaration = current as NamespaceDeclaration;

                if (parentTypeDeclaration != null)
                {
                    result = parentTypeDeclaration.Name + result;
                }
                else if (parentNamespaceDeclaration != null)
                {
                    result = parentNamespaceDeclaration.Name + "." + result;
                }
                else
                {
                    break;
                }
            }

            return result;
        }
    }
}
