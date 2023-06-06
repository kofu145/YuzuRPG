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
    
    protected void DoDamage(BattleState battleState, Actor source, Actor target)
    {
        int attack;
        int defense;
        if (SkillType == AttackType.Physical)
        {
            attack = source.Attack;
            defense = target.Defense;
        }
        if (SkillType == AttackType.Magical)
        {
            attack = source.Magic;
            defense = target.Defense;
        }

        var damage = ((((float)source.Level / 5)) * Power * (source.Attack / (float)target.Defense)) / 100 + 3;
        // random
        damage = (int)Math.Round(damage * random.Next(85, 100) / 100);
        
        // type matchup
        damage = (int)Math.Round(damage * battleData.TypeModifiers[(int)Element][(int)target.Element]);
        
        // STAB
        damage = Element == source.Element ? (int)Math.Round(damage * 1.5) : damage;

        var accCheck = random.Next(0, 100) < Accuracy;
        
        if (accCheck)
            target.HP -= (int)Math.Round(damage);
        //Console.WriteLine($"Slash is attacking: {target.Name} ({damage} damage to make {target.HP} hp)");
        
        if (accCheck)
            battleState.DialogueBuffer += $"{source.Name} used Slash on {target.Name}!" + Environment.NewLine;
        else if (!accCheck)
            battleState.DialogueBuffer += $"{source.Name} used Slash on {target.Name}, but missed!!" + Environment.NewLine;
        
        if (battleData.TypeModifiers[(int)Element][(int)target.Element] == .5f)
            battleState.DialogueBuffer += $"{Name} was not very effective!" + Environment.NewLine;
        else if (battleData.TypeModifiers[(int)Element][(int)target.Element] == 2f)
            battleState.DialogueBuffer += $"{Name} was very effective!" + Environment.NewLine;
        
    }

    protected void PayMana(Actor source)
    {
        source.Mana -= ManaCost;
    }
}