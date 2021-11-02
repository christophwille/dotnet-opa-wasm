opa_windows_amd64 build -t wasm -e "example/hello" example.rego
tar -xzf bundle.tar.gz /policy.wasm
copy policy.wasm example.wasm
del policy.wasm
del bundle.tar.gz

opa_windows_amd64 build -t wasm -e "app/rbac" rbac.rego
tar -xzf bundle.tar.gz /policy.wasm
copy policy.wasm rbac.wasm
del policy.wasm
del bundle.tar.gz

.\opa_windows_amd64.exe build example.rego --target wasm --entrypoint "example/hello" --output bundle-example.tar.gz

.\opa_windows_amd64 build -t wasm -e "example" -e "example/one" example-one.rego
tar -xzf bundle.tar.gz /policy.wasm
copy policy.wasm multi.wasm
del policy.wasm