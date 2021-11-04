./opa version

Write-Output "Generating example"
./opa build -t wasm -e "example/hello" example.rego
tar -xzf bundle.tar.gz /policy.wasm
Copy-Item policy.wasm -Destination example.wasm
Remove-Item policy.wasm
Remove-Item bundle.tar.gz

Write-Output "Generating rbac"
./opa build -t wasm -e "app/rbac" rbac.rego
tar -xzf bundle.tar.gz /policy.wasm
Copy-Item policy.wasm -Destination rbac.wasm
Remove-Item policy.wasm
Remove-Item bundle.tar.gz

Write-Output "Generating bundle-example"
./opa build example.rego --target wasm --entrypoint "example/hello" --output bundle-example.tar.gz

Write-Output "Generating multi"
./opa build -t wasm -e "example" -e "example/one" example-one.rego
tar -xzf bundle.tar.gz /policy.wasm
Copy-Item policy.wasm -Destination multi.wasm
Remove-Item policy.wasm

Write-Output "Generating simplebuiltincall"
./opa build -t wasm -e "builtincallpkg" --capabilities v0.34.0.json simple-custom-builtincall.rego
tar -xzf bundle.tar.gz /policy.wasm
Copy-Item policy.wasm -Destination simplebuiltincall.wasm
Remove-Item policy.wasm
Remove-Item bundle.tar.gz

Write-Output "Generating builtincall"
./opa build -t wasm -e "builtincallsallpkg" --capabilities v0.34.0.json custom-builtincall.rego
tar -xzf bundle.tar.gz /policy.wasm
Copy-Item policy.wasm -Destination builtincall.wasm
Remove-Item policy.wasm
Remove-Item bundle.tar.gz

Write-Output "Generating abort"
./opa build -t wasm -e "aborttestpkg" abort.rego
tar -xzf bundle.tar.gz /policy.wasm
Copy-Item policy.wasm -Destination abort.wasm
Remove-Item policy.wasm
Remove-Item bundle.tar.gz