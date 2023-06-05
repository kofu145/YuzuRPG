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
    
    protected BattleData battleData;

    public Skill(BattleData battleData)
    {
        this.battleData = battleData;
    }
    
    public abstract void Perform(BattleState battleState, Actor source, List<Actor> targets);
    
    protected void DoDamage(Actor source, Actor target)
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

        Random random = new Random();
        var damage = ((((float)source.Level / 5)) * Power * (source.Attack / (float)target.Defense)) / 100 + 3;
        // random
        damage = (int)Math.Round(damage * random.Next(85, 100) / 100);
        
        // type matchup
        damage = (int)Math.Round(damage * battleData.TypeModifiers[(int)Element][(int)target.Element]);
        
        // STAB
        damage = Element == source.Element ? (int)Math.Round(damage * 1.5) : damage;

        
        target.HP -= (int)Math.Round(damage);
        //Console.WriteLine($"Slash is attacking: {target.Name} ({damage} damage to make {target.HP} hp)");
    }

    protected void PayMana(Actor source)
    {
        source.Mana -= ManaCost;
    }
}