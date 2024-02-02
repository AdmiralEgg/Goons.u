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
    public TMP_FontAsset Font;
    public Color FontColor = Color.white;

    public void SetFont(TMP_FontAsset font)
    {
        Font = font;
    }

    public void SetFontColor(Color fontColor)
    {
        FontColor = fontColor;
    }
}
