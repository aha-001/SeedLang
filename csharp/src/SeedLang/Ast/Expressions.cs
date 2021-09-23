// Copyright 2021 The Aha001 Team.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using SeedLang.Runtime;

namespace SeedLang.Ast {
  // The base class of all expression nodes.
  internal abstract class Expression : AstNode {
    // The factory method to create a binary expression.
    internal static BinaryExpression Binary(Expression left, BinaryOperator op, Expression right) {
      return new BinaryExpression(left, op, right);
    }

    // The factory method to create an identifier expression.
    internal static IdentifierExpression Identifier(string name) {
      return new IdentifierExpression(name);
    }

    // The factory method to create a number constant expression from a string.
    internal static NumberConstantExpression Number(string valueStr) {
      try {
        return Number(double.Parse(valueStr));
      } catch (Exception) {
        return Number(0);
      }
    }

    // The factory method to create a number constant expression.
    internal static NumberConstantExpression Number(double value) {
      return new NumberConstantExpression(value);
    }

    // The factory method to create a string constant expression.
    internal static StringConstantExpression String(string value) {
      return new StringConstantExpression(value);
    }

    // The factory method to create a unary expression.
    internal static UnaryExpression Unary(UnaryOperator op, Expression expr) {
      return new UnaryExpression(op, expr);
    }
  }

  internal class BinaryExpression : Expression {
    public Expression Left { get; set; }
    public BinaryOperator Op { get; set; }
    public Expression Right { get; set; }

    internal BinaryExpression(Expression left, BinaryOperator op, Expression right) {
      Left = left;
      Op = op;
      Right = right;
    }
  }

  internal class IdentifierExpression : Expression {
    public string Name { get; set; }

    internal IdentifierExpression(string name) {
      Name = name;
    }
  }

  internal class NumberConstantExpression : Expression {
    public double Value { get; set; }

    internal NumberConstantExpression(double value) {
      Value = value;
    }
  }

  internal class StringConstantExpression : Expression {
    public string Value { get; set; }

    internal StringConstantExpression(string value) {
      Value = value;
    }
  }

  internal class UnaryExpression : Expression {
    public UnaryOperator Op { get; set; }
    public Expression Expr { get; set; }

    internal UnaryExpression(UnaryOperator op, Expression expr) {
      Op = op;
      Expr = expr;
    }
  }
}