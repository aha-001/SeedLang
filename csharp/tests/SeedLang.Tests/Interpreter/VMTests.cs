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
using System.Collections.Generic;
using System.IO;
using SeedLang.Ast;
using SeedLang.Common;
using SeedLang.Runtime;
using SeedLang.Tests.Helper;
using SeedLang.Visualization;
using SeedLang.X;
using Xunit;

namespace SeedLang.Interpreter.Tests {
  public class VMTests {
    [Fact]
    public void TestBinary() {
      var expr = AstHelper.ExpressionStmt(AstHelper.Binary(AstHelper.NumberConstant(1),
                                                           BinaryOperator.Add,
                                                           AstHelper.NumberConstant(2)));

      (string output, VisualizerHelper vh) = Run(expr, new Type[] { typeof(Event.Binary) });
      Assert.Equal("3" + Environment.NewLine, output);
      var expectedOutput = $"{AstHelper.TextRange} 1 Add 2 = 3" + Environment.NewLine;
      Assert.Equal(expectedOutput, vh.EventsToString());
    }

    [Fact]
    public void TestInComparison() {
      var expr = AstHelper.ExpressionStmt(
        AstHelper.Comparison(AstHelper.NumberConstant(1),
                             AstHelper.CompOps(ComparisonOperator.In),
                             AstHelper.Tuple(AstHelper.NumberConstant(1),
                                             AstHelper.NumberConstant(2)))
      );

      (string output, VisualizerHelper vh) = Run(expr, Array.Empty<Type>());
      Assert.Equal("True" + Environment.NewLine, output);
      Assert.Equal("", vh.EventsToString());
    }

    [Fact]
    public void TestUnary() {
      var expr = AstHelper.ExpressionStmt(AstHelper.Unary(UnaryOperator.Negative,
                                                          AstHelper.NumberConstant(1)));
      var eventTypes = new Type[] { typeof(Event.Binary), typeof(Event.Unary) };
      (string output, VisualizerHelper vh) = Run(expr, eventTypes);
      Assert.Equal("-1" + Environment.NewLine, output);
      var expected = $"{AstHelper.TextRange} Negative 1 = -1" + Environment.NewLine;
      Assert.Equal(expected, vh.EventsToString());

      expr = AstHelper.ExpressionStmt(AstHelper.Unary(UnaryOperator.Negative,
        AstHelper.Binary(AstHelper.NumberConstant(1),
                         BinaryOperator.Add,
                         AstHelper.NumberConstant(2))
      ));
      (output, vh) = Run(expr, eventTypes);
      Assert.Equal("-3" + Environment.NewLine, output);
      expected = (
        $"{AstHelper.TextRange} 1 Add 2 = 3\n" +
        $"{AstHelper.TextRange} Negative 3 = -3\n"
      ).Replace("\n", Environment.NewLine);
      Assert.Equal(expected, vh.EventsToString());
    }

    [Fact]
    public void TestFunctionCall() {
      string name = "add";
      string a = "a";
      string b = "b";
      var program = AstHelper.Block(
        AstHelper.FuncDef(name, AstHelper.Params(a, b),
          AstHelper.Return(AstHelper.Binary(AstHelper.Id(a),
                                            BinaryOperator.Add,
                                            AstHelper.Id(b))
        )),
        AstHelper.ExpressionStmt(AstHelper.Call(AstHelper.Id(name),
                                                AstHelper.NumberConstant(1),
                                                AstHelper.NumberConstant(2)))
      );
      var eventTypes = new Type[] {
        typeof(Event.Binary),
        typeof(Event.FuncCalled),
        typeof(Event.FuncReturned),
      };
      (string output, VisualizerHelper vh) = Run(program, eventTypes);
      Assert.Equal("3" + Environment.NewLine, output);
      var expectedOutput = (
        $"{AstHelper.TextRange} 1 Add 2 = 3\n" +
        $"{AstHelper.TextRange} FuncCalled: add(1, 2)\n" +
        $"{AstHelper.TextRange} FuncReturned: add 3\n"
      ).Replace("\n", Environment.NewLine);
      Assert.Equal(expectedOutput, vh.EventsToString());
    }

