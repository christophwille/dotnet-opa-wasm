package memorytest

import rego.v1

default allow := false

allow if { input == "open sesame" }