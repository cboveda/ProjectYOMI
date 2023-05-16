using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterComboPathSet", menuName = "Characters/Combo Path Set")]
public class ComboPathSet : ScriptableObject
{
    [SerializeField] private ComboPath _light;
    [SerializeField] private ComboPath _heavy;
    [SerializeField] private ComboPath _grab;
    [SerializeField] private ComboPath _parry;

    public ComboPath Light { get => _light; }
    public ComboPath Heavy { get => _heavy; }
    public ComboPath Grab { get => _grab; }
    public ComboPath Parry { get => _parry; }
}
