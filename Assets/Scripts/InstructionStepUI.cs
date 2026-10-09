using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InstructionStepUI : MonoBehaviour
{
    [Header("References")]
    public Button headerButton;
    public RectTransform body;
    public LayoutElement bodyLayout;
    public CanvasGroup bodyCanvasGroup;
    public TMP_Text headerText;

    [Header("Settings")]
    public string stepTitle = "Step 1";
    public bool openOnStart = false;
    public float animationDuration = 0.25f;

    private bool isOpen = false;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        headerButton.onClick.AddListener(Toggle);

        // Start closed
        bodyLayout.preferredHeight = 0;
        bodyCanvasGroup.alpha = 0;

        bodyCanvasGroup.interactable = false;
        bodyCanvasGroup.blocksRaycasts = false;

        UpdateHeader();
    }

    private void Start()
    {
        if (openOnStart)
        {
            StartCoroutine(OpenAtStart());
        }
    }

    private IEnumerator OpenAtStart()
    {
        yield return null;

        SetOpen(true);
    }

    private void Toggle()
    {
        Debug.Log("Clicked: " + stepTitle);

        SetOpen(!isOpen);
    }

    public void SetOpen(bool open)
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
        }

        if (open)
        {
            // Temporarily allow Body to calculate its natural height
            bodyLayout.preferredHeight = -1;

            LayoutRebuilder.ForceRebuildLayoutImmediate(body);

            float targetHeight = LayoutUtility.GetPreferredHeight(body);

            Debug.Log("Target height: " + targetHeight);

            // Start from zero
            bodyLayout.preferredHeight = 0;

            animationCoroutine = StartCoroutine(
                Animate(0, targetHeight, 0, 1)
            );
        }
        else
        {
            float currentHeight = bodyLayout.preferredHeight;

            animationCoroutine = StartCoroutine(
                Animate(
                    currentHeight,
                    0,
                    bodyCanvasGroup.alpha,
                    0
                )
            );
        }

        isOpen = open;

        UpdateHeader();
    }

    private IEnumerator Animate(
        float startHeight,
        float endHeight,
        float startAlpha,
        float endAlpha)
    {
        float elapsed = 0;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = elapsed / animationDuration;

            t = Mathf.SmoothStep(0, 1, t);

            bodyLayout.preferredHeight =
                Mathf.Lerp(startHeight, endHeight, t);

            bodyCanvasGroup.alpha =
                Mathf.Lerp(startAlpha, endAlpha, t);

            yield return null;
        }

        bodyLayout.preferredHeight = endHeight;
        bodyCanvasGroup.alpha = endAlpha;

        bodyCanvasGroup.interactable = endAlpha > 0.5f;
        bodyCanvasGroup.blocksRaycasts = endAlpha > 0.5f;

        animationCoroutine = null;
    }

    private void UpdateHeader()
    {
        if (isOpen)
        {
            headerText.text = "v  " + stepTitle;
        }
        else
        {
            headerText.text = ">  " + stepTitle;
        }
    }

    private IEnumerator OpenAfterLayout()
    {
        yield return null;

        SetOpen(true);
    }

}


