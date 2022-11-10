Run `\bin\release\net6.0> .\Opa.Wasm.Benchmarks.exe`

``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.819)
AMD Ryzen 7 PRO 6850U with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.100
  [Host]     : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.11 (6.0.1122.52304), X64 RyuJIT AVX2


|              Method |       Mean |    Error |   StdDev |
|-------------------- |-----------:|---------:|---------:|
|           RunPolicy |   982.3 us |  6.93 us |  6.48 us |
|  FastEvaluatePolicy |   961.5 us | 16.47 us | 15.40 us |
|          RunPolicyX | 2,652.3 us | 30.96 us | 28.96 us |
| FastEvaluatePolicyX | 1,356.3 us | 26.79 us | 49.65 us |

```