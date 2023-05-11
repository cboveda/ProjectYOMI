using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinCodeButton : MonoBehaviour
{
    [SerializeField] TMP_Text _joinCodeText;

    public void HandleClick()
    {
        GUIUtility.systemCopyBuffer = _joinCodeText.text;
        Debug.Log($"Copied {_joinCodeText.text} to clipboard");
    }
}
