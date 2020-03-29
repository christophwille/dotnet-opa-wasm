opa_windows_amd64 build -d example.rego -o example.wasm "data.example = result"
opa_windows_amd64 build -d rbac.rego -o rbac.wasm "data.app.rbac = result"