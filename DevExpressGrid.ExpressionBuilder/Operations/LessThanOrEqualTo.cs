﻿using DevExpressGrid.ExpressionBuilder.Common;
using System.Linq.Expressions;

namespace DevExpressGrid.ExpressionBuilder.Operations
{
    /// <summary>
    /// Operation representing an "less than or equal" comparison.
    /// </summary>
    public class LessThanOrEqualTo : OperationBase
    {
        /// <inheritdoc />
        public LessThanOrEqualTo()
            : base("LessThanOrEqualTo", 1, TypeGroup.Number | TypeGroup.Date) { }

        /// <inheritdoc />
        public override Expression GetExpression(MemberExpression member, ConstantExpression constant1, ConstantExpression constant2)
        {
            return Expression.LessThanOrEqual(member, constant1);
        }
    }
}