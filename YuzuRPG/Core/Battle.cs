using System.Globalization;
using System.Text.RegularExpressions;
using Pastel;
using YuzuRPG.Battle;

namespace YuzuRPG.Core;

public class Battle
{
    private BattleState battleState;
    private BattleData battleData;
    private GameData gameData;

    public Battle(List<Actor> party, List<Actor> enemies, BattleData battleData, GameData gameData)
    {
        battleState = new BattleState(party, enemies, battleData);
        this.gameData = gameData;
        this.battleData = battleData;
        Utils.ResizeForDefaultSize(false);

    }

    public void Render()
    {
        
        var battling = true;
        while (battling)
        {

            // input
            RenderBattleUI();
            // input
            GetBattleInputs(Console.CursorTop);
            battleState.CalculateAIInputs();

            // see it play out
            while (battleState.TurnState.Count > 0 && battling)
            {
                var state = battleState.IsBattleOver();
                Conversation endText;
                switch (state)
                {
                    case BattleStates.ONGOING:
                        battleState.AdvanceTurnState();
                        // print what is in buffer as if it were a conversation
                        List<string> dialogue = battleState.DialogueBuffer.Split(Environment.NewLine).ToList();

                        var battleDialogue = new Conversation(battleState.DialogueBuffer, 1.2f, true, false);
                        RenderBattleUI();
                        battleDialogue.Render(gameData);

                        battleState.DialogueBuffer = "";
                        break;
                    
                    case BattleStates.PLAYERWIN:
                        battling = false;
                        
                        int givenExp = 0;
                        foreach (var actor in battleState.Enemies)
                        {
                            var exp = actor.Level * 2 / 3 + actor.Level * 2;
                            givenExp += exp;
                        }
                        string dialogueBuffer = $"All party members gained {givenExp} exp!" + Environment.NewLine;

                        foreach (var actor in battleState.Party)
                        {
                            var leveledUp = actor.AddEXP(givenExp);
                            if (leveledUp)
                                dialogueBuffer += $"{actor.Name} Leveled up!"+ Environment.NewLine;
                        }
                        var expText = new Conversation(dialogueBuffer, 1.2f);
                        RenderBattleUI();
                        expText.Render(gameData);
                        RenderBattleUI();

                        endText = new Conversation("Your party defeated all the enemies!",
                            1.2f, false, false);
                        endText.Render(gameData);
                        break;
                    
                    case BattleStates.ENEMYWIN:
                        battling = false;
                        RenderBattleUI();
                        endText = new Conversation("Your party was defeated!",
                            1.2f, false, false);
                        endText.Render(gameData);
                        break;
                    
                    case BattleStates.PLAYERRUN:
                        battling = false;
                        RenderBattleUI();
                        endText = new Conversation("You ran away!", 1.2f, false, false);
                        endText.Render(gameData);
                        break;
                    
                }
                
            }
        }
    }

    /// <summary>
    /// Renders all the battle UI for the current ongoing battle
    /// </summary>
    /// <returns>the current cursor top, for GetBattleInputs() to use</returns>
    public void RenderBattleUI()
    {
        Console.SetCursorPosition(0, 0);
        // goal: create boxes on top, print the enemy sprite, then boxes for player, then input options
        var exampleString = @"Slime\nLevel 5\n#####-----";
        // then just wrap this into Utils.BorderTextWrap();

        // creating enemy boxes
        var boxes = ConstructBoxes(battleState.Enemies);

        // currently can't concatenate everything correctly, so rendering one for now, even for multiple
        var image = GetImages(battleState.Enemies[0].ActorBase.Image, battleState.Enemies.Count);
        var partyBoxes = ConstructBoxes(battleState.Party);
        var width = Math.Max(image.Split(Environment.NewLine).First().Length,
            boxes.Split(Environment.NewLine).First().Length);
        
        if (OperatingSystem.IsWindows() && Console.WindowWidth < width)
        {
            // doing this leads to VERY wonky behavior after going back to overworld
            //Console.SetWindowSize(width, Console.WindowHeight);
        }
        
        RenderBoxesWithColor(boxes);
        Console.Write(image);
        Console.WriteLine(new string('=', Console.WindowWidth - 1 > 0 ? Console.WindowWidth - 1 : 0));
        RenderBoxesWithColor(partyBoxes);
        

    }

