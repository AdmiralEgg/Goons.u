using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ActData", order = 5)]
public class ActData : ScriptableObject
{
    [Header("Act")]
    public StageManager.GameState Act = StageManager.GameState.Act1;
    public string ActTitle = "Act 1";
    public string ActSubtitle = "Cult of Celebrity";
    public StageManager.GameState NextAct = StageManager.GameState.Act2;

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
    public StagePositionController.StagePosition HagStagePosition;

    public bool ToffEnabled;
    [ShowIf("ToffEnabled", true), Required]
    public StagePositionController.StagePosition ToffStagePosition;
    
    public bool YorkyEnabled;
    [ShowIf("YorkyEnabled", true), Required]
    public StagePositionController.StagePosition YorkyStagePosition;
}
