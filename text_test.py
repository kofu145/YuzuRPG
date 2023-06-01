test = ""
print("bla")

def textWrap(text, cutoff):
	output = ""
	dialgoueWords = text.split(" ")
	wordCount = 0
	for word in dialgoueWords:
		wordCount += len(word) + 1
		if (wordCount < cutoff):
			output += word + " "

		else:
			print(output)
			output += "\n"
			output += word + " "
			wordCount = 0


	return output

def borderTextWrap(text, spacing=0, lengthChar = '-', heightChar = '|'):

	strings = text.split("\n")
	greatest = 0
	for strin in strings:
		if len(strin) > greatest:
			greatest = len(strin)

	greatest = 80
	wrappedText = ""
	wrappedBorder = "+"
	spaces = ""
	for i in range(spacing):
		spaces += " "

	for i in range(greatest + spacing * 2):
		wrappedBorder += lengthChar

	wrappedBorder += "+"
	wrappedText += wrappedBorder + "\n"
	for strin in strings:
		extra_space=spaces
		for i in range(greatest - len(strin)):
			extra_space += " "
		wrappedText += heightChar + spaces + strin + extra_space + heightChar + "\n"
	wrappedText += wrappedBorder

	return wrappedText


somethin = """Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua."""
ello= "Hello! I'm a test npc! It's nice to meet you!"
print(borderTextWrap(textWrap(ello, 80), spacing=2))
print(textWrap(somethin, 80))