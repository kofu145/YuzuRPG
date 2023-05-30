using System;
using System.Xml.Linq;

namespace YuzuRPG.Core
{
	public static class Utils
	{
		public static string BorderWrapText(string text, int spacing=0, char lengthBorder='-', char heightBorder='|')
		{
            string wrappedText = "";
            string wrappedBorder = "+" + new string(lengthBorder, text.Length + spacing * 2) + "+";
            wrappedText += wrappedBorder + Environment.NewLine;

            wrappedText += heightBorder + new string(' ', spacing);
            wrappedText += text;
            wrappedText += new string(' ', spacing) + heightBorder + Environment.NewLine;

            wrappedText += wrappedBorder;

            return wrappedText;
        }
	}
}

