using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationMethods.Compiler
{
    partial class ASTCompiler
    {
        delegate ASTSimpleNode FunctionDerivative(List<ASTSimpleNode> node);
        class FunctionEntry
        {
            public FunctionExec Exec { get; set; }
            public List<FunctionDerivative> Der { get; set; }
            public int ArgNumber { get; set; }
        }
        class MetaData
        {
            static void Init()
            {
                functionTable = new Dictionary<string, FunctionEntry>()
            {
                { "sin", new FunctionEntry{ Exec=Sin,Der=new List<FunctionDerivative>{SinDer},ArgNumber=1}},
                { "cos", new FunctionEntry{ Exec=Cos,Der=new List<FunctionDerivative>{CosDer },ArgNumber=1}},
                { "tan", new FunctionEntry{ Exec=Tan,Der=new List<FunctionDerivative>{TanDer},ArgNumber=1}},
                { "ctg", new FunctionEntry{ Exec=Ctg,Der=new List<FunctionDerivative>{CtgDer },ArgNumber=1}},
                { "atan", new FunctionEntry{ Exec=Atan,Der=new List<FunctionDerivative>{AtanDer},ArgNumber=1}},
                { "actg", new FunctionEntry{ Exec=Actg,Der=new List<FunctionDerivative>{ActgDer},ArgNumber=1}},
                { "asin", new FunctionEntry{ Exec=Asin,Der=new List<FunctionDerivative>{AsinDer},ArgNumber=1}},
                { "acos", new FunctionEntry{ Exec=Acos,Der=new List<FunctionDerivative>{AcosDer},ArgNumber=1}},
                { "exp", new FunctionEntry{ Exec=Exp,Der=new List<FunctionDerivative>{ExpDer },ArgNumber=1}},
                { "pi", new FunctionEntry{ Exec=Pi,Der=null,ArgNumber=0}},
                { "e", new FunctionEntry{ Exec=E,Der=null,ArgNumber=0}},
                { "ln", new FunctionEntry{ Exec=Ln,Der=new List<FunctionDerivative>{LnDer },ArgNumber=1}},
                { "log", new FunctionEntry{ Exec=Log,Der=new List<FunctionDerivative>{LogDer1,LogDer2 },ArgNumber=2}},
                { "sqrt", new FunctionEntry{ Exec=Sqrt,Der=new List<FunctionDerivative>{SqrtDer },ArgNumber=1}},
                { "sqr", new FunctionEntry{ Exec=Sqr,Der=new List<FunctionDerivative>{SqrDer },ArgNumber=1}},
                { "pow", new FunctionEntry{ Exec=Pow,Der=new List<FunctionDerivative>{ PowDer1,PowDer2},ArgNumber=2}}
            };
            }
            static Dictionary<string, FunctionEntry> functionTable;
            static public FunctionEntry getFunctionEntry(string functionName)
            {
                try
                {
                    return functionTable[functionName];
                }
                catch (Exception exc)
                {
                    throw new Exception("Incorrect function name [\"" + functionName + "\"].");
                }
            }
            static public Operand Sin(List<Operand> args)
            {
                return new Operand(Math.Sin(args[0].value));
            }
            static public Operand Cos(List<Operand> args)
            {
                return new Operand(Math.Sin(args[0].value));
            }
            static public Operand Tan(List<Operand> args)
            {
                return new Operand(Math.Sin(args[0].value));
            }
            static public Operand Ctg(List<Operand> args)
            {
                return new Operand(Math.Sin(args[0].value));
            }
            static public Operand Exp(List<Operand> args)
            {
                return new Operand(Math.Exp(args[0].value));
            }
            static public Operand Pow(List<Operand> args)
            {
                return new Operand(Math.Pow(args[0].value, args[1].value));
            }
            static public Operand Log(List<Operand> args)
            {
                return new Operand(Math.Log(args[0].value,args[1].value));
            }
            static public Operand Ln(List<Operand> args)
            {
                return new Operand(Math.Log(args[0].value));
            }
            static public Operand Asin(List<Operand> args)
            {
                return new Operand(Math.Asin(args[0].value));
            }
            static public Operand Acos(List<Operand> args)
            {
                return new Operand(Math.Acos(args[0].value));
            }
            static public Operand Atan(List<Operand> args)
            {
                return new Operand(Math.Atan(args[0].value));
            }
            static public Operand Actg(List<Operand> args)
            {
                return new Operand(Math.Atan2(1.0,args[0].value));
            }
            static public Operand E(List<Operand> args)
            {
                return new Operand(Math.E);
            }
            static public Operand Pi(List<Operand> args)
            {
                return new Operand(Math.PI);
            }
            static public Operand Sqrt(List<Operand> args)
            {
                return new Operand(Math.Sqrt(args[0].value));
            }
            static public Operand Sqr(List<Operand> args)
            {
                return new Operand(args[0].value*args[0].value);
            }
            static public ASTSimpleNode SinDer(List<ASTSimpleNode> args)
            {
                return new SimpleFunctionNode(functionTable["cos"],args);
            }
            static public ASTSimpleNode CosDer(List<ASTSimpleNode> args)
            {
                return new SimpleNegationNode{
                    Node =new SimpleFunctionNode(functionTable["sin"], args)
                };
            }
            static public ASTSimpleNode TanDer(List<ASTSimpleNode> args)
            {
                return new SimpleDivisionNode {
                    Left = new SimpleFloatNode(1.0),
                    Right = new SimpleFunctionNode(functionTable["cos"], args)
                };
            }
            static public ASTSimpleNode CtgDer(List<ASTSimpleNode> args)
            {
                return new SimpleDivisionNode
                    {
                        Left = new SimpleFloatNode(-1.0),
                        Right = new SimpleFunctionNode(functionTable["sin"], args)
                    };
            }
            static public ASTSimpleNode AtanDer(List<ASTSimpleNode> args)
            {
                return new SimpleDivisionNode
                {
                    Left = new SimpleFloatNode(1.0),
                    Right = new SimpleAdditionNode
                    {
                        Left = new SimpleFloatNode(1.0),
                        Right = new SimplePowerNode
                        {
                            Left = args[0],
                            Right = new SimpleFloatNode(2.0)
                        },
                    }
                };
            }
            static public ASTSimpleNode ActgDer(List<ASTSimpleNode> args)
            {
                return new SimpleNegationNode
                {
                    Node = new SimpleDivisionNode
                    {
                        Left = new SimpleFloatNode(1.0),
                        Right = new SimpleAdditionNode
                        {
                            Left = new SimpleFloatNode(1.0),
                            Right = new SimplePowerNode
                            {
                                Left = args[0],
                                Right = new SimpleFloatNode(2.0)
                            },
                        }
                    }
                };
            }
            static public ASTSimpleNode AcosDer(List<ASTSimpleNode> args)
            {
                return new SimpleNegationNode
                {
                    Node = new SimplePowerNode
                    {
                        Left = new SimpleSubtractionNode
                        {
                            Left = new SimpleFloatNode(1.0),
                            Right = new SimplePowerNode
                            {
                                Left = args[0],
                                Right = new SimpleFloatNode(2.0)
                            }
                        },
                        Right = new SimpleFloatNode(-0.5)
                    }
                };
            }
            static public ASTSimpleNode AsinDer(List<ASTSimpleNode> args)
            {
                return new SimplePowerNode{
                        Left = new SimpleSubtractionNode
                        {
                            Left = new SimpleFloatNode(1.0),
                            Right = new SimplePowerNode
                            {
                                Left = args[0],
                                Right = new SimpleFloatNode(2.0)
                            }
                        },
                        Right = new SimpleFloatNode(-0.5)
                    };
            }
            static public ASTSimpleNode ExpDer(List<ASTSimpleNode> args)
            {
                return new SimpleFunctionNode(functionTable["exp"],args);
            }
            static public ASTSimpleNode LnDer(List<ASTSimpleNode> args)
            {
                return new SimpleDivisionNode
                {
                    Left = new SimpleFloatNode(1.0),
                    Right = args[0]
                };
            }
            static public ASTSimpleNode LogDer1(List<ASTSimpleNode> args)
            {
                return new SimpleDivisionNode
                {
                    Left = new SimpleFloatNode(1.0),
                    Right = new SimpleMultiplicationNode
                    {
                        Left = args[0],
                        Right = new SimpleFunctionNode(functionTable["ln"], new List<ASTSimpleNode> { args[1] })
                    }
                };
            }
            static public ASTSimpleNode LogDer2(List<ASTSimpleNode> args)
            {
                return new SimpleNegationNode
                {
                    Node = new SimpleDivisionNode
                    {
                        Left = new SimpleMultiplicationNode
                        {
                            Left = new SimpleFunctionNode(functionTable["ln"], new List<ASTSimpleNode> { args[0] }),
                            Right = args[0],
                        },
                        Right = new SimpleMultiplicationNode
                        {
                            Left = args[1],
                            Right = new SimpleMultiplicationNode
                            {
                                Left = new SimpleFunctionNode(functionTable["ln"], new List<ASTSimpleNode> { args[1] }),
                                Right = new SimpleFunctionNode(functionTable["ln"], new List<ASTSimpleNode> { args[1] })
                            }
                        },
                    }
                };
            }
            static public ASTSimpleNode SqrtDer(List<ASTSimpleNode> args)
            {
                return new SimpleMultiplicationNode{
                        Left=new SimpleFloatNode(-0.5),
                        Right=new SimplePowerNode {
                            Left=args[0],
                            Right=new SimpleFloatNode(-0.5)
                        }
                    };
            }
            static public ASTSimpleNode SqrDer(List<ASTSimpleNode> args)
            {
                return new SimpleMultiplicationNode
                {
                    Left = new SimpleFloatNode(2.0),
                    Right = args[0]
                };
            }
            static public ASTSimpleNode PowDer1(List<ASTSimpleNode> args)
            {
                return new SimpleMultiplicationNode
                {
                    Left = new SimplePowerNode
                    {
                        Left = args[0],
                        Right = args[1]
                    },
                    Right = new SimpleFunctionNode(functionTable["ln"], new List<ASTSimpleNode> { args[0] })
                };
            }
            static public ASTSimpleNode PowDer2(List<ASTSimpleNode> args)
            {
                return new SimpleMultiplicationNode
                {
                    Left = new SimplePowerNode
                    {
                        Left = args[0],
                        Right = new SimpleSubtractionNode {
                            Left=args[1],
                            Right=new SimpleFloatNode(1.0)
                        }
                    },
                    Right = args[1]
                };
            }
        }
        delegate Operand FunctionExec(List<Operand> args);
        public enum StackElementType
        {
            Operand=0,
            Variable=14,
            Addition=1,
            Subtraction=3,
            Multiplication=5,
            Division=7,
            Power=9,
            Negation=11,
            Function=12
        }
        public abstract class StackElement
        {
            protected StackElementType type;
            public StackElementType Type { get { return type; } }
            public StackElement(StackElementType type)
            {
                this.type = type;
            }
        }
        class Operand : StackElement
        {
            public double value;
            public Operand(double value) : base(StackElementType.Operand)
            {
                this.value = value;
            }
        }
        class Variable : StackElement
        {
            public int index;
            public Variable(int index) : base(StackElementType.Variable)
            {
                this.index = index;
            }
        }
        abstract class BinaryOperator : StackElement
        {
            public BinaryOperator(StackElementType type) : base(type) { }
            public abstract Operand exec(Operand op1, Operand op2);
        }
        abstract class UnaryOperator : StackElement
        {
            public UnaryOperator(StackElementType type) : base(type) { }
            public abstract Operand exec(Operand op);
        }
        class AdditionOperator : BinaryOperator
        {
            public AdditionOperator() : base(StackElementType.Addition) { }
            public override Operand exec(Operand op1, Operand op2)
            {
                throw new NotImplementedException();
            }
        }
        class SubtractionOperator : BinaryOperator
        {
            public SubtractionOperator() : base(StackElementType.Subtraction) { }
            public override Operand exec(Operand op1, Operand op2)
            {
                throw new NotImplementedException();
            }
        }
        class PowerOperator : BinaryOperator
        {
            public PowerOperator() : base(StackElementType.Power) { }
            public override Operand exec(Operand op1, Operand op2)
            {
                throw new NotImplementedException();
            }

        }
        class DivisionOperator : BinaryOperator
        {
            public DivisionOperator() : base(StackElementType.Division) { }
            public override Operand exec(Operand op1, Operand op2)
            {
                throw new NotImplementedException();
            }

        }
        class MultiplicationOperator : BinaryOperator
        {
            public MultiplicationOperator() : base(StackElementType.Multiplication) { }
            public override Operand exec(Operand op1, Operand op2)
            {
                throw new NotImplementedException();
            }

        }
        class NegationOperator : UnaryOperator
        {
            public NegationOperator() : base(StackElementType.Negation) { }
            public override Operand exec(Operand op)
            {
                op.value = -op.value;
                return op;
            }
        }
        class Function : StackElement
        {
            FunctionEntry func;
            public Function(FunctionEntry func) : base(StackElementType.Function)
            {
                this.func = func;
            }
            public void exec(Queue<Operand> operands)
            {
                List<Operand> arguments = new List<Operand>();
                for (int i = 0; i < func.ArgNumber; i++)
                {
                    arguments.Add(operands.Dequeue());
                }
                operands.Enqueue(func.Exec(arguments));
            }
        }
        public class ExpressionStack
        {
            List<StackElement> rpn;
            float[] variables;
            Dictionary<string, int> varIndicies;
            public ExpressionStack(List<StackElement> rpn, Dictionary<string, int>  varIndicies)
            {
                this.rpn = rpn;
                this.variables = new float[varIndicies.Count];
                this.varIndicies = varIndicies;
            }
            public void set(string var, float value)
            {
                variables[varIndicies[var]] = value;
            }
            public double execute()
            {
                Queue<Operand> operands=new Queue<Operand>();
                for (int i = 0; i < rpn.Count; i++)
                {
                    if (rpn[i].Type == StackElementType.Negation)
                    {
                        operands.Enqueue(((NegationOperator)rpn[i]).exec(operands.Dequeue()));
                    }
                    if ((rpn[i].Type & StackElementType.Addition) == StackElementType.Addition)
                    {
                        operands.Enqueue(((BinaryOperator)rpn[i]).exec(operands.Dequeue(), operands.Dequeue()));
                    }
                    else if (rpn[i].Type == StackElementType.Function)
                    {
                        ((Function)rpn[i]).exec(operands);
                    }
                    else {
                        operands.Enqueue((Operand)rpn[i]);
                    }
                }
                return operands.Dequeue().value;
            }
        }
        /*class CompiledExpression
        {
            ExpressionStack expression;
            ExpressionStack derivatives;
            float execute()
            {
                throw new NotImplementedException();
            }
        }*/
    }
}
