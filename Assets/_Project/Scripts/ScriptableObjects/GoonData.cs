using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GoonData", order = 3)]
public class GoonData : ScriptableObject
{
    public Goon.GoonType GoonType;

    [Header("Scrap Details")]
    public TMP_FontAsset WordFont;
    public Color WordColour;

    [Header("Audio Clips")]
    public AudioClip NiceCatchAudio;
    public AudioClip StickTouchAudio;
    public AudioClip SpeakerTouchAudio;
    public AudioClip CrowdTouchAudio;

    [Header("All Words")]
    public WordData[] WordData;
}
