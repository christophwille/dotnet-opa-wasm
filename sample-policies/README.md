# Sample Policies

* **example.rego** is taken from https://github.com/open-policy-agent/npm-opa-wasm/tree/master/examples/nodejs-app
* **rbac.rego** is simplified from https://play.openpolicyagent.org/

## Step 1: Merging Capabilities

Main file from https://github.com/open-policy-agent/opa/tree/main/capabilities

`
.\concat-capabilities.ps1 -Files v0.46.1.json, simple-custom-builtincall.capabilities.json, custom-builtincall.capabilities.json -Destination unittest.capabilities.json
`

## Step 2: Building WASMs

Binaries: https://github.com/open-policy-agent/opa/releases (Windows: rename to `opa.exe`, place in folder)

The WASM files in this folder were built on Windows using `build.ps1`