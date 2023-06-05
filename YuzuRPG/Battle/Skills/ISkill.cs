namespace YuzuRPG.Battle.Skills;

public interface ISkill
{
    public string Name { get; protected set; }
    public void Perform(BattleState state, Actor source, List<Actor> targets);
}