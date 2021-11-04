function Write-Wasm {
    param (
        [string]$WasmName
    )
    tar -xzf bundle.tar.gz /policy.wasm
    Copy-Item policy.wasm -Destination $WasmName
    Remove-Item policy.wasm
    Remove-Item bundle.tar.gz
}

./opa version

Write-Output "Generating example"
./opa build -t wasm -e "example/hello" example.rego
Write-Wasm -WasmName "example.wasm"

Write-Output "Generating rbac"
./opa build -t wasm -e "app/rbac" rbac.rego
Write-Wasm -WasmName "rbac.wasm"

Write-Output "Generating bundle-example"
./opa build example.rego --target wasm --entrypoint "example/hello" --output bundle-example.tar.gz

Write-Output "Generating multi"
./opa build -t wasm -e "example" -e "example/one" example-one.rego
Write-Wasm -WasmName "multi.wasm"

Write-Output "Generating simplebuiltincall"
./opa build -t wasm -e "builtincallpkg" --capabilities v0.34.0.json simple-custom-builtincall.rego
Write-Wasm -WasmName "simplebuiltincall.wasm"

Write-Output "Generating builtincall"
./opa build -t wasm -e "builtincallsallpkg" --capabilities v0.34.0.json custom-builtincall.rego
Write-Wasm -WasmName "builtincall.wasm"

Write-Output "Generating abort"
./opa build -t wasm -e "aborttestpkg" abort.rego
Write-Wasm -WasmName "abort.wasm"