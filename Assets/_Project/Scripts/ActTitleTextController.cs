using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ActTitleTextController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _actTitle;
    [SerializeField]
    private TextMeshProUGUI _actSubtitle;

    private void Awake()
    {
        
    }

    public void DisplayActTitle(GameManager.GameState act)
    {
        switch (act)
        {
            case GameManager.GameState.Act1:
                _actTitle.text = "Act 1";
                _actSubtitle.text = "Act 1 Subtitle Text";
                break;
            case GameManager.GameState.Act2:
                _actTitle.text = "Act 2";
                _actSubtitle.text = "Act 2 Subtitle Text";
                break;
            case GameManager.GameState.Act3:
                _actTitle.text = "Act 3";
                _actSubtitle.text = "Act 3 Subtitle Text";
                break;
            case GameManager.GameState.Act4:
                _actTitle.text = "Act 4";
                _actSubtitle.text = "Act 4 Subtitle Text";
                break;
            case GameManager.GameState.Act5:
                _actTitle.text = "Act 5";
                _actSubtitle.text = "Act 5 Subtitle Text";
                break;
            case GameManager.GameState.Sandbox:
                _actTitle.text = "Sandbox";
                _actSubtitle.text = "Fine. Just skip to the end. See if I care.";
                break;
        }
    }

}
