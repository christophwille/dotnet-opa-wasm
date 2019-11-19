# csharp-opa-wasm

Call Open Policy Agent (OPA) policies in WASM (Web Assembly) from C# .NET Core

A working sample is in PlayGroundApp. It mirrors the node sample, but not the node library (currently builtins not hooked up properly).

## References

(KubeCon NA is just starting, so this is an old video) You want to watch [Deep Dive: Open Policy Agent - Torin Sandall, Styra](https://www.youtube.com/watch?v=Vdy26oA3py8) first.

Docs are at https://github.com/open-policy-agent/opa/blob/master/docs/content/wasm.md

JS impl is at https://github.com/open-policy-agent/npm-opa-wasm/blob/master/src/opa.js

Writing policies https://www.openpolicyagent.org/docs/latest/how-do-i-write-policies/

## WasmerSharp Infos

GitHub repo https://github.com/migueldeicaza/WasmerSharp/

Intro article https://migueldeicaza.github.io/WasmerSharp/articles/intro.html