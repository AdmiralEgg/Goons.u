using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SongData", order = 1)]
public class SongData : ScriptableObject
{
    public string Name;
    public AudioClip AudioClip;
    public int BPM;
}
