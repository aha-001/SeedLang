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
using System.Diagnostics;
using SeedLang.Common;

namespace SeedLang.Runtime {
  // A helper class to do value operations.
  internal static class ValueHelper {
    internal static Value Add(in Value lhs, in Value rhs) {
      if ((lhs.IsBoolean || lhs.IsNumber) && (rhs.IsBoolean || rhs.IsNumber)) {
        double result = lhs.AsNumber() + rhs.AsNumber();
        CheckOverflow(result);
        return new Value(result);
      } else if (lhs.IsString && rhs.IsString) {
        return new Value(lhs.AsString() + rhs.AsString());
      } else if (lhs.IsList && rhs.IsList) {
        var list = new List<Value>(lhs.AsList());
        list.AddRange(rhs.AsList());
        return new Value(list);
      } else if (lhs.IsTuple && rhs.IsTuple) {
        var list = new List<Value>(lhs.AsTuple());
        list.AddRange(rhs.AsTuple());
        return new Value(list.ToArray());
      } else {
        throw new DiagnosticException(SystemReporters.SeedRuntime, Severity.Error, "", null,
                                      Message.RuntimeErrorUnsupportedOperads);
      }
    }

    internal static Value Subtract(in Value lhs, in Value rhs) {
      double result = lhs.AsNumber() - rhs.AsNumber();
      CheckOverflow(result);
      return new Value(result);
    }

    internal static Value Multiply(in Value lhs, in Value rhs) {
      double result = lhs.AsNumber() * rhs.AsNumber();
      CheckOverflow(result);
      return new Value(result);
    }

    internal static Value Divide(in Value lhs, in Value rhs) {
      if (rhs.AsNumber() == 0) {
        throw new DiagnosticException(SystemReporters.SeedRuntime, Severity.Error, "", null,
                                      Message.RuntimeErrorDivideByZero);
      }
      double result = lhs.AsNumber() / rhs.AsNumber();
      CheckOverflow(result);
      return new Value(result);
    }

    internal static Value FloorDivide(in Value lhs, in Value rhs) {
      if (rhs.AsNumber() == 0) {
        throw new DiagnosticException(SystemReporters.SeedRuntime, Severity.Error, "", null,
                                      Message.RuntimeErrorDivideByZero);
      }
      double result = System.Math.Floor(lhs.AsNumber() / rhs.AsNumber());
      CheckOverflow(result);
      return new Value(result);
    }

    internal static Value Power(in Value lhs, in Value rhs) {
      double result = System.Math.Pow(lhs.AsNumber(), rhs.AsNumber());
      CheckOverflow(result);
      return new Value(result);
    }

    internal static Value Modulo(in Value lhs, in Value rhs) {
      if (rhs.AsNumber() == 0) {
        throw new DiagnosticException(SystemReporters.SeedRuntime, Severity.Error, "", null,
                                      Message.RuntimeErrorDivideByZero);
      }
      double result = lhs.AsNumber() % rhs.AsNumber();
      CheckOverflow(result);
      return new Value(result);
    }

    internal static double BooleanToNumber(bool value) {
      return value ? 1 : 0;
    }

    internal static string BooleanToString(bool value) {
      return $"{value}";
    }

    internal static bool NumberToBoolean(double value) {
      return value != 0;
    }

    internal static string NumberToString(double value) {
      return $"{value}";
    }

    internal static bool StringToBoolean(string value) {
      Debug.Assert(!(value is null));
      return value != "";
    }

    internal static double StringToNumber(string value) {
      try {
        return double.Parse(value);
      } catch (System.Exception) {
        return 0;
      }
    }

    internal static void CheckOverflow(double value, Range range = null) {
      // TODO: do we need separate NaN as another runtime error?
      if (double.IsInfinity(value) || double.IsNaN(value)) {
        throw new DiagnosticException(SystemReporters.SeedRuntime, Severity.Fatal, "", range,
                                      Message.RuntimeErrorOverflow);
      }
    }
  }
}
