﻿using System.Diagnostics;

namespace YuzuRPG.Core;

public class Conversation
{
    private GameData gameData;
    private List<string> dialogue;
    private float divOffset;
    private bool blocking;
    private bool clear;
    
    public Conversation(string dialogueFileName, GameData gameData, float divOffset=1.3f, bool blocking=true, bool clear=true)
    {
        dialogue = new List<string>();
        this.divOffset = divOffset;
        this.blocking = blocking;
        this.clear = clear;
        this.gameData = gameData;
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

    public Conversation(string dialogueBuffer, float divOffset = 1.5f, bool blocking = true, bool clear = true)
    {
        dialogue = dialogueBuffer.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).ToList();
        this.blocking = blocking;
        this.clear = clear;
        this.divOffset = divOffset;
    }

    public void Render(GameData gameData)
    {
        string img = GetNPCImage("priestess.yrpg");
        Console.SetCursorPosition(0, 0);
        Console.Write(img);
        
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
        if (clear)
            Console.Clear();
    }

    public string GetNPCImage(string filename)
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

        var imgCutoff = 25;

        // only get cutoff (first 25 lines of the image)
        string retImg = "";
        var imgArr = image.Split(Environment.NewLine);
        if (imgArr.Length > imgCutoff)
        {
            for (int i = 0; i<imgCutoff; i++)
            {
                retImg += imgArr[i];
                retImg += Environment.NewLine;
            }
        }

        return retImg;
    }
}