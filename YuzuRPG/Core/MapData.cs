using System;
using Newtonsoft;
using Newtonsoft.Json;

namespace YuzuRPG.Core
{
	public class MapData
	{
		public string[] Tiles;

		public Dictionary<string, Dictionary<string, string[]>> Warps;

		public Dictionary<string, Dictionary<string, string>> NPC;
	}
}