    public void GetBattleInputs(int cursorTopStart)
    {
        Utils.ClearConsoleKeyBuffer();
        
        for (int actorId=0; actorId<battleState.Party.Count; actorId++)
        {
            Actor actor = battleState.Party[actorId];
            int currChoice = 0;
            int currSkillChoice = 0;
            int currEnemyChoice = 0;
            List<string> selections = new List<string>(){ "1. Skills", "2. Items", "3. Run" };
            
            List<string> skillSelections = new List<string>();
            for (int i=0; i<actor.Skills.Count; i++)
            {
                skillSelections.Add($"{i+1}. {battleData.SkillTable[actor.Skills[i]].Name}");
            }
            
            List<string> enemySelections = new List<string>();
            for (int i=0; i<battleState.Enemies.Count; i++)
            {
                enemySelections.Add($"{i+1}. {battleState.Enemies[i].Name}");
            }

            bool choosing = true;
            while (choosing)
            {
                currChoice = GetPromptLoop(selections, cursorTopStart);
                switch (currChoice){
                    case 0:
                        currSkillChoice = GetPromptLoop(skillSelections, cursorTopStart);
                        currEnemyChoice = GetPromptLoop(enemySelections, cursorTopStart);

                        battleState.InputAction(currSkillChoice, actorId, new List<int>() { currEnemyChoice });
                        choosing = false;
                        break;
                    case 2:
                        var run = battleState.random.Next(0, 100) < 80;
                        if (run)
                            battleState.State = BattleStates.PLAYERRUN;
                        else
                            battleState.DialogueBuffer += "Your party tried to run, but failed!" + Environment.NewLine;
                        choosing = false;
                        break;
                }

            }


        }

        
    }

    public string ConstructBoxes(List<Actor> actors)
    {
        string boxes = "";
        bool first = true;
        foreach (var member in actors)
        {
            var boxString = member.Name + Environment.NewLine;
            boxString += $"Level {member.Level} " + Environment.NewLine;
            boxString += $"Element:   " + 
                         $"{member.Element.ToString()}" + 
                         Environment.NewLine;
            // calculate HP string based on thing
            //int HPChars = Utils.RescaleNormal(member.HP, 0, member.MaxHP, 0, 20);
            int HPChars = (int)Math.Round(member.HP / (float)member.MaxHP * 14);
            string HPBar = "";
            for (int i = 0; i < 14; i++)
            {
                if (i < HPChars)
                    HPBar += "#";
                else
                {
                    HPBar += "-";
                }
            }
            HPBar = $"|{HPBar}| ({member.HP}/{member.MaxHP})";
            boxString += HPBar;
            boxString = Utils.BorderWrapText(boxString, 1, HPBar.Length + 2, 4, true);
            if (first)
            {
                boxes = boxString += Environment.NewLine;
                first = false;
            }
            else
            {
                string[] alreadyMade = boxes.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                string[] toAdd = boxString.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                var temp = "";
                for (int i = 0; i < alreadyMade.Length; i++)
                {
                    temp += alreadyMade[i] + toAdd[i] + Environment.NewLine;
                }

                boxes = temp;
            }
        }

        string finalBox = "";
        foreach (var str in boxes.Split(Environment.NewLine))
        {
            var count = Console.WindowWidth - str.Length - 1;
            finalBox += str + new string(' ', count > 0 ? count : 0) + Environment.NewLine;
        }

        return finalBox;
    }

