## About

Built and tested against Open Policy Agent v0.34.

## Features

* TBD

## Usage

TBD

```csharp
using var opaModule = new OpaModule();
using var module = opaModule.Load("example.wasm");
using var opaPolicy = new OpaPolicy(opaModule, module);

opaPolicy.SetData(@"{""world"": ""world""}");

string input = @"{""message"": ""world""}";
string output = opaPolicy.Evaluate(input);
```

