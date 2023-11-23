using YuzuRPG.Battle.Skills;

namespace YuzuRPG.Battle;

public class Actor
{
    public ActorModel ActorBase;
    public List<SkillID> Skills;
    public string Name;
    public int Level;
    public Elements Element { get; private set; }
    public int MaxExp { get; private set; }
    public int Exp;
    public int MaxHP { get; private set; }
    public int HP;
    public int MaxMana { get; private set; }
    public int Mana;
    public int Attack { get; private set; }
    public int Defense { get; private set; }
    public int Magic { get; private set; }
    public int Resist { get; private set; }
    public int Speed { get; private set; }

    private Dictionary<string, int> modifiers;

    public Actor(ActorModel actorBase, int level=5)
    {
        ActorBase = actorBase;
        Element = actorBase.Element;
        Name = actorBase.Name;
        Level = level;
        MaxExp = CalculateEXP(Level);
        Exp = MaxExp;
        RecalcStats();
        HP = MaxHP;
        Mana = MaxMana;
        Skills = new List<SkillID>();
        ResetModifiers();
    }

    public bool AddEXP(int exp)
    {
        Exp += exp;
        if (Exp > MaxExp)
        {
            LevelUp();
            return true;
        }

        return false;
    }

    public void LevelUp()
    {
        Level++;
        MaxExp = CalculateEXP(Level);
        Exp = MaxExp - Exp;
        
        RecalcStats();
    }

    private void RecalcStats()
    {
        HP = HP + CalculateStat(Level, ActorBase.HP, true) - MaxHP;
        Mana = Mana + CalculateStat(Level, ActorBase.Mana, true) - MaxMana;
        MaxHP = CalculateStat(Level, ActorBase.HP, true);
        MaxMana = CalculateStat(Level, ActorBase.Mana, true);
        Attack = CalculateStat(Level, ActorBase.Attack);
        Defense = CalculateStat(Level, ActorBase.Defense);
        Magic = CalculateStat(Level, ActorBase.Magic);
        Resist = CalculateStat(Level, ActorBase.Resist);
        Speed = CalculateStat(Level, ActorBase.Speed);
    }

    public static int CalculateStat(int level, int baseStat, bool isHP = false)
    {
        var stat = (2 * baseStat * level) / 100;
        if (isHP)
            stat += level + 10;
        return stat;
    }

    public static int CalculateEXP(int level)
    {
        // TODO: this shit
        return (4 * (level^ 3)) / 5;
    }

    public void IncrementModifier(string stat, int stages)
    {
        if (modifiers[stat] + stages > 4)
            modifiers[stat] = 4;
        else if (modifiers[stat] + stages < -4)
            modifiers[stat] = -4;
        else
            modifiers[stat] += stages;
    }

    public void ResetModifiers()
    {
        modifiers = new Dictionary<string, int>()
        {
            { "Attack", 0 },
            { "Defense", 0 },
            { "Magic", 0 },
            { "Resist", 0 },
            { "Speed", 0 },
        };
    }

    public double GetModifier(string stat)
    {
        double baseNum = 4;
        double baseDenom = 4;

        if (modifiers[stat] > 0)
            baseNum += modifiers[stat];
        if (modifiers[stat] < 0)
            // -= so that subtracting the negative adds to the value
            baseDenom -= modifiers[stat];

        return baseNum / baseDenom;
    }
}