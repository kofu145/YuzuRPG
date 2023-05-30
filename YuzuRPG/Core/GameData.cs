using System;
using Newtonsoft;
using Newtonsoft.Json;

namespace YuzuRPG.Core
{
	public class GameData
	{
		public string[] Tiles;

		public Dictionary<string, Dictionary<string, string>> Warps;
    }
}

