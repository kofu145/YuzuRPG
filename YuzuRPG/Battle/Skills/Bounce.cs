using YuzuRPG.Core;

namespace YuzuRPG.Battle.Skills;

public class Bounce : Skill
{
    public Bounce(BattleData battleData) : base(battleData)
    {
        Name = "Bounce";
        Power = 10;
        Accuracy = 20;
        ManaCost = 2;
        Element = Elements.FORCE;
        SkillType = AttackType.Physical; 
    }

    public override void Perform(BattleState battleState, Actor source, List<Actor> targets)
    {
        DoDamage(battleState, source, targets[0], AccCheck(battleState, source, targets[0]));
        PayMana(source);
    }

    
}