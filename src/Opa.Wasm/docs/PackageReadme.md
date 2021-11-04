## About

Built and tested against Open Policy Agent v0.34.

## Features

* TBD

## Usage

TBD

```csharp
using var runtime = new OpaRuntime();
using var module = runtime.Load("example.wasm");
using var opaPolicy = new OpaPolicy(runtime, module);

opaPolicy.SetData(@"{""world"": ""world""}");

string input = @"{""message"": ""world""}";
string output = opaPolicy.Evaluate(input);
```