    private void RenderBoxesWithColor(string box)
    {
        string[] toPrint = box.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        var count = 0;
        foreach (var str in toPrint)
        {
            count++;
            var printing = str;
            var textColorWords = str.Split(" ");
            for (int i=0; i<textColorWords.Length; i++)
            {
                if (battleData.ElementsStrings.Contains(textColorWords[i]))
                {
                    var color = battleData.ElementToColor[battleData.ElementsStrings.IndexOf(textColorWords[i])];
                    string result = Regex.Replace(str,
                        @$"  {textColorWords[i]}\b",
                        " ".PastelBg(color) + $" {textColorWords[i]}".Pastel(color)
                        );

                    printing = result;
                }
            }
            foreach (var ch in printing)
            {
                if (ch == '#')
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(ch);
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(ch);
                }
            }
            if (count < toPrint.Length)
                Console.WriteLine();
        }
    }

    private int GetPromptLoop(List<string> selections, int cursorTopStart)
    {
        var currChoice = 0;
        var choosingPrompt = true;
        while (choosingPrompt)
        {
            Console.SetCursorPosition(0, cursorTopStart);

            var inputSelection = ConstructInputPrompts(selections, currChoice);
            var count = Console.WindowWidth - inputSelection.Length;
            Console.WriteLine(inputSelection + new string(' ', count > 0 ? count : 0));
            Console.Write(new string('=', Console.WindowWidth - 1 > 0 ? Console.WindowWidth - 1 : 0));

            ConsoleKeyInfo input = Console.ReadKey(true);
            var tuple = SelectionInput(input, currChoice, selections.Count);
            choosingPrompt = tuple.Item1;
            currChoice = tuple.Item2;
        }

        return currChoice;
    }

    public Tuple<bool, int> SelectionInput(ConsoleKeyInfo input, int currChoice, int max)
    {
        bool choosing = true;
        switch (input.Key)
        {
            case ConsoleKey.D:
                if (currChoice + 1 < max)
                    currChoice++;
                break;
            case ConsoleKey.A:
                if (currChoice - 1 >= 0)
                    currChoice--;
                break;
            case ConsoleKey.Enter:
                choosing = false;
                break;
            default:
                break;
                        
        }

        return new Tuple<bool, int>(choosing, currChoice);
    }

    public string ConstructInputPrompts(List<string> choices, int currChoice)
    {
        // make input selection string
        string inputSelection = "";
        for (int i=0; i<choices.Count; i++)
        {
            if (i == currChoice)
                inputSelection += " *";
            else
            {
                inputSelection += "  ";
            }

            inputSelection += choices[i];
        }

        return inputSelection + new string(' ', 
            Console.WindowWidth - inputSelection.Length > 0 ? Console.WindowWidth - inputSelection.Length : 0);
    }

    public string GetImages(string filename, int amount)
    {
        var imagePath = gameData.IMAGEFILEPATH + filename;
        string image;
        if (imagePath == filename)
        {
            throw new FileNotFoundException(gameData.DIALOGUEFILEPATH);
        }

        using (var sr = new StreamReader(imagePath))
        {
            var line = sr.ReadLine();
            if (line != "Image")
            {
                throw new InvalidDataException("Invalid yrpg image file!");
            }

            image = sr.ReadToEnd();

        }

        bool first = true;
        string images = "";

        for (int i = 0; i < amount; i++)
        {
            if (first)
            {
                images = image;
                first = false;
            }
            else
            {
                string[] alreadyMade = images.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                string[] toAdd = image.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                var temp = "";
                for (int j = 0; j < toAdd.Length; j++)
                {
                    temp += alreadyMade[j] + toAdd[j];
                    if (j < toAdd.Length - 1)
                        temp += Environment.NewLine;
                }

                images = temp;
            }
        }
        
        string finalImage = "";
        foreach (var str in images.Split(Environment.NewLine))
        {
            var count = Console.WindowWidth - images.Split(Environment.NewLine).Last().Length;
            finalImage += str + new string(' ', count > 0 ? count : 0) + Environment.NewLine;
        }
        
        return finalImage;

    }

}