using YuzuRPG.Core;

namespace YuzuRPG.Battle.Skills;


public enum AttackType
{
    Physical = 0,
    Magical = 1,
    Utility = 2
}

public abstract class Skill : ISkill
{
    // convert these to be JSON serializable in the future maybe???
    public string Name { get; set; }
    public int Power { get; protected set; }
    public int Accuracy { get; protected set; }
    public int ManaCost { get; protected set; }
    public Elements Element { get; protected set; }
    public AttackType SkillType { get; protected set; }
    protected Random random;
    
    protected BattleData battleData;

    public Skill(BattleData battleData)
    {
        this.battleData = battleData;
        random = new Random();

    }
    
    public abstract void Perform(BattleState battleState, Actor source, List<Actor> targets);
    
    protected void DoDamage(BattleState battleState, Actor source, Actor target, bool accCheck)
    {
        int attack = 0;
        int defense = 0;
        if (SkillType == AttackType.Physical)
        {
            attack = (int)Math.Round(source.Attack * source.GetModifier("Attack"));
            defense = (int)Math.Round(target.Defense * target.GetModifier("Defense"));
        }
        if (SkillType == AttackType.Magical)
        {
            attack = (int)Math.Round(source.Magic * source.GetModifier("Magic"));
            defense = (int)Math.Round(target.Resist * target.GetModifier("Resist"));
        }

        var damage = ((((float)source.Level / 5)) * Power * (attack / (float)defense)) / 100 + 3;
        // random
        damage = (int)Math.Round(damage * random.Next(85, 100) / 100);
        
        // type matchup
        damage = (int)Math.Round(damage * battleData.TypeModifiers[(int)Element][(int)target.Element]);
        
        // STAB
        damage = Element == source.Element ? (int)Math.Round(damage * 1.5) : damage;
        
        if (accCheck)
            target.HP -= (int)Math.Round(damage);
        
        if (battleData.TypeModifiers[(int)Element][(int)target.Element] == .5f)
            battleState.DialogueBuffer += $"{Name} was not very effective!" + Environment.NewLine;
        else if (battleData.TypeModifiers[(int)Element][(int)target.Element] == 2f)
            battleState.DialogueBuffer += $"{Name} was very effective!" + Environment.NewLine;
        
    }

    // here in case we need it out in the regular logic, also acccheck is a param in the other functions so that
    // a stat mod and dodamage can share the same acc
    protected bool AccCheck(BattleState battleState, Actor source, Actor target)
    {
        var accCheck = random.Next(0, 100) < Accuracy;
        
        if (accCheck && source != target)
            battleState.DialogueBuffer += $"{source.Name} used {Name} on {target.Name}!" + Environment.NewLine;
        else if (accCheck && source == target)
            battleState.DialogueBuffer += $"{source.Name} used {Name}!" + Environment.NewLine;
        else if (!accCheck && source != target)
            battleState.DialogueBuffer +=
                $"{source.Name} used {Name} on {target.Name}, but missed!!" + Environment.NewLine;
        else
            battleState.DialogueBuffer += 
                $"{source.Name} used {Name}, but it failed!!" + Environment.NewLine;

        return accCheck;
    }

    protected void ModStat(BattleState battleState, Actor target, string stat, int stages)
    {
        var modifiedDialogue = "";

        target.IncrementModifier(stat, stages);

        if (stages > 0)
            modifiedDialogue = "raised";
        else
            modifiedDialogue = "decreased";

        battleState.DialogueBuffer +=
            $"{target.Name}'s {stat} was {modifiedDialogue} by {stages} stages!" + Environment.NewLine;

    }

    protected bool PayMana(Actor source)
    {
        if (source.Mana > ManaCost)
            source.Mana -= ManaCost;

        return source.Mana > ManaCost;
    }
}