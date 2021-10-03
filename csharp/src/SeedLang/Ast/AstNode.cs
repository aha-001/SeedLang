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

using SeedLang.Common;

namespace SeedLang.Ast {
  // The base class of all AST nodes.
  internal abstract class AstNode {
    // The source code range of this AST node. It could be BlockRange or TextRange.
    public Range Range { get; }

    internal AstNode(Range range) {
      Range = range;
    }

    // Creates the string representation of the AST node.
    public override string ToString() {
      return AstStringBuilder.AstToString(this);
    }
  }
}
