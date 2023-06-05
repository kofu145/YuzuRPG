using YuzuRPG.Core;

namespace YuzuRPG.Battle.Skills;

public class Bounce : Skill
{
    public Bounce(BattleData battleData) : base(battleData)
    {
        Name = "Bounce";
        Power = 10;
        Accuracy = 30;
        ManaCost = 2;
        Element = Elements.FORCE;
        SkillType = AttackType.Physical; 
    }

    public override void Perform(BattleState battleState, Actor source, List<Actor> targets)
    {
        var prevHp = targets[0].HP;
        DoDamage(source, targets[0]);
        PayMana(source);
        battleState.DialogueBuffer += $"{source.Name} used {Name} on {targets[0].Name}!" + Environment.NewLine;
        if (targets[0].HP == prevHp)
        {
            battleState.DialogueBuffer += $"{source.Name} missed!!" + Environment.NewLine;
        }
        if (battleData.TypeModifiers[(int)Element][(int)targets[0].Element] == .5f)
            battleState.DialogueBuffer += $"{Name} was not very effective!" + Environment.NewLine;
        else if (battleData.TypeModifiers[(int)Element][(int)targets[0].Element] == 2f)
            battleState.DialogueBuffer += $"{Name} was very effective!" + Environment.NewLine;
    }

    
}