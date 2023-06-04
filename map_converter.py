tiles = ["O", "π","#", "|", "/", "\\", "_", "-", "█", "@"]
battle_tiles = ['"', '^']
warp_tiles = ["█"]

input_str = '''
OOOOOOOOOOOOOOOOOOOOO      OOOOOOOOOOOOO
O     ____  |        ......        |   O
O    /    \  \       ....         /    O
O    |    |  /  OOO ....   ____  /  /\ O
O   /     | |  OOOO....   |_/\_| |     O
O  |     /  | OOOO.....   |    |/  /\  O
O  / /\ /   / OOO .. ..   |_█_#||  /\  O
O | | | |  |        ....        |  /\  O
O | | | |  / ""      ...   ^^^^  \   /\O
O | | / / |  """"    ... ^^^^^^^^| /\  O
O | \| /  |  """"    ...   ^^^^^^|     O
O |    |  | """""" .....   ^^^^^ |  /\ O
O \    |  / """""  ....          |     O
O  |   | | """""" .....           \    O
O   \  / | """""  ....   ______    \_  O
O    |/  |  " "   ...   //___ \\\\     \\ O
O    _   |       ...   /|/   /||OO  O| O
O   / \   \ OO  ... .  ||\__// |OO  O| O
O  //\ \  /OOO .....   | \--/_/OO  OO| O
O // | | | OOO  ....  /   _/ OOO   OO| O
O || \ | | OO  ....   |  /      ^^^^^/ O
O ||__|| |    .....   \_/   ^^^^^^^^|  O
O /____\_/ ^^^^^^.       ^^^^^^^^^^ /  O
O      /^^^^^^^^^^^^^^^^ OO        |   O
O  /\ /^^^^^^^^^^^^^^^ OOOO """"   /   O
O     |   ^^^^^^^^ ^^^ OO """"""" |    O
O /\ / OO   .^^^^^^^^^   """""""""|    O
O    | OOO  ....  ^^    """"""""""| /\ O
O /\ /OOO   ....       """""""""""| || O
O   / OO     ....      """""""""""| || O
O  /       """""""""  """"""""""""| /\ O
O |     """""""""""""O""""""""""""| || O
O |    """"""""""""""O""""""""""""| /\ O
O  \  """""""""""""""OO """""""" /     O
O   \ """"""""""""""OOOO         |  __ O
O   |"""""""""""""""OOOO        /  /|||O
O   /"""""""""""""""OOOO       /  // ||O
O  /""""""""""""""""OOO       /  //_ /|O
O  |""""""""""""""""OOO      /   /___\|O
O  /   """"....""""OOO      |   ___    O
O /       .....    OO   __  |  /| |\   O
O/         ....        /  \ |  ||_||   O
O           ....      /   | |  █___\   O
O           ....      \___/ |          O
OOOOOOOOOOO ... OOOOOOOOOOOOOOOOOOOOOOOO'''
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