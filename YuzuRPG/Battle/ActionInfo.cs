using YuzuRPG.Battle.Skills;

namespace YuzuRPG.Battle;

public class ActionInfo
{
    public ISkill Skill;
    public Actor Parent;
    public List<Actor> Targets;

    public ActionInfo(ISkill skill, Actor parent, List<Actor> targets)
    {
        Skill = skill;
        Parent = parent;
        Targets = targets;
    }
}