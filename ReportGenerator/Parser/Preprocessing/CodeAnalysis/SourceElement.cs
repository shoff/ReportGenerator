using System;
using ICSharpCode.NRefactory.PatternMatching;

namespace Palmmedia.ReportGenerator.Parser.Preprocessing.CodeAnalysis
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Represents an element in a source code file.
    /// </summary>
    public abstract class SourceElement
    {
        /// <summary>Initializes a new instance of the <see cref="SourceElement"/> class.</summary>
        /// <param name="classname">The classname.</param>
        [Pure]
        protected SourceElement(string classname)
        {
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(classname));

            this.Classname = classname;
        }

        /// <summary>Gets the classname.</summary>
        public string Classname { get; private set; }

        /// <summary>Determines whether the given <see cref="ICSharpCode.NRefactory.PatternMatching.INode"/> matches the <see cref="SourceElement"/>.</summary>
        /// <param name="node">The node.</param>
        /// <returns>A <see cref="SourceElementPosition"/> or <c>null</c> if <see cref="SourceElement"/> 
        /// does not match the <see cref="ICSharpCode.NRefactory.PatternMatching.INode"/>.</returns>
        public abstract SourceElementPosition GetSourceElementPosition(INode node);
    }
}
