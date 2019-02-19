using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using OptimizationMethods.Compiler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OptimizationMethods.Compiler.ASTCompiler;

namespace OptimizationMethods.Test
{
    class ErrorListener : IAntlrErrorListener<int>
    {
        List<string> errors;
        public ErrorListener()
        {
            errors = new List<string>();
        }
        public List<string> getErrors()
        {
            return errors;
        }
        public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] int offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            errors.Add(msg);
            Console.WriteLine(msg);
        }
    }
    class ParserErrorListener : IAntlrErrorListener<IToken>
    {
        List<string> errors;
        public ParserErrorListener()
        {
            errors = new List<string>();
        }
        public List<string> getErrors()
        {
            return errors;
        }
        public void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
        {
            errors.Add(msg);
            Console.WriteLine(msg);
        }
    }
    public class Tester
    {
        static public void TestASTDerivative()//+
        {
            ASTCompiler.MetaData.Init();
            string expression = "x*x+5*sin(x)*tan(x^3)";
            AntlrInputStream inputStream = new AntlrInputStream(expression);
            ExpGrammarLexer expLexer = new ExpGrammarLexer(inputStream);
            expLexer.RemoveErrorListeners();
            ErrorListener lexerListener = new ErrorListener();
            expLexer.AddErrorListener(lexerListener);
            List<string> errors = lexerListener.getErrors();
            if (errors.Count > 0)
            {
                return;
            }
            CommonTokenStream commonTokenStream = new CommonTokenStream(expLexer);
            /*commonTokenStream.Fill();
            foreach (var token in commonTokenStream.GetTokens())
            {
                Console.WriteLine(token.Text+ " " + (token.Type >= 0 ? expLexer.TokenNames[token.Type] : ""));
            }
            return;*/
            ExpGrammarParser expParser = new ExpGrammarParser(commonTokenStream);
            ParserErrorListener parserListener = new ParserErrorListener();
            expParser.RemoveErrorListeners();
            expParser.AddErrorListener(parserListener);
            errors = parserListener.getErrors();
            if (errors.Count > 0)
            {
                return;
            }
            ExpGrammarParser.CompileUnitContext expContext = expParser.compileUnit();
            ExpressionVisitor visitor = new ExpressionVisitor();
            ASTNode root = visitor.VisitCompileUnit(expContext);
            var rootSimple=Compiler.ASTCompiler.validate(root);
            rootSimple=Compiler.ASTCompiler.simplify(rootSimple);
            Console.WriteLine((new ASTPrint()).print(rootSimple));
            var der=Compiler.ASTCompiler.computeDerivative(rootSimple,"x");
            Console.WriteLine((new ASTPrint()).print(ASTCompiler.simplify(der)));
        }
        static public void TestASTRPN()
        {
            ASTCompiler.MetaData.Init();
            string expression = "x*x+5*sin(2.0*x*pi())";
            AntlrInputStream inputStream = new AntlrInputStream(expression);
            ExpGrammarLexer expLexer = new ExpGrammarLexer(inputStream);
            expLexer.RemoveErrorListeners();
            ErrorListener lexerListener = new ErrorListener();
            expLexer.AddErrorListener(lexerListener);
            List<string> errors = lexerListener.getErrors();
            if (errors.Count > 0)
            {
                return;
            }
            CommonTokenStream commonTokenStream = new CommonTokenStream(expLexer);
            /*commonTokenStream.Fill();
            foreach (var token in commonTokenStream.GetTokens())
            {
                Console.WriteLine(token.Text+ " " + (token.Type >= 0 ? expLexer.TokenNames[token.Type] : ""));
            }
            return;*/
            ExpGrammarParser expParser = new ExpGrammarParser(commonTokenStream);
            ParserErrorListener parserListener = new ParserErrorListener();
            expParser.RemoveErrorListeners();
            expParser.AddErrorListener(parserListener);
            errors = parserListener.getErrors();
            if (errors.Count > 0)
            {
                return;
            }
            ExpGrammarParser.CompileUnitContext expContext = expParser.compileUnit();
            ExpressionVisitor visitor = new ExpressionVisitor();
            ASTNode root = visitor.VisitCompileUnit(expContext);
            var rootSimple = Compiler.ASTCompiler.validate(root);
            rootSimple = Compiler.ASTCompiler.simplify(rootSimple);
            Console.WriteLine((new ASTPrint()).print(rootSimple));
            ExpressionStack exp = compileASTExpression(rootSimple);
            exp.set("x",2.0f);
            double result=exp.execute();
            Console.WriteLine(result);
        }
    }
}
