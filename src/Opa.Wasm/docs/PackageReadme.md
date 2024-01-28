## About

Built and tested against Open Policy Agent v0.61.0.


## Features

* Simple API on top of Open Policy Agent Web Assembly ABI
* Support for advanced scenarios (runtime and module caching)
* Fast execution path on ABI version 1.2 and greater
* Ability to define builtin0-4
* Extended error messages via opa_abort / Wasmtime.TrapException

## Usage

The simplest use case - load a WASM module into a policy object, pass data and evaluate based on input:

```csharp
using var module = OpaPolicyModule.Load("example.wasm");
using var opaPolicy = module.CreatePolicyInstance();

// Use the typed methods to interact with the policy instance
opaPolicy.SetData(new { world = "world" });
var output = opaPolicy.Evaluate<bool>(new { message = "world" });

// Alternatively, send raw Json for the utmost control (advanced scenario)
opaPolicy.SetDataJson(@"{""world"": ""world""}");
string output = opaPolicy.EvaluateJson(@"{""message"": ""world""}");
```

For higher-performance scenarios, you can keep the engine as well as the loaded WASM module around.
Note that one engine can handle multiple modules, and the OpaPolicyModule keeps the correct reference to 
the engine to guarantee thread safety:

```csharp
using var engine = OpaPolicyModule.CreateEngine();
using var opaPolicyModule = OpaPolicyModule.Load("example.wasm", engine);

// Now instantiate as many policy objects you want on top of the engine & module
using var opaPolicy = opaPolicyModule.CreatePolicyInstance();
```