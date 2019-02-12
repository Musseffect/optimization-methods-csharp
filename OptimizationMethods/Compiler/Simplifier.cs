﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OptimizationMethods.Compiler
{
    partial class ASTCompiler
    {
       public class ASTSimpleNode
       {
            public enum EType
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

            };
            public EType Type { get; protected set; }
            public ASTSimpleNode(EType t) {
                Type = t;
            }
       }
        class SimpleBinaryNode : ASTSimpleNode
        {
            public ASTSimpleNode Left { get; set; }
            public ASTSimpleNode Right { get; set; }
            public SimpleBinaryNode(EType type):base(type)
            {
            }
        }
        class SimpleNegationNode : ASTSimpleNode
        {
            public ASTSimpleNode Node { get; set; }
            public SimpleNegationNode() : base(EType.Negation)
            { }
        }
        class SimplePowerNode : SimpleBinaryNode
        {
            public SimplePowerNode() : base(EType.Power)
            { }
        }
        class SimpleAdditionNode : SimpleBinaryNode
        {
            public SimpleAdditionNode() : base(EType.Addition)
            { }
        }
        class SimpleSubtractionNode : SimpleBinaryNode
        {
            public SimpleSubtractionNode() : base(EType.Subtraction)
            { }
        }
        class SimpleMultiplicationNode : SimpleBinaryNode
        {
            public SimpleMultiplicationNode() : base(EType.Multiplication)
            { }
        }
        class SimpleDivisionNode : SimpleBinaryNode
        {
            public SimpleDivisionNode() : base(EType.Division)
            { }
        }
        class SimpleIdentifierNode : ASTSimpleNode
        {
            public string VariableName { get; }
            public SimpleIdentifierNode(string name) : base(EType.Identifier)
            {
                VariableName = name;
            }
        }
        class SimpleFloatNode : ASTSimpleNode
        {
            public double Value { get; }
            bool zero;
            bool one;
            public SimpleFloatNode(double value) : base(EType.Float)
            {
                this.Value = value;
                zero = (Math.Abs(value) < 0.0000001);
                one = (Math.Abs(value - 1.0) < 0.0000001);
            }
            public static SimpleFloatNode operator +(SimpleFloatNode a, SimpleFloatNode b)
            {
                return new SimpleFloatNode(a.Value+b.Value);
            }
            public static SimpleFloatNode operator -(SimpleFloatNode a, SimpleFloatNode b)
            {
                return new SimpleFloatNode(a.Value - b.Value);
            }
            public static SimpleFloatNode operator -(SimpleFloatNode a)
            {
                return new SimpleFloatNode(-a.Value);
            }
            public static SimpleFloatNode operator *(SimpleFloatNode a, SimpleFloatNode b)
            {
                return new SimpleFloatNode(a.Value*b.Value);
            }
            public static SimpleFloatNode operator /(SimpleFloatNode a, SimpleFloatNode b)
            {
                return new SimpleFloatNode(a.Value/b.Value);
            }
            public static SimpleFloatNode pow(SimpleFloatNode a, SimpleFloatNode b)
            {
                return new SimpleFloatNode(Math.Pow(a.Value, b.Value));
            }
            public bool isZero()
            {
                return zero;
            }
            public bool isOne()
            {
                return one;
            }
        }
        class SimpleFunctionNode : ASTSimpleNode
        {
            public FunctionEntry Func { get; }
            public List<ASTSimpleNode> Args { get; }
            public SimpleFunctionNode(FunctionEntry func,List<ASTSimpleNode> args):base(EType.Function)
            {
                Func = func;
                Args = args;
            }
        }

        static ASTSimpleNode simplifyNegation(SimpleNegationNode node)
        {
            ASTSimpleNode innerNode = simplify(node);
            if (innerNode.Type == ASTSimpleNode.EType.Float)
                return -((SimpleFloatNode)innerNode);
            return node;
        }
        static ASTSimpleNode simplifyPower(SimplePowerNode node)
        {
            ASTSimpleNode left = simplify(node.Left);
            ASTSimpleNode right = simplify(node.Right);
            if (left.Type == ASTSimpleNode.EType.Float)
            {
                SimpleFloatNode nl = (SimpleFloatNode)left;
                if (nl.isZero()|| nl.isOne())
                {
                    return left;
                }
                if (right.Type == ASTSimpleNode.EType.Float)
                {
                    SimpleFloatNode nr = (SimpleFloatNode)left;
                    return SimpleFloatNode.pow(nl,nr);
                }
            }
            if (right.Type == ASTSimpleNode.EType.Float)
            {
                SimpleFloatNode nr = (SimpleFloatNode)left;
                if (nr.isZero())
                {
                    return new SimpleFloatNode(1.0);
                }
                if (nr.isOne())
                {
                    return left;
                }
            }
            return new SimplePowerNode { Left = left, Right = right };
        }
        static ASTSimpleNode simplifyMultiplication(SimpleMultiplicationNode node)
        {
            ASTSimpleNode left = simplify(node.Left);
            ASTSimpleNode right = simplify(node.Right);
            if (left.Type == ASTSimpleNode.EType.Float)
            {
                SimpleFloatNode nl = (SimpleFloatNode)left;
                if (nl.isZero())
                {
                    return left;
                }
                if (nl.isOne())
                {
                    return right;
                }
                if (right.Type == ASTSimpleNode.EType.Float)
                {
                    SimpleFloatNode nr = (SimpleFloatNode)left;
                    return nl * nr;
                }
            }
            if (right.Type == ASTSimpleNode.EType.Float)
            {
                SimpleFloatNode nr = (SimpleFloatNode)left;
                if (nr.isZero())
                {
                    return right;
                }
                if (nr.isOne())
                {
                    return left;
                }
            }
            return new SimpleMultiplicationNode { Left = left, Right = right };
        }
        static ASTSimpleNode simplifyDivision(SimpleDivisionNode node)
        {
            ASTSimpleNode left = simplify(node.Left);
            ASTSimpleNode right = simplify(node.Right);
            if (left.Type == ASTSimpleNode.EType.Float)
            {
                SimpleFloatNode nl = (SimpleFloatNode)left;
                if (nl.isZero())
                {
                    return left;
                }
                if (right.Type == ASTSimpleNode.EType.Float)
                {
                    SimpleFloatNode nr = (SimpleFloatNode)left;
                    return nl * nr;
                }
            }
            if (right.Type == ASTSimpleNode.EType.Float)
            {
                SimpleFloatNode nr = (SimpleFloatNode)left;
                if (nr.isOne())
                {
                    return left;
                }
            }
            return new SimpleDivisionNode { Left = left, Right = right };
        }
        static ASTSimpleNode simplifyFunction(SimpleFunctionNode node)
        {
            List<ASTSimpleNode> args=new List<ASTSimpleNode>();
            foreach (var arg in node.Args)
            {
                args.Add(simplify(arg));
            }
            return new SimpleFunctionNode(node.Func,args);
        }
        static ASTSimpleNode simplifySubtraction(SimpleSubtractionNode node)
        {
            ASTSimpleNode left = simplify(node.Left);
            ASTSimpleNode right = simplify(node.Right);

            if (left.Type == ASTSimpleNode.EType.Float)
            {
                SimpleFloatNode nl = (SimpleFloatNode)left;
                if (nl.isZero())
                {
                    return right;
                }
                if (right.Type == ASTSimpleNode.EType.Float)
                {
                    SimpleFloatNode nr = (SimpleFloatNode)left;
                    return nl - nr;
                }
            }
            if (right.Type == ASTSimpleNode.EType.Float)
            {
                SimpleFloatNode nr = (SimpleFloatNode)left;

                if (nr.isZero())
                {
                    return left;
                }
            }
            return new SimpleSubtractionNode { Left = left, Right = right };
        }
        static ASTSimpleNode simplifyAddition(SimpleAdditionNode node)
        {
            ASTSimpleNode left = simplify(node.Left);
            ASTSimpleNode right = simplify(node.Right);

            if (left.Type == ASTSimpleNode.EType.Float)
            {
                SimpleFloatNode nl=(SimpleFloatNode)left;
                if (nl.isZero())
                {
                    return right;
                }
                if (right.Type == ASTSimpleNode.EType.Float)
                {
                    SimpleFloatNode nr = (SimpleFloatNode)left;
                    return nl + nr;
                }
            }
            if (right.Type == ASTSimpleNode.EType.Float)
            {
                SimpleFloatNode nr = (SimpleFloatNode)left;
                if (nr.isZero())
                {
                    return left;
                }
            }
            return new SimpleAdditionNode { Left=left,Right=right };
        }
        static public ASTSimpleNode simplify(ASTSimpleNode node)
        {
            switch (node.Type)
            {
                case ASTSimpleNode.EType.Negation:
                    return simplifyNegation((SimpleNegationNode)node);
                case ASTSimpleNode.EType.Power:
                    return simplifyPower((SimplePowerNode)node);
                case ASTSimpleNode.EType.Multiplication:
                    return simplifyMultiplication((SimpleMultiplicationNode)node);
                case ASTSimpleNode.EType.Division:
                    return simplifyDivision((SimpleDivisionNode)node);
                case ASTSimpleNode.EType.Function:
                    return simplifyFunction((SimpleFunctionNode)node);
                case ASTSimpleNode.EType.Subtraction:
                    return simplifySubtraction((SimpleSubtractionNode)node);
                case ASTSimpleNode.EType.Addition:
                    return simplifyAddition((SimpleAdditionNode)node);
                case ASTSimpleNode.EType.Identifier:
                    return node;
                case ASTSimpleNode.EType.Float:
                    return node;
            }
            throw new Exception();
        }
    }
}
