// Copyright 2021-2022 The SeedV Lab.
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
using SeedLang.Common;
using SeedLang.Runtime;
using Xunit;

namespace SeedLang.Ast.Tests {
  public class ExecutorStatementsTests {
    internal class TestData : TheoryData<Statement, string> {
      private static TextRange _textRange => new TextRange(0, 1, 2, 3);

      public TestData() {
        AddAssignment();
        AddBlock();
        AddExpression();
        AddIf();
        AddIfElse();
        AddList();
        AddSubscript();
        AddSubscriptAssignment();
        AddWhile();
        AddNativeFunctionCall();
        AddVoidFunctionCall();
        AddAddFunction();
      }

      private void AddAssignment() {
        string name = "id";
        var identifier = Expression.Identifier(name, _textRange);
        var one = Expression.NumberConstant(1, _textRange);
        var assignment = Statement.Assignment(identifier, one, _textRange);
        var expectedOutput = $"{_textRange} {name} = 1\n";
        Add(assignment, expectedOutput);
      }

      private void AddBlock() {
        string name = "id";
        var identifier = Expression.Identifier(name, _textRange);
        var one = Expression.NumberConstant(1, _textRange);
        var assignment = Statement.Assignment(identifier, one, _textRange);
        var two = Expression.NumberConstant(2, _textRange);
        var binary = Expression.Binary(identifier, BinaryOperator.Add, two, _textRange);
        var expr = Statement.Expression(binary, _textRange);
        var block = Statement.Block(new Statement[] { assignment, expr }, _textRange);
        var expectedOutput = $"{_textRange} {name} = 1\n" +
                             $"{_textRange} 1 Add 2 = 3\n" +
                             $"{_textRange} Eval 3\n";
        Add(block, expectedOutput);
      }

      private void AddExpression() {
        var one = Expression.NumberConstant(1, _textRange);
        var two = Expression.NumberConstant(2, _textRange);
        var three = Expression.NumberConstant(3, _textRange);
        var left = Expression.Binary(one, BinaryOperator.Add, two, _textRange);
        var binary = Expression.Binary(left, BinaryOperator.Multiply, three, _textRange);
        var expr = Statement.Expression(binary, _textRange);
        var expectedOutput = $"{_textRange} 3 Multiply 3 = 9\n" +
                             $"{_textRange} Eval 9\n";
        Add(expr, expectedOutput);
      }

      private void AddIf() {
        var @true = Expression.BooleanConstant(true, _textRange);
        var @false = Expression.BooleanConstant(false, _textRange);
        var one = Statement.Expression(Expression.NumberConstant(1, _textRange), _textRange);
        var ifTrue = Statement.If(@true, one, null, _textRange);
        var expectedTrueOutput = $"{_textRange} Eval 1\n";
        Add(ifTrue, expectedTrueOutput);
        var ifFalse = Statement.If(@false, one, null, _textRange);
        Add(ifFalse, "");
      }

      private void AddIfElse() {
        var @true = Expression.BooleanConstant(true, _textRange);
        var @false = Expression.BooleanConstant(false, _textRange);
        var one = Statement.Expression(Expression.NumberConstant(1, _textRange), _textRange);
        var two = Statement.Expression(Expression.NumberConstant(2, _textRange), _textRange);
        var ifTrue = Statement.If(@true, one, two, _textRange);
        var expectedTrueOutput = $"{_textRange} Eval 1\n";
        Add(ifTrue, expectedTrueOutput);
        var ifFalse = Statement.If(@false, one, two, _textRange);
        var expectedFalseOutput = $"{_textRange} Eval 2\n";
        Add(ifFalse, expectedFalseOutput);
      }

      private void AddList() {
        var one = Expression.NumberConstant(1, _textRange);
        var two = Expression.NumberConstant(2, _textRange);
        var three = Expression.NumberConstant(3, _textRange);
        var list = Expression.List(new Expression[] { one, two, three }, _textRange);
        var eval = Statement.Expression(list, _textRange);
        var expectedOutput = $"{_textRange} Eval [1, 2, 3]\n";
        Add(eval, expectedOutput);
      }

      private void AddSubscript() {
        var one = Expression.NumberConstant(1, _textRange);
        var two = Expression.NumberConstant(2, _textRange);
        var three = Expression.NumberConstant(3, _textRange);
        var list = Expression.List(new Expression[] { one, two, three }, _textRange);
        var subscript = Expression.Subscript(list, one, _textRange);
        var eval = Statement.Expression(subscript, _textRange);
        var expectedOutput = $"{_textRange} Eval 2\n";
        Add(eval, expectedOutput);
      }

      private void AddSubscriptAssignment() {
        var one = Expression.NumberConstant(1, _textRange);
        var two = Expression.NumberConstant(2, _textRange);
        var three = Expression.NumberConstant(3, _textRange);
        var list = Expression.List(new Expression[] { one, two, three }, _textRange);

        var identifier = Expression.Identifier("a", _textRange);
        var assignList = Statement.Assignment(identifier, list, _textRange);

        var subscript = Expression.Subscript(identifier, one, _textRange);
        var assignment = Statement.Assignment(subscript, three, _textRange);
        var block = Statement.Block(new Statement[] { assignList, assignment }, _textRange);
        var expectedOutput = $"{_textRange} a = [1, 3, 3]\n";
        Add(block, expectedOutput);
      }

