using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ActData", order = 5)]
public class ActData : ScriptableObject
{
    public GameManager.GameState Act = GameManager.GameState.Act1;

    [Header("Mechanisms")]
    public bool ProjectorEnabled;
    public bool MusicEnabled = true;
    public bool ScrapEnabled;
    public bool MelodyLong1Enabled = true;
    public bool MelodyShort1Enabled;
    public bool MelodyShort2Enabled;

    [Header("Goons")]
    public bool HagEnabled = true;
    [ShowIf("HagEnabled", true), Required]
    public StagePositionPoint HagStagePosition;

    public bool ToffEnabled;
    [ShowIf("ToffEnabled", true), Required]
    public StagePositionPoint ToffStagePosition;
    
    public bool YorkyEnabled;
    [ShowIf("YorkyEnabled", true), Required]
    public StagePositionPoint YorkyStagePosition;

    [Header("Goon Lights")]
    public bool GoonLightsLeftEnabled = true;
    public bool GoonLightsLeftMidEnabled;
    public bool GoonLightsMiddleEnabled;
    public bool GoonLightsRightMidEnabled;
    public bool GoonLightsRightEnabled;

    [Header("House Lights")]
    public bool HouseLightLeftEnabled;
    public bool HouseLightRightEnabled;
    public bool CrowdLightEnabled = true;
}
