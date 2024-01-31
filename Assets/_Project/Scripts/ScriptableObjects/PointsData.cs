using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/PointsData", order = 4)]
public class PointsData : ScriptableObject
{
    public float PointsTarget;

    [Header("Goon Points")]
    public float GoonSpeakPoints;
    public float GoonSpeakPointOnTimeBonus;
    public float GoonSpeakPointOnTimeThreshold;

    [Header("Melody Points")]
    public float MelodyPoints;
    public float MelodyOnTimeBonus;
    public float MelodyOnTimeThreshold;

    [Header("Scrap Points")]
    public float ScrapCaught;

    [Header("Music Points")]
    public float MusicPointsPerBeat;
}
