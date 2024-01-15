using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Scrap : MonoBehaviour
{
    public enum ScrapState
    {
        Falling,
        Free,
        Inventory,
        Selected
    }

    [SerializeField, ReadOnly, Tooltip("State of the scrap, governs rigidbody properties and deletion over time")]
    private ScrapState _currentScrapState;

    private TextMeshProUGUI _text;

    public static Action<Scrap> ScrapCaught, ScrapSelected;

    //public static Action<Scrap> ;

    void Awake()
    {
        // Set the state
        SetScrapState(ScrapState.Falling);

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

    public void SetFontColor(UnityEngine.Color fontColor)
    {
        _text.color = fontColor;
    }

    public void SetWord(string word)
    {
        _text.text = word;
    }

    private void OnClickedTrigger()
    {
        if (_currentScrapState == ScrapState.Falling) 
        {
            // Move the scrap to the inventory
            ScrapCaught.Invoke(this);
            return;
        }

        if (_currentScrapState == ScrapState.Inventory)
        {
            // Click to click. Highlight object somehow. Make it wiggle? Wiggle plus emission.
            SetScrapState(ScrapState.Selected);

            ScrapSelected?.Invoke(this);
            return;
        }
    }

    public void SetScrapState(ScrapState scrapState)
    {
        _currentScrapState = scrapState;

        switch (scrapState)
        {
            case ScrapState.Free:
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                break;
            case ScrapState.Inventory:
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                // Reset rotation
                this.transform.rotation = Quaternion.identity;
                this.transform.Rotate(-90, 0, 0);
                break;
            case ScrapState.Falling:
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                break;
            case ScrapState.Selected:
                GetComponent<Rigidbody>().isKinematic = true;
                GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
                //GetComponent<MMWiggle>().RotationActive = true;
                break;
        }
    }

    public ScrapState GetScrapState()
    {
        return _currentScrapState;
    }
}
