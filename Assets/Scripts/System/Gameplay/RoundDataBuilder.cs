using UnityEngine;

public class RoundDataBuilder
{
    private RoundData _roundData;

    public void StartNewRoundData()
    {
        _roundData = ScriptableObject.CreateInstance<RoundData>();
    }

    public void SetMoveIdPlayer1(int moveId = -1)
    {
        _roundData.MoveIdPlayer1 = moveId;
    }

    public void SetMoveIdPlayer2(int moveId = -1)
    {
        _roundData.MoveIdPlayer2 = moveId;
    }

    public void SetDamageToPlayer1(int value = 0)
    {
        _roundData.DamageToPlayer1 = value;
    }

    public void SetDamageToPlayer2(int value = 0)
    {
        _roundData.DamageToPlayer2 = value;
    }

    public RoundData GetRoundData()
    {
        return _roundData;
    }
}
