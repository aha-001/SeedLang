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

using Xunit;

namespace SeedLang.Interpreter.Tests {
  public class InstructionTests {
    [Fact]
    public void TestInstructions() {
      var ret = new Instruction(Opcode.RETURN, 1);
      Assert.Equal(Opcode.RETURN, ret.Opcode);
      Assert.Equal(1u, ret.A);
      Assert.Equal(0u, ret.B);
      Assert.Equal(0u, ret.C);

      var add = new Instruction(Opcode.ADD, 1, 2, 3);
      Assert.Equal(Opcode.ADD, add.Opcode);
      Assert.Equal(1u, add.A);
      Assert.Equal(2u, add.B);
      Assert.Equal(3u, add.C);

      var loadK = new Instruction(Opcode.LOADK, 1, 2);
      Assert.Equal(Opcode.LOADK, loadK.Opcode);
      Assert.Equal(1u, loadK.A);
      Assert.Equal(2u, loadK.Bx);
    }

    [Fact]
    public void TestInstructionsToString() {
      var ret = new Instruction(Opcode.RETURN, 1);
      Assert.Equal("RETURN 1", ret.ToString());

      var add = new Instruction(Opcode.ADD, 1, 2, 3);
      Assert.Equal("ADD 1 2 3", add.ToString());

      var loadK = new Instruction(Opcode.LOADK, 1, 2);
      Assert.Equal("LOADK 1 2", loadK.ToString());
    }
  }
}