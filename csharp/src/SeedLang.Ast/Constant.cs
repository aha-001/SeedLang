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
using System.Collections.Generic;

namespace SeedLang.Ast {
  // The base class of constant expressions.
  public abstract class Constant : Expression {
  }

  public class NumberConstantExpression : Constant {
    public double Value { get; set; }

    internal NumberConstantExpression(double value) {
      Value = value;
    }

    protected internal override void Accept(IVisitor visitor) {
      visitor.VisitNumberConstant(this);
    }
  }

  public class StringConstantExpression : Constant {
    public string Value { get; set; }

    internal StringConstantExpression(string value) {
      Value = value;
    }

    protected internal override void Accept(IVisitor visitor) {
      visitor.VisitStringConstant(this);
    }
  }
}
