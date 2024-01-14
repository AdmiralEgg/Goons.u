using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Scrap : MonoBehaviour
{
    public enum ScrapState
    {
        Default,
        Inventory
    }

    [SerializeField, ReadOnly, Tooltip("State of the scrap, governs rigidbody properties and deletion over time")]
    private ScrapState _currentScrapState;

    private TextMeshProUGUI _text;

    public static Action<Scrap> ScrapCaught;

    void Awake()
    {
        // Set the state
        SetScrapState(ScrapState.Default);

        // Look at camera, set random rotation
        this.transform.Rotate(-90, 0, 0);
        
        int randomRotationX = UnityEngine.Random.Range(-2, 3);
        int randomRotationZ = UnityEngine.Random.Range(-5, 5);
        this.transform.Rotate(randomRotationX, 0, randomRotationZ);

        // Get TMP Component
        _text = GetComponentInChildren<TextMeshProUGUI>();

        // Apply random text rotation
        int randomRotation = UnityEngine.Random.Range(-15, 18);

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

    private void OnClickedTrigger()
    {
        Debug.Log("Scrap got a click!");

        // Move the scrap to the inventory
        ScrapCaught(this);
    }

    public void SetScrapState(ScrapState scrapState)
    {
        switch (scrapState)
        {
            case ScrapState.Default:
                _currentScrapState = ScrapState.Default;
                GetComponent<Rigidbody>().isKinematic = false;
                break;
            case ScrapState.Inventory:
                _currentScrapState = ScrapState.Inventory;
                GetComponent<Rigidbody>().isKinematic = true;
                break;
        }
    }
}
