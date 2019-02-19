using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OptimizationMethods.Compiler.ASTCompiler;

namespace OptimizationMethods.Compiler
{
    public class ASTPrint
    {
        public string print(ASTSimpleNode root)
        {
            astString = "";
            printAST(root);
            return astString;
        }
        void printNegation(SimpleNegationNode node)
        {
            astString += "-";
            printAST(node.Node);
        }
        void printPower(SimplePowerNode node)
        {
            astString += "(";
            printAST(node.Left);
            astString += "^";
            printAST(node.Right);
            astString += ")";
        }
        void printMultiplication(SimpleMultiplicationNode node)
        {
            astString += "(";
            printAST(node.Left);
            astString += "*";
            printAST(node.Right);
            astString += ")";
        }
        void printDivision(SimpleDivisionNode node)
        {
            astString += "(";
            printAST(node.Left);
            astString += "/";
            printAST(node.Right);
            astString += ")";
        }
        void printFunction(SimpleFunctionNode node)
        {
            //check function signature
            astString += node.Func.FuncName+"(";
            for (int i=0;i<node.Args.Count;i++)
            {
                ASTSimpleNode arg = node.Args[i];
                printAST(arg);
                if(i!=node.Args.Count-1)
                    astString += ",";
            }
            astString += ")";
        }
        void printSubtraction(SimpleSubtractionNode node)
        {
            astString += "(";
            printAST(node.Left);
            astString += "-";
            printAST(node.Right);
            astString += ")";
        }
        void printAddition(SimpleAdditionNode node)
        {
            astString += "(";
            printAST(node.Left);
            astString += "+";
            printAST(node.Right);
            astString += ")";
        }
        void printIdentifier(SimpleIdentifierNode node)
        {
            astString +=node.VariableName;
        }
        void printConstant(SimpleFloatNode node)
        {
            astString += node.Value.ToString();
        }
        string astString;
        void printAST(ASTSimpleNode node)
        {
            switch (node.Type)
            {
                case ASTSimpleNode.EType.Negation:
                    printNegation((SimpleNegationNode)node);
                    break;
                case ASTSimpleNode.EType.Power:
                    printPower((SimplePowerNode)node);
                    break;
                case ASTSimpleNode.EType.Multiplication:
                    printMultiplication((SimpleMultiplicationNode)node);
                    break;
                case ASTSimpleNode.EType.Division:
                    printDivision((SimpleDivisionNode)node);
                    break;
                case ASTSimpleNode.EType.Function:
                    printFunction((SimpleFunctionNode)node);
                    break;
                case ASTSimpleNode.EType.Subtraction:
                    printSubtraction((SimpleSubtractionNode)node);
                    break;
                case ASTSimpleNode.EType.Addition:
                    printAddition((SimpleAdditionNode)node);
                    break;
                case ASTSimpleNode.EType.Identifier:
                    printIdentifier((SimpleIdentifierNode)node);
                    break;
                case ASTSimpleNode.EType.Float:
                    printConstant((SimpleFloatNode)node);
                    break;
            }
        }
    }
}
