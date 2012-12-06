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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;
using Vertesaur.Contracts;

namespace Vertesaur {

	/// <summary>
	/// A straight ray of infinite length emanating from a point.
	/// </summary>
	public sealed class Ray2 :
		IRay2<double>,
		IEquatable<Ray2>,
		IHasMbr<Mbr,double>,
		IHasDistance<Point2,double>,
		IRelatableIntersects<Point2>,
		IRelatableIntersects<Segment2>,
		IRelatableIntersects<Ray2>,
		IRelatableIntersects<Line2>,
		ISpatiallyEquatable<Ray2>,
		IHasIntersectionOperation<Segment2, IPlanarGeometry>,
		IHasIntersectionOperation<Ray2, IPlanarGeometry>,
		IHasIntersectionOperation<Line2, IPlanarGeometry>,
		ICloneable
	{

		/// <summary>
		/// The point of origin for the ray.
		/// </summary>
		public readonly Point2 P;

		/// <summary>
		/// The direction of the ray from P.
		/// </summary>
		public readonly Vector2 Direction;

		/// <summary>
		/// Constructs a ray with infinite length originating from <paramref name="p"/> with direction <paramref name="d"/>.
		/// </summary>
		/// <param name="p">Point of origin.</param>
		/// <param name="d">The direction of the ray.</param>
		public Ray2(Point2 p, Vector2 d) {
			P = p;
			Direction = d;
		}
		/// <summary>
		/// Constructs a ray with infinite length originating from <paramref name="p"/> with direction <paramref name="v"/>.
		/// </summary>
		/// <param name="p">Point of origin.</param>
		/// <param name="v">The direction of the ray.</param>
		public Ray2([NotNull] IPoint2<double> p, [NotNull] IVector2<double> v)
			: this(new Point2(p), new Vector2(v))
		{
			Contract.Requires(p != null);
			Contract.Requires(v != null);
		}
		/// <summary>
		/// Constructs a ray with infinite length originating from <paramref name="a"/> and passing through <paramref name="b"/>.
		/// </summary>
		/// <param name="a">Point of origin.</param>
		/// <param name="b">A point on the ray.</param>
		public Ray2(Point2 a, Point2 b)
			: this(a, b - a) { }
		/// <summary>
		/// Constructs a ray with infinite length originating from <paramref name="a"/> and passing through <paramref name="b"/>.
		/// </summary>
		/// <param name="a">Point of origin.</param>
		/// <param name="b">A point on the ray.</param>
		public Ray2([NotNull] IPoint2<double> a, [NotNull] IPoint2<double> b)
			: this(new Point2(a), new Point2(b))
		{
			Contract.Requires(a != null);
			Contract.Requires(b != null);
		}
		/// <summary>
		/// Constructs a ray identical to the given <paramref name="ray"/>.
		/// </summary>
		/// <param name="ray">A ray.</param>
		public Ray2([NotNull] Ray2 ray) {
			if(null == ray) throw new ArgumentNullException("ray");
			Contract.EndContractBlock();
			P = ray.P;
			Direction = ray.Direction;
		}

		/// <inheritdoc/>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IPoint2<double> IRay2<double>.P { get { return P; } }

		/// <inheritdoc/>
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		IVector2<double> IRay2<double>.Direction { get { return Direction; } }

		/// <summary>
		/// Determines if the ray is valid.
		/// </summary>
		public bool IsValid {
			get { return P.IsValid && Direction.IsValid && Direction != Vector2.Zero; }
		}

		/// <summary>
		/// Indicates whether the current object is equal to another object of the same type.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.
		/// </returns>
		/// <param name="other">An object to compare with this object.</param>
		[ContractAnnotation("null=>false")]
		public bool Equals([CanBeNull] Ray2 other) {
			return !ReferenceEquals(null, other)
				&& P.Equals(other.P)
				&& Direction.Equals(other.Direction);
		}

		/// <summary>
		/// Determines if this ray is geometrically equal to another. 
		/// </summary>
		/// <param name="other">Another ray</param>
		/// <returns><c>true</c> if the rays are spatially equal.</returns>
		[ContractAnnotation("null=>false")]
		public bool SpatiallyEqual(Ray2 other) {
			return !ReferenceEquals(null, other)
				&& P.Equals(other.P)
				&& (
					Direction.Equals(other.Direction)
					||
					Direction.GetNormalized().Equals(other.Direction.GetNormalized())
				);
		}

		/// <inheritdoc/>
		[ContractAnnotation("null=>false")]
		public override bool Equals([CanBeNull] object obj) {
			return Equals(obj as Ray2);
		}

		/// <inheritdoc/>
		public override int GetHashCode() {
			return P.GetHashCode() ^ -123456;
		}

		/// <inheritdoc/>
		public override string ToString() {
			return String.Concat('(', P, ") (", Direction, ')');
		}

		/// <summary>
		/// Calculates the length of this ray.
		/// </summary>
		/// <returns>The length.</returns>
		public double GetMagnitude() {
			return Vector2.Zero.Equals(Direction) ? 0 : Double.PositiveInfinity;
		}

		/// <summary>
		/// Calculates the squared length of this ray.
		/// </summary>
		/// <returns>The length.</returns>
		public double GetMagnitudeSquared() {
			return GetMagnitude();
		}

		/// <summary>
		/// Creates a copy of this ray.
		/// </summary>
		/// <returns>
		/// A new identical ray.
		/// </returns>
		/// <filterpriority>2</filterpriority>
		/// <remarks>Functions as a deep clone.</remarks>
		[NotNull]
		public Ray2 Clone() {
			Contract.Ensures(Contract.Result<Ray2>() != null);
			Contract.EndContractBlock();
			return new Ray2(this);
		}

		object ICloneable.Clone() {
			return Clone();
		}

		/// <summary>
		/// Calculates a minimum bounding rectangle for this ray.
		/// </summary>
		/// <returns>A minimum bounding rectangle.</returns>
		public Mbr GetMbr() {
			// ReSharper disable CompareOfFloatsByEqualityOperator
			return (
				(0 == Direction.X)
				? (
					(0 == Direction.Y)
					? new Mbr(P)
					: (
						(Direction.Y > 0)
						? new Mbr(P.X, P.Y, P.X, Double.PositiveInfinity)
						: new Mbr(P.X, Double.NegativeInfinity, P.X, P.Y)
					)
				)
				: (
					(0 == Direction.Y)
					? (
						(Direction.X > 0)
						? new Mbr(P.X, P.Y, Double.PositiveInfinity, P.Y)
						: new Mbr(Double.NegativeInfinity, P.Y, P.X, P.Y)
					)
					: (
						(Direction.X > 0)
						? (
							(Direction.Y > 0)
							? new Mbr(P.X, P.Y, Double.PositiveInfinity, Double.PositiveInfinity)
							: new Mbr(P.X, Double.NegativeInfinity, Double.PositiveInfinity, P.Y)
						)
						: (
							(Direction.Y > 0)
							? new Mbr(Double.NegativeInfinity, P.Y, P.X, Double.PositiveInfinity)
							: new Mbr(Double.NegativeInfinity, Double.NegativeInfinity, P.X, P.Y)
						)
					)
				)
			);
			// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		/// <summary>
		/// Calculates the distance between this ray and <paramref name="p"/>
		/// </summary>
		/// <param name="p">The point to calculate distance to.</param>
		/// <returns>The distance.</returns>
		public double Distance(Point2 p) {
			var v0 = p - P;
			var aDot = Direction.Dot(v0);
			return (
				(aDot <= 0)
				? v0.GetMagnitude()
				: Math.Sqrt((v0.GetMagnitudeSquared() - ((aDot * aDot) / Direction.GetMagnitudeSquared())))
			);
		}

		/// <summary>
		/// Calculates the squared distance between this ray and <paramref name="p"/>
		/// </summary>
		/// <param name="p">The point to calculate squared distance to.</param>
		/// <returns>The squared distance.</returns>
		public double DistanceSquared(Point2 p) {
			var v0 = p - P;
			var aDot = Direction.Dot(v0);
			return (
				(aDot <= 0)
				? v0.GetMagnitudeSquared()
				: (v0.GetMagnitudeSquared() - ((aDot * aDot) / Direction.GetMagnitudeSquared()))
			);
		}

		/// <summary>
		/// Determines if a <paramref name="p"/> intersects this ray.
		/// </summary>
		/// <param name="p">A point.</param>
		/// <returns>True when intersecting.</returns>
		public bool Intersects(Point2 p) {
			// ReSharper disable CompareOfFloatsByEqualityOperator
			var v0 = p - P;
			var aDot = Direction.Dot(v0);
			return (
				(aDot <= 0)
				? 0 == v0.X && 0 == v0.Y
				: v0.GetMagnitudeSquared() == ((aDot * aDot) / Direction.GetMagnitudeSquared())
			);
			// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		/// <summary>
		/// Determines if this ray intersect another <paramref name="segment"/>.
		/// </summary>
		/// <param name="segment">A segment.</param>
		/// <returns><c>true</c> when another object intersects this object.</returns>
		[ContractAnnotation("null=>false")]
		public bool Intersects(Segment2 segment) {
			if (ReferenceEquals(null, segment))
				return false;

			// ReSharper disable CompareOfFloatsByEqualityOperator
			var b = P + Direction; // candidate for variable recycling
			var c = segment.A;
			var d = segment.B;
			var dymcy = d.Y - c.Y;
			var bxmax = b.X - P.X;
			var dxmcx = d.X - c.X;
			var bymay = b.Y - P.Y;
			var den = (dymcy * bxmax) - (dxmcx * bymay);
			if (0 == den) {
				return Intersects(segment.A) || Intersects(segment.B);
			}
			var aymcy = P.Y - c.Y;
			var axmcx = P.X - c.X;
			if ((((dxmcx * aymcy) - (dymcy * axmcx)) / den) >= 0) {
				var ub = ((bxmax * aymcy) - (bymay * axmcx)) / den;
				return ub >= 0 && ub <= 1;
			}
			return false;
			// ReSharper restore CompareOfFloatsByEqualityOperator
		}


		/// <summary>
		/// Determines if this ray intersect another <paramref name="ray"/>.
		/// </summary>
		/// <param name="ray">A ray.</param>
		/// <returns><c>true</c> when another object intersects this object.</returns>
		[ContractAnnotation("null=>false")]
		public bool Intersects(Ray2 ray) {
			return null != Intersection(ray);
		}

		/// <summary>
		/// Determines if this ray intersect another <paramref name="line"/>.
		/// </summary>
		/// <param name="line">A line.</param>
		/// <returns><c>true</c> when another object intersects this object.</returns>
		[ContractAnnotation("null=>false")]
		public bool Intersects(Line2 line) {
			return null != Intersection(line);
		}

		/// <summary>
		/// Calculates the intersection geometry between this ray and a segment.
		/// </summary>
		/// <param name="segment">The segment to find the intersection with.</param>
		/// <returns>The intersection geometry or <c>null</c> for no intersection.</returns>
		[ContractAnnotation("null=>null")]
		public IPlanarGeometry Intersection(Segment2 segment) {
			if(ReferenceEquals(null, segment))
				return null;
			throw new NotImplementedException();
		}
		/// <summary>
		/// Calculates the intersection geometry between this ray and another.
		/// </summary>
		/// <param name="ray">The ray to find the intersection with.</param>
		/// <returns>The intersection geometry or <c>null</c> for no intersection.</returns>
		[ContractAnnotation("null=>null")]
		public IPlanarGeometry Intersection(Ray2 ray) {
			// ReSharper disable CompareOfFloatsByEqualityOperator
			if (ReferenceEquals(null, ray))
				return null;
			Point2 a, c;
			Vector2 d0, d1;
			// next order the segments
			var compareResult = P.CompareTo(ray.P);
			if (0 < ((compareResult == 0) ? Direction.CompareTo(ray.Direction) : compareResult)) {
				a = ray.P;
				c = P;
				d0 = ray.Direction;
				d1 = Direction;
			} else {
				a = P;
				c = ray.P;
				d0 = Direction;
				d1 = ray.Direction;
			}
			var e = c - a;
			var cross = (d0.X * d1.Y) - (d1.X * d0.Y);
			var magnitudeSquared0 = d0.GetMagnitudeSquared();

			if (cross * cross > magnitudeSquared0 * d1.GetMagnitudeSquared() * Double.Epsilon) {
				// not parallel
				var s = ((e.X * d1.Y) - (e.Y * d1.X)) / cross;
				if (s < 0)
					return null; // not intersecting on this ray

				var t = ((e.X * d0.Y) - (e.Y * d0.X)) / cross;
				if (t < 0)
					return null; // not intersecting on other segment

				if (0 == s)
					return a;
				if (0 == t)
					return c;
				return a + d0.GetScaled(s); // it must intersect at a point, so find where
			}

			// parallel
			cross = (e.X * d0.Y) - (e.Y * d0.X);
			if (cross * cross > magnitudeSquared0 * e.GetMagnitudeSquared() * Double.Epsilon)
				return null; // no intersection

			var sa = d0.Dot(e) / magnitudeSquared0;
			var sb = sa + (d0.Dot(d1) / magnitudeSquared0);
			var sd = sb - sa;


			if(sd < 0) {
				// going against the grain
				if(sa == 0)
					return a;
				if(sa > 0)
					return new Segment2(a, c);
				return null; // they don’t touch
			}
			if (sd > 0) {
				// going with the grain
				return sa > 0
					? new Ray2(c, d1)
					: new Ray2(a, d0);
			}
			return null;
			// ReSharper restore CompareOfFloatsByEqualityOperator
		}

		/// <summary>
		/// Calculates the intersection geometry between this ray and a line.
		/// </summary>
		/// <param name="line">The line to find the intersection with.</param>
		/// <returns>The intersection geometry or <c>null</c> for no intersection.</returns>
		[ContractAnnotation("null=>null")]
		public IPlanarGeometry Intersection(Line2 line) {
			if(ReferenceEquals(null, line))
				return null;
			throw new NotImplementedException();
		}

	}
}