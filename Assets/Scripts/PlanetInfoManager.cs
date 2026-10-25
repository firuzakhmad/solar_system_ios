
using UnityEngine;

public class PlanetInfoManager : MonoBehaviour
{
    [SerializeField] private ObjectInfoPanel infoPanel;
    [SerializeField] private ImageTracker imageTracker;

    private string selectedObjectName;

    private void Start()
    {
        infoPanel.Hide();
    }

    public void ShowInfo(string objectName, string description)
    {
        // Release a previously selected object if necessary.
        if (!string.IsNullOrEmpty(selectedObjectName) &&
            selectedObjectName != objectName)
        {
            imageTracker.UnpinAndHideObject(selectedObjectName);
        }

        selectedObjectName = objectName;

        // Keep the selected 3D object visible.
        imageTracker.PinObject(selectedObjectName);

        // Show its information panel.
        infoPanel.Show(objectName, description);
    }

    public void HideInfo()
    {
        infoPanel.Hide();

        if (!string.IsNullOrEmpty(selectedObjectName))
        {
            imageTracker.UnpinAndHideObject(selectedObjectName);
            selectedObjectName = null;
        }
    }
}
