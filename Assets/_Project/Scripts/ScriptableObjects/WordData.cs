using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WordData", order = 1)]
public class WordData : ScriptableObject
{
    public Goon.GoonType Goon;
    public string Word;
    public int Syllables;
    public AudioClip WordAudio;
    public AnimationClip FaceClip;
    public AnimationClip StickClip;
}
