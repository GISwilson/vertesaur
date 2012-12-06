﻿// ===============================================================================
//
// Copyright (c) 2012 Aaron Dandy 
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
using System.Linq;
using JetBrains.Annotations;
using Vertesaur.Contracts;
using System.Collections;
using System.Diagnostics.Contracts;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Vertesaur.Transformation
{

	/// <summary>
	/// A transformation that is composed of a sequence of composite transformations.
	/// </summary>
	public class ConcatenatedTransformation : ITransformation
	{

		[NotNull] private readonly ITransformation[] _transformations;

		/// <summary>
		/// Creates a new concatenated transformation composed of a sequence of transformations.
		/// </summary>
		/// <param name="transformations">A sequence of transformations.</param>
		public ConcatenatedTransformation([NotNull, InstantHandle] IEnumerable<ITransformation> transformations) {
			if (null == transformations)
				throw new ArgumentNullException("transformations");
			Contract.EndContractBlock();

			var txArray = transformations.ToArray();
			for (int i = 0; i < txArray.Length; i++)
				if (txArray[i] == null)
					throw new ArgumentException("null transformations are not valid.");

			_transformations = txArray;
				
		}

		/// <summary>
		/// Creates a new concatenated transformation composed of a sequence of transformations.
		/// </summary>
		/// <param name="transformations">A sequence of transformations.</param>
		/// <remarks>
		/// The parameter is used as is so make sure to clone before passing it in.
		/// </remarks>
		private ConcatenatedTransformation([NotNull] ITransformation[] transformations) {
			Contract.Requires(transformations != null);
			Contract.Requires(Contract.ForAll(transformations, x => null != x));
			Contract.EndContractBlock();

			_transformations = transformations;
		}

		/// <summary>
		/// Generates an inverse set of transformation operations that represent the inverse of this transformations..
		/// </summary>
		/// <returns></returns>
		[NotNull]
		protected ITransformation[] CreateInverseOperations() {
			if(!HasInverse)
				throw new NoInverseException();
			Contract.Ensures(Contract.Result<ITransformation[]>() != null);
			Contract.EndContractBlock();

			var inverseTransformations = new ITransformation[_transformations.Length];
			for(int i = 0; i < inverseTransformations.Length; i++) {
				var tx = _transformations[_transformations.Length - 1 - i];
				var ix = tx.GetInverse();
				inverseTransformations[i] = ix;
			}
			return inverseTransformations;
		}

		/// <summary>
		/// Creates a new concatenated transformation that is the inverse of this transformation.
		/// </summary>
		/// <returns>The inverse transformation.</returns>
		[NotNull]
		protected virtual ConcatenatedTransformation CreateInverseConcatenatedOperation() {
			Contract.Requires(HasInverse);
			Contract.Ensures(Contract.Result<ConcatenatedTransformation>() != null);
			Contract.EndContractBlock();

			return new ConcatenatedTransformation(CreateInverseOperations());
		}

		/// <summary>
		/// Gets the transformations that make up this concatenated transformation.
		/// </summary>
		public ReadOnlyCollection<ITransformation> Transformations {
			[NotNull] get {
				Contract.Ensures(Contract.Result<ReadOnlyCollection<ITransformation>>() != null);
				Contract.EndContractBlock();

				return Array.AsReadOnly(_transformations);
			}
		}

		/// <summary>
		/// Creates a new concatenated transformation that is the inverse of this transformation.
		/// </summary>
		/// <returns>The inverse transformation.</returns>
		[NotNull]
		public ConcatenatedTransformation GetInverse() {
			Contract.Requires(HasInverse);
			Contract.Ensures(Contract.Result<ConcatenatedTransformation>() != null);
			Contract.EndContractBlock();

			return CreateInverseConcatenatedOperation();
		}

		ITransformation ITransformation.GetInverse() {
			return GetInverse();
		}

		/// <inheritdoc/>
		public bool HasInverse {
			get {
				if(_transformations.Length == 0)
					return true;
				for (int i = 0; i < _transformations.Length; i++)
					if (!_transformations[i].HasInverse)
						return false;
				return true;
			}
		}
	}

	/// <summary>
	/// A transformation that is composed of a sequence of transformations as a chained expression.
	/// </summary>
	public class ConcatenatedTransformation<TFrom, TTo> : ConcatenatedTransformation, ITransformation<TFrom, TTo>
	{

		[NotNull] private readonly TransformationCastNode[] _transformationPath;

		/// <summary>
		/// Creates a new concatenated transformation composed of a sequence of transformations.
		/// </summary>
		/// <param name="transformations">A sequence of transformations.</param>
		public ConcatenatedTransformation([NotNull, InstantHandle] IEnumerable<ITransformation> transformations) : base(transformations) {
			Contract.Requires(transformations != null);
			Contract.EndContractBlock();

			var path = TransformationCastNode.FindCastPath(Transformations, typeof(TFrom), typeof(TTo));
			if(null == path)
				throw new InvalidOperationException("A concatenated transformation casting path could not be found.");
			_transformationPath = path;
		}

		/// <summary>
		/// The chosen transformation cast path used when compiling the concatenated transformation.
		/// </summary>
		[NotNull] public ReadOnlyCollection<TransformationCastNode> TransformationPath {
			[Pure] get {
				Contract.Ensures(Contract.Result<ReadOnlyCollection<TransformationCastNode>>() != null);
				Contract.EndContractBlock();

				return Array.AsReadOnly(_transformationPath);
			}
		}

		/// <inheritdoc/>
		public virtual TTo TransformValue(TFrom value) {
			if(_transformationPath.Length == 1)
				return ((ITransformation<TFrom, TTo>)(_transformationPath[0].Core)).TransformValue(value);

			object tempValue = value;
			for (int i = 0; i < _transformationPath.Length; i++) {
				tempValue = _transformationPath[i].TransformValue(tempValue);
			}
			return (TTo)tempValue;
		}

		/// <inheritdoc/>
		public virtual IEnumerable<TTo> TransformValues(IEnumerable<TFrom> values) {
			Contract.Ensures(Contract.Result<IEnumerable<TTo>>() != null);
			Contract.EndContractBlock();

			if(Transformations.Count == 1)
				return ((ITransformation<TFrom, TTo>)(_transformationPath[0].Core)).TransformValues(values);

			IEnumerable tempValues = values;
			for (int i = 0; i < _transformationPath.Length; i++) {
				tempValues = _transformationPath[i].TransformValues(tempValues);
			}
			return (IEnumerable<TTo>)tempValues;
		}

		/// <inheritdoc/>
		[NotNull]
		public new ConcatenatedTransformation<TTo, TFrom> GetInverse(){
			Contract.Requires(HasInverse);
			Contract.Ensures(Contract.Result<ConcatenatedTransformation<TTo, TFrom>>() != null);
			Contract.EndContractBlock();

			return (ConcatenatedTransformation<TTo,TFrom>)CreateInverseConcatenatedOperation();
		}

		/// <inheritdoc/>
		ITransformation<TTo, TFrom> ITransformation<TFrom, TTo>.GetInverse() {
			Contract.Ensures(Contract.Result<ITransformation<TTo, TFrom>>() != null);
			Contract.EndContractBlock();

			return GetInverse();
		}

		/// <inheritdoc/>
		protected override ConcatenatedTransformation CreateInverseConcatenatedOperation() {
			return new ConcatenatedTransformation<TTo, TFrom>(CreateInverseOperations());
		}

		[ContractInvariantMethod]
		private void CodeContractInvariant() {
			Contract.Invariant(null != _transformationPath);
			Contract.Invariant(Contract.ForAll(_transformationPath, x => null != x));
		}
	}

	/// <summary>
	/// A transformation that is composed of a sequence of transformations as a chained expression.
	/// </summary>
	public class ConcatenatedTransformation<TValue> : ConcatenatedTransformation<TValue, TValue>, ITransformation<TValue>
	{
		/// <summary>
		/// Creates a new concatenated transformation composed of the given transformations.
		/// </summary>
		/// <param name="transformations"></param>
		public ConcatenatedTransformation([NotNull, InstantHandle] IEnumerable<ITransformation> transformations)
			: base(transformations)
		{
			Contract.Requires(transformations != null);
			Contract.EndContractBlock();
		}

		/// <inheritdoc/>
		public virtual void TransformValues(TValue[] values){
			for(int i = 0; i < values.Length; i++)
				values[i] = TransformValue(values[i]);
		}

		/// <inheritdoc/>
		[NotNull]
		public new ConcatenatedTransformation<TValue> GetInverse(){
			Contract.Requires(HasInverse);
			Contract.Ensures(Contract.Result<ConcatenatedTransformation<TValue>>() != null);
			Contract.EndContractBlock();

			return (ConcatenatedTransformation<TValue>)CreateInverseConcatenatedOperation();
		}

		/// <inheritdoc/>
		ITransformation<TValue> ITransformation<TValue>.GetInverse(){
			return GetInverse();
		}

		/// <inheritdoc/>
		protected override ConcatenatedTransformation CreateInverseConcatenatedOperation() {
			return new ConcatenatedTransformation<TValue>(CreateInverseOperations());
		}

	}

}