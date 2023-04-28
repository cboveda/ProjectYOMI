using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    public float maximum;
    public float current;
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
        float fillAmount = current / maximum;
        mask.fillAmount = fillAmount;
        mask.fillOrigin = (int) fillDirection;
        fill.color = fillColor;
    }

    public void SetCurrent(float newValue)
    {
        current = newValue;
    }

    public void SetMaximum(float newValue)
    {
        maximum = newValue;
    }
}
