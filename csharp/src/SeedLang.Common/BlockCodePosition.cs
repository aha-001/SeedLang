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

namespace SeedLang.Common {
  // An immutable class to represent a position in a SeedBlock source code.
  //
  // Typically a position in a block-style language is an ID of a specified block.
  public class BlockCodePosition : CodePostion {
    public string BlockId { get; }

    public BlockCodePosition(string blockId) {
      BlockId = blockId;
    }

    public override int GetHashCode() {
      return BlockId.GetHashCode();
    }

    public override string ToString() {
      return $"Block {BlockId}";
    }

    public override int CompareTo(CodePostion pos) {
      throw new NotSupportedException();
    }

    public override bool Equals(CodePostion pos) {
      return (pos is BlockCodePosition blockCodePosition) && (BlockId == blockCodePosition.BlockId);
    }
  }
}
