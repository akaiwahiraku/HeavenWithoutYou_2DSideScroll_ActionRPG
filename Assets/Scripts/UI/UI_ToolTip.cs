using TMPro;
using UnityEngine;

public class UI_ToolTip : MonoBehaviour
{
    [SerializeField] private float xLimit = 960;
    [SerializeField] private float yLimit = 540;

    [SerializeField] private float xOffset = 150;
    [SerializeField] private float yOffset = 150;

    public virtual void AdjustPosition()
    {
        // 新しいInput Systemでのマウス位置取得
        //Vector2 mousePosition = Mouse.current.position.ReadValue();

        //float newXoffset = 0;
        //float newYoffset = 0;

        //if (mousePosition.x > xLimit)
        //    newXoffset = -xOffset;
        //else
        //    newXoffset = xOffset;

        //if (mousePosition.y > yLimit)
        //    newYoffset = -yOffset;
        //else
        //    newYoffset = yOffset;

        //transform.position = new Vector2(mousePosition.x + newXoffset, mousePosition.y + newYoffset);
    }

    public void AdjustFontSize(TextMeshProUGUI _text)
    {
        if (_text.text.Length > 12)
            _text.fontSize = _text.fontSize * .8f;
    }
}
