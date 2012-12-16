﻿using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using Vertesaur.Generation.Contracts;

namespace Vertesaur.Generation.Expressions
{
	/// <summary>
	/// An arc-tangent expression.
	/// </summary>
	public class AtanExpression : ReducibleUnaryExpressionBase
	{
		private static readonly MethodInfo MathAtanMethod;

		static AtanExpression() {
			MathAtanMethod = typeof(Math).GetMethod(
				"Atan",
				BindingFlags.Public | BindingFlags.Static,
				null, new[] { typeof(double) }, null);
		}

		/// <summary>
		/// Creates a new arc-tangent expression.
		/// </summary>
		/// <param name="input">The expression to calculate the arc-tangent of.</param>
		/// <param name="generator">The optional expression generator used during reduction.</param>
		public AtanExpression(Expression input, IExpressionGenerator generator = null)
			: base(input, generator) { Contract.Requires(null != input); }

		/// <inheritdoc/>
		public override Expression Reduce() {
			return typeof(double) == Type
				? (Expression)Call(MathAtanMethod, UnaryParameter)
				: Convert(Call(MathAtanMethod, Convert(UnaryParameter, typeof(double))), Type);
		}
	}
}