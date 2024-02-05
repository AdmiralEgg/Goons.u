using TMPro;
using UnityEngine;

public class ActTitleTextController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _actTitle;
    [SerializeField]
    private TextMeshProUGUI _actSubtitle;

    public void DisplayActTitle(string title, string subtitle)
    {
        _actTitle.text = title;
        _actSubtitle.text = subtitle;
    }
}
