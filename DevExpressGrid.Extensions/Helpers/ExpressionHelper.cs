using DevExpressGrid.Extensions.Common;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace DevExpressGrid.Extensions.Helpers
{
    internal static class ExpressionHelper
    {
        public static Expression<Func<T, object>> ToLambda<T>(string propertyName)
        {
            var parameter = Expression.Parameter(typeof(T));
            var property = Expression.Property(parameter, propertyName);
            var propAsObject = Expression.Convert(property, typeof(object));

            return Expression.Lambda<Func<T, object>>(propAsObject, parameter);
        }

        public static Expression<Func<T, bool>> And<T>(params Expression<Func<T, bool>>[] predicates)
        {
            Expression<Func<T, bool>> predicate = x => true;
            if (predicates == null || predicates.Length == 0)
            {
                return predicate;
            }

            for (int i = 0; i < predicates.Length; i++)
            {
                if (i == 0)
                {
                    predicate = predicates[i];
                    continue;
                }

                ParameterExpression p = predicate.Parameters[0];
                SubstExpressionVisitor visitor = new SubstExpressionVisitor();
                visitor.subst[predicates[i].Parameters[0]] = p;
                Expression body = Expression.And(predicate.Body, visitor.Visit(predicates[i].Body));
                predicate = Expression.Lambda<Func<T, bool>>(body, p);
            }

            return predicate;
        }

        public static Expression<Func<T, bool>> Or<T>(params Expression<Func<T, bool>>[] predicates)
        {
            Expression<Func<T, bool>> predicate = x => true;
            if (predicates == null || predicates.Length == 0)
            {
                return predicate;
            }

            for (int i = 0; i < predicates.Length; i++)
            {
                if (i == 0)
                {
                    predicate = predicates[i];
                    continue;
                }

                ParameterExpression p = predicate.Parameters[0];
                SubstExpressionVisitor visitor = new SubstExpressionVisitor();
                visitor.subst[predicates[i].Parameters[0]] = p;
                Expression body = Expression.Or(predicate.Body, visitor.Visit(predicates[i].Body));
                predicate = Expression.Lambda<Func<T, bool>>(body, p);
            }

            return predicate;
        }

        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var candidateExpr = expression.Parameters[0];
            var body = Expression.Not(expression.Body);

            return Expression.Lambda<Func<T, bool>>(body, candidateExpr);
        }

        public static Expression StringContains(Expression member, Expression constant)
        {
            var methodInfo = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) });
            return Expression.Call(member, methodInfo, constant);
        }

        public static Expression StringNotContains(Expression member, Expression constant)
        {
            return Expression.Not(StringContains(member, constant));
        }

        public static Expression StringStartsWith(Expression member, Expression constant)
        {
            var methodInfo = typeof(string).GetMethod(nameof(string.StartsWith), new[] { typeof(string) });
            return Expression.Call(member, methodInfo, constant);
        }

        public static Expression StringEndsWith(Expression member, Expression constant)
        {
            var methodInfo = typeof(string).GetMethod(nameof(string.EndsWith), new[] { typeof(string) });
            return Expression.Call(member, methodInfo, constant);
        }
    }
}
