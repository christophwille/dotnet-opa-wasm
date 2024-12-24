package example

import rego.v1

default hello := false

hello if input.message == data.world
