using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationMethods.Compiler
{
    partial class ASTCompiler
    {
        static ExpressionStack compileASTExpression(ASTNode root)
        {


            throw new NotImplementedException();
        }
        static void compileNegation(SimpleNegationNode node)
        {
            compileVisitor(node.Node);
            rpn.Push(new NegationOperator());
        }
        static void compilePower(SimplePowerNode node)
        {
            compileVisitor(node.Left);
            compileVisitor(node.Right);
            rpn.Push(new PowerOperator());
        }
        static void compileMultiplication(SimpleMultiplicationNode node)
        {
            compileVisitor(node.Left);
            compileVisitor(node.Right);
            rpn.Push(new MultiplicationOperator());
        }
        static void compileDivision(SimpleDivisionNode node)
        {
            compileVisitor(node.Left);
            compileVisitor(node.Right);
            rpn.Push(new DivisionOperator());
        }
        static void compileFunction(SimpleFunctionNode node)
        {
            //check function signature
            foreach (var arg in node.Args)
                compileVisitor(arg);
            rpn.Push(new Function(node.Func));
        }
        static void compileSubtraction(SimpleSubtractionNode node)
        {
            compileVisitor(node.Left);
            compileVisitor(node.Right);
            rpn.Push(new SubtractionOperator());
        }
        static void compileAddition(SimpleAdditionNode node)
        {
            compileVisitor(node.Left);
            compileVisitor(node.Right);
            rpn.Push(new AdditionOperator());
        }
        static void compileIdentifier(SimpleIdentifierNode node)
        {
            rpn.Push(new Variable());
        }
        static void compileConstant(SimpleFloatNode node)
        {
            rpn.Push(new Operand(node.Value));
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
        static Stack<StackElement> rpn;
        static HashSet<string> variables;
        public static CompiledExpression compileAST(ASTNode root)
        {
            rpn = new Stack<StackElement>();
            throw new NotImplementedException();
            try
            {
                compileVisitor(root);
            }
            catch (Exception exc)
            {

            }
            return new CompiledExpression();
        }
    }
}
