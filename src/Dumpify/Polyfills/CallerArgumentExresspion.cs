#if NETFRAMEWORK || NETSTANDARD || NETCOREAPP2X

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Runtime.CompilerServices;

[ExcludeFromCodeCoverage]
[DebuggerNonUserCode]
[AttributeUsage(AttributeTargets.Parameter)]
public sealed class CallerArgumentExpressionAttribute :
    Attribute
{
    public CallerArgumentExpressionAttribute(string parameterName) =>
        ParameterName = parameterName;

    public string ParameterName { get; }
}

#endif