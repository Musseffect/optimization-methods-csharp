using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;

namespace OptimizationMethods
{
    public enum ASTNodeType
    {
        Division,
        Multiplication,
        Negation,
        Addition,
        Subtraction,
        Function,
        Float,
        Identifier,
        Power
    }
    public class ASTNode
    {
        public ASTNodeType Type { get; protected set; }
        public int Line { get; set; }
        public int Position { get; set; }
    }
    public class InfixExpressionNode : ASTNode
    {
        public ASTNode Left { get; set; }
        public ASTNode Right { get; set; }
    }
    public class AdditionNode : InfixExpressionNode
    {
        public AdditionNode()
        {
            Type = ASTNodeType.Addition;
        }
    }
    public class SubtractionNode : InfixExpressionNode
    {
        public SubtractionNode()
        {
            Type = ASTNodeType.Subtraction;
        }
    }
    public class MultiplicationNode : InfixExpressionNode
    {
        public MultiplicationNode()
        {
            Type = ASTNodeType.Multiplication;
        }
    }
    public class DivisionNode : InfixExpressionNode
    {
        public DivisionNode()
        {
            Type = ASTNodeType.Division;
        }
    }
    public class PowerNode : InfixExpressionNode
    {
        public PowerNode()
        {
            Type = ASTNodeType.Power;
        }
    }
    public class NegationNode : ASTNode
    {
        public NegationNode()
        {
            Type = ASTNodeType.Negation;
        }
        public ASTNode InnerNode { get; set; }
    }
    public class FunctionNode : ASTNode
    {
        public FunctionNode()
        {
            Type = ASTNodeType.Function;
        }
        public string FunctionName { get; set; }
        public List<ASTNode> Arguments { get; set; }
    }
    public class IdentifierNode : ASTNode
    {
        public IdentifierNode()
        {
            Type = ASTNodeType.Identifier;
        }
        public string Value { get; set; }
    }
    public class FloatNode : ASTNode
    {
        public FloatNode()
        {
            Type = ASTNodeType.Float;
        }
        public double Value { get; set; }
    }
    class ExpressionVisitor:ExpGrammarBaseVisitor<ASTNode>
    {
        public override ASTNode VisitBinaryOperatorExpression([NotNull] ExpGrammarParser.BinaryOperatorExpressionContext context)
        {
            InfixExpressionNode node;
            int type = context.op.Type;
            switch (type)
            {
                case ExpGrammarLexer.CARET:
                    node = new PowerNode();
                    break;
                case ExpGrammarLexer.PLUS:
                    node = new AdditionNode();
                    break;
                case ExpGrammarLexer.MINUS:
                    node = new SubtractionNode();
                    break;
                case ExpGrammarLexer.ASTERISK:
                    node = new MultiplicationNode();
                    break;
                case ExpGrammarLexer.DIVISION:
                    node = new DivisionNode();
                    break;
                default:
                    throw new NotSupportedException();
            }
            node.Left = (ASTNode)Visit(context.left);
            node.Right = (ASTNode)Visit(context.right);
            node.Line = context.start.Line;
            node.Position = context.start.Column;
            return node;
        }

        public override ASTNode VisitBracketExpression([NotNull] ExpGrammarParser.BracketExpressionContext context)
        {
            return Visit(context.expression());
        }

        public override ASTNode VisitCompileUnit([NotNull] ExpGrammarParser.CompileUnitContext context)
        {
            return Visit(context.expression());
        }

        public override ASTNode VisitConstantExpression([NotNull] ExpGrammarParser.ConstantExpressionContext context)
        {
            return Visit(context.value);
        }

        public override ASTNode VisitFunctionExpression([NotNull] ExpGrammarParser.FunctionExpressionContext context)
        {
            var functionName = context.func.Text;
            List<ASTNode> arguments = new List<ASTNode>();
            var args = context.functionArguments();
            foreach (var arg in args.expression())
            {
                arguments.Add((ASTNode)Visit(arg));
            }
            return new FunctionNode
            {
                FunctionName = functionName,
                Arguments = arguments,
                Line = context.func.Line,
                Position = context.func.Column
            };
        }

        public override ASTNode VisitIdentifierExpression([NotNull] ExpGrammarParser.IdentifierExpressionContext context)
        {
            return new IdentifierNode
            {
                Value = context.id.Text,
                Line = context.id.Line,
                Position = context.id.Column
            };
        }

        public override ASTNode VisitNumber([NotNull] ExpGrammarParser.NumberContext context)
        {
                return new FloatNode
                {
                    Value = double.Parse(context.value.Text),
                    Line = context.value.Line,
                    Position = context.value.Column
                };
        }

        public override ASTNode VisitUnaryOperatorExpression([NotNull] ExpGrammarParser.UnaryOperatorExpressionContext context)
        {
            switch (context.unaryOperator().op.Type)
            {
                case ExpGrammarLexer.PLUS:
                    return Visit(context.expression());

                case ExpGrammarLexer.MINUS:
                    return new NegationNode
                    {
                        InnerNode = (ASTNode)Visit(context.expression()),
                        Line = context.start.Line,
                        Position = context.start.Column
                    };

                default:
                    throw new NotSupportedException();
            }
        }

    }
}
