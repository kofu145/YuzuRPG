using System;

namespace YuzuRPG.Core
{
	public class Area
	{
		private int[][] map;
		public int[][] Map {
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

					if (line == null || !line.All(char.IsDigit))
					{
						throw new InvalidDataException("yrpg map has no valid content!");
					}

					// initialize size of our map
					var rows = (File.ReadLines(path).Count() - 2)/2;
					var columns = line.Count();
					map = new int[rows][];
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
							map[currRow] = new int[columns];
						else
							colMap[currRow] = new int[columns];

						for (int i = 0; i < line.Length; i++)
						{
							// c - '0' converts char to int
							var c = line[i];
							if (!makingColMap)
								map[currRow][i] = (int)(c - '0');
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

		public void Render(string[] tileRef, Player player)
		{
			Console.Clear();

			string namePlate = Utils.BorderWrapText(Name, 1);
			Console.WriteLine(namePlate);

			string borderStr = "+" + new string('-', Width) + "+";
			
            Console.WriteLine(borderStr);
            for (int i = 0; i < map.Length; i++)
			{
				Console.Write("|");
				for (int j = 0; j < map[0].Length; j++)
				{
					
					if (player.X == j && player.Y == i)
						Console.Write(player.Model);
					else
						Console.Write(tileRef[map[i][j]]);
				}
                Console.Write("|");
				Console.WriteLine();
			}
            Console.WriteLine(borderStr);
			Console.WriteLine(player.X);
			Console.WriteLine(player.Y);
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

        public void CheckForEventTrigger(Player player, Vector2i intent)
        {

        }

    }
}

