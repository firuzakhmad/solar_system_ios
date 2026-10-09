using UnityEngine;

public class PlanetTap : MonoBehaviour
{
    [SerializeField] private PlanetInfo planetInfo;

    private PlanetInfoManager infoManager;

    private void Start()
    {
        infoManager = FindFirstObjectByType<PlanetInfoManager>();
    }

    private void OnMouseDown()
    {
        if (planetInfo == null || infoManager == null)
            return;

        infoManager.ShowInfo(
            planetInfo.ObjectName,
            planetInfo.Description
        );
    }
}