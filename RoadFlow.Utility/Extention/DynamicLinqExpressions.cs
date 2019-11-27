using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace RoadFlow.Utility
{
    /// <summary>
    /// 动态Linq表达式
    /// </summary>
    public static class DynamicLinqExpressions
    {
        /// <summary>
        /// 并且
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> And1<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            InvocationExpression expression = Expression.Invoke((Expression)expr2, Enumerable.Cast<Expression>((IEnumerable)expr1.Parameters));
            return Expression.Lambda<Func<T, bool>>((Expression)Expression.And(expr1.Body, (Expression)expression), (IEnumerable<ParameterExpression>)expr1.Parameters);
        }
        /// <summary>
        /// False
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> False1<T>()
        {
            ParameterExpression[] expressionArray1 = new ParameterExpression[] { Expression.Parameter((Type)typeof(T), "f") };
            return Expression.Lambda<Func<T, bool>>((Expression)Expression.Constant((bool)false, (Type)typeof(bool)), expressionArray1);
        }

        /// <summary>
        /// 或者
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expr1"></param>
        /// <param name="expr2"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> Or1<T>(this Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            InvocationExpression expression = Expression.Invoke((Expression)expr2, Enumerable.Cast<Expression>((IEnumerable)expr1.Parameters));
            return Expression.Lambda<Func<T, bool>>((Expression)Expression.Or(expr1.Body, (Expression)expression), (IEnumerable<ParameterExpression>)expr1.Parameters);
        }

        /// <summary>
        /// 真
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Expression<Func<T, bool>> True1<T>()
        {
            ParameterExpression[] expressionArray1 = new ParameterExpression[] { Expression.Parameter((Type)typeof(T), "f") };
            return Expression.Lambda<Func<T, bool>>((Expression)Expression.Constant((bool)true, (Type)typeof(bool)), expressionArray1);
        }

        // Nested Types
        [Serializable, CompilerGenerated]
        private sealed class s_c__0<T>
        {
            // Fields
            public static readonly DynamicLinqExpressions.s_c__0<T> s_9;

            // Methods
            static s_c__0()
            {
                DynamicLinqExpressions.s_c__0<T>.s_9 = new DynamicLinqExpressions.s_c__0<T>();
            }
        }

        [Serializable, CompilerGenerated]
        private sealed class s_c__1<T>
        {
            // Fields
            public static readonly DynamicLinqExpressions.s_c__1<T> s_9;

            // Methods
            static s_c__1()
            {
                DynamicLinqExpressions.s_c__1<T>.s_9 = new DynamicLinqExpressions.s_c__1<T>();
            }
        }
    }


}
