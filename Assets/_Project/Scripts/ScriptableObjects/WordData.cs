using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WordData", order = 1)]
public class WordData : ScriptableObject
{
    public enum WordSize
    {
        Little,
        Big
    }
    
    public Goon.GoonType Goon;
    public string Word;
    public WordSize wordSize;
    public AudioClip WordAudio;
    public AnimationClip FaceClip;
    public AnimationClip StickClip;
}
