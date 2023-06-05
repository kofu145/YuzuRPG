using System.Diagnostics;

namespace YuzuRPG.Core;

public class Conversation
{
    private List<string> dialogue;
    private float divOffset;
    private bool blocking;
    public Conversation(string dialogueFileName, GameData gameData, float divOffset=2f, bool blocking=true)
    {
        dialogue = new List<string>();
        this.divOffset = divOffset;
        this.blocking = blocking;
        using (var sr = new StreamReader(gameData.DIALOGUEFILEPATH + dialogueFileName + ".yrpgd"))
        {
            var line = sr.ReadLine();
            if (line != "Dialogue")
            {
                throw new InvalidDataException("Invalid yrpgd file!");
            }

            line = sr.ReadLine();
            while (line != null)
            {
                dialogue.Add(line);
                line = sr.ReadLine();
            }
        }
        
    }

    public Conversation(string dialogueBuffer, float divOffset=1.5f, bool blocking=true)
    {
        dialogue = dialogueBuffer.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
        this.blocking = blocking;
        this.divOffset = divOffset;
    }

    public void Render(GameData gameData)
    {
        Console.SetCursorPosition(0, (int)(Console.WindowHeight/divOffset));

        var textBox = "";
        for (int i = 0; i < dialogue.Count; i++)
        {
            // split it into multiple lines (text wrap basically) 
            var displayLine = Utils.TextWrap(dialogue[i], gameData.DIALOGUECUTOFF);
            if (blocking)
                displayLine += Environment.NewLine + "Press enter to continue...";
            
            for (int j = 0; j < displayLine.Length; j++)
            {

                Thread.Sleep(gameData.TEXTSCROLLSPEED);
                var toBorderWrap = Utils.ReplaceUpTo(displayLine, j + 1);
                textBox = Utils.BorderWrapText(toBorderWrap, 2, gameData.DIALOGUECUTOFF, 4);
                Console.Write(textBox);
                Console.SetCursorPosition(0, (int)(Console.WindowHeight/divOffset));
                

                
            }
            
            Utils.ClearConsoleKeyBuffer();
            
            while (blocking)
            {
                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Enter)
                    break;
            }
            
        }
        if (blocking)
            Console.Clear();
    }
}