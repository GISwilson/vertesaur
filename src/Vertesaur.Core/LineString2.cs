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
using System.Text;
using JetBrains.Annotations;
using Vertesaur.Contracts;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Vertesaur {

	/// <summary>
	/// A connected string of line segments.
	/// </summary>
	/// <remarks>
	/// The name of this class may be confusing. Although a line may be of
	/// infinite length, the word line with respect to this class refers to
	/// a line segment.
	/// </remarks>
	/// <seealso cref="Vertesaur.Segment2"/>
	public class LineString2 :
		Collection<Point2>,
		IPlanarGeometry,
		IHasMagnitude<double>,
		IEquatable<LineString2>,
		IRelatableIntersects<Point2>,
		IHasDistance<Point2,double>,
		ICloneable
	{

		/// <summary>
		/// Constructs a new empty line string.
		/// </summary>
		public LineString2() : this(null) { }

		/// <summary>
		/// Creates a new empty line string expecting the given number of points.
		/// </summary>
		/// <param name="expectedCapacity">The expected number of points.</param>
		public LineString2(int expectedCapacity)
			: this(new List<Point2>(expectedCapacity)) { }

		/// <summary>
		/// Constructs a new line string containing the given ordered set of points.
		/// </summary>
		/// <param name="points">The ordered set of points the line string will be composed of.</param>
		public LineString2([CanBeNull] IEnumerable<Point2> points)
			: this(null == points ? null : new List<Point2>(points)) { }

		/// <summary>
		/// This private constructor is used to initialize the collection with a new list.
		/// All constructors must eventually call this constructor.
		/// </summary>
		/// <param name="points">The list that will store the points. This list MUST be owned by this class.</param>
		/// <remarks>
		/// All public access to the points must be through the Collection wrapper around the points list.
		/// </remarks>
		private LineString2([CanBeNull] List<Point2> points)
			: base(points ?? new List<Point2>()) {
			//_pointList = points;
		}

		/// <inheritdoc/>
		[ContractAnnotation("null=>false")]
		public bool Equals([CanBeNull] LineString2 other) {
			if (ReferenceEquals(null, other))
				return false;
			if (ReferenceEquals(this, other))
				return true;
			if (Count != other.Count)
				return false;

			for (var i = 0; i < Count; i++) {
				if (!this[i].Equals(other[i]))
					return false;
			}
			return true;
		}

		/// <inheritdoc/>
		[ContractAnnotation("null=>false")]
		public override bool Equals([CanBeNull] object obj) {
			return Equals(obj as LineString2);
		}

		/// <inheritdoc/>
		public override int GetHashCode() {
			return GetMbr().GetHashCode() ^ 1020930680;
		}

		/// <inheritdoc/>
		public override string ToString() {
			var sb = new StringBuilder("LineString, " + Count + "point");
			if (1 != Count) {
				sb.Append('s');
			}
			if (Count < 4) {
				for (var i = 0; i < Count; i++) {
					sb.Append(' ');
					sb.Append(this[i].ToString());
				}
			}
			return sb.ToString();
		}

		/// <summary>
		/// Creates an identical line string.
		/// </summary>
		/// <returns>A line string.</returns>
		/// <remarks>Functions as a deep clone.</remarks>
		[NotNull]
		public LineString2 Clone() {
			Contract.Ensures(Contract.Result<LineString2>() != null);
			Contract.EndContractBlock();
			return new LineString2(new List<Point2>(this));
		}

		object ICloneable.Clone() {
			return Clone();
		}

		/// <summary>
		/// The number of line segments the line string is composed of.
		/// </summary>
		[Pure] public int SegmentCount {
			get {
				var c = Count;
				return c == 0 ? 0 : c - 1;
			}
		}

		/// <summary>
		/// Extracts a line segment from the line string at the given segment index <paramref name="i"/>.
		/// </summary>
		/// <param name="i">The segment index.</param>
		/// <returns>A line segment.</returns>
		[NotNull]
		public Segment2 GetSegment(int i) {
			if(i < 0)
				throw new ArgumentOutOfRangeException("i", "i must be a positive number");
			if (i+1 >= Count)
				throw new ArgumentOutOfRangeException("i", "i must be a valid segment index");
			Contract.EndContractBlock();

			return new Segment2(this[i], this[i+1]);
		}

		/// <summary>
		/// Calculates the distance between this line string and the point, <paramref name="p"/>.
		/// </summary>
		/// <param name="p">The point to calculate distance to.</param>
		/// <returns>The distance.</returns>
		public double Distance(Point2 p) {
			return Math.Sqrt(DistanceSquared(p));
		}

		/// <summary>
		/// Calculates the squared distance between this line string and the point, <paramref name="p"/>.
		/// </summary>
		/// <param name="p">The point to calculate squared distance to.</param>
		/// <returns>The squared distance.</returns>
		public double DistanceSquared(Point2 p) {
			var lastIndex = Count - 1;
			if (lastIndex < 1)
				return 0 == lastIndex ? this[0].DistanceSquared(p) : Double.NaN;
			
			var minDistance = Segment2.DistanceSquared(this[0], this[1], p);
			for (int i = 1, nextIndex = 2; i < lastIndex; i = nextIndex++) {
				var localDistance = Segment2.DistanceSquared(this[i], this[nextIndex], p);
				if (localDistance < minDistance)
					minDistance = localDistance;
			}
			return minDistance;
		}

		/// <summary>
		/// Determines if a point intersects this line string.
		/// </summary>
		/// <param name="p">A point to test intersection with.</param>
		/// <returns>True when a point intersects this line string.</returns>
		public bool Intersects(Point2 p) {
			var lastIndex = Count - 1;
			if (lastIndex >= 1) {
				for (int i = 0, nextIndex = 1; i < lastIndex; i = nextIndex++) {
					if (Segment2.Intersects(this[i], this[nextIndex], p))
						return true;
				}
			}
			return lastIndex == 0 && this[0].Equals(p);
		}

		/// <summary>
		/// Calculates a minimum bounding rectangle for this line string.
		/// </summary>
		/// <returns>A minimum bounding rectangle.</returns>
		public Mbr GetMbr() {
			return Mbr.Create(this);
		}

		/// <summary>
		/// Calculates the centroid.
		/// </summary>
		/// <returns>A centroid.</returns>
		public Point2 GetCentroid() {
			var lastIndex = Count - 1;
			if (lastIndex > 0) {
				var mSum = 0.0;
				var xSum = 0.0;
				var ySum = 0.0;
				for (int i = 0, nextIndex = 1; i < lastIndex; i = nextIndex++) {
					var m = this[i].DistanceSquared(this[nextIndex]);
					mSum = mSum + m;
					m = m / 2.0;
					xSum = xSum + ((this[i].X + this[nextIndex].X) * m);
					ySum = ySum + ((this[i].Y + this[nextIndex].Y) * m);
				}
				
				// ReSharper disable CompareOfFloatsByEqualityOperator
				if (0 != mSum) {
					return new Point2(xSum / mSum, ySum / mSum);
				}
				// ReSharper restore CompareOfFloatsByEqualityOperator
			}
			return lastIndex == 0 ? this[0] : Point2.Invalid;
		}

		/// <summary>
		/// Calculates the magnitude of this line string.
		/// </summary>
		/// <returns>The magnitude.</returns>
		public double GetMagnitude() {
			var lastIndex = Count - 1;
			if (1 <= lastIndex) {
				var sum = this[0].Distance(this[1]);
				for (int i = 1, nextIndex = 2; i < lastIndex; i = nextIndex++) {
					sum += this[i].Distance(this[nextIndex]);
				}
				return sum;
			}
			return 0;
		}

		/// <summary>
		/// Calculates the squared magnitude of this line string.
		/// </summary>
		/// <returns>The magnitude.</returns>
		public double GetMagnitudeSquared() {
			var m = GetMagnitude();
			return m * m;
		}

	}
}