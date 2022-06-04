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
using SeedLang.Visualization;

namespace SeedLang.Interpreter {
  internal class VMProxy : IVM {
    private VM _vm;

    internal VMProxy(VM vm) {
      _vm = vm;
    }

    public bool GetGlobals(out IReadOnlyList<IVM.VariableInfo> globals) {
      if (_vm is null) {
        globals = new List<IVM.VariableInfo>();
        return false;
      }
      return _vm.GetGlobals(out globals);
    }

    public bool GetLocals(out IReadOnlyList<IVM.VariableInfo> locals) {
      if (_vm is null) {
        locals = new List<IVM.VariableInfo>();
        return false;
      }
      return _vm.GetLocals(out locals);
    }

    public void Pause() {
      _vm?.Pause();
    }

    public void Stop() {
      _vm?.Stop();
    }

    internal void Invalid() {
      _vm = null;
    }
  }
}
