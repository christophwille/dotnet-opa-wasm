param(
  [Parameter(Mandatory=$true)]
  [string[]] $Files=@(),
  [Parameter(Mandatory=$true)]
  [string] $Destination
)

[string[]]$duplicatedBuiltins = jq -s -r "[ .[].builtins | .[] ] | group_by(.name) | map(select(length > 1)) | map((.[0].name))" $Files | ConvertFrom-Json
if ($LASTEXITCODE -ne 0) { throw }

if ($duplicatedBuiltins.Count -gt 0) {
  throw "Duplicated builtins defined: $($duplicatedBuiltins -join ' ')`nExiting."
}

<# Originally from https://gist.github.com/iinuwa/af7f2f038ae817e640e2569e256c3268#file-mergecapabilities-ps1
{
    allow_net: [
      .[]
      | .allow_net[]
    ] | unique,
    builtins:
      [ .[].builtins | .[]]
      | group_by(.name)
      | map(.[-1])
} #>
$output = jq -s "{ builtins: [ .[].builtins | .[]] | group_by(.name) | map(.[-1]), future_keywords: [ .[] | .future_keywords[] ], wasm_abi_versions: [ .[] | .wasm_abi_versions[] ] }" $Files
if ($LASTEXITCODE -ne 0) { throw }

$output | Out-File -FilePath $Destination