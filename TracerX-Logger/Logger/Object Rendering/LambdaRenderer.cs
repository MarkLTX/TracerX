using System;
using System.Linq.Expressions;
using System.Reflection;

namespace TracerX
{
    /// <summary>
    /// Renders an array of Expression&lt;Func&lt;object&gt;&gt; expressions created
    /// from lambdas like "() =&gt; LambdaBody" where LambdaBody is anything
    /// that yields a value (e.g. a local variable, object property or
    /// function call).  Each lambda expression is rendered as
    /// "LambdaBody = value_of_LambdaBody".  If there is more than one
    /// expression in the array, line breaks are inserted between them.
    /// </summary>
    internal class LambdaRenderer : IObjectRenderer
    {
        public void RenderObject(object obj, System.IO.TextWriter writer)
        {
            var lambdas = (Expression<Func<object>>[])obj;

            if (lambdas == null || lambdas.Length == 0)
            {
                writer.Write( "<no lambdas>");
            }
            else if (lambdas.Length == 1)
            {
                writer.Write( LambdaToString(lambdas[0]));
            }
            else
            {
                for (int i = 0; i < lambdas.Length; ++i)
                {
                    if (i == lambdas.Length - 1)
                    {
                        // No newline on the last one.
                        writer.Write(LambdaToString(lambdas[i]));
                    }
                    else
                    {
                        writer.WriteLine(LambdaToString(lambdas[i]));
                    }
                }
            }
        }

        private static string LambdaToString(Expression<Func<object>> exp)
        {
            string ret = string.Empty;

            try
            {
                if (exp == null)
                {
                    ret = "<null expression>";
                }
                else if (exp.NodeType != ExpressionType.Lambda)
                {
                    ret = "<non-lambda expression>";
                }
                else
                {
                    // Given an Expression created from "() => LambdaBody", we want
                    // to get the literal body text (e.g. "LambdaBody") and the current
                    // value of what the LambdaBody expression evaluates to. It might
                    // be a field, property, local variable, method call, or something else.

                    string body = GetExprBody(exp);
                    object value = "<unknown>";

                    try
                    {
                        // Try to get the value of the body of the lambda expression.
                        MemberExpression me = exp.Body as MemberExpression;

                        if (me != null)
                        {
                            value = GetExprValue(exp, me);
                        }
                        else
                        {
                            var unary = exp.Body as UnaryExpression;

                            if (unary == null ||
                                (me = unary.Operand as MemberExpression) == null)
                            {
                                value = exp.Compile().Invoke();
                            }
                            else
                            {
                                value = GetExprValue(exp, me);
                            }
                        }

                        if (value is string)
                        {
                            ret = string.Format("{0} = \"{1}\"", body, value);
                        }
                        else
                        {
                            ret = string.Format("{0} = {1}", body, value ?? Logger.TextForNull);
                        }
                    }
                    catch (Exception ex)
                    {
                        ret = string.Format("{0} = <Exception getting lambda value: {1}>", body, ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                ret = string.Format("[exception evaluating lambda] -> {0}", ex.Message);
            }

            return ret;
        }

        private static string GetExprBody(Expression<Func<object>> exp)
        {
            // Given an Expression created from "() => LambdaBody", sometimes 
            // exp.Body.ToString() really is just "LambdaBody". However, 
            // it's often one of these:
            //      value(PrtVarTest.Program+<>c__DisplayClass0).ActualBody
            //      Convert(value(PrtVarTest.Program+<>c__DisplayClass0).ActualBody)
            // or even...
            //      Convert((value(PrtVarTest.Program+<>c__DisplayClass0).ActualBody))
            // So, we look for "Convert(" and/or "value(".

            string body = exp.Body.ToString();

            if (body.StartsWith("Convert(", StringComparison.OrdinalIgnoreCase))
            {
                body = body.Substring(8, body.Length - 9).Trim('(', ')');
            }

            if (body.StartsWith("value(", StringComparison.OrdinalIgnoreCase))
            {
                // Find the closing ')'
                int pos = 6;

                for (int nesting = 1; nesting > 0; ++pos)
                {
                    switch (body[pos])
                    {
                        case '(':
                            ++nesting;
                            break;
                        case ')':
                            --nesting;
                            break;
                    }
                }

                // pos should now point at the '.' after the "value(whatever)" substring of the body.

                body = body.Substring(pos + 1);
            }

            return body;
        }

        private static object GetExprValue(Expression<Func<object>> exp, MemberExpression me)
        {
            object result;

            switch (me.Member.MemberType)
            {
                case MemberTypes.Field:
                    var constExp = me.Expression as ConstantExpression;

                    if (constExp == null)
                    {
                        result = exp.Compile().Invoke();
                    }
                    else
                    {
                        result = ((FieldInfo)me.Member).GetValue(constExp.Value);
                    }
                    break;
                case MemberTypes.Property:
                    result = exp.Compile().Invoke();
                    break;
                default:
                    result = string.Format("<unsupported MemberType {0}>", me.Member.MemberType);
                    break;
            }

            return result;
        }

    }
}
