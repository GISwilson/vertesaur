﻿using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reflection;
using Vertesaur.Generation.Contracts;

namespace Vertesaur.Generation.Expressions
{

	/// <summary>
	/// A minimum value expression which determines the minimum of two values.
	/// </summary>
	public class MinExpression : ReducibleBinaryExpressionBase
	{

		/// <summary>
		/// Creates a new minimum value expression.
		/// </summary>
		/// <param name="left">An expression to use.</param>
		/// <param name="right">An expression to use.</param>
		/// <param name="generator">The optional expression generator used during reduction.</param>
		public MinExpression(Expression left, Expression right, IExpressionGenerator generator = null)
			: base(left, right, generator) { Contract.Requires(null != left); Contract.Requires(null != right);}

		/// <inheritdoc/>
		public override Type Type {
			get { return  LeftParameter.Type; }
		}

		/// <inheritdoc/>
		public override Expression Reduce() {
			var method = typeof(Math).GetMethod(
				"Min",
				BindingFlags.Public | BindingFlags.Static,
				null, new[] { LeftParameter.Type,RightParameter.Type }, null);
			if (null != method)
				return Call(method, LeftParameter, RightParameter);

			if (LeftParameter.IsMemoryLocationOrConstant() && RightParameter.IsMemoryLocationOrConstant())
				return GenerateExpression(LeftParameter, RightParameter);

			return new BlockExpressionBuilder()
				.AddUsingMemoryLocationsOrConstants(
					args => new[] { GenerateExpression(args[0], args[1]) },
					LeftParameter, RightParameter
				)
				.GetExpression();
		}

		private Expression GenerateExpression(Expression left, Expression right) {
			Contract.Assume(null != left);
			Contract.Assume(null != right);
			Contract.Assume(left.IsMemoryLocationOrConstant());
			Contract.Assume(right.IsMemoryLocationOrConstant());
			return Condition(
				ReductionExpressionGenerator.Generate("LESSEQUAL", left, right),
				left,
				right
			);
		}
	}
}