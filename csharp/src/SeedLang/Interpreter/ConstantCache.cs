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

using System.Collections.Generic;
using System.Diagnostics;
using SeedLang.Runtime;

namespace SeedLang.Interpreter {
  // A cache class to cache the constant id of constants. It only adds the unique constant into the
  // constant list of the chunk.
  internal class ConstantCache {
    // A list to collect constatnt values during compilation.
    public List<Value> Constants { get; } = new List<Value>();

    private readonly Dictionary<double, uint> _numbers = new Dictionary<double, uint>();
    private readonly Dictionary<string, uint> _strings = new Dictionary<string, uint>();

    // Returns the id of a given number constant. The number is added into the constant list if it
    // is not exist.
    internal uint IdOfConstant(double number) {
      if (!_numbers.ContainsKey(number)) {
        Constants.Add(new Value(number));
        _numbers[number] = IdOfLastConst();
      }
      return _numbers[number];
    }

    // Returns the id of a given string constant. The string is added into the constant list if it
    // is not exist.
    internal uint IdOfConstant(string str) {
      if (!_strings.ContainsKey(str)) {
        Constants.Add(new Value(str));
        _strings[str] = IdOfLastConst();
      }
      return _strings[str];
    }

    private uint IdOfLastConst() {
      Debug.Assert(Constants.Count >= 1);
      return (uint)Constants.Count - 1 + Chunk.MaxRegisterCount;
    }
  }
}
