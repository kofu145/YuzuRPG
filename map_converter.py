tiles = [" ", "O", "π","#", "|", "/", "\\", "_", "-", "█"]

input_str = """
 _______________
|_______________|
|L              |
|               |
|               |
|___█___________|

"""
col_str = ""
output_str = ""
for char in input_str:
	if char in tiles:
		output_str+=str(tiles.index(char))
		if char != " ":
			col_str+="1"
		else:
			col_str+="0"
	else:
		output_str+=char
		col_str+=char

print(output_str)

print(col_str)