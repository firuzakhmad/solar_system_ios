using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FeatureSelectionAnimation : MonoBehaviour
{
    [Header("Containers")]
    [SerializeField] private RectTransform planetsContainer;
    [SerializeField] private RectTransform otherObjectsContainer;

    [Header("Animation")]
    [SerializeField] private float animationDuration = 0.6f;
    [SerializeField] private float offScreenPadding = 100f;

    [SerializeField]
    private AnimationCurve animationCurve =
        AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector2 planetsStartPosition;
    private Vector2 otherObjectsStartPosition;

    private bool isAnimating = false;

    private RectTransform backToMMButton;
    private RectTransform backToPGButton;

    private void Awake()
    {
        backToMMButton = GameObject.Find("BackToMMButton")
                           .GetComponent<RectTransform>();
        backToPGButton = GameObject.Find("BackToPGButton")
                           .GetComponent<RectTransform>();
    }

    private void Start()
    {
        backToMMButton.gameObject.SetActive(true);
        backToPGButton.gameObject.SetActive(false);


        planetsStartPosition = planetsContainer.anchoredPosition;
        otherObjectsStartPosition = otherObjectsContainer.anchoredPosition;

        RegisterButtons();
    }

    private void RegisterButtons()
    {
        // All planet buttons
        Button[] planetButtons =
            planetsContainer.GetComponentsInChildren<Button>(true);

        foreach (Button button in planetButtons)
        {
            button.onClick.AddListener(() => OnAnyButtonSelected(button));
        }

        // All other-object buttons
        Button[] otherButtons =
            otherObjectsContainer.GetComponentsInChildren<Button>(true);

        foreach (Button button in otherButtons)
        {
            button.onClick.AddListener(() => OnAnyButtonSelected(button));
        }
    }

    private void OnAnyButtonSelected(Button button)
    {
        if (isAnimating)
            return;

        string sceneName = button.gameObject.name;

        StartCoroutine(AnimateBothContainers(sceneName));
    }

    private IEnumerator AnimateBothContainers(string sceneName)
    {
        isAnimating = true;

        backToMMButton.gameObject.SetActive(false);
        backToPGButton.gameObject.SetActive(true);

        Canvas.ForceUpdateCanvases();

        Vector2 planetsStart =
            planetsContainer.anchoredPosition;

        Vector2 objectsStart =
            otherObjectsContainer.anchoredPosition;

        // Calculate how far they need to move.
        RectTransform canvasRect =
            planetsContainer.GetComponentInParent<Canvas>()
                           .GetComponent<RectTransform>();

        float canvasWidth = canvasRect.rect.width;

        // Planets goes LEFT
        Vector2 planetsTarget =
            planetsStart +
            Vector2.left *
            (canvasWidth + planetsContainer.rect.width + offScreenPadding);

        // OtherObjects goes RIGHT
        Vector2 objectsTarget =
            objectsStart +
            Vector2.right *
            (canvasWidth + otherObjectsContainer.rect.width + offScreenPadding);

        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;

            float progress =
                Mathf.Clamp01(elapsed / animationDuration);

            float easedProgress =
                animationCurve.Evaluate(progress);

            planetsContainer.anchoredPosition =
                Vector2.Lerp(
                    planetsStart,
                    planetsTarget,
                    easedProgress
                );

            otherObjectsContainer.anchoredPosition =
                Vector2.Lerp(
                    objectsStart,
                    objectsTarget,
                    easedProgress
                );

            yield return null;
        }

        // Make sure they reach the exact final positions.
        planetsContainer.anchoredPosition = planetsTarget;
        otherObjectsContainer.anchoredPosition = objectsTarget;

        // Disable both after the animation.
        planetsContainer.gameObject.SetActive(false);
        otherObjectsContainer.gameObject.SetActive(false);

        isAnimating = false;

        SceneManager.LoadScene(sceneName);
    }

    public void ResetContainers()
    {
        StopAllCoroutines();

        planetsContainer.gameObject.SetActive(true);
        otherObjectsContainer.gameObject.SetActive(true);

        planetsContainer.anchoredPosition =
            planetsStartPosition;

        otherObjectsContainer.anchoredPosition =
            otherObjectsStartPosition;

        backToMMButton.gameObject.SetActive(true);
        backToPGButton.gameObject.SetActive(false);

        isAnimating = false;
    }
}
