using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GoonData", order = 3)]
public class GoonData : ScriptableObject
{
    public Goon.GoonType GoonType;

    [Header("Scrap Details")]
    public TMP_FontAsset WordFont;
    public Color WordColor;

    [Header("All Words")]
    public WordData[] WordData;
}
