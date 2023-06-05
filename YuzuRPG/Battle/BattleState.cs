using YuzuRPG.Core;

namespace YuzuRPG.Battle;

public enum BattleStates
{
    ONGOING = 0,
    PLAYERWIN = 1,
    ENEMYWIN = 2
}

public class BattleState
{
    public List<Actor> Party;
    public List<Actor> Enemies;
    private BattleData battleData;
    private Random random;
    public PriorityQueue<ActionInfo, int> TurnState;
    public string DialogueBuffer;

    public BattleState(List<Actor> party, List<Actor> enemies, BattleData battleData)
    {
        Party = party;
        Enemies = enemies;
        // descending sort for priority queue (fastest actors should dequeue first)
        TurnState = new PriorityQueue<ActionInfo, int>(Comparer<int>.Create((x, y) => y - x));
        random = new Random();
        this.battleData = battleData;
    }

    public void AdvanceTurnState()
    {
        var action = TurnState.Dequeue();
        if (action.Parent.HP > 0)
            action.Skill.Perform(this, action.Parent, action.Targets);
    }

    public void InputAction(int skillId, int actorId, List<int> enemyId)
    {
        var parentActor = Party[actorId];
        List<Actor> targets = new List<Actor>();
        foreach (var id in enemyId)
        {
            targets.Add(Enemies[id]);
        }
        TurnState.Enqueue(
            new ActionInfo(battleData.SkillTable[parentActor.Skills[skillId]], parentActor, targets),
            parentActor.Speed);
    }

    public void CalculateAIInputs()
    {
        foreach (var enemy in Enemies)
        {
            var skillId = random.Next(0, enemy.Skills.Count);
            var target = random.Next(0, Party.Count);
            var targets = new List<Actor>() { Party[target] };

            TurnState.Enqueue(
                new ActionInfo(battleData.SkillTable[enemy.Skills[skillId]], enemy, targets),
                enemy.Speed);
        }
    }

    public BattleStates IsBattleOver()
    {
        bool playerWin = true;
        bool enemyWin = true;
        foreach (var member in Party)
        {
            if (member.HP > 0)
                enemyWin = false;
        }

        foreach (var member in Enemies)
        {
            if (member.HP > 0)
                playerWin = false;
        }

        if (playerWin)
            return BattleStates.PLAYERWIN;
        if (enemyWin)
            return BattleStates.ENEMYWIN;

        return BattleStates.ONGOING;
    }
}