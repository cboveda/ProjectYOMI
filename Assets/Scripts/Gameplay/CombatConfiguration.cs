using UnityEngine;

[CreateAssetMenu(fileName = "NewCombatConfiguration", menuName = "Configuration/CombatConfiguration")]
public class CombatConfiguration : ScriptableObject
{
    [SerializeField] private float _baseDamage = 10f;
    [SerializeField] private float _baseSpecialGain = 25f;
    [SerializeField] private float _chipDamageModifier = 0.5f;
    [SerializeField] private float _specialGainOnLossModifier = 0.35f;
    [SerializeField] private float _comboDamageMultiplier = 1.5f;

    public float BaseDamage { get => _baseDamage; }
    public float BaseSpecialGain { get => _baseSpecialGain; }
    public float ChipDamageModifier { get => _chipDamageModifier; }
    public float SpecialGainOnLossModifier { get => _specialGainOnLossModifier; }
    public float ComboDamageMultiplier { get => _comboDamageMultiplier; }
}
