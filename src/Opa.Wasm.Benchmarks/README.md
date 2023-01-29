Run `\bin\release\net6.0> .\Opa.Wasm.Benchmarks.exe`

``` ini

BenchmarkDotNet=v0.13.4, OS=Windows 11 (10.0.22621.1194)
AMD Ryzen 7 PRO 6850U with Radeon Graphics, 1 CPU, 16 logical and 8 physical cores
.NET SDK=7.0.102
  [Host]     : .NET 6.0.13 (6.0.1322.58009), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.13 (6.0.1322.58009), X64 RyuJIT AVX2


|              Method |       Mean |    Error |   StdDev |
|-------------------- |-----------:|---------:|---------:|
|           RunPolicy |   964.7 us | 10.88 us |  8.49 us |
|  FastEvaluatePolicy |   940.9 us | 11.58 us | 10.83 us |
|          RunPolicyX | 2,351.5 us | 25.63 us | 23.98 us |
| FastEvaluatePolicyX | 1,256.8 us | 24.43 us | 42.15 us |
```