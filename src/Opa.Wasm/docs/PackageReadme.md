## About

Built and tested against Open Policy Agent v0.34.

## Features

* Simple API on top of Open Policy Agent Web Assembly ABI
* Support for advanced scenarios (runtime and module caching)
* Fast execution path on ABI version 1.2 and greater
* Ability to define builtin0-4
* Extended error messages via opa_abort / Wasmtime.TrapException

## Usage

The simplest use case - load a WASM module into a policy object, pass data and evaluate based on input:

```csharp
using var opaPolicy = new OpaPolicy("example.wasm");
opaPolicy.SetData(@"{""world"": ""world""}");
string output = opaPolicy.Evaluate(@"{""message"": ""world""}");
```

For higher-performance scenarios, you can keep the runtime as well as the loaded WASM module around:

```csharp
using var opaRuntime = new OpaRuntime();
using var module = opaRuntime.Load("example.wasm");

// Now instantiate as many policy objects you want on top of the runtime & module
using var opaPolicy = new OpaPolicy(opaRuntime, module);
```