    [Fact]
    public void TestRecursiveFib() {
      string fib = "fib";
      string n = "n";
      var test = AstHelper.Boolean(BooleanOperator.Or,
        AstHelper.Comparison(AstHelper.Id(n), AstHelper.CompOps(ComparisonOperator.EqEqual),
                             AstHelper.NumberConstant(1)),
        AstHelper.Comparison(AstHelper.Id(n), AstHelper.CompOps(ComparisonOperator.EqEqual),
                             AstHelper.NumberConstant(2))
      );
      var trueBlock = AstHelper.Return(AstHelper.NumberConstant(1));
      var falseBlock = AstHelper.Return(AstHelper.Binary(
        AstHelper.Call(AstHelper.Id(fib), AstHelper.Binary(AstHelper.Id(n), BinaryOperator.Subtract,
                                                           AstHelper.NumberConstant(1))),
        BinaryOperator.Add,
        AstHelper.Call(AstHelper.Id(fib), AstHelper.Binary(AstHelper.Id(n), BinaryOperator.Subtract,
                                                           AstHelper.NumberConstant(2)))
      ));
      var program = AstHelper.Block(
        AstHelper.FuncDef(fib, new string[] { n }, AstHelper.Block(
          AstHelper.If(test, trueBlock, falseBlock)
        )),
        AstHelper.ExpressionStmt(AstHelper.Call(AstHelper.Id(fib), AstHelper.NumberConstant(10)))
      );
      (string output, VisualizerHelper _) = Run(program, Array.Empty<Type>());
      Assert.Equal("55" + Environment.NewLine, output);
    }

    [Fact]
    public void TestSubscript() {
      var program = AstHelper.ExpressionStmt(AstHelper.Subscript(
        AstHelper.List(AstHelper.NumberConstant(1), AstHelper.NumberConstant(2),
                       AstHelper.NumberConstant(3)),
        AstHelper.NumberConstant(1)
      ));
      (string output, VisualizerHelper _) = Run(program, Array.Empty<Type>());
      Assert.Equal("2" + Environment.NewLine, output);
    }

    [Fact]
    public void TestSubscriptAssignment() {
      string a = "a";
      var program = AstHelper.Block(
        AstHelper.Assign(AstHelper.Targets(AstHelper.Id(a)),
                         AstHelper.List(AstHelper.NumberConstant(1),
                                        AstHelper.NumberConstant(2),
                                        AstHelper.NumberConstant(3))),
        AstHelper.Assign(AstHelper.Targets(AstHelper.Subscript(AstHelper.Id(a),
                                                               AstHelper.NumberConstant(1))),
                         AstHelper.NumberConstant(5)),
        AstHelper.ExpressionStmt(AstHelper.Subscript(AstHelper.Id(a), AstHelper.NumberConstant(1)))
      );
      (string output, VisualizerHelper vh) = Run(program,
                                                 new Type[] { typeof(Event.SubscriptAssignment) });
      Assert.Equal("5" + Environment.NewLine, output);
      var expected = $"{AstHelper.TextRange} (global.a: Global)[1] = 5" + Environment.NewLine;
      Assert.Equal(expected, vh.EventsToString());
    }

    [Fact]
    public void TestDict() {
      var program = AstHelper.ExpressionStmt(
        AstHelper.Dict(
          AstHelper.KeyValue(AstHelper.NumberConstant(1), AstHelper.NumberConstant(1)),
          AstHelper.KeyValue(AstHelper.StringConstant("a"), AstHelper.NumberConstant(2))
        )
      );
      (string output, VisualizerHelper _) = Run(program, Array.Empty<Type>());
      Assert.Equal("{1: 1, 'a': 2}" + Environment.NewLine, output);
    }

    [Fact]
    public void TestTuple() {
      var program = AstHelper.ExpressionStmt(
        AstHelper.Tuple(AstHelper.NumberConstant(1),
                        AstHelper.NumberConstant(2),
                        AstHelper.NumberConstant(3))
      );
      (string output, VisualizerHelper _) = Run(program, Array.Empty<Type>());
      Assert.Equal("(1, 2, 3)" + Environment.NewLine, output);
    }

    [Fact]
    public void TestAssignment() {
      string name = "name";
      var program = AstHelper.Block(
        AstHelper.Assign(AstHelper.Targets(AstHelper.Id(name)), AstHelper.NumberConstant(1)),
        AstHelper.ExpressionStmt(AstHelper.Id(name))
      );
      (string output, VisualizerHelper vh) = Run(program, new Type[] { typeof(Event.Assignment) });
      Assert.Equal("1" + Environment.NewLine, output);
      var expected = $"{AstHelper.TextRange} global.{name}: Global = 1" + Environment.NewLine;
      Assert.Equal(expected, vh.EventsToString());
    }

