using UnityEngine;
using ImGuiNET;

public class DearImGuiDemo : MonoBehaviour
{
    private void OnEnable() => ImGuiUn.Layout += OnLayout;

    private void OnDisable() => ImGuiUn.Layout -= OnLayout;

    private bool m_WindowEnabled = false;

    private GameObject _lastRaycastHit = null;

    private void Awake()
    {
        InputManager.ReportHit += (GameObject) =>
        {
            _lastRaycastHit = GameObject;
        };
    }

    void OnLayout()
    {
        // Begins ImGui window
        if (!ImGui.Begin("Game Manager",
                         ref m_WindowEnabled,
                         ImGuiWindowFlags.MenuBar))
            return;

        ImGui.Text($"Input State: {InputManager.GetCurrentInputState()}");
        ImGui.Text($"Last Hit: {_lastRaycastHit?.name}");

        ImGui.End();
    }
}