using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class JoinCodeButton : MonoBehaviour
{
    [SerializeField] TMP_Text _joinCodeText;
    private string _joinCode;

    public void HandleClick()
    {
        _joinCode = _joinCodeText.text;
        GUIUtility.systemCopyBuffer = _joinCode;
        Debug.Log($"Copied {_joinCodeText.text} to clipboard");
        
        _joinCodeText.text = "COPIED";
        StartCoroutine(ResetText());
    }  

    private IEnumerator ResetText()
    {
        yield return new WaitForSeconds(1);
        _joinCodeText.text = _joinCode;  
    }

}
