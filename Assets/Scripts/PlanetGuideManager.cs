
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlanetGuideManager : MonoBehaviour
{
    [Header("Object Data")]
    [SerializeField] private CelestialObjectData[] allObjects;

    [Header("Button Prefab and Lists")]
    [SerializeField] private CelestialObjectButton buttonPrefab;
    [SerializeField] private Transform planetsGrid;
    [SerializeField] private Transform otherObjectsGrid;

    [Header("Detail Panel")]
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private Image detailImage;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text factText;
    [SerializeField] private Button backButton;

    private void Start()
    {
        detailPanel.SetActive(false);

        backButton.onClick.RemoveListener(ShowList);
        backButton.onClick.AddListener(ShowList);

        PopulateList();
    }

    private void PopulateList()
    {
        foreach (CelestialObjectData data in allObjects)
        {
            if (data == null)
                continue;

            Transform parent = GetParentForObject(data);

            if (parent == null)
            {
                Debug.LogWarning(
                    $"No grid assigned for {data.objectName}"
                );
                continue;
            }

            CelestialObjectButton item =
                Instantiate(buttonPrefab, parent);

            item.Setup(data, this);
        }
    }

    private Transform GetParentForObject(
        CelestialObjectData data)
    {
        // Use your existing object categories here.
        // Example: if your data has an objectType enum,
        // route planets to planetsGrid and everything
        // else to otherObjectsGrid.

        return data.objectType == CelestialObjectType.Planet
            ? planetsGrid
            : otherObjectsGrid;
    }

    public void SelectObject(CelestialObjectData data)
    {
        if (data == null)
            return;

        titleText.text = data.objectName;
        descriptionText.text = data.description;
        factText.text = data.interestingFact;

        detailImage.sprite = data.detailImage;
        detailImage.enabled = data.detailImage != null;

        detailPanel.SetActive(true);

        // Hide the complete list section.
        planetsGrid.parent.gameObject.SetActive(false);
        otherObjectsGrid.parent.gameObject.SetActive(false);
    }

    public void ShowList()
    {
        if (detailPanel == null)
        {
            Debug.LogError("Detail Panel is not assigned!");
            return;
        }

        detailPanel.SetActive(false);

        if (planetsGrid != null)
            planetsGrid.parent.gameObject.SetActive(true);

        if (otherObjectsGrid != null)
            otherObjectsGrid.parent.gameObject.SetActive(true);
    }

    private void OnDestroy()
    {
        if (backButton != null)
            backButton.onClick.RemoveListener(ShowList);
    }
}
