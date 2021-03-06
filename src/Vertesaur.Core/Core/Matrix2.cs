﻿using System;
using System.Diagnostics.Contracts;
using Vertesaur;

namespace Vertesaur
{

    /// <summary>
    /// A square matrix with two rows and two columns with an element type of double.
    /// </summary>
    public sealed class Matrix2 :
        IMatrixSquare<double>,
        IMatrixMutable<double>,
        IEquatable<Matrix2>,
        ICloneable
    {

        private const int OrderValue = 2;
        private const int ElementCountValue = OrderValue * OrderValue;

        /// <summary>
        /// Determines if two matrices are equal.
        /// </summary>
        /// <param name="left">A matrix.</param>
        /// <param name="right">A matrix.</param>
        /// <returns>True when equal.</returns>
        public static bool operator ==(Matrix2 left, Matrix2 right) {
            return ReferenceEquals(null, left) ? ReferenceEquals(null, right) : left.Equals(right);
        }

        /// <summary>
        /// Determines if two matrices are not equal.
        /// </summary>
        /// <param name="left">A matrix.</param>
        /// <param name="right">A matrix.</param>
        /// <returns>True when not equal.</returns>
        public static bool operator !=(Matrix2 left, Matrix2 right) {
            return !(ReferenceEquals(null, left) ? ReferenceEquals(null, right) : left.Equals(right));
        }

        /// <summary>
        /// Implements the operator *.
        /// </summary>
        /// <param name="left">The left matrix.</param>
        /// <param name="right">The right matrix.</param>
        /// <returns>The result of multiplying the <paramref name="left"/> matrix by the <paramref name="right"/> matrix.</returns>
        public static Matrix2 operator *(Matrix2 left, Matrix2 right) {
            if (null == left) throw new ArgumentNullException("left");
            if (null == right) throw new ArgumentNullException("right");
            Contract.Ensures(Contract.Result<Matrix2>() != null);
            return left.Multiply(right);
        }

        /// <summary>
        /// Multiplies a matrix by a factor value.
        /// </summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="factor">The factor value to multiply the matrix by.</param>
        /// <returns>A matrix with all elements scaled.</returns>
        public static Matrix2 operator *(Matrix2 matrix, double factor) {
            if (null == matrix) throw new ArgumentNullException("matrix");
            Contract.Ensures(Contract.Result<Matrix2>() != null);
            return matrix.Multiply(factor);
        }

        /// <summary>
        /// Multiplies a matrix by a factor value.
        /// </summary>
        /// <param name="matrix">The matrix to multiply.</param>
        /// <param name="factor">The factor value to multiply the matrix by.</param>
        /// <returns>A matrix with all elements scaled.</returns>
        public static Matrix2 operator *(double factor, Matrix2 matrix) {
            if (null == matrix) throw new ArgumentNullException("matrix");
            Contract.Ensures(Contract.Result<Matrix2>() != null);
            return matrix.Multiply(factor);
        }

        /// <summary>
        /// Implements the operator +.
        /// </summary>
        /// <param name="left">The left matrix.</param>
        /// <param name="right">The right matrix.</param>
        /// <returns>The resulting matrix.</returns>
        public static Matrix2 operator +(Matrix2 left, Matrix2 right) {
            if (null == left) throw new ArgumentNullException("left");
            if (null == right) throw new ArgumentNullException("right");
            Contract.Ensures(Contract.Result<Matrix2>() != null);
            return left.Add(right);
        }

        /// <summary>
        /// Creates a matrix with all elements set to 0.
        /// </summary>
        /// <returns>A matrix of zeros.</returns>
        public static Matrix2 CreateZero() {
            Contract.Ensures(Contract.Result<Matrix2>() != null);
            return new Matrix2(0, 0, 0, 0);
        }

        /// <summary>
        /// The element at row 0 and column 0.
        /// </summary>
        public double E00;
        /// <summary>
        /// The element at row 0 and column 1.
        /// </summary>
        public double E01;
        /// <summary>
        /// The element at row 1 and column 0.
        /// </summary>
        public double E10;
        /// <summary>
        /// The element at row 1 and column 1.
        /// </summary>
        public double E11;

        /// <summary>
        /// Constructs a new identity matrix.
        /// </summary>
        public Matrix2() {
            SetIdentity();
        }

        /// <summary>
        /// Constructs a new matrix with the given element values.
        /// </summary>
        /// <param name="e00">The value for the element at 0,0.</param>
        /// <param name="e01">The value for the element at 0,1.</param>
        /// <param name="e10">The value for the element at 1,0.</param>
        /// <param name="e11">The value for the element at 1,1.</param>
        public Matrix2(
            double e00, double e01,
            double e10, double e11
        ) {
            E00 = e00;
            E01 = e01;
            E10 = e10;
            E11 = e11;
        }

        /// <summary>
        /// Copies the element values from the given matrix.
        /// </summary>
        /// <param name="m">A matrix to copy from.</param>
        public Matrix2(Matrix2 m) {
            if (m == null) throw new ArgumentNullException("m");
            Contract.EndContractBlock();
            CopyFrom(m);
        }

        /// <summary>
        /// Copies the element values from the given matrix.
        /// </summary>
        /// <param name="m">A matrix to copy from.</param>
        public Matrix2(IMatrix<double> m) {
            if (m == null) throw new ArgumentNullException("m");
            if (m.RowCount != OrderValue) throw new ArgumentException("Matrix must have 2 rows", "m");
            if (m.ColumnCount != OrderValue) throw new ArgumentException("Matrix must have 2 columns.", "m");
            Contract.EndContractBlock();
            E00 = m.Get(0, 0);
            E01 = m.Get(0, 1);
            E10 = m.Get(1, 0);
            E11 = m.Get(1, 1);
        }

        /// <summary>
        /// Sets all elements in the matrix.
        /// </summary>
        /// <param name="e00">The value for the element at 0,0.</param>
        /// <param name="e01">The value for the element at 0,1.</param>
        /// <param name="e10">The value for the element at 1,0.</param>
        /// <param name="e11">The value for the element at 1,1.</param>
        public void SetElements(
            double e00, double e01,
            double e10, double e11
        ) {
            E00 = e00;
            E01 = e01;
            E10 = e10;
            E11 = e11;
        }

        /// <summary>
        /// Sets the elements of the matrix to that of the identity matrix.
        /// </summary>
        public void SetIdentity() {
            E00 = E11 = 1.0;
            E01 = E10 = 0;
        }

        private void CopyFrom(Matrix2 m) {
            Contract.Requires(m != null);
            E00 = m.E00;
            E01 = m.E01;
            E10 = m.E10;
            E11 = m.E11;
        }

        /// <summary>
        /// Sets the element at the given row and column to the given <paramref name="value"/>.
        /// </summary>
        /// <param name="r">The row.</param>
        /// <param name="c">The column.</param>
        /// <param name="value">The value to store in the matrix element.</param>
        public void Set(int r, int c, double value) {
            if (c < 0 || c > 1) throw new ArgumentOutOfRangeException("c", "Invalid column.");
            if (r < 0 || r > 1) throw new ArgumentOutOfRangeException("r", "Invalid row.");
            Contract.EndContractBlock();

            if (r == 0) {
                if (c == 0)
                    E00 = value;
                else
                    E01 = value;
            }
            else {
                if (c == 0)
                    E10 = value;
                else
                    E11 = value;
            }
        }

        /// <summary>
        /// Retrieves the element at the given row and column location.
        /// </summary>
        /// <param name="r">The row.</param>
        /// <param name="c">The column.</param>
        /// <returns>The element at the given location.</returns>
        public double Get(int r, int c) {
            if (c < 0 || c > 1) throw new ArgumentOutOfRangeException("c", "Invalid column.");
            if (r < 0 || r > 1) throw new ArgumentOutOfRangeException("r", "Invalid row.");
            Contract.EndContractBlock();

            return c == 0
                ? (0 == r ? E00 : E10)
                : (0 == r ? E01 : E11);
        }

        /// <summary>
        /// Indicates whether another matrix is equal to this instance.
        /// </summary>
        /// <param name="other">A matrix to compare.</param>
        /// <returns><see langword="true"/> when the given matrix is equal to this instance.</returns>
        public bool Equals(Matrix2 other) {
            return !ReferenceEquals(null, other)
                && E00 == other.E00
                && E01 == other.E01
                && E10 == other.E10
                && E11 == other.E11
            ;
        }

        /// <summary>
        /// Indicates whether another matrix is equal to this instance.
        /// </summary>
        /// <param name="other">A matrix to compare.</param>
        /// <returns><see langword="true"/> when the given matrix is equal to this instance.</returns>
        public bool Equals(IMatrix<double> other) {
            return !ReferenceEquals(null, other)
                && other.RowCount == OrderValue
                && other.ColumnCount == OrderValue
                && other.Get(0, 0) == E00
                && other.Get(0, 1) == E01
                && other.Get(1, 0) == E10
                && other.Get(1, 1) == E11;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj) {
            return null != obj && (
                (obj is Matrix2 && Equals(obj as Matrix2))
                ||
                (obj is IMatrix<double> && Equals(obj as IMatrix<double>))
            );
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode() {
            return E00.GetHashCode() ^ E11.GetHashCode() ^ -OrderValue;
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString() {
            return String.Concat(
                E00, ' ', E01, "\n",
                E10, ' ', E11
            );
        }

        /// <inheritdoc/>
        public double CalculateDiagonalProduct() {
            return E00 * E11;
        }

        /// <summary>
        /// Calculates the determinant of the matrix.
        /// </summary>
        /// <returns>The determinant.</returns>
        public double CalculateDeterminant() {
            if (IsUpperTriangular || IsLowerTriangular)
                return E00 * E11;
            if (E00 == 0.0 || E11 == 0.0)
                return -(E01 * E10);
            return (E00 * E11) - (E10 * E01);
        }

        /// <inheritdoc/>
        public bool IsDiagonal {
            get { return 0 == E10 && 0 == E01; }
        }

        /// <inheritdoc/>
        public bool IsScalar {
            get { return E00 == E11 && IsDiagonal; }
        }

        /// <inheritdoc/>
        public bool IsIdentity {
            get { return 1 == E00 && 1 == E11 && IsDiagonal; }
        }

        /// <inheritdoc/>
        public bool IsUpperTriangular {
            get { return 0 == E10; }
        }

        /// <inheritdoc/>
        public bool IsLowerTriangular {
            get { return 0 == E01; }
        }

        /// <inheritdoc/>
        public int Order {
            get { return OrderValue; }
        }

        /// <inheritdoc/>
        public int RowCount {
            get { return OrderValue; }
        }

        /// <inheritdoc/>
        public int ColumnCount {
            get { return OrderValue; }
        }

        /// <inheritdoc/>
        int IMatrix<double>.ElementCount {
            get { return ElementCountValue; }
        }

        /// <summary>
        /// Adds this left matrix and the given <paramref name="right"/> matrix and returns the result.
        /// </summary>
        /// <param name="right">The right matrix to add.</param>
        /// <returns>The result.</returns>
        public Matrix2 Add(Matrix2 right) {
            if (null == right) throw new ArgumentNullException("right");
            Contract.Ensures(Contract.Result<Matrix2>() != null);
            return new Matrix2(
                E00 + right.E00,
                E01 + right.E01,
                E10 + right.E10,
                E11 + right.E11
            );
        }

        /// <summary>
        /// Adds this left matrix by the given <paramref name="right"/> matrix and overwrites this matrix with the sum.
        /// </summary>
        /// <param name="right">The right matrix to add.</param>
        public void AddAssign(Matrix2 right) {
            if (null == right) throw new ArgumentNullException("right");
            Contract.EndContractBlock();
            E00 += right.E00;
            E01 += right.E01;
            E10 += right.E10;
            E11 += right.E11;
        }

        /// <summary>
        /// Subtracts the given <paramref name="right"/> matrix from this left matrix and returns the result.
        /// </summary>
        /// <param name="right">The right matrix to subtract.</param>
        /// <returns>The result.</returns>
        public Matrix2 Subtract(Matrix2 right) {
            if (null == right) throw new ArgumentNullException("right");
            Contract.Ensures(Contract.Result<Matrix2>() != null);
            return new Matrix2(
                E00 - right.E00,
                E01 - right.E01,
                E10 - right.E10,
                E11 - right.E11
            );
        }

        /// <summary>
        /// Subtracts the elements of the <paramref name="right"/> matrix from this matrix and assigns the results back to this matrix.
        /// </summary>
        /// <param name="right">The right matrix use to subtract from this matrix.</param>
        public void SubtractAssign(Matrix2 right) {
            if (null == right) throw new ArgumentNullException("right");
            Contract.EndContractBlock();
            E00 -= right.E00;
            E01 -= right.E01;
            E10 -= right.E10;
            E11 -= right.E11;
        }

        /// <summary>
        /// Multiplies all elements of this matrix by a single factor, returning the result as a new matrix.
        /// </summary>
        /// <param name="factor">The factor to multiply by.</param>
        /// <returns>The scaled matrix.</returns>
        public Matrix2 Multiply(double factor) {
            Contract.Ensures(Contract.Result<Matrix2>() != null);
            return new Matrix2(
                E00 * factor,
                E01 * factor,
                E10 * factor,
                E11 * factor
            );
        }

        /// <summary>
        /// Multiplies all elements of this matrix by a single factor, assigning the result to this matrix.
        /// </summary>
        /// <param name="factor">The factor to multiply by.</param>
        public void MultiplyAssign(double factor) {
            E00 *= factor;
            E01 *= factor;
            E10 *= factor;
            E11 *= factor;
        }

        /// <summary>
        /// Multiplies this matrix by another.
        /// </summary>
        /// <param name="right">The right matrix.</param>
        /// <returns>The result of multiplying this matrix by the <paramref name="right"/> matrix.</returns>
        public Matrix2 Multiply(Matrix2 right) {
            if (null == right) throw new ArgumentNullException("right");
            Contract.Ensures(Contract.Result<Matrix2>() != null);
            return new Matrix2(
                (E00 * right.E00) + (E01 * right.E10),
                (E00 * right.E01) + (E01 * right.E11),
                (E10 * right.E00) + (E11 * right.E10),
                (E10 * right.E01) + (E11 * right.E11)
            );
        }

        /// <summary>
        /// Multiplies this matrix by another and stores the result in this matrix.
        /// </summary>
        /// <param name="right">The right matrix.</param>
        public void MultiplyAssign(Matrix2 right) {
            if (null == right) throw new ArgumentNullException("right");
            Contract.EndContractBlock();
            var t = E00;
            E00 = (t * right.E00) + (E01 * right.E10);
            E01 = (t * right.E01) + (E01 * right.E11);
            t = E10;
            E10 = (t * right.E00) + (E11 * right.E10);
            E11 = (t * right.E01) + (E11 * right.E11);
        }

        /// <summary>
        /// Inverts the matrix.
        /// </summary>
        /// <exception cref="Vertesaur.NoInverseException">An inverse requires a valid non-zero finite determinant.</exception>
        public void Invert() {
            var copy = Clone();
            var result = new Matrix2();
            Contract.Assume(result.IsIdentity);
            if (SquareMatrixOperations.GaussJordanEliminationDestructive(copy, result))
                CopyFrom(result);
            else
                throw new NoInverseException();
        }

        /// <summary>
        /// Generates a matrix which is the inverse.
        /// </summary>
        /// <returns>The inverse of the matrix.</returns>
        /// <exception cref="Vertesaur.NoInverseException">An inverse requires a valid non-zero finite determinant.</exception>
        public Matrix2 GetInverse() {
            Contract.Ensures(Contract.Result<Matrix2>() != null);
            var copy = Clone();
            var result = new Matrix2();
            Contract.Assume(result.IsIdentity);
            if (SquareMatrixOperations.GaussJordanEliminationDestructive(copy, result))
                return result;
            else
                throw new NoInverseException();
        }

        /// <summary>
        /// Transposes this matrix.
        /// </summary>
        public void Transpose() {
            var temp = E01;
            E01 = E10;
            E10 = temp;
        }

        /// <summary>
        /// Generates a new matrix which is the transpose of this matrix.
        /// </summary>
        /// <returns>The transpose of this matrix.</returns>
        public Matrix2 GetTransposed() {
            Contract.Ensures(Contract.Result<Matrix2>() != null);
            return new Matrix2(
                E00, E10,
                E01, E11
            );
        }

        /// <summary>
        /// Interchanges two rows.
        /// </summary>
        /// <param name="rowA">One of the row indices to interchange.</param>
        /// <param name="rowB">The other row index to interchange with.</param>
        [Obsolete("needs testing")]
        public void InterchangeRows(int rowA, int rowB) {
            if (rowA < 0 || rowA > 1)
                throw new ArgumentOutOfRangeException("rowA", "Row must be 0 or 1.");
            if (rowB < 0 || rowB > 1)
                throw new ArgumentOutOfRangeException("rowB", "Row must be 0 or 1.");
            Contract.EndContractBlock();

            if (rowA != rowB) {
                var temp0 = E00;
                var temp1 = E01;
                E00 = E10;
                E01 = E11;
                E10 = temp0;
                E11 = temp1;
            }
        }

        /// <summary>
        /// Multiplies all elements in a row by a scalar.
        /// </summary>
        /// <param name="scalar">The value to multiply the row by.</param>
        /// <param name="row">The row to apply multiplication to.</param>
        [Obsolete("needs testing")]
        public void MultiplyRow(double scalar, int row) {
            if (row < 0 || row > 1) throw new ArgumentOutOfRangeException("row", "Row must be 0 or 1.");
            Contract.EndContractBlock();

            if (row == 0) {
                E00 *= scalar;
                E01 *= scalar;
            }
            else {
                E10 *= scalar;
                E11 *= scalar;
            }
        }

        /// <summary>
        /// Creates a new matrix composed of the same elements.
        /// </summary>
        /// <returns>A copy of this matrix.</returns>
        public Matrix2 Clone() {
            Contract.Ensures(Contract.Result<Matrix2>() != null);
            return new Matrix2(this);
        }

        object ICloneable.Clone() {
            return Clone();
        }

        /// <inheritdoc/>
        public void SwapRows(int ra, int rb) {
            if (ra < 0 || ra > 1) throw new ArgumentOutOfRangeException("ra", "Row must be 0 or 1.");
            if (rb < 0 || rb > 1) throw new ArgumentOutOfRangeException("ra", "Row must be 0 or 1.");
            Contract.EndContractBlock();

            if (ra == rb)
                return;

            var tmp = E00;
            E00 = E10;
            E10 = tmp;

            tmp = E01;
            E01 = E11;
            E11 = tmp;
        }

        /// <inheritdoc/>
        public void SwapColumns(int ca, int cb) {
            if (ca < 0 || ca > 1) throw new ArgumentOutOfRangeException("ca", "Column must be 0 or 1.");
            if (cb < 0 || cb > 1) throw new ArgumentOutOfRangeException("cb", "Row must be 0 or 1.");
            Contract.EndContractBlock();

            if (ca == cb)
                return;

            var tmp = E00;
            E00 = E01;
            E01 = tmp;

            tmp = E10;
            E10 = E11;
            E11 = tmp;
        }

        /// <inheritdoc/>
        public void AddSourceRowToTarget(int sourceRow, int targetRow) {
            if (sourceRow < 0 || sourceRow > 1) throw new ArgumentOutOfRangeException("sourceRow", "Row must be 0 or 1.");
            if (targetRow < 0 || targetRow > 1) throw new ArgumentOutOfRangeException("targetRow", "Row must be 0 or 1.");
            Contract.EndContractBlock();
            
            for (int c = 0; c < OrderValue; c++) {
                Set(targetRow, c, Get(sourceRow, c) + Get(targetRow, c));
            }
        }

        /// <inheritdoc/>
        public void AddSourceRowToTarget(int sourceRow, int targetRow, double factor) {
            if (sourceRow < 0 || sourceRow > 1) throw new ArgumentOutOfRangeException("sourceRow", "Row must be 0 or 1.");
            if (targetRow < 0 || targetRow > 1) throw new ArgumentOutOfRangeException("targetRow", "Row must be 0 or 1.");
            Contract.EndContractBlock();

            for (int c = 0; c < OrderValue; c++) {
                Set(targetRow, c, (Get(sourceRow, c) * factor) + Get(targetRow, c));
            }
        }

        /// <inheritdoc/>
        public void AddSourceColumnToTarget(int sourceColumn, int targetColumn) {
            if (sourceColumn < 0 || sourceColumn > 1) throw new ArgumentOutOfRangeException("sourceColumn", "Column must be 0 or 1.");
            if (targetColumn < 0 || targetColumn > 1) throw new ArgumentOutOfRangeException("targetColumn", "Column must be 0 or 1.");
            Contract.EndContractBlock();

            for (int r = 0; r < OrderValue; r++) {
                Set(r, targetColumn, Get(r, sourceColumn) + Get(r, targetColumn));
            }
        }

        /// <inheritdoc/>
        public void AddSourceColumnToTarget(int sourceColumn, int targetColumn, double factor) {
            if (sourceColumn < 0 || sourceColumn > 1) throw new ArgumentOutOfRangeException("sourceColumn", "Column must be 0 or 1.");
            if (targetColumn < 0 || targetColumn > 1) throw new ArgumentOutOfRangeException("targetColumn", "Column must be 0 or 1.");
            Contract.EndContractBlock();

            for (int r = 0; r < OrderValue; r++) {
                Set(r, targetColumn, (Get(r, sourceColumn) * factor) + Get(r, targetColumn));
            }
        }

        /// <inheritdoc/>
        public void ScaleRow(int r, double value) {
            if (r < 0 || r > 1) throw new ArgumentOutOfRangeException("r", "Row must be 0 or 1.");
            Contract.EndContractBlock();

            if (r == 0) {
                E00 *= value;
                E01 *= value;
            }
            else {
                E10 *= value;
                E11 *= value;
            }
        }

        /// <inheritdoc/>
        public void ScaleColumn(int c, double value) {
            if (c < 0 || c > 1) throw new ArgumentOutOfRangeException("c", "Column must be 0 or 1.");
            Contract.EndContractBlock();

            if (c == 0) {
                E00 *= value;
                E10 *= value;
            }
            else {
                E01 *= value;
                E11 *= value;
            }
        }

        /// <inheritdoc/>
        public void DivideRow(int r, double value) {
            if (r < 0 || r > 1) throw new ArgumentOutOfRangeException("r", "Row must be 0 or 1.");
            Contract.EndContractBlock();

            if (r == 0) {
                E00 /= value;
                E01 /= value;
            }
            else {
                E10 /= value;
                E11 /= value;
            }
        }

        /// <inheritdoc/>
        public void DivideColumn(int c, double value) {
            if (c < 0 || c > 1) throw new ArgumentOutOfRangeException("c", "Column must be 0 or 1.");
            Contract.EndContractBlock();

            if (c == 0) {
                E00 /= value;
                E10 /= value;
            }
            else {
                E01 /= value;
                E11 /= value;
            }
        }
    }
}
