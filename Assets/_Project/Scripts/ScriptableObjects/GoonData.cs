using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GoonData", order = 3)]
public class GoonData : ScriptableObject
{
    public Goon.GoonType GoonType;
    public TMP_FontAsset WordFont;
    public Color WordColour;
}
