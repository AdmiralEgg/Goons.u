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

    [SerializeField]
    private TextMeshProUGUI _scrapText;
    [SerializeField]
    private MeshRenderer _meshRenderer;
    [SerializeField]
    private Rigidbody _rigidBody;
    [SerializeField]
    private Animator _animator;

    public static Action<Scrap> ScrapCaught, ScrapSelected, ScrapDestroyed;

    void Awake()
    {
        // Set the state
        SetScrapState(ScrapState.Free);
        SetScrapAttachedState(ScrapAttachedState.None);

        // Look at camera, set random rotation
        this.transform.Rotate(270, 0, 0);

        this.transform.Rotate(new Vector3(0, 0, 1), -5);
        this.transform.Rotate(new Vector3(0, 1, 0), 35);
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
        _scrapText.color = wordData.GetFontColour();
        _scrapText.font = wordData.GetFont();
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
                _meshRenderer.material.DisableKeyword("_EMISSION");
                break;
            case ScrapState.Selected:
                _meshRenderer.material.EnableKeyword("_EMISSION");
                break;
        }
    }

    public void SetScrapAttachedState(ScrapAttachedState scrapAttachedState)
    {
        _currentScrapAttachedState = scrapAttachedState;

        switch (scrapAttachedState)
        {
            case ScrapAttachedState.None:
                _rigidBody.isKinematic = false;
                break;
            case ScrapAttachedState.Inventory:
                _rigidBody.isKinematic = true;
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

    public void PlayProdAnimation()
    {
        _animator.Play("ScrapProd");
    }

    private void OnDestroy()
    {
        ScrapDestroyed?.Invoke(this);
    }
}
