using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationMethods.Compiler
{
    public partial class ASTCompiler
    {
        static ASTSimpleNode derivativeNegation(SimpleNegationNode node)
        {
            return new SimpleNegationNode
            {
                Node = derivative(node.Node)
            };
        }
        static ASTSimpleNode derivativePower(SimplePowerNode node)
        {
            return new SimpleAdditionNode
            {
                Left = new SimpleMultiplicationNode
                {
                    Left = node,
                    Right = new SimpleMultiplicationNode
                    {
                        Left = derivative(node.Right),
                        Right = new SimpleFunctionNode(MetaData.getFunctionEntry("ln"), new List<ASTSimpleNode> { node.Left })
                    }
                },
                Right = new SimpleMultiplicationNode
                {
                    Left = new SimplePowerNode
                    {
                        Left = node.Left,
                        Right = new SimpleSubtractionNode
                        {
                            Left = node.Right,
                            Right = new SimpleFloatNode(1.0f)
                        }
                    },
                    Right = new SimpleMultiplicationNode
                    {
                        Left = node.Right,
                        Right = derivative(node.Left)
                    }
                }
            };
            /*return new SimpleMultiplicationNode
            {
                Left = node,
                Right = new SimpleAdditionNode
                {
                    Left = new SimpleMultiplicationNode
                    {
                        Left = derivative(node.Right),
                        Right = new SimpleFunctionNode(MetaData.getFunctionEntry("ln"), new List<ASTSimpleNode> { node.Left })
                    },
                    Right = new SimpleDivisionNode
                    {
                        Left = new SimpleMultiplicationNode
                        {
                            Left = node.Right,
                            Right = derivative(node.Left)
                        },
                        Right = node.Left
                    }
                }
            };*/
        }
        static ASTSimpleNode derivativeMultiplication(SimpleMultiplicationNode node)
        {
            return new SimpleAdditionNode
            {
                Left = new SimpleMultiplicationNode { Left = derivative(node.Left), Right = node.Right },
                Right = new SimpleMultiplicationNode { Left = node.Left, Right = derivative(node.Right) }
            };
        }
        static ASTSimpleNode derivativeDivision(SimpleDivisionNode node)
        {
            return new SimpleDivisionNode
            {
                Left = new SimpleSubtractionNode
                {
                    Left = new SimpleMultiplicationNode
                    {
                        Left = derivative(node.Left),
                        Right = node.Right
                    },
                    Right = new SimpleMultiplicationNode
                    {
                        Left = node.Left,
                        Right = derivative(node.Right)

                    }
                },
                Right = new SimpleMultiplicationNode
                {
                    Left = node.Right,
                    Right = node.Right
                },
            };
        }
        static ASTSimpleNode derivativeFunction(SimpleFunctionNode node)
        {
            if (node.Args.Count == 0)
                return new SimpleFloatNode(0.0);
            if (node.Args.Count == 1)
            {
                return
                    new SimpleMultiplicationNode
                    {
                        Left = derivative(node.Args[0]),
                        Right = node.Func.Der[0](node.Args)
                    };
            }
            SimpleAdditionNode root = new SimpleAdditionNode();
            SimpleAdditionNode current = root;
            for (int i = 0; i < node.Args.Count; i++)
            {
                current.Left = new SimpleMultiplicationNode {
                    Left = derivative(node.Args[i]),
                    Right = node.Func.Der[i](node.Args) };
                current.Right = new SimpleAdditionNode();
                current = (SimpleAdditionNode)current.Right;
            }
            current.Right = node.Func.Der[node.Args.Count - 1](node.Args);
            return root;
        }
        static ASTSimpleNode derivativeSubtraction(SimpleSubtractionNode node)
        {
            return new SimpleSubtractionNode { Left = derivative(node.Left), Right = derivative(node.Right) };
        }
        static ASTSimpleNode derivativeAddition(SimpleAdditionNode node)
        {
            return new SimpleAdditionNode { Left = derivative(node.Left), Right = derivative(node.Right) };
        }
        static ASTSimpleNode derivativeIdentifier(SimpleIdentifierNode node)
        {
            if (node.VariableName == variable)
            {
                return new SimpleFloatNode(1.0);
            }
            else
                return new SimpleFloatNode(0.0);
        }
        static string variable;
        public static ASTSimpleNode computeDerivative(ASTSimpleNode root, string var)
        {
            variable = var;
            return derivative(root);
        }
        static ASTSimpleNode derivative(ASTSimpleNode node)
        {
            switch (node.Type)
            {
                case ASTSimpleNode.EType.Negation:
                    return derivativeNegation((SimpleNegationNode)node);
                case ASTSimpleNode.EType.Power:
                    return derivativePower((SimplePowerNode)node);
                case ASTSimpleNode.EType.Multiplication:
                    return derivativeMultiplication((SimpleMultiplicationNode)node);
                case ASTSimpleNode.EType.Division:
                    return derivativeDivision((SimpleDivisionNode)node);
                case ASTSimpleNode.EType.Function:
                    return derivativeFunction((SimpleFunctionNode)node);
                case ASTSimpleNode.EType.Subtraction:
                    return derivativeSubtraction((SimpleSubtractionNode)node);
                case ASTSimpleNode.EType.Addition:
                    return derivativeAddition((SimpleAdditionNode)node);
                case ASTSimpleNode.EType.Identifier:
                    return derivativeIdentifier((SimpleIdentifierNode)node);
                case ASTSimpleNode.EType.Float:
                    return new SimpleFloatNode(0.0f);
            }
            throw new Exception();
        }
    }
}
