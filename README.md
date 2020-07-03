# Open Policy Agent for .NET

[NuGet package](https://www.nuget.org/packages/Opa.Wasm/)

Call Open Policy Agent (OPA) policies in WASM (Web Assembly) from C# .NET Core

A working sample is in src\Opa.Wasm.ConsoleSample. It mirrors the node sample, but not the node library (currently builtins not hooked up properly).

## References

(KubeCon NA is just starting, so this is an old video) You want to watch [Deep Dive: Open Policy Agent - Torin Sandall, Styra](https://www.youtube.com/watch?v=Vdy26oA3py8) first.

Docs are at https://github.com/open-policy-agent/opa/blob/master/docs/content/wasm.md

Writing policies https://www.openpolicyagent.org/docs/latest/how-do-i-write-policies/

Example and Integrations https://github.com/open-policy-agent/contrib

## Other Open Policy Agent WebAssemby SDKs

* https://github.com/open-policy-agent/npm-opa-wasm/
* https://github.com/open-policy-agent/golang-opa-wasm

## Wasmtime Infos

GitHub repo https://github.com/bytecodealliance/wasmtime-dotnet

Docs https://bytecodealliance.github.io/wasmtime-dotnet/articles/intro.html