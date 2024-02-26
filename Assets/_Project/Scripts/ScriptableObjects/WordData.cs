using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WordData", order = 1)]
public class WordData : ScriptableObject
{
    public enum WordSize { Little, Big }

    [Required]
    public Goon.GoonType Goon;
    [Required]
    public string Word;
    [Required, Tooltip("Name of the sound file returned from FMOD")]
    public string FMODWordDataName;
    public WordSize SizeOfWord;
    private TMP_FontAsset _font;
    private Color _fontColor = Color.white;

    public void SetFont(TMP_FontAsset font)
    {
        _font = font;
    }

    public void SetFontColor(Color fontColor)
    {
        _fontColor = fontColor;
    }

    public Color GetFontColour()
    {
        return _fontColor;
    }
    public TMP_FontAsset GetFont()
    {
        return _font;
    }
}
