using TMPro;
using UnityEngine;

public class ObjectInfoPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private GameObject panel;

    private void Awake()
    {
        Hide();
    }

    public void Show(string title, string description)
    {
        titleText.text = title;
        descriptionText.text = description;

        panel.SetActive(true);
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}