using System;
namespace YuzuRPG.Core
{
	public class Player
	{
		public int X;
		public int Y;
		public char Model;

		public Player(int startX, int startY, char model)
		{
			X = startX;
			Y = startY;
			Model = model;
		}
	}
}

