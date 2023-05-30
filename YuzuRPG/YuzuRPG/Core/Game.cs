using System;
using Newtonsoft.Json;
using System.Numerics;

namespace YuzuRPG.Core
{
	public class Game
	{
        public readonly GameData gameData;
        public readonly List<Area> Areas;
        public int CurrentArea = 0;
        public Player player;

		public Game(string basePath= @"./Content/")
		{
            Areas = new List<Area>();
            player = new Player(0, 0, 'J');
            string json;
            using (var sr = new StreamReader(basePath + "data.json"))
            {
                json = sr.ReadToEnd();
            }

            var data = JsonConvert.DeserializeObject<GameData>(json);

            if (data != null)
            {
                gameData = data;
            }
            else
                throw new InvalidDataException("Invalid Game Data file!");

            string[] maps = Directory.GetFiles(basePath + "Maps/", "*.yrpg");
            for (int i = 0; i < maps.Length; i++)
            {
                Areas.Add(new Area(maps[i]));
            }

           

        }

        public void Run()
        {
            SimOverworld();
        }

        public void SimOverworld()
        {
            var running = true;
            while (running)
            {
                Console.WriteLine("Rendering!");
                Areas[CurrentArea].Render(gameData.Tiles, player);
                ConsoleKeyInfo input = Console.ReadKey(true);
                Vector2i intent = new Vector2i(0, 0);

                switch (input.Key)
                {
                    case ConsoleKey.W:
                        intent.Y = -1;
                        break;
                    case ConsoleKey.A:
                        intent.X = -1;
                        break;
                    case ConsoleKey.S:
                        intent.Y = 1;
                        break;
                    case ConsoleKey.D:
                        intent.X = 1;
                        break;
                }

                // pre step check!
                if (Areas[CurrentArea].CheckIfIntentCollides(player, intent))
                {
                    player.X += intent.X;
                    player.Y += intent.Y;
                }

                // post step check!
            }
        }

        public void SimNPCConvo()
        {

        }

        public void SimBattle()
        {

        }

	}
}

