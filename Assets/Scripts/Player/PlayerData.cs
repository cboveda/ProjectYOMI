using UnityEngine;

public class PlayerData : ScriptableObject
{
    private float _health;
    private float _specialMeter;
    private int _action;
    private int _comboCount;

    public float Health { get => _health; set => _health = value; }
    public float SpecialMeter { get => _specialMeter; set => _specialMeter = value; }
    public int Action { get => _action; set => _action = value; }
    public int ComboCount { get => _comboCount; set => _comboCount = value; }

    public (float health, float specialMeter, int action, int comboCount) GetPlayerDataTuple()
    {
        return (
            health: _health,
            specialMeter: _specialMeter,
            action: _action,
            comboCount: _comboCount);
    }
}