      private void AddWhile() {
        var sum = Expression.Identifier("sum", _textRange);
        var i = Expression.Identifier("i", _textRange);
        var zero = Expression.NumberConstant(0, _textRange);
        var one = Expression.NumberConstant(1, _textRange);
        var initialSum = Statement.Assignment(sum, zero, _textRange);
        var initialI = Statement.Assignment(i, zero, _textRange);
        var ops = new ComparisonOperator[] { ComparisonOperator.LessEqual };
        var exprs = new Expression[] { Expression.NumberConstant(10, _textRange) };
        var test = Expression.Comparison(i, ops, exprs, _textRange);
        var addSum = Expression.Binary(sum, BinaryOperator.Add, i, _textRange);
        var assignSum = Statement.Assignment(sum, addSum, _textRange);
        var addI = Expression.Binary(i, BinaryOperator.Add, one, _textRange);
        var assignI = Statement.Assignment(i, addI, _textRange);
        var body = Statement.Block(new Statement[] { assignSum, assignI }, _textRange);
        var @while = Statement.While(test, body, _textRange);
        var evalSum = Statement.Expression(sum, _textRange);
        var program = Statement.Block(new Statement[] { initialSum, initialI, @while, evalSum },
                                      _textRange);
        var expectedOutput = $"{_textRange} i = 11\n" +
                             $"{_textRange} 10 Add 1 = 11\n" +
                             $"{_textRange} 11 LessEqual 10 = False\n" +
                             $"{_textRange} Eval 55\n";
        Add(program, expectedOutput);
      }

      private void AddNativeFunctionCall() {
        var one = Expression.NumberConstant(1, _textRange);
        var two = Expression.NumberConstant(2, _textRange);
        var three = Expression.NumberConstant(3, _textRange);
        var list = Expression.List(new Expression[] { one, two, three }, _textRange);
        var func = Expression.Identifier("len", _textRange);
        var len = Expression.Call(func, new Expression[] { list }, _textRange);
        var eval = Statement.Expression(len, _textRange);
        var expectedOutput = $"{_textRange} Eval 3\n";
        Add(eval, expectedOutput);
      }

      private void AddVoidFunctionCall() {
        var body = Statement.Block(new Statement[] {
          Statement.Expression(Expression.NumberConstant(1, _textRange), _textRange),
          Statement.Return(null, _textRange),
        }, _textRange);
        var func = Statement.FuncDef("func", Array.Empty<string>(), body, _textRange);
        var call = Expression.Call(Expression.Identifier(func.Name, _textRange),
                                   Array.Empty<Expression>(), _textRange);
        string variableName = "a";
        var assignment = Statement.Assignment(Expression.Identifier(variableName, _textRange), call,
                                              _textRange);
        var block = Statement.Block(new Statement[] { func, assignment }, _textRange);
        var expectedOutput = $"{_textRange} {variableName} = None\n" +
                             $"{_textRange} Eval 1\n";
        Add(block, expectedOutput);
      }

      private void AddAddFunction() {
        string variableA = "a";
        string variableB = "b";
        var binary = Expression.Binary(Expression.Identifier(variableA, _textRange),
                                       BinaryOperator.Add,
                                       Expression.Identifier(variableB, _textRange),
                                       _textRange);
        var body = Statement.Block(new Statement[] {
          Statement.Return(binary, _textRange),
        }, _textRange);
        var funcDef = Statement.FuncDef("add", new string[] { variableA, variableB }, body,
                                        _textRange);
        var identifier = Expression.Identifier(funcDef.Name, _textRange);
        var call = Expression.Call(identifier, new Expression[] {
          Expression.NumberConstant(1, _textRange),
          Expression.NumberConstant(2, _textRange),
        }, _textRange);
        string resultName = "a";
        var assignment = Statement.Assignment(Expression.Identifier(resultName, _textRange), call,
                                              _textRange);
        var block = Statement.Block(new Statement[] { funcDef, assignment }, _textRange);
        var expectedOutput = $"{_textRange} {resultName} = 3\n" +
                             $"{_textRange} 1 Add 2 = 3\n";
        Add(block, expectedOutput);
      }
    }

    [Theory]
    [ClassData(typeof(TestData))]
    internal void TestStatement(Statement statement, string expectedOutput) {
      (var executor, var visualizer) = NewExecutorWithVisualizer();
      executor.Run(statement);
      Assert.Equal(expectedOutput, visualizer.ToString());
    }

    private static (Executor, MockupVisualizer) NewExecutorWithVisualizer() {
      var visualizer = new MockupVisualizer();
      var visualizerCenter = new VisualizerCenter();
      visualizerCenter.Register(visualizer);
      var executor = new Executor(visualizerCenter);
      return (executor, visualizer);
    }
  }
}
