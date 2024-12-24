package math.builtins

import rego.v1

firstNumber := to_number(input.firstNumber)
secondNumber := to_number(input.secondNumber)

valid if {
  is_number(firstNumber)
  is_number(secondNumber)
}

result = x if {
  valid
  x := custom.func2(firstNumber, secondNumber)
}
else = -1
