﻿// ===============================================================================
//
// Copyright (c) 2011 Aaron Dandy 
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using Vertesaur.Contracts;

namespace Vertesaur {

	// ReSharper disable LoopCanBeConvertedToQuery

	/// <summary>
	/// A collection of line strings.
	/// </summary>
	/// <seealso cref="Vertesaur.LineString2"/>
	public class MultiLineString2 :
		Collection<LineString2>,
		IPlanarGeometry,
		IHasMagnitude<double>,
		IEquatable<MultiLineString2>,
		IRelatableIntersects<Point2>,
		IHasDistance<Point2,double>,
		IHasMbr<Mbr,double>,
		IHasCentroid<Point2>,
		ICloneable
	{

		/// <summary>
		/// Constructs a new empty multi-line string.
		/// </summary>
		public MultiLineString2() { }
		/// <summary>
		/// Constructs a new empty multi-line string expecting the given number of line strings.
		/// </summary>
		/// <param name="expectedCapacity">The expected number of line strings.</param>
		public MultiLineString2(int expectedCapacity)
			: this(new List<LineString2>(expectedCapacity)) { }
		/// <summary>
		/// Constructs a new multi-line string containing the given line strings.
		/// </summary>
		/// <param name="lineStrings">The line strings.</param>
		public MultiLineString2(IEnumerable<LineString2> lineStrings)
			: this(null == lineStrings ? null : new List<LineString2>(lineStrings)) { }
		
		/// <summary>
		/// This private constructor is used to initialize the collection with a new list.
		/// All constructors must eventually call this constructor.
		/// </summary>
		/// <param name="lineStrings">The list that will store the line strings. This list MUST be owned by this class.</param>
		/// <remarks>
		/// All public access to the points must be through the Collection wrapper around the points list.
		/// </remarks>
		private MultiLineString2(List<LineString2> lineStrings)
			: base(lineStrings ?? new List<LineString2>()) { }

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		public bool Equals(MultiLineString2 other) {
			if (ReferenceEquals(null, other))
				return false; 
			if (ReferenceEquals(this, other))
				return true;
			if (Count != other.Count)
				return false;

			for (var i = 0; i < Count; i++) {
				if (!this[i].Equals(other[i])) {
					return false;
				}
			}
			return true;
		}

		/// <inheritdoc/>
		public override bool Equals(object obj) {
			return Equals(obj as MultiLineString2);
		}

		/// <inheritdoc/>
		public override int GetHashCode() {
			return GetMbr().GetHashCode() ^ -700084810;
		}


		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString() {
			var result = "MultiLineString, " + Count + " LineString";
			return 1 != Count ? String.Concat(result, 's') : result;
		}

		/// <summary>
		/// Clones this multi-line string.
		/// </summary>
		/// <returns>A multi-line string.</returns>
		/// <remarks>Functions as a deep clone.</remarks>
		public MultiLineString2 Clone() {
			Contract.Ensures(Contract.Result<MultiLineString2>() != null);

			var lines = new List<LineString2>(Count);
			for (var i = 0; i < Count; i++) {
				lines.Add(this[i].Clone());
			}
			return new MultiLineString2(lines);
		}

		object ICloneable.Clone() {
			return Clone();
		}

		/// <summary>
		/// Calculates a minimum bounding rectangle for this multi-line string.
		/// </summary>
		/// <returns>A minimum bounding rectangle.</returns>
		public Mbr GetMbr() {
			if (Count <= 0)
				return null;
			
			var mbr = this[0].GetMbr();
			for (var i = 1; i < Count; i++) {
				mbr = mbr.Encompass(this[i].GetMbr());
			}
			return mbr;
		}

		/// <summary>
		/// Calculates the centroid of all the line strings.
		/// </summary>
		/// <returns>A centroid.</returns>
		public Point2 GetCentroid() {
			// ReSharper disable CompareOfFloatsByEqualityOperator
			var lastIndex = Count - 1;
			if (lastIndex > 0) {
				var mSum = 0.0;
				var xSum = 0.0;
				var ySum = 0.0;
				foreach (var ls in this) {
					if (0 == ls.Count)
						continue;
					
					var m = ls.GetMagnitude();
					var p = ls.GetCentroid();
					xSum += p.X * m;
					ySum += p.Y * m;
					mSum += m;
				}

				if (0 != mSum)
					return new Point2(xSum / mSum, ySum / mSum);
			}
			return lastIndex == 0 ? this[0].GetCentroid() : Point2.Invalid;
			// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		/// <summary>
		/// Calculates the magnitude of this multi-line string which is the sum of the magnitudes for all line strings within.
		/// </summary>
		/// <returns>The magnitude or length.</returns>
		public double GetMagnitude() {
			if (0 == Count)
				return 0;

			var sum = this[0].GetMagnitude();
			for (var i = 1; i < Count; i++) {
				sum += this[i].GetMagnitude();
			}
			return sum;
		}
		/// <summary>
		/// Calculates the squared magnitude of this multi-line string.
		/// </summary>
		/// <returns>The squared magnitude or squared length.</returns>
		public double GetMagnitudeSquared() {
			var m = GetMagnitude();
			return m * m;
		}

		/// <summary>
		/// Calculates the distance between this multi-line string and the point, <paramref name="p"/>.
		/// </summary>
		/// <param name="p">The point to calculate distance to.</param>
		/// <returns>The distance.</returns>
		public double Distance(Point2 p) {
			return Math.Sqrt(DistanceSquared(p));
		}

		/// <summary>
		/// Calculates the squared distance between this multi-line string and the point, <paramref name="p"/>
		/// </summary>
		/// <param name="p">The point to calculate squared distance to.</param>
		/// <returns>The squared distance.</returns>
		public double DistanceSquared(Point2 p) {
			if (0 == Count)
				return 0;

			var minDist = this[0].DistanceSquared(p);
			for (var i = 1; i < Count; i++) {
				var localDist = this[i].DistanceSquared(p);
				if (localDist < minDist)
					minDist = localDist;
			}
			return minDist;
		}

		/// <summary>
		/// Determines if a point intersects this multi-line string.
		/// </summary>
		/// <param name="p">A point to test intersection with.</param>
		/// <returns>True when a point intersects this multi-line string.</returns>
		public bool Intersects(Point2 p) {
			for(var i = 0; i < Count; i++) {
				if (this[i].Intersects(p))
					return true;
			}
			return false;
		}

	}

	// ReSharper restore LoopCanBeConvertedToQuery
}
