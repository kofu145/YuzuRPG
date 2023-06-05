﻿using YuzuRPG.Core;

namespace YuzuRPG.Battle.Skills;

public class Slash : Skill
{
    public Slash(BattleData battleData) : base(battleData)
    {
        Name = "Slash";
        Power = 50;
        Accuracy = 80;
        ManaCost = 2;
        Element = Elements.FORCE;
        SkillType = AttackType.Physical; 
    }

    public override void Perform(BattleState battleState, Actor source, List<Actor> targets)
    {
        DoDamage(source, targets[0]);
        PayMana(source);
        battleState.DialogueBuffer += $"{source.Name} used Slash on {targets[0].Name}!" + Environment.NewLine;
        if (battleData.TypeModifiers[(int)Element][(int)targets[0].Element] == .5f)
            battleState.DialogueBuffer += "Slash was not very effective!" + Environment.NewLine;
        else if (battleData.TypeModifiers[(int)Element][(int)targets[0].Element] == 2f)
            battleState.DialogueBuffer += "Slash was very effective!" + Environment.NewLine;
    }

    
}