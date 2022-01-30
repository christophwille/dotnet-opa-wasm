package math.builtins

firstNumber := to_number(input.firstNumber)
secondNumber := to_number(input.secondNumber)

valid {
	is_number(firstNumber)
  is_number(secondNumber)
}

result = x {
  valid
  x := custom.func2(firstNumber, secondNumber)
}
else = -1
