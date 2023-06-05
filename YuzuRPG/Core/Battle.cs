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
        this.gameData = new GameData();
        this.battleData = battleData;
    }

    public void Render()
    {
        var battling = true;
        while (battling)
        {

            // input
            RenderBattleUI();
            battleState.CalculateAIInputs();

            // see it play out
            while (battleState.TurnState.Count > 0)
            {
                battleState.AdvanceTurnState();
                // print what is in buffer as if it were a conversation
                List<string> dialogue = battleState.DialogueBuffer.Split(Environment.NewLine).ToList();

                var battleDialogue = new Conversation(battleState.DialogueBuffer);
                battleDialogue.Render(gameData);

                battleState.DialogueBuffer = "";
                if (battleState.IsBattleOver() != BattleStates.ONGOING)
                {
                    battling = false;
                    break;
                }
            }
        }
    }

    public void RenderBattleUI()
    {
        Console.SetCursorPosition(0, 0);
        // goal: create boxes on top, print the enemy sprite, then boxes for player, then input options
        var exampleString = @"Slime\nLevel 5\n#####-----";
        // then just wrap this into Utils.BorderTextWrap();

        
        // creating enemy boxes
        var boxes = ConstructBoxes(battleState.Enemies);

        Console.Write(boxes);
        var image = GetImage(battleState.Enemies[0].ActorBase.Image);
        Console.WriteLine(new string('=', Console.WindowWidth));
        
        // input
        GetBattleInputs(Console.CursorTop);

    }

    public void GetBattleInputs(int cursorTopStart)
    {
        
        for (int actorId=0; actorId<battleState.Party.Count; actorId++)
        {
            Actor actor = battleState.Party[actorId];
            int currChoice = 0;
            int currSkillChoice = 0;
            int currEnemyChoice = 0;
            bool choosingPrompt = true;
            while (choosingPrompt)
            {
                Console.SetCursorPosition(0, cursorTopStart);
                
                List<string> selections = new List<string>(){ "1. Skills", "2. Items", "3. Run" };
                
                var inputSelection = ConstructInputPrompts(selections, currChoice);
                Console.WriteLine(inputSelection);
                
                ConsoleKeyInfo input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.A:
                        if (currChoice + 1 < selections.Count)
                            currChoice++;
                        break;
                    case ConsoleKey.D:
                        if (currChoice - 1 >= 0)
                            currChoice--;
                        break;
                    case ConsoleKey.Enter:
                        if (currChoice == 0)
                            choosingPrompt = false;
                        break;
                    default:
                        break;
                        
                }
            }

            choosingPrompt = true;
            while (choosingPrompt)
            {
                Console.SetCursorPosition(0, cursorTopStart);

                List<string> skillSelections = new List<string>();

                for (int i=0; i<actor.Skills.Count; i++)
                {
                    skillSelections.Add($"{i+1}. {battleData.SkillTable[actor.Skills[i]].Name}");
                }
                
                var inputSelection = ConstructInputPrompts(skillSelections, currSkillChoice);
                Console.WriteLine(inputSelection);
                
                ConsoleKeyInfo input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.A:
                        if (currSkillChoice + 1 < skillSelections.Count)
                            currSkillChoice++;
                        break;
                    case ConsoleKey.D:
                        if (currSkillChoice - 1 >= 0)
                            currSkillChoice--;
                        break;
                    case ConsoleKey.Enter:
                        choosingPrompt = false;
                        break;
                    default:
                        break;
                        
                }
            }
            
            choosingPrompt = true;
            while (choosingPrompt)
            {
                Console.SetCursorPosition(0, cursorTopStart);

                List<string> enemySelections = new List<string>();

                for (int i=0; i<battleState.Enemies.Count; i++)
                {
                    enemySelections.Add($"{i+1}. {battleState.Enemies[i].Name}");
                }
                
                var inputSelection = ConstructInputPrompts(enemySelections, currEnemyChoice);
                Console.WriteLine(inputSelection);
                
                ConsoleKeyInfo input = Console.ReadKey(true);
                switch (input.Key)
                {
                    case ConsoleKey.A:
                        if (currEnemyChoice + 1 < enemySelections.Count)
                            currEnemyChoice++;
                        break;
                    case ConsoleKey.D:
                        if (currEnemyChoice - 1 >= 0)
                            currEnemyChoice--;
                        break;
                    case ConsoleKey.Enter:
                        choosingPrompt = false;
                        break;
                    default:
                        break;
                        
                }
            }
            battleState.InputAction(currSkillChoice, actorId, new List<int>() { currEnemyChoice });


        }

        
    }

    public string ConstructBoxes(List<Actor> actors)
    {
        string boxes = "";
        bool first = true;
        foreach (var member in battleState.Enemies)
        {
            var boxString = member.Name + Environment.NewLine;
            boxString += $"Level {member.Level}" + Environment.NewLine;
            boxString += $"Element: {member.Element.ToString()}" + Environment.NewLine;
            // calculate HP string based on thing
            int HPChars = Utils.RescaleNormal(member.HP, 0, member.MaxHP, 0, 14);
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

            boxString += $"|{HPBar}|";
            boxString = Utils.BorderWrapText(boxString, 2, 16, 4);
            if (first)
            {
                boxes = boxString;
                first = false;
            }
            else
            {
                string[] alreadyMade = boxes.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                string[] toAdd = boxString.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
                var temp = "";
                for (int i = 0; i < alreadyMade.Length; i++)
                {
                    temp = alreadyMade[i] + toAdd[i];
                }

                boxes = temp;
            }
        }

        return boxes;
    }

    public string ConstructInputPrompts(List<string> choices, int currChoice)
    {
        // make input selection string
        string inputSelection = "";
        for (int i=0; i<choices.Count; i++)
        {
            if (i == currChoice)
                inputSelection += "*";
            else
            {
                inputSelection += " ";
            }

            inputSelection += choices[i];
        }

        return inputSelection;
    }

    public string GetImage(string filename)
    {
        var imagePath = gameData.IMAGEFILEPATH + filename;
        string image;
        
        using (var sr = new StreamReader(imagePath))
        {
            var line = sr.ReadLine();
            if (line != "Image")
            {
                throw new InvalidDataException("Invalid yrpg image file!");
            }

            image = sr.ReadToEnd();
            
        }

        return image;

    }

}