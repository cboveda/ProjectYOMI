
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacter", menuName = "Characters/Character")]
public class Character : ScriptableObject
{
    [SerializeField] private int id = -1;
    [SerializeField] private string displayName = "New Display Name";
    [SerializeField] private Sprite icon;
    [SerializeField] private GameObject introPrefab;

    public int Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public GameObject IntroPrefab => introPrefab;
}