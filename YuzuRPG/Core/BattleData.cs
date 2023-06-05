using YuzuRPG.Battle;
using YuzuRPG.Battle.Skills;

namespace YuzuRPG.Core;

public class BattleData
{
    public List<ActorModel> ActorModels;
    
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