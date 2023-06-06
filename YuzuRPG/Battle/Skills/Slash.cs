using YuzuRPG.Core;

namespace YuzuRPG.Battle.Skills;

public class Slash : Skill
{
    public Slash(BattleData battleData) : base(battleData)
    {
        Name = "Slash";
        Power = 50;
        Accuracy = 90;
        ManaCost = 2;
        Element = Elements.FORCE;
        SkillType = AttackType.Physical; 
    }

    public override void Perform(BattleState battleState, Actor source, List<Actor> targets)
    {
        DoDamage(battleState, source, targets[0]);
        PayMana(source);
        
        
    }

    
}