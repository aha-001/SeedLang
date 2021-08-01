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
  // The base class of all the concrete code range classes.
  public abstract class Range : IEquatable<Range> {
    // Returns if the range is empty. A range can be empty when a diagnostic cannot be associated to
    // a particular code position.
    public abstract bool IsEmpty();

    public abstract override int GetHashCode();

    public abstract override string ToString();

    public abstract bool Equals(Range range);

    public override bool Equals(object obj) {
      return obj is Range objRange && Equals(objRange);
    }

    public static bool operator ==(Range range1, Range range2) {
      if (range1 is null) {
        return range2 is null;
      }
      return range1.Equals(range2);
    }

    public static bool operator !=(Range range1, Range range2) {
      return !(range1 == range2);
    }
  }
}