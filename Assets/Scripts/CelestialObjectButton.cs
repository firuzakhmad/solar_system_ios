using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CelestialObjectButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text objectNameText;

    [SerializeField] private Image thumbnail;

    private CelestialObjectData objectData;
    private PlanetGuideManager manager;

    public void Setup(
        CelestialObjectData data,
        PlanetGuideManager guideManager)
    {
        objectData = data;
        manager = guideManager;

        if (button == null)
            button = GetComponent<Button>();

        if (objectNameText != null)
            objectNameText.text = data.objectName;

        if (thumbnail != null)
        {
            thumbnail.sprite = data.thumbnail;
            Debug.Log(data.thumbnail.name);
            thumbnail.enabled = data.thumbnail != null;
        }

        button.onClick.RemoveListener(OnClicked);
        button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        if (manager != null && objectData != null)
            manager.SelectObject(objectData);
    }
}