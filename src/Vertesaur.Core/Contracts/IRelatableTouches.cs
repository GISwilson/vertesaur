﻿// ===============================================================================
//
// Copyright (c) 2011,2012 Aaron Dandy 
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// ===============================================================================

using JetBrains.Annotations;

namespace Vertesaur.Contracts {
	/// <summary>
	/// Functionality to determine if this object touches another.
	/// </summary>
	/// <typeparam name="TObject">The other object type.</typeparam>
	public interface IRelatableTouches<in TObject>
	{
		/// <summary>
		/// Determines if this object and some <paramref name="other"/> share only
		/// boundaries.
		/// </summary>
		/// <param name="other">An object.</param>
		/// <returns>True when only boundaries occupy the same space.</returns>
		/// <remarks>
		/// Two objects are touching only when they intersect only at their boundaries
		/// and there is no intersection between interior regions. At least one object
		/// must have a dimensionality greater than 0, meaning both can not be points.
		/// </remarks>
		bool Touches([CanBeNull] TObject other);
	}
}