using System;
using YuzuRPG.Battle;
using YuzuRPG.Battle.Skills;

namespace YuzuRPG.Core
{
	public class Player
	{
		public int X;
		public int Y;
		public char Model;
		public List<Actor> Party;

		public Player(int startX, int startY, char model, BattleData battleData)
		{
			X = startX;
			Y = startY;
			Model = model;
			Party = new List<Actor>();
			
			// some test stuff for now
			Party.Add(new Actor(battleData.ActorModels[1], 5));
			Party[0].Skills.Add(SkillID.SLASH);
		}
	}
}

