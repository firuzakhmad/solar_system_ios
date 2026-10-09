using UnityEngine;

public class PlanetInfoManager : MonoBehaviour
{
    [SerializeField] private ObjectInfoPanel infoPanel;

    private void Start()
    {
        infoPanel.Hide();
    }

    public void ShowInfo(string objectName, string description)
    {
        infoPanel.Show(objectName, description);
    }

    public void HideInfo()
    {
        infoPanel.Hide();
    }
}