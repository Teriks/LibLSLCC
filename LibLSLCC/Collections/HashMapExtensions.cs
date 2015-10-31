#region FileInfo
// 
// File: HashMapExtensions.cs
// 
// 
// ============================================================
// ============================================================
// 
// 
// Copyright (c) 2015, Teriks
// 
// All rights reserved.
// 
// 
// This file is part of LibLSLCC.
// 
// LibLSLCC is distributed under the following BSD 3-Clause License
// 
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
// 
// 1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
// 
// 2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer
//     in the documentation and/or other materials provided with the distribution.
// 
// 3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse or promote products derived
//     from this software without specific prior written permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
// LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT
// HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
// LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
// ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// 
// 
// ============================================================
// ============================================================
// 
// 
#endregion
using System;
using System.Collections.Generic;

namespace LibLSLCC.Collections
{
    /// <summary>
    /// Extensions for <see cref="HashMap{TKey,TValue}"/> objects.
    /// </summary>
    public static class HashMapExtensions
    {

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> object into a <see cref="HashMap{TKey,TSource}"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to convert.</param>
        /// <param name="keySelector">The key selector function.</param>
        /// <typeparam name="TSource"> </typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns>The generated <see cref="HashMap{TKey,TValue}"/> object.</returns>
        public static HashMap<TKey, TSource> ToHashMap<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            return source.ToHashMap(keySelector, x => x, EqualityComparer<TKey>.Default);
        }


        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> object into a <see cref="HashMap{TKey,TSource}"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to convert.</param>
        /// <param name="keySelector">The key selector function.</param>
        /// <param name="elementSelector">The element selector function.</param>
        /// <typeparam name="TSource"> </typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <returns>The generated <see cref="HashMap{TKey,TValue}"/> object.</returns>
        public static HashMap<TKey, TElement> ToHashMap<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector)
        {
            return source.ToHashMap(keySelector, elementSelector, EqualityComparer<TKey>.Default);
        }


        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> object into a <see cref="HashMap{TKey,TSource}"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to convert.</param>
        /// <param name="keySelector">The key selector function.</param>
        /// <param name="comparer">The key comparer function.</param>
        /// <typeparam name="TSource"> </typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <returns>The generated <see cref="HashMap{TKey,TValue}"/> object.</returns>
        public static HashMap<TKey, TSource> ToHashMap<TSource, TKey>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            IEqualityComparer<TKey> comparer)
        {
            return source.ToHashMap(keySelector, x => x, comparer);
        }


        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> object into a <see cref="HashMap{TKey,TSource}"/>
        /// </summary>
        /// <param name="source">The <see cref="IEnumerable{T}"/> to convert.</param>
        /// <param name="keySelector">The key selector function.</param>
        /// <param name="elementSelector">The element selector function.</param>
        /// <param name="comparer">The key comparer function.</param>
        /// <typeparam name="TSource"> </typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TElement"></typeparam>
        /// <returns>The generated <see cref="HashMap{TKey,TValue}"/> object.</returns>
        public static HashMap<TKey, TElement> ToHashMap<TSource, TKey, TElement>(
            this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector,
            Func<TSource, TElement> elementSelector,
            IEqualityComparer<TKey> comparer)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }
            if (elementSelector == null)
            {
                throw new ArgumentNullException("elementSelector");
            }
            comparer = comparer ?? EqualityComparer<TKey>.Default;
            ICollection<TSource> list = source as ICollection<TSource>;
            var ret = list == null ? new HashMap<TKey, TElement>(comparer) : new HashMap<TKey, TElement>(list.Count, comparer);
            foreach (TSource item in source)
            {
                ret.Add(keySelector(item), elementSelector(item));
            }
            return ret;
        }

    }
}