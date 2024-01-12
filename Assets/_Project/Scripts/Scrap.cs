using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    private TextMeshProUGUI _text;
    
    void Awake()
    {
        // Look at camera, set random rotation
        this.transform.Rotate(-90, 0, 0);
        
        int randomRotationX = Random.Range(-2, 3);
        int randomRotationZ = Random.Range(-5, 5);
        this.transform.Rotate(randomRotationX, 0, randomRotationZ);

        // Get TMP Component
        _text = GetComponentInChildren<TextMeshProUGUI>();

        // Apply random text rotation
        int randomRotation = Random.Range(-15, 18);

        _text.rectTransform.Rotate(0, 0, randomRotation);
    }

    public void SetFont(TMP_FontAsset font)
    {
        _text.font = font;
    }

    public void SetFontColor(Color fontColor)
    {
        _text.color = fontColor;
    }

    public void SetWord(string word)
    {
        _text.text = word;
    }
}
