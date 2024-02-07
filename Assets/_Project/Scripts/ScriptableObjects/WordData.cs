using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WordData", order = 1)]
public class WordData : ScriptableObject
{
    public enum WordSize { Little, Big }
    
    public Goon.GoonType Goon;
    public string Word;
    public WordSize SizeOfWord;
    public AudioClip WordAudio;
    public AnimationClip FaceClip;
    public AnimationClip StickClip;
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
