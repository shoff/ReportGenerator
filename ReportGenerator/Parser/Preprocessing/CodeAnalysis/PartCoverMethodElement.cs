namespace Palmmedia.ReportGenerator.Parser.Preprocessing.CodeAnalysis
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text.RegularExpressions;
    using ICSharpCode.NRefactory.CSharp;
    using ICSharpCode.NRefactory.PatternMatching;

    /// <summary>
    ///   Represents method information extracted from a PartCover report.
    ///   This class is used to compensate a deficiency of PartCover 2.3.0.35109.
    ///   PartCover does not record coverage information for unexecuted methods any more.
    ///   To provide correct reports the line numbers are determined from the source code files instead of
    ///   PartCover's coverage information.
    /// </summary>
    public class PartCoverMethodElement : SourceElement
    {
        private static readonly string[] parameterDelimiter = new[] { ", " };
        private static readonly Dictionary<string, string> typeReplacements = InitializeTypeReplacements();
        private readonly string methodname;
        private readonly string[] parameters;
        private readonly string returnType;

        /// <summary>Initializes a new instance of the <see cref="PartCoverMethodElement" /> class.</summary>
        /// <param name="classname">The name of the class.</param>
        /// <param name="methodname">The name of the method.</param>
        /// <param name="signature">The signature of the method.</param>
        public PartCoverMethodElement(string classname, string methodname, string signature)
            : base(classname)
        {
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(classname));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(methodname));
            Contract.Requires<ArgumentNullException>(!string.IsNullOrWhiteSpace(signature));

            this.methodname = methodname;

            var match = Regex.Match(signature, @"(?<returnType>^\S*).*\((?<arguments>.*)\)", RegexOptions.Compiled);
            this.returnType = match.Groups["returnType"].Value;
            this.parameters = match.Groups["arguments"].Value.Split(parameterDelimiter, StringSplitOptions.RemoveEmptyEntries);
        }

        private bool IsConstructor
        {
            get { return this.methodname == ".ctor"; }
        }

        /// <summary>
        ///   Determines whether the given <see cref="ICSharpCode.NRefactory.PatternMatching.INode" /> matches the
        ///   <see cref="SourceElement" />.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>
        ///   A <see cref="SourceElementPosition" /> or <c>null</c> if <see cref="SourceElement" /> does not match the
        ///   <see cref="ICSharpCode.NRefactory.PatternMatching.INode" />.
        /// </returns>
        public override SourceElementPosition GetSourceElementPosition(INode node)
        {
            Contract.Requires<ArgumentNullException>(node != null);

            if (this.IsConstructor)
            {
                var constructorDeclaration = node as ConstructorDeclaration;

                if (constructorDeclaration != null)
                {
                    if (!this.DoesMethodnameMatch(constructorDeclaration.Name)
                        || !this.AreParametersMatching(constructorDeclaration.Parameters))
                    {
                        return null;
                    }

                    if (constructorDeclaration.Body != null)
                    {
                        return new SourceElementPosition(
                            constructorDeclaration.Body.StartLocation.Line, 
                            constructorDeclaration.Body.EndLocation.Line);
                    }
                }

                // hate all of these return nulls
                return null;
            }

            var methodDeclaration = node as MethodDeclaration;
            if (methodDeclaration != null)
            {
                if (!this.DoesMethodnameMatch(methodDeclaration.Name) || !AreTypesEqual(this.returnType, methodDeclaration.ReturnType)
                    || !this.AreParametersMatching(methodDeclaration.Parameters))
                {
                    return null;
                }

                if (methodDeclaration.Body != null)
                {
                    return new SourceElementPosition(
                        methodDeclaration.Body.StartLocation.Line, 
                        methodDeclaration.Body.EndLocation.Line);
                }
            }

            return null;
        }

        private static bool AreTypesEqual(string expectedType, AstType typeReference)
        {
            var simpleType = typeReference as SimpleType; // Generic types

            if (simpleType != null)
            {
                if (simpleType.TypeArguments.Count == 1)
                {
                    typeReference = simpleType.TypeArguments.First();
                }
                else if (simpleType.TypeArguments.Count >= 2)
                {
                    // This can't be handled correctly
                    return true;
                }
            }

            var composedType = typeReference as ComposedType; // Arrays

            if (composedType != null)
            {
                if (!expectedType.EndsWith("[]", StringComparison.Ordinal))
                {
                    return false;
                }
                else
                {
                    expectedType = expectedType.Replace("[]", string.Empty);
                    typeReference = composedType.BaseType;
                }
            }

            if (expectedType.StartsWith("ref ", StringComparison.Ordinal))
            {
                expectedType = expectedType.Replace("ref ", string.Empty);
            }

            string typeReplacement;
            if (typeReplacements.TryGetValue(expectedType, out typeReplacement))
            {
                expectedType = typeReplacement;
            }

            // Validate
            var typeName = typeReference.ToString();
            if (expectedType.Equals(typeName, StringComparison.OrdinalIgnoreCase)
                || ("System." + expectedType).Equals(typeName, StringComparison.OrdinalIgnoreCase)
                || expectedType.Substring(expectedType.LastIndexOf('.') + 1).Equals(typeName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        private static Dictionary<string, string> InitializeTypeReplacements()
        {
            var typeReplacements = new Dictionary<string, string>();
            typeReplacements.Add("unsigned byte", "byte");
            typeReplacements.Add("byte", "sbyte");
            typeReplacements.Add("unsigned short", "ushort");
            typeReplacements.Add("unsigned int", "uint");
            typeReplacements.Add("unsigned long", "ulong");

            return typeReplacements;
        }

        private bool AreParametersMatching(ICollection<ParameterDeclaration> parameters)
        {
            if (this.parameters.Length != parameters.Count)
            {
                return false;
            }

            for (var i = 0; i < parameters.Count; i++)
            {
                if (!AreTypesEqual(this.parameters[i], parameters.ElementAt(i).Type))
                {
                    return false;
                }
            }

            return true;
        }

        private bool DoesMethodnameMatch(string methodname)
        {
            if (this.IsConstructor)
            {
                var classname = this.Classname.Substring(this.Classname.LastIndexOf('.') + 1); // Remove namespace declaration
                return classname.Equals(methodname);
            }
            else
            {
                return this.methodname.Equals(methodname);
            }
        }
    }
}