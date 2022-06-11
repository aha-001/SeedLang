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

using System.Collections.Generic;

namespace SeedLang.Visualization {
  // The interface of SeedLang VM. The methods of it can only be called during visualization
  // notification.
  public interface IVM {
    public class VariableInfo {
      public string Name { get; }
      public Value Value { get; }

      public VariableInfo(string name, Value value) {
        Name = name;
        Value = value;
      }
    }

    // Gets the list of global variables. Returns false if variable tracking is not enabled.
    bool GetGlobals(out IReadOnlyList<VariableInfo> globals);
    // Gets the list of local variables. Returns false if variable tracking is not enabled.
    bool GetLocals(out IReadOnlyList<VariableInfo> locals);

    // Pauses execution.
    void Pause();
    // Stops execution.
    void Stop();
  }

  internal interface IVMProxy : IVM {
    void Invalid();
  }
}
