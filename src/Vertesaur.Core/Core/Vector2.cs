﻿using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace Vertesaur
{
    /// <summary>
    /// A vector in 2D space.
    /// </summary>
    public struct Vector2 :
        IVector2<double>,
        IEquatable<Vector2>,
        IComparable<Vector2>
    {

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="a">A vector from the left argument.</param>
        /// <param name="b">A vector from the right argument.</param>
        /// <returns>True if both vectors have the same component values.</returns>
        public static bool operator ==(Vector2 a, Vector2 b) {
            return a.Equals(b);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="a">A vector from the left argument.</param>
        /// <param name="b">A vector from the right argument.</param>
        /// <returns>True if both vectors do not have the same component values.</returns>
        public static bool operator !=(Vector2 a, Vector2 b) {
            return !a.Equals(b);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="leftHandSide">A vector from the left argument.</param>
        /// <param name="rightHandSide">A vector from the right argument.</param>
        /// <returns>The result.</returns>
        public static Vector2 operator +(Vector2 leftHandSide, Vector2 rightHandSide) {
            return leftHandSide.Add(rightHandSide);
        }

        /// <summary>
        /// Implements the operator -.
        /// </summary>
        /// <param name="leftHandSide">A vector from the left argument.</param>
        /// <param name="rightHandSide">A vector from the right argument.</param>
        /// <returns>The result.</returns>
        public static Vector2 operator -(Vector2 leftHandSide, Vector2 rightHandSide) {
            return leftHandSide.Difference(rightHandSide);
        }

        /// <summary>
        /// Implements the operator * as the dot operator.
        /// </summary>
        /// <param name="leftHandSide">A vector from the left argument.</param>
        /// <param name="rightHandSide">A vector from the right argument.</param>
        /// <returns>The dot product.</returns>
        public static double operator *(Vector2 leftHandSide, Vector2 rightHandSide) {
            return leftHandSide.Dot(rightHandSide);
        }

        /// <summary>
        /// Multiplies the vector by a scalar.
        /// </summary>
        /// <param name="tuple">The vector to multiply.</param>
        /// <param name="factor">The scalar value to multiply by.</param>
        /// <returns>The resulting scaled vector.</returns>
        public static Vector2 operator *(Vector2 tuple, double factor) {
            return tuple.GetScaled(factor);
        }

        /// <summary>
        /// Multiplies the vector by a scalar.
        /// </summary>
        /// <param name="tuple">The vector to multiply.</param>
        /// <param name="factor">The scalar value to multiply by.</param>
        /// <returns>The resulting scaled vector.</returns>
        public static Vector2 operator *(double factor, Vector2 tuple) {
            return tuple.GetScaled(factor);
        }

        /// <inheritdoc/>
        public static implicit operator Point2(Vector2 value) {
            return new Point2(value);
        }

        /// <summary>
        /// A vector with all components set to zero.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly Vector2 Zero = new Vector2(0, 0);
        /// <summary>
        /// A vector with all components set to zero.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly Vector2 Invalid = new Vector2(Double.NaN, Double.NaN);
        /// <summary>
        /// A vector with a magnitude of one and oriented in the direction of the positive X axis.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly Vector2 XUnit = new Vector2(1, 0);
        /// <summary>
        /// A vector with a magnitude of one and oriented in the direction of the positive Y axis.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static readonly Vector2 YUnit = new Vector2(0, 1);

        /// <summary>
        /// The x-coordinate of this vector.
        /// </summary>
        public readonly double X;

        /// <summary>
        /// The y-coordinate of this vector.
        /// </summary>
        public readonly double Y;

        /// <summary>
        /// Creates a 2D vector.
        /// </summary>
        /// <param name="x">The x-coordinate.</param>
        /// <param name="y">The y-coordinate.</param>
        public Vector2(double x, double y) {
            X = x;
            Y = y;
        }
        /// <summary>
        /// Creates a 2D vector.
        /// </summary>
        /// <param name="v">The coordinate tuple to copy values from.</param>
        public Vector2(ICoordinatePair<double> v) {
            if (null == v) throw new ArgumentNullException("v");
            Contract.EndContractBlock();
            X = v.X;
            Y = v.Y;
        }

        /// <summary>
        /// Clones a vector from a point.
        /// </summary>
        /// <param name="p">The point to clone.</param>
        public Vector2(Point2 p)
            : this(p.X, p.Y) { }

        /// <summary>
        /// The x-coordinate of this point.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        double ICoordinatePair<double>.X { get { return X; } }
        /// <summary>
        /// The y-coordinate of this point.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        double ICoordinatePair<double>.Y { get { return Y; } }

        /// <inheritdoc/>
        [Pure] public int CompareTo(Vector2 other) {
            var c = X.CompareTo(other.X);
            return 0 == c ? Y.CompareTo(other.Y) : c;
        }

        /// <inheritdoc/>
        [Pure] public bool Equals(Vector2 other) {
            return X == other.X && Y == other.Y;
        }

        /// <inheritdoc/>
        [Pure] public bool Equals(ICoordinatePair<double> other) {
            return !ReferenceEquals(null, other) && X == other.X && Y == other.Y;
        }

        /// <inheritdoc/>
        [Pure] public override bool Equals(object obj) {
            return !ReferenceEquals(null, obj) && (
                (obj is Vector2 && Equals((Vector2)obj))
                || Equals(obj as ICoordinatePair<double>)
            );
        }

        /// <inheritdoc/>
        [Pure] public override int GetHashCode() {
            return X.GetHashCode();
        }

        /// <inheritdoc/>
        [Pure] public override string ToString() {
            return String.Concat(X, ' ', Y);
        }

        /// <summary>
        /// Calculates the magnitude of this vector.
        /// </summary>
        /// <returns>The magnitude.</returns>
        [Pure] public double GetMagnitude() {
            Contract.Ensures(
                Contract.Result<double>() >= 0.0
                || Double.IsNaN(Contract.Result<double>())
                || Double.IsPositiveInfinity(Contract.Result<double>()));
            return Math.Sqrt((X * X) + (Y * Y));
        }

        /// <summary>
        /// Calculates the squared magnitude of this vector.
        /// </summary>
        /// <returns>The squared magnitude.</returns>
        [Pure] public double GetMagnitudeSquared() {
            Contract.Ensures(
                Contract.Result<double>() >= 0.0
                || Double.IsNaN(Contract.Result<double>())
                || Double.IsPositiveInfinity(Contract.Result<double>()));
            return (X * X) + (Y * Y);
        }

        /// <summary>
        /// Calculates a vector resulting from adding the given vector to this vector.
        /// </summary>
        /// <param name="rightHandSide">The vector to add.</param>
        /// <returns>A result of adding this vector with the given vector.</returns>
        [Pure] public Vector2 Add(Vector2 rightHandSide) {
            return new Vector2(
                X + rightHandSide.X,
                Y + rightHandSide.Y
            );
        }

        /// <summary>
        /// Calculates the point offset by this vector and a given point.
        /// </summary>
        /// <param name="rightHandSide">The point to add this vector to.</param>
        /// <returns>A point offset by this vector from the given point.</returns>
        [Pure] public Point2 Add(Point2 rightHandSide) {
            return new Point2(
                X + rightHandSide.X,
                Y + rightHandSide.Y
            );
        }

        /// <summary>
        /// Calculates a vector resulting from subtracting the given vector to this vector.
        /// </summary>
        /// <param name="rightHandSide">The vector to subtract.</param>
        /// <returns>A result of subtracting the given vector from this vector.</returns>
        [Pure] public Vector2 Difference(Vector2 rightHandSide) {
            return new Vector2(
                X - rightHandSide.X,
                Y - rightHandSide.Y
            );
        }

        /// <summary>
        /// Calculates the difference resulting from subtracting the given point from this vector.
        /// </summary>
        /// <param name="rightHandSide">The point to subtract.</param>
        /// <returns>A result of subtracting the given point from this vector.</returns>
        [Pure] public Point2 Difference(Point2 rightHandSide) {
            return new Point2(
                X - rightHandSide.X,
                Y - rightHandSide.Y
            );
        }

        /// <summary>
        /// Calculates a vector with the same direction but a magnitude of one.
        /// </summary>
        /// <returns>A unit length vector.</returns>
        [Pure] public Vector2 GetNormalized() {
            var m = GetMagnitude();
            return 0 == m ? Zero : new Vector2(X / m, Y / m);
        }

        /// <summary>
        /// Calculates the dot product between this vector and another vector.
        /// </summary>
        /// <param name="rightHandSide">Another vector to use for the calculation of the dot product.</param>
        /// <returns>The dot product.</returns>
        [Pure] public double Dot(Vector2 rightHandSide) {
            return (X * rightHandSide.X) + (Y * rightHandSide.Y);
        }

        [Pure] internal double Dot(ref Vector2 rightHandSide) {
            return (X * rightHandSide.X) + (Y * rightHandSide.Y);
        }

        /// <summary>
        /// Calculates a vector oriented in the opposite direction.
        /// </summary>
        /// <returns>A vector with the same component values but different signs.</returns>
        [Pure] public Vector2 GetNegative() {
            return new Vector2(-X, -Y);
        }

        /// <summary>
        /// Creates a new vector which is scaled from this vector.
        /// </summary>
        /// <param name="factor">The scaling factor.</param>
        /// <returns>A scaled vector.</returns>
        [Pure] public Vector2 GetScaled(double factor) {
            return new Vector2(X * factor, Y * factor);
        }

        /// <summary>
        /// Creates a new vector resulting from dividing the elements by a given <paramref name="denominator"/>.
        /// </summary>
        /// <param name="denominator">The value to divide all elements by.</param>
        /// <returns>A new vector with elements that are the result of division.</returns>
        [Pure] public Vector2 GetDivided(double denominator) {
            return new Vector2(X / denominator, Y / denominator);
        }

        /// <summary>
        /// Calculates the dot product of this vector and a vector perpendicular to the other vector.
        /// </summary>
        /// <param name="rightHandSide">A vector.</param>
        /// <returns>The z-coordinate of the cross product.</returns>
        /// <remarks>Also calculates the z-coordinate of the cross product of this vector and another vector.</remarks>
        [Pure] public double PerpendicularDot(Vector2 rightHandSide) {
            return (X * rightHandSide.Y) - (Y * rightHandSide.X);
        }

        [Pure] internal double PerpendicularDot(ref Vector2 rightHandSide) {
            return (X * rightHandSide.Y) - (Y * rightHandSide.X);
        }

        /// <summary>
        /// Gets a clock-wise perpendicular vector with the same magnitude as this vector.
        /// </summary>
        /// <returns>A vector.</returns>
        [Pure] public Vector2 GetPerpendicularClockwise() {
            return new Vector2(Y, -X);
        }

        /// <summary>
        /// Gets a counter clock-wise perpendicular vector with the same magnitude as this vector.
        /// </summary>
        /// <returns>A vector.</returns>
        [Pure] public Vector2 GetPerpendicularCounterClockwise() {
            return new Vector2(-Y, X);
        }

        /// <summary>
        /// Determines if the vector is valid.
        /// </summary>
        public bool IsValid {
            [Pure] get { return !Double.IsNaN(X) && !Double.IsNaN(Y); }
        }

    }
}
