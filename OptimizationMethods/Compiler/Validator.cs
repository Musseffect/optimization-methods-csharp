using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationMethods.Compiler
{
    partial class ASTCompiler
    {
        ASTSimpleNode validateNegation(NegationNode node)
        {
            return new SimpleNegationNode { Node = validate(node.InnerNode) };
        }
        ASTSimpleNode validatePower(PowerNode node)
        {
            return new SimplePowerNode {
                Left=validate(node.Left),
                Right=validate(node.Right)
            };
        }
        ASTSimpleNode validateMultiplication(MultiplicationNode node)
        {
            return new SimpleMultiplicationNode
            {
                Left = validate(node.Left),
                Right = validate(node.Right)
            };
        }
        ASTSimpleNode validateDivision(DivisionNode node)
        {
            return new SimpleDivisionNode
            {
                Left = validate(node.Left),
                Right = validate(node.Right)
            };
        }
        ASTSimpleNode validateFunction(FunctionNode node)
        {
                FunctionEntry func = MetaData.getFunctionEntry(node.FunctionName);
                //check function name
                if (func.ArgNumber != node.Arguments.Count)
                    throw new Exception("Incorrect number of arguments in function [\"" + node.FunctionName + "\"]");

                List<ASTSimpleNode> args=new List<ASTSimpleNode>();
                foreach (var arg in node.Arguments)
                    args.Add(validate(arg));
                return new SimpleFunctionNode(func,args);
        }
        ASTSimpleNode validateSubtraction(SubtractionNode node)
        {
            return new SimpleSubtractionNode
            {
                Left = validate(node.Left),
                Right = validate(node.Right)
            };
        }
        ASTSimpleNode validateAddition(AdditionNode node)
        {
            return new SimpleAdditionNode
            {
                Left = validate(node.Left),
                Right = validate(node.Right)
            };
        }
        ASTSimpleNode validate(ASTNode node)
        {
            switch (node.Type)
            {
                case ASTNodeType.Negation:
                    return validateNegation((NegationNode)node);
                case ASTNodeType.Power:
                    return validatePower((PowerNode)node);
                case ASTNodeType.Multiplication:
                    return validateMultiplication((MultiplicationNode)node);
                case ASTNodeType.Division:
                    return validateDivision((DivisionNode)node);
                case ASTNodeType.Function:
                    return validateFunction((FunctionNode)node);
                case ASTNodeType.Subtraction:
                    return validateSubtraction((SubtractionNode)node);
                case ASTNodeType.Addition:
                    return validateAddition((AdditionNode)node);
                case ASTNodeType.Identifier:
                    return new SimpleIdentifierNode(((IdentifierNode)node).Value);
                case ASTNodeType.Float:
                    return new SimpleFloatNode(((FloatNode)node).Value);
            }
            throw new Exception();
        }
    }
}
