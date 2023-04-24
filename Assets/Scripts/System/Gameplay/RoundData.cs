using UnityEngine;

public class RoundData : ScriptableObject
{
    private int _moveIdPlayer1;
    private int _moveIdPlayer2;
    private int _damageToPlayer1;
    private int _damageToPlayer2;

    public int MoveIdPlayer1 { get => _moveIdPlayer1; set => _moveIdPlayer1 = value; }
    public int MoveIdPlayer2 { get => _moveIdPlayer2; set => _moveIdPlayer2 = value; }
    public int DamageToPlayer1 { get => _damageToPlayer1; set => _damageToPlayer1 = value; }
    public int DamageToPlayer2 { get => _damageToPlayer2; set => _damageToPlayer2 = value; }

    public RoundData ()
    {
        _moveIdPlayer1 = -1;
        _moveIdPlayer2 = -1;
        _damageToPlayer1 = 0;
        _damageToPlayer2 = 0;
    }
}
