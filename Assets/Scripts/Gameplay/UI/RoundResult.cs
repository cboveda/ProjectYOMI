using TMPro;
using UnityEngine;

public class RoundResult : MonoBehaviour
{
    [SerializeField] private TMP_Text _damageToPlayer1;
    [SerializeField] private TMP_Text _damageToPlayer2;
    [SerializeField] private TMP_Text _moveNamePlayer1;
    [SerializeField] private TMP_Text _moveNamePlayer2;
    [SerializeField] private TMP_Text _moveTypePlayer1;
    [SerializeField] private TMP_Text _moveTypePlayer2;
    [SerializeField] private TMP_Text _result;

    public string DamageToPlayer1 { set => _damageToPlayer1.text = value; }
    public string DamageToPlayer2 { set => _damageToPlayer2.text = value; }
    public string MoveNamePlayer1 { set { _moveNamePlayer1.text = value; } }
    public string MoveNamePlayer2 { set { _moveNamePlayer2.text = value; } }
    public string MoveTypePlayer1 { set { _moveTypePlayer1.text = value; } }
    public string MoveTypePlayer2 { set { _moveTypePlayer2.text = value; } }
    public string Result { set { _result.text = value;} }
}
