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
        private Player player;
        private Camera camera;

		public Game(string basePath= @"./Content/")
		{
            Areas = new List<Area>();
            player = new Player(7, 7, 'J');
            camera = new Camera(player, Utils.CAMERAVIEWWIDTH, Utils.CAMERAVIEWHEIGHT);
            
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
            Console.Clear();
            var running = true;
            while (running)
            {
                Console.WriteLine("Rendering!");
                
                // this is still bugged lol, you kinda still need your console window to be big enough
                var renderWidth = Utils.CAMERAVIEWWIDTH < Console.WindowWidth ? Utils.CAMERAVIEWWIDTH : Console.WindowWidth;
                var renderHeight = Utils.CAMERAVIEWHEIGHT < Console.WindowHeight ? Utils.CAMERAVIEWHEIGHT : Console.WindowHeight;

                camera.Width = renderWidth;
                camera.Height = renderHeight;
                
                Areas[CurrentArea].Render(gameData.Tiles, player, camera.GetRender(Areas[CurrentArea].Map), true);
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

                var stepFlag = Areas[CurrentArea].CheckForEventTrigger(player, intent);
                // if true, we've triggered some event!
                switch (stepFlag)
                {
                    case EVENTFLAGS.NONE:
                        break;
                    case EVENTFLAGS.COLLIDE:
                        break;
                    case EVENTFLAGS.WARP:
                        player.X += intent.X;
                        player.Y += intent.Y;
                        var warpTo = Utils.SearchForWarpPoint(player, Areas[CurrentArea].Name, gameData);
                        if (warpTo == null)
                            break;
                        // warpto format: 0 name 1 coordinates
                        var coords = Utils.DecodeStringCoords(warpTo[1]);
                        CurrentArea = Utils.SearchAreaByName(Areas, warpTo[0]);
                        player.X = coords[0];
                        player.Y = coords[1];
                        Console.Clear();
                        break;
                    case EVENTFLAGS.NPC:
                        var intentX = player.X + intent.X;
                        var intentY = player.Y + intent.Y;
                        var getDialogue = Utils.SearchForNPC(intentX, intentY, Areas[CurrentArea].Name, gameData);
                        var convo = new Conversation(getDialogue);
                        convo.Render();
                        
                        break;
                    case EVENTFLAGS.BATTLE:
                        break;
                }
                
                // pre step check!
                if (Areas[CurrentArea].CheckIfIntentCollides(player, intent) && 
                    (stepFlag == EVENTFLAGS.NONE || stepFlag == EVENTFLAGS.COLLIDE))
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

