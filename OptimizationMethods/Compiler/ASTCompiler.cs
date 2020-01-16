using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationMethods.Compiler
{
    public partial class ASTCompiler
    {
        public static ExpressionStack compileASTExpression(ASTSimpleNode root)
        {
            varCount = 0;
            indicies = new Dictionary<string, int>();
            rpn = new List<StackElement>();
            try
            {
                compileVisitor(root);
            }
            catch (Exception exc)
            {
                throw new Exception(exc.Message);
            }
            return new ExpressionStack(rpn,indicies);
        }
        static void compileNegation(SimpleNegationNode node)
        {
            compileVisitor(node.Node);
            rpn.Add(new NegationOperator());
        }
        static void compilePower(SimplePowerNode node)
        {
            compileVisitor(node.Left);
            compileVisitor(node.Right);
            rpn.Add(new PowerOperator());
        }
        static void compileMultiplication(SimpleMultiplicationNode node)
        {
            compileVisitor(node.Left);
            compileVisitor(node.Right);
            rpn.Add(new MultiplicationOperator());
        }
        static void compileDivision(SimpleDivisionNode node)
        {
            compileVisitor(node.Left);
            compileVisitor(node.Right);
            rpn.Add(new DivisionOperator());
        }
        static void compileFunction(SimpleFunctionNode node)
        {
            //check function signature
            foreach (var arg in node.Args)
                compileVisitor(arg);
            rpn.Add(new Function(node.Func));
        }
        static void compileSubtraction(SimpleSubtractionNode node)
        {
            compileVisitor(node.Left);
            compileVisitor(node.Right);
            rpn.Add(new SubtractionOperator());
        }
        static void compileAddition(SimpleAdditionNode node)
        {
            compileVisitor(node.Left);
            compileVisitor(node.Right);
            rpn.Add(new AdditionOperator());
        }
        static void compileIdentifier(SimpleIdentifierNode node)
        {
            int index;
            if (!indicies.TryGetValue(node.VariableName,out index))
            {
                index = varCount;
                indicies[node.VariableName] = index;
                varCount++;
            }
            rpn.Add(new Variable(index));
        }
        static void compileConstant(SimpleFloatNode node)
        {
            rpn.Add(new Operand(node.Value));
        }
        static void compileVisitor(ASTSimpleNode node)
        {
            switch (node.Type)
            {
                case ASTSimpleNode.EType.Negation:
                    compileNegation((SimpleNegationNode)node);
                    break;
                case ASTSimpleNode.EType.Power:
                    compilePower((SimplePowerNode)node);
                    break;
                case ASTSimpleNode.EType.Multiplication:
                    compileMultiplication((SimpleMultiplicationNode)node);
                    break;
                case ASTSimpleNode.EType.Division:
                    compileDivision((SimpleDivisionNode)node);
                    break;
                case ASTSimpleNode.EType.Function:
                    compileFunction((SimpleFunctionNode)node);
                    break;
                case ASTSimpleNode.EType.Subtraction:
                    compileSubtraction((SimpleSubtractionNode)node);
                    break;
                case ASTSimpleNode.EType.Addition:
                    compileAddition((SimpleAdditionNode)node);
                    break;
                case ASTSimpleNode.EType.Identifier:
                    compileIdentifier((SimpleIdentifierNode)node);
                    break;
                case ASTSimpleNode.EType.Float:
                    compileConstant((SimpleFloatNode)node);
                    break;
            }
            return;
        }
        static List<StackElement> rpn;
        static Dictionary<string,int> indicies;
        static int varCount;
    }
}
