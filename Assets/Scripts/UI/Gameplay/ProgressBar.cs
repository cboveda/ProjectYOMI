using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    public int maximum;
    public int current;
    public Image mask;
    public Image fill;
    public Direction fillDirection;
    public Color fillColor;

    public enum Direction : int
    {
        left = Image.OriginHorizontal.Left,
        right = Image.OriginHorizontal.Right
    }

    void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill()
    {
        float fillAmount = (float) current / (float) maximum;
        mask.fillAmount = fillAmount;
        mask.fillOrigin = (int) fillDirection;
        fill.color = fillColor;
    }

    public void SetCurrent(int newValue)
    {
        current = newValue;
    }
}
