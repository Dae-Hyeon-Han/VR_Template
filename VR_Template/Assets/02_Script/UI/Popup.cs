using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public TextMeshProUGUI Text;
    public TextMeshProUGUI Text2;
    public TextMeshProUGUI Button;
    public TextMeshProUGUI Button2;
    public TextMeshProUGUI ToolTip;
    public Image Image;

    public List<Sprite> ImageList = new List<Sprite>();

    public void SetText(string text)
    {
        if(Text != null)
            Text.text = text;
    }
    
    public void SetText2(string text)
    {
        if (Text2 != null)
            Text2.text = text;
    }

    public void SetButtonText(string text)
    {
        if (Button != null)
            Button.text = text;
    }

    public void SetButtonText2(string text)
    {
        if (Button2 != null)
            Button2.text = text;
    }

    public void SetToolTip(string text)
    {
        if (ToolTip != null)
            ToolTip.text = text;
    }

    public void SetImage(string imageId)
    {
        foreach(var imgSprite in ImageList)
        {
            if(imgSprite.name == imageId )
            {
                Image.sprite = imgSprite;
            }
        }
    }

    internal void UpdateLayout()
    {
        var layout = gameObject.GetComponentInChildren<VerticalLayoutGroup>();
        if (layout != null)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
        }
    }
}
