using Evil.MsiOrm.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Evil.MsiOrm.Core
{
    internal class MsiQueryTranslator : ExpressionVisitor
    {
        StringBuilder sb;

        private readonly MsiRowToObjectConverter rowToObjectConverter;

        internal MsiQueryTranslator(MsiRowToObjectConverter converter)
        {
            rowToObjectConverter = converter;
        }

        internal string Translate(Expression expression)
        {
            this.sb = new StringBuilder();
            sb.Append("SELECT * FROM `");
            sb.Append(rowToObjectConverter.TableName);
            sb.Append("` WHERE ");
            this.Visit(expression);
            return this.sb.ToString();
        }

        protected override Expression VisitMethodCall(MethodCallExpression m)
        {
            throw new NotSupportedException(string.Format("The method '{0}' is not supported", m.Method.Name));
        }

        protected override Expression VisitUnary(UnaryExpression u)
        {
            switch (u.NodeType)
            {
                case ExpressionType.Not:
                    sb.Append(" NOT ");
                    this.Visit(u.Operand);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The unary operator '{0}' is not supported", u.NodeType));
            }

            return u;
        }

        protected override Expression VisitBinary(BinaryExpression b)
        {
            sb.Append("(");
            this.Visit(b.Left);
            switch (b.NodeType)
            {
                case ExpressionType.AndAlso:
                case ExpressionType.And:
                    sb.Append(" AND ");
                    break;
                case ExpressionType.Or:
                    sb.Append(" OR");
                    break;
                case ExpressionType.Equal:
                    if (b.Right.NodeType == ExpressionType.Constant && ((ConstantExpression)b.Right).Value == null)
                    {
                        sb.Append(" IS ");
                    }
                    else
                    {
                        sb.Append(" = ");
                    }

                    break;
                case ExpressionType.NotEqual:
                    if (b.Right.NodeType == ExpressionType.Constant && ((ConstantExpression)b.Right).Value == null)
                    {
                        sb.Append(" IS NOT ");
                    }
                    else
                    {
                        sb.Append(" <> ");
                    }

                    break;
                case ExpressionType.LessThan:
                    sb.Append(" < ");
                    break;
                case ExpressionType.LessThanOrEqual:
                    sb.Append(" <= ");
                    break;
                case ExpressionType.GreaterThan:
                    sb.Append(" > ");
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    sb.Append(" >= ");
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));
            }

            this.Visit(b.Right);
            sb.Append(")");
            return b;
        }

        protected override Expression VisitConstant(ConstantExpression c)
        {
            ParseConstant(c.Value);
            return c;
        }

        private void ParseConstant(object value)
        {
            if (value == null)
            {
                sb.Append("NULL");
                return;
            }

            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.String:
                    sb.Append("'");
                    sb.Append(value);
                    sb.Append("'");
                    break;
                case TypeCode.Boolean:
                case TypeCode.Object:
                    throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));
                default:
                    sb.Append(value);
                    break;
            }
        }

        protected override Expression VisitMember(MemberExpression m)
        {
            if ((m.Expression == null && m.NodeType == ExpressionType.MemberAccess) 
                || m.Expression.NodeType == ExpressionType.Constant)
            {
                ParseConstant(GetValue(m));
                return m;
            }

            if (m.Expression == null)
            {
                throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
            }

            if (m.Expression.NodeType != ExpressionType.Parameter)
            {
                throw new NotSupportedException(string.Format("The member '{0}' is not supported", m.Member.Name));
            }

            var property = m.Member as PropertyInfo;
            if (property == null)
            {
                throw new NotSupportedException(string.Format("Only properties are supported, Member: '{0}' is not supported", 
                    m.Member.Name));
            }

            var attribute = property.GetCustomAttributes(typeof(MsiColumnAttribute), false).FirstOrDefault() as MsiColumnAttribute;
            if (attribute == null)
            {
                throw new NotSupportedException(string.Format("The member '{0}' doesn't have MsiColumn attribute", m.Member.Name));
            }

            sb.Append('`');
            sb.Append(attribute.ColumnName);
            sb.Append('`');
            return m;
        }

        private object GetValue(MemberExpression member)
        {
            var objectMember = Expression.Convert(member, typeof(object));
            var getterLambda = Expression.Lambda<Func<object>>(objectMember);
            var getter = getterLambda.Compile();
            return getter();
        }
    }
}