    [Fact]
    public void TestMultipleAssignment() {
      string a = "a";
      string b = "b";
      var block = AstHelper.Block(
        AstHelper.Assign(AstHelper.Targets(AstHelper.Id(a), AstHelper.Id(b)),
                         AstHelper.NumberConstant(1),
                         AstHelper.NumberConstant(2)),
        AstHelper.ExpressionStmt(AstHelper.Id(a)),
        AstHelper.ExpressionStmt(AstHelper.Id(b))
      );
      (string output, VisualizerHelper vh) = Run(block, new Type[] { typeof(Event.Assignment) });
      var expectedOutput = (
        $"1\n" +
        $"2\n"
      ).Replace("\n", Environment.NewLine);
      Assert.Equal(expectedOutput, output);
      var expected = (
        $"{AstHelper.TextRange} global.{a}: Global = 1\n" +
        $"{AstHelper.TextRange} global.{b}: Global = 2\n"
      ).Replace("\n", Environment.NewLine);
      Assert.Equal(expected, vh.EventsToString());
    }

    [Fact]
    public void TestPackAssignment() {
      string name = "id";
      var block = AstHelper.Block(
        AstHelper.Assign(AstHelper.Targets(AstHelper.Id(name)),
                         AstHelper.NumberConstant(1),
                         AstHelper.NumberConstant(2)),
        AstHelper.ExpressionStmt(AstHelper.Id(name))
      );
      (string output, VisualizerHelper vh) = Run(block, new Type[] { typeof(Event.Assignment) });
      Assert.Equal("(1, 2)" + Environment.NewLine, output);
      var expected = $"{AstHelper.TextRange} global.{name}: Global = (1, 2)" + Environment.NewLine;
      Assert.Equal(expected, vh.EventsToString());
    }

    [Fact]
    public void TestUnpackAssignment() {
      string a = "a";
      string b = "b";
      var block = AstHelper.Block(
        AstHelper.Assign(AstHelper.Targets(AstHelper.Id(a), AstHelper.Id(b)),
                         AstHelper.List(AstHelper.NumberConstant(1), AstHelper.NumberConstant(2))),
        AstHelper.ExpressionStmt(AstHelper.Id(a)),
        AstHelper.ExpressionStmt(AstHelper.Id(b))
      );
      (string output, VisualizerHelper _) = Run(block, Array.Empty<Type>());
      var expectedOutput = (
        $"1\n" +
        $"2\n"
      ).Replace("\n", Environment.NewLine);
      Assert.Equal(expectedOutput, output);
    }

    [Fact]
    public void TestSingleStepNotification() {
      string source = @"
# [[ Assign(a) ]]
a = 1
b = 2
";
      (string _, VisualizerHelper vh) = Run(Compile(source), new Type[] {
        typeof(Event.SingleStep),
        typeof(Event.VTagEntered),
        typeof(Event.VTagExited),
      });
      var expected = (
        "[Ln 3, Col 0 - Ln 3, Col 0] SingleStep\n" +
        "[Ln 4, Col 0 - Ln 4, Col 0] SingleStep\n" +
        "[Ln 2, Col 0 - Ln 3, Col 4] VTagEntered: Assign(a)\n" +
        "[Ln 2, Col 0 - Ln 3, Col 4] VTagExited: Assign(1)\n"
      ).Replace("\n", Environment.NewLine);
      Assert.Equal(expected, vh.EventsToString());
    }

    private static (string, VisualizerHelper) Run(Statement program,
                                                  IReadOnlyList<Type> eventTypes) {
      var vm = new VM();
      var vh = new VisualizerHelper(eventTypes);
      vh.RegisterToVisualizerCenter(vm.VisualizerCenter);
      var stringWriter = new StringWriter();
      vm.RedirectStdout(stringWriter);
      var compiler = new Compiler();
      Function func = compiler.Compile(program, vm.Env, vm.VisualizerCenter, RunMode.Interactive);
      vm.Run(func);
      return (stringWriter.ToString(), vh);
    }

    private static Statement Compile(string source) {
      new SeedPython().Parse(source, "", new DiagnosticCollection(), out Statement program,
                             out IReadOnlyList<TokenInfo> _);
      return program;
    }
  }
}
