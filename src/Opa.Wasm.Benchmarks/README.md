Run `\bin\release\netcoreapp3.1> .\Opa.Wasm.Benchmarks.exe`

``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19041.388 (2004/?/20H1)
Intel Core i7-6600U CPU 2.60GHz (Skylake), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=5.0.100-preview.7.20366.6
  [Host]     : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT
  DefaultJob : .NET Core 3.1.6 (CoreCLR 4.700.20.26901, CoreFX 4.700.20.31603), X64 RyuJIT


|    Method |     Mean |     Error |    StdDev |
|---------- |---------:|----------:|----------:|
| RunPolicy | 1.561 ms | 0.0288 ms | 0.0270 ms |
