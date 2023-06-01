my_map = """07777777777777770
47777777777777774
40000000000000004
40000000000000004
40000000000000004
47779777777777774"""

tiles = [" ", "O", "π","#", "|", "/", "\\", "_", "-", "█", "@"]
output_str = ""
for tile in my_map:
	if tile != "\n":
		output_str += tiles[int(tile)]
	else:
		output_str += "\n"
print(output_str)