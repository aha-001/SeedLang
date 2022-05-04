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

using System.Collections.Immutable;
using SeedLang.Common;
using Xunit;

namespace SeedLang.Runtime.Tests {
  using Range = HeapObject.Range;

  public class HeapObjectTests {
    [Fact]
    public void TestRange() {
      var range = new HeapObject(new Range(10));
      Assert.Equal(10, range.Length);
      for (int i = 0; i < 10; i++) {
        Assert.Equal(i, range[new VMValue(i)].AsNumber());
      }

      int start = 3;
      int stop = 10;
      int step = 2;
      int length = 4;
      range = new HeapObject(new Range(start, stop, step));
      Assert.Equal(length, range.Length);
      for (int i = 0; i < length; i++) {
        Assert.Equal(start + i * step, range[new VMValue(i)].AsNumber());
      }

      start = 10;
      stop = 2;
      step = -3;
      length = 3;
      range = new HeapObject(new Range(start, stop, step));
      Assert.Equal(length, range.Length);
      for (int i = 0; i < length; i++) {
        Assert.Equal(start + i * step, range[new VMValue(i)].AsNumber());
      }

      start = 1;
      stop = 10;
      step = -1;
      length = 0;
      range = new HeapObject(new Range(start, stop, step));
      Assert.Equal(length, range.Length);
    }

    [Fact]
    public void TestTuple() {
      var tuple = new HeapObject(ImmutableArray.Create<VMValue>());
      Assert.Equal(0, tuple.Length);
      Assert.Equal("()", tuple.AsString());

      tuple = new HeapObject(ImmutableArray.Create(new VMValue(1), new VMValue(2)));
      Assert.Equal(2, tuple.Length);
      Assert.Equal(1, tuple[new VMValue(0)].AsNumber());
      Assert.Equal(2, tuple[new VMValue(1)].AsNumber());
      Assert.Equal("(1, 2)", tuple.AsString());

      var anotherTuple = new HeapObject(ImmutableArray.Create(new VMValue(1), new VMValue(2)));
      Assert.Equal(anotherTuple, tuple);
      Assert.Equal(anotherTuple.GetHashCode(), tuple.GetHashCode());

      var ex = Assert.Throws<DiagnosticException>(() => tuple[new VMValue(1)] = new VMValue());
      Assert.Equal(Message.RuntimeErrorNotSupportAssignment, ex.Diagnostic.MessageId);
    }
  }
}
