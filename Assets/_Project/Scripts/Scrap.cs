using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Scrap : MonoBehaviour
{
    public enum ScrapState { Free, Selected }
    public enum ScrapAttachedState { None, Inventory }

    [SerializeField, ReadOnly, Tooltip("State of the scrap, governs rigidbody properties and deletion over time")]
    private ScrapState _currentScrapState;

    [SerializeField, ReadOnly, Tooltip("Whether the scrap is attached to the inventory, or a goon")]
    private ScrapAttachedState _currentScrapAttachedState;

    [SerializeField, ReadOnly]
    private WordData _wordData;

    [SerializeField, ReadOnly]
    private TextMeshProUGUI _scrapText;

    public static Action<Scrap> ScrapCaught, ScrapSelected, ScrapDestroyed;

    void Awake()
    {
        // Set the state
        SetScrapState(ScrapState.Free);
        SetScrapAttachedState(ScrapAttachedState.None);

        // Look at camera, set random rotation
        this.transform.Rotate(-90, 0, 0);
        
        int randomRotationX = UnityEngine.Random.Range(-2, 3);
        int randomRotationZ = UnityEngine.Random.Range(-5, 5);
        this.transform.Rotate(randomRotationX, 0, randomRotationZ);

        // Get TMP Component
        _scrapText = GetComponentInChildren<TextMeshProUGUI>();

        // Apply random text rotation
        int randomRotation = UnityEngine.Random.Range(-15, 18);

        _scrapText.rectTransform.Rotate(0, 0, randomRotation);
    }

    public void SetFont(TMP_FontAsset font)
    {
        _scrapText.font = font;
    }

    public void SetFontColor(UnityEngine.Color fontColor)
    {
        _scrapText.color = fontColor;
    }

    public void SetWordData(WordData wordData)
    {
        _wordData = wordData;
        _scrapText.text = wordData.Word;
        _scrapText.color = wordData.FontColor;
        _scrapText.font = wordData.Font;
        _scrapText.alpha = 255;
    }

    public WordData GetWordData()
    {
        return _wordData;
    }

    private void OnClickedTrigger()
    {
        // If clicking something already selected, ignore it.
        if (_currentScrapState == ScrapState.Selected) return;

        if (_currentScrapState == ScrapState.Free) 
        {
            if (_currentScrapAttachedState == ScrapAttachedState.None)
            {
                // Try moving the scrap to the inventory
                ScrapCaught?.Invoke(this);
                return;
            }

            if (_currentScrapAttachedState == ScrapAttachedState.Inventory)
            {
                // Try moving the scrap to the inventory
                SetScrapState(ScrapState.Selected);
                ScrapSelected?.Invoke(this);
                return;
            }
                
            return;
        }
    }

    public void SetScrapState(ScrapState scrapState)
    {
        _currentScrapState = scrapState;

        switch (scrapState)
        {
            case ScrapState.Free:
                GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
                break;
            case ScrapState.Selected:
                GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");

                break;
        }
    }

    public void SetScrapAttachedState(ScrapAttachedState scrapAttachedState)
    {
        _currentScrapAttachedState = scrapAttachedState;

        switch (scrapAttachedState)
        {
            case ScrapAttachedState.None:
                GetComponent<Rigidbody>().isKinematic = false;
                break;
            case ScrapAttachedState.Inventory:
                GetComponent<Rigidbody>().isKinematic = true;
                // Reset rotation
                this.transform.rotation = Quaternion.identity;
                this.transform.Rotate(-90, 0, 0);
                break;
        }
    }

    public ScrapState GetScrapState()
    {
        return _currentScrapState;
    }

    public Goon.GoonType GetScrapGoonType()
    {
        return _wordData.Goon;
    }

    private void OnDestroy()
    {
        ScrapDestroyed?.Invoke(this);
    }
}
