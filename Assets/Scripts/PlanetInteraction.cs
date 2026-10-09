using UnityEngine;

public class PlanetInteraction : MonoBehaviour
{
    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 0.2f;

    [Header("Scale")]
    [SerializeField] private float scaleSpeed = 0.005f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;

    private Camera arCamera;

    private void Start()
    {
        arCamera = Camera.main;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        HandleTouch();
    }

    private void HandleTouch()
    {
        // ONE FINGER - ROTATE
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Moved)
            {
                float rotationAmount =
                    -touch.deltaPosition.x * rotationSpeed;

                transform.Rotate(
                    Vector3.up,
                    rotationAmount,
                    Space.World
                );
            }
        }

        // TWO FINGERS - PINCH TO SCALE
        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 previousTouch0 =
                touch0.position - touch0.deltaPosition;

            Vector2 previousTouch1 =
                touch1.position - touch1.deltaPosition;

            float previousDistance =
                Vector2.Distance(
                    previousTouch0,
                    previousTouch1
                );

            float currentDistance =
                Vector2.Distance(
                    touch0.position,
                    touch1.position
                );

            float difference =
                currentDistance - previousDistance;

            float scaleChange =
                difference * scaleSpeed;

            Vector3 newScale =
                transform.localScale +
                Vector3.one * scaleChange;

            float clampedScale =
                Mathf.Clamp(
                    newScale.x,
                    minScale,
                    maxScale
                );

            transform.localScale =
                Vector3.one * clampedScale;
        }
    }
}