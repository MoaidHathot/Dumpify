#if NETSTANDARD2_0
// Taken from https://github.com/dotnet/runtime/blob/e5ddc6c9b7e481d36d6350960868188d7b275ef7/src/libraries/System.Private.CoreLib/src/System/Collections/Generic/CollectionExtensions.cs
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Collections.Generic
{
    public static class CollectionExtensions
    {
        public static TValue? GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) =>
           dictionary.GetValueOrDefault(key, default!);

        public static TValue GetValueOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            if (dictionary is null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }

            return dictionary.TryGetValue(key, out TValue? value) ? value : defaultValue;
        }
    }
}
#endif