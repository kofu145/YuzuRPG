using System;
using System.Diagnostics;
using Newtonsoft.Json;
using YuzuRPG.Core.Audio;
using System.Numerics;
using YuzuRPG.Battle;
using YuzuRPG.Battle.Skills;

namespace YuzuRPG.Core
{
	public class Game
	{
        public readonly MapData mapData;
        public readonly GameData gameData;
        public readonly BattleData battleData;
        public readonly List<Area> Areas;
        public int CurrentArea;
        private Player player;
        private Camera camera;
        public AudioManager audioManager;
        private Transition transition;
        private Stopwatch stepTimer;

		public Game(string basePath= @"./Content/")
        {
            mapData = DeserializeJson<MapData>(basePath + "mapdata.json");
            gameData = DeserializeJson<GameData>(basePath + "gamedata.json");
            battleData = DeserializeJson<BattleData>(basePath + "battledata.json");
            battleData.ActorModels = DeserializeJson <List<ActorModel>>(basePath + "models.json");
            
            Areas = new List<Area>();
            player = new Player(gameData.PLAYERSTARTX, gameData.PLAYERSTARTY, 'J', battleData);
            camera = new Camera(player, gameData.CAMERAVIEWWIDTH, gameData.CAMERAVIEWHEIGHT);
            audioManager = new AudioManager(50);
            transition = new Transition(gameData.SCREENTRANSITIONSPEED, TransitionType.STRIPEDSIDEBYSIDE);
            stepTimer = new Stopwatch();

            string[] maps = Directory.GetFiles(basePath + "Maps/", "*.yrpg", SearchOption.AllDirectories);
            
            for (int i = 0; i < maps.Length; i++)
            {
                Areas.Add(new Area(maps[i]));
                if (Areas[i].Name == gameData.STARTINGMAP)
                    CurrentArea = i;
            }

        }

        public void Run()
        {
            stepTimer.Start();
            SimOverworld();
        }

        public void SimOverworld()
        {
            Console.Clear();
            var running = true;
            audioManager.PlayMusic(Areas[CurrentArea].AudioTrack);
            while (running)
            {
                Console.WriteLine("Rendering!");
                
                // this is still bugged lol, you kinda still need your console window to be big enough
                //var renderWidth = Utils.CAMERAVIEWWIDTH < Console.WindowWidth ? Utils.CAMERAVIEWWIDTH : Console.WindowWidth;
                //var renderHeight = Utils.CAMERAVIEWHEIGHT < Console.WindowHeight ? Utils.CAMERAVIEWHEIGHT : Console.WindowHeight;

                camera.Width = Console.WindowWidth;
                camera.Height = Console.WindowHeight - 10;
                Console.CursorVisible = false;
                
                Areas[CurrentArea].Render(mapData.Tiles, player, camera.GetRender(Areas[CurrentArea].Map), true);
                if (stepTimer.Elapsed.Milliseconds > 1)
                {
                    Utils.ClearConsoleKeyBuffer();
                    SimOverworldInput();
                    stepTimer.Restart();
                }

            }
        }

        public void SimNPCConvo()
        {

        }

        public void SimBattle()
        {

        }

        public void SimOverworldInput()
        {
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
                    var warpTo = Utils.SearchForWarpPoint(player, Areas[CurrentArea].Name, mapData);
                    if (warpTo == null)
                        break;
                    // warpto format: 0 name 1 coordinates
                    var coords = Utils.DecodeStringCoords(warpTo[1]);
                    var prevArea = CurrentArea;
                    CurrentArea = Utils.SearchAreaByName(Areas, warpTo[0]);
                    player.X = coords[0];
                    player.Y = coords[1];
                    transition.Render();
                    if (Areas[CurrentArea].AudioTrack != "Transfer" &&
                        Areas[prevArea].AudioTrack != "Transfer")
                    {
                        audioManager.StopMusic();
                        audioManager.PlayMusic(Areas[CurrentArea].AudioTrack);
                    }
                    break;
                case EVENTFLAGS.NPC:
                    var intentX = player.X + intent.X;
                    var intentY = player.Y + intent.Y;
                    var getDialogue = Utils.SearchForNPC(intentX, intentY, Areas[CurrentArea].Name, mapData);
                    var convo = new Conversation(getDialogue, gameData);
                    convo.Render(gameData);
                    
                    break;
                case EVENTFLAGS.BATTLE:
                    Random random = new Random();
                    if (random.Next(1, 50) < 25)
                    {
                        audioManager.StopMusic();
                        audioManager.PlayMusic("Prepare for Battle!");
                        transition.Render();
                        List<Actor> enemies = new List<Actor>();
                        enemies.Add(new Actor(battleData.ActorModels[0], random.Next(3,5)));
                        enemies[0].Skills.Add(SkillID.SLASH);
                        var battle = new Battle(player.Party, enemies, battleData, gameData);
                        battle.Render();
                        audioManager.StopMusic();
                        audioManager.PlayMusic(Areas[CurrentArea].AudioTrack);
                        transition.Render();
                    } 
                    break;
            }
            
            // pre step check!
            if (Areas[CurrentArea].CheckIfIntentCollides(player, intent) && 
                (stepFlag == EVENTFLAGS.NONE || stepFlag == EVENTFLAGS.COLLIDE ||
                 stepFlag == EVENTFLAGS.BATTLE))
            {
                player.X += intent.X;
                player.Y += intent.Y;
            }

            // post step check!
        }

        public T DeserializeJson<T>(string pathName)
        {
            string json;
            T deserializeData;
            using (var sr = new StreamReader(pathName))
            {
                json = sr.ReadToEnd();
                var data = JsonConvert.DeserializeObject<T>(json);
                if (data != null)
                {
                    deserializeData = data;
                }
                else
                    throw new InvalidDataException($"Invalid Data file at {pathName}!");
            }

            return deserializeData;
        }
        
        
	}
}

