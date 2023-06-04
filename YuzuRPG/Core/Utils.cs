using System;
using System.Security;
using System.Xml.Linq;

namespace YuzuRPG.Core
{
	public static class Utils
	{
		
		public static string BorderWrapText(string text, int spacing=0, int explicitLength=0, int lines=0, char lengthBorder='-', char heightBorder='|')
		{
			if (explicitLength == 0)
			{
				explicitLength = text.Length + spacing;
			}
			
			string[] strings = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
			
			
            string wrappedText = "";
            string wrappedBorder = "+" + new string(lengthBorder, explicitLength + spacing) + "+";
            wrappedText += wrappedBorder + Environment.NewLine;
            var lineCount = 0;
            foreach (string str in strings)
            {
	            var extraSpaces = new string(' ', explicitLength - str.Length);
	            wrappedText += heightBorder + new string(' ', spacing) + str + 
	                           extraSpaces + 
	                           heightBorder +
	                           Environment.NewLine;
	            lineCount++;
            }

            while (lineCount < lines)
            {
	            wrappedText += heightBorder + new string(' ', explicitLength + spacing) + heightBorder +
	                           Environment.NewLine;
	            lineCount++;
            }
            

            wrappedText += wrappedBorder;

            return wrappedText;
        }

		public static string TextWrap(string text, int cutoff)
		{
			var toPrintLine = "";
			var dialogueWords = text.Split(" ");
			var wordCount = 0;
			for (int i = 0; i < dialogueWords.Length; i++)
			{
				wordCount += dialogueWords[i].Length + 1; // + 1 is to account for the space
				if (wordCount < cutoff)
					toPrintLine += dialogueWords[i] + " ";
				else
				{
					toPrintLine += Environment.NewLine;
					toPrintLine += dialogueWords[i] + " ";
					wordCount = 0;
				}
			}
			// get rid of the last " "
			toPrintLine = toPrintLine.Substring(0, toPrintLine.Length - 1);
			return toPrintLine;
		}

		public static string ReplaceUpTo(string text, int upTo)
		{
			string[] strings = text.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
			var output = "";
			var count = 0;
			foreach (string str in strings)
			{
				
				foreach (char c in str)
				{
					if (count < upTo)
						output += c;
					else
						output += " ";
					count++;
				}

				output += Environment.NewLine;
			}
			

			return output;
		}

		public static string[] SearchForWarpPoint(Player player, string areaName, MapData mapData)
		{
			string currentPos = $"{player.X},{player.Y}";
			if (mapData.Warps.ContainsKey(areaName) && mapData.Warps[areaName].ContainsKey(currentPos))
				return mapData.Warps[areaName][currentPos];
			
			return null;
			
		}
		
		public static string SearchForNPC(int x, int y, string areaName, MapData mapData)
		{
			string currentPos = $"{x},{y}";
			if (mapData.NPC.ContainsKey(areaName) && mapData.NPC[areaName].ContainsKey(currentPos))
				return mapData.NPC[areaName][currentPos];
			
			return null;
			
		}

		public static int[] DecodeStringCoords(string coords)
		{
			var returnCoords = new int[2];

			var count = 0;
			foreach (string coord in coords.Split(",").ToArray())
			{
				returnCoords[count] = Int32.Parse(coord);
				count++;
			}

			return returnCoords;

		}
        
		public static int SearchAreaByName(List<Area> areas, string nameQuery)
		{
			var area = areas.First(e => e.Name == nameQuery);
			return areas.IndexOf(area);
		}

		public static void ClearConsoleKeyBuffer()
		{
			while (Console.KeyAvailable)
			{
				Console.ReadKey(true);
			}
		}
	}
}

