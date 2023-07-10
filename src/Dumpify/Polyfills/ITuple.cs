#if NETSTANDARD2_0
// Taken from https://github.com/dotnet/runtime/blob/e5ddc6c9b7e481d36d6350960868188d7b275ef7/src/libraries/System.Private.CoreLib/src/System/Runtime/CompilerServices/ITuple.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// This interface is required for types that want to be indexed into by dynamic patterns.
    /// </summary>
    public interface ITuple
    {
        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        object? this[int index] { get; }
    }
}
#endif
