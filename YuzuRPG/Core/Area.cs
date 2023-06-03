using System;

namespace YuzuRPG.Core
{
	public class Area
	{
		private char[][] map;
		public char[][] Map {
			get { return map; }
			private set { map = value; }
		}

		public int Width => map[0].Length;
		public int Height => map.Length;

		private int[][] colMap;
		public int[][] CollisionMap
		{
			get { return colMap; }
			private set { colMap = value; }
		}

		public string Name;

		public Area(string path)
		{
			if (File.Exists(path))
			{
				using (var sr = new StreamReader(path))
				{
					var line = sr.ReadLine();
					if (line != "Map")
					{
						throw new InvalidDataException("Invalid yrpg file!");
					}

					Name = sr.ReadLine();

					line = sr.ReadLine();

					if (line == null)
					{
						throw new InvalidDataException("yrpg map has no valid content!");
					}

					// initialize size of our map
					var rows = (File.ReadLines(path).Count() - 2)/2;
					var columns = line.Count();
					map = new char[rows][];
					colMap = new int[rows][];
					int currRow = 0;
					bool makingColMap = false;
					while (line != null)
					{
						if (line == "Collision")
						{
							// we've reached collision data, handle it separately
							makingColMap = true;
							currRow = 0;
							line = sr.ReadLine();
							continue;
						}
						// note: columns is based upon first line of yrpg file

						if (!makingColMap)
							map[currRow] = new char[columns];
						else
							colMap[currRow] = new int[columns];

						for (int i = 0; i < line.Length; i++)
						{
							// c - '0' converts char to int
							var c = line[i];
							if (!makingColMap)
								map[currRow][i] = c;
							else
								colMap[currRow][i] = (int)(c - '0');
						}

						currRow++;
						line = sr.ReadLine();
					}
					
				}
			}
			else
			{
				throw new FileNotFoundException("Invalid File at path specified!");
			}

		}

		public void Render(string[] tileRef, Player player, char[][] customMap = null, bool center = false)
		{
			var renderMap = map;
			if (customMap != null)
				renderMap = customMap;
			Console.SetCursorPosition(0, 0);

			string namePlate = Utils.BorderWrapText(Name, 1);
			Console.WriteLine(namePlate);

			string borderStr = "+" + new string('-', Width) + "+";
			
            for (int i = 0; i < renderMap.Length; i++)
			{
				for (int j = 0; j < renderMap[0].Length; j++)
				{

					if (player.X == j && player.Y == i && !center)
					{
						Console.Write(player.Model);
					}
					else if (j == renderMap[0].Length / 2 && i == renderMap.Length / 2 && center)
					{
						Console.Write(player.Model);
					}
					else
					{
						switch (renderMap[i][j])
						{
							case '@':
								Console.ForegroundColor = ConsoleColor.White;
								Console.Write(renderMap[i][j]);
								Console.ResetColor();
								break;
							
							case 'O':
								Console.ForegroundColor = ConsoleColor.DarkGray;
								Console.Write(renderMap[i][j]);
								Console.ResetColor();
								break;
							
							default:
								Console.Write(renderMap[i][j]);
								break;
						}
					}
					
				}
				Console.WriteLine();
			}
            Console.WriteLine(new string('=', Console.WindowWidth));
			Console.WriteLine($"Position: {player.X}, {player.Y}");
		}

		public bool CheckIfIntentCollides(Player player, Vector2i intent)
		{
			int x = player.X;
			int y = player.Y;
			int intentX = x + intent.X;
			int intentY = y + intent.Y;

			bool inBounds = intentX >= 0 && intentX < Width &&
					intentY >= 0 && intentY < Height;

			if (!inBounds)
				return false;

			// there's something in the way!
			if (colMap[intentY][intentX] == 1)
			{
				return false;
			}

			return true;
		}

        public EVENTFLAGS CheckForEventTrigger(Player player, Vector2i intent)
        {
	        int x = player.X;
	        int y = player.Y;
	        int intentX = x + intent.X;
	        int intentY = y + intent.Y;
	        
	        bool inBounds = intentX >= 0 && intentX < Width &&
	                        intentY >= 0 && intentY < Height;

	        if (!inBounds)
		        return EVENTFLAGS.NONE;

	        return (EVENTFLAGS)colMap[intentY][intentX];
        }

    }
}

