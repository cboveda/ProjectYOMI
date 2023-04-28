using TMPro;
using UnityEngine;

public class GameResult : MonoBehaviour
{
    [SerializeField] private TMP_Text _result;
    public string Result { set { _result.text = value; } }
}
