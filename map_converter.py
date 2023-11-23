tiles = ["O", "π","#", "|", "/", "\\", "_", "-", "@", "[", "]"]
battle_tiles = ['"', '^']
warp_tiles = ["█"]

input_str = '''
 ______________ 
|______________|
|         & &  |
|[]&           |
|[]      +===+ |
|[]      +===+ |
|___█___█______|'''
col_str = ""
for char in input_str:
	if char in tiles:
		col_str+="1"
	elif char == "\n":
		col_str+="\n"
	elif char in battle_tiles:
		col_str+="4"
	elif char in warp_tiles:
		col_str+="2"
	else:
		col_str+="0"

print(col_str)