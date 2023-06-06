using YuzuRPG.Battle;
using YuzuRPG.Battle.Skills;

namespace YuzuRPG.Core;

public class BattleData
{
    public List<ActorModel> ActorModels;
    public List<ConsoleColor> ElementToColor = new List<ConsoleColor>()
    {
        ConsoleColor.Red,
        ConsoleColor.Blue,
        ConsoleColor.DarkGreen,
        ConsoleColor.DarkYellow,
        ConsoleColor.Green,
        ConsoleColor.Yellow,
        ConsoleColor.DarkGray,
        ConsoleColor.White,
        ConsoleColor.DarkMagenta
    };
    public List<string> ElementsStrings = Enum.GetNames(typeof(Elements)).ToList();

    public float[][] TypeModifiers;

    // lol this is hella skimpy, if there was a better way I'd be down for it
    public readonly Dictionary<SkillID, ISkill> SkillTable;

    public BattleData()
    {
        SkillTable = new Dictionary<SkillID, ISkill>()
        {
            { SkillID.SLASH, new Slash(this) },
            { SkillID.BOUNCE, new Bounce(this) }
        
        };
    }
}