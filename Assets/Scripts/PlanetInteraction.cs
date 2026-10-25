
using UnityEngine;

public class PlanetInteraction : MonoBehaviour
{
    [Header("Automatic Rotation")]
    [SerializeField] private bool enableAutoRotation = true;
    [SerializeField] private bool rotateX = false;
    [SerializeField] private bool rotateY = true;
    [SerializeField] private bool rotateZ = false;

    [Tooltip("Rotation speed in degrees per second. Negative values reverse direction.")]
    [SerializeField] private float autoRotationSpeed = 15f;

    [Header("Manual Rotation")]
    [SerializeField] private float rotationSpeed = 0.2f;

    [Header("Scale")]
    [SerializeField] private float scaleSpeed = 0.005f;
    [SerializeField] private float minScale = 0.5f;
    [SerializeField] private float maxScale = 2.0f;

    private Camera arCamera;
    private bool isSelected;

    private void Start()
    {
        arCamera = Camera.main;
    }

    private void Update()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (arCamera == null)
            arCamera = Camera.main;

        // Rotate automatically, even when the user is not touching it.
        HandleAutomaticRotation();

        if (arCamera == null)
            return;

        HandleTouch();
    }

    private void HandleAutomaticRotation()
    {
        if (!enableAutoRotation)
            return;

        Vector3 rotationAxis = new Vector3(
            rotateX ? 1f : 0f,
            rotateY ? 1f : 0f,
            rotateZ ? 1f : 0f
        );

        if (rotationAxis.sqrMagnitude == 0f)
            return;

        rotationAxis.Normalize();

        transform.Rotate(
            rotationAxis,
            autoRotationSpeed * Time.deltaTime,
            Space.Self
        );
    }

    private bool IsTouchingThisPlanet(Vector2 screenPosition)
    {
        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform == transform ||
                   hit.transform.IsChildOf(transform);
        }

        return false;
    }

    private void HandleTouch()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isSelected = IsTouchingThisPlanet(touch.position);
            }

            if (isSelected && touch.phase == TouchPhase.Moved)
            {
                float rotationAmount =
                    -touch.deltaPosition.x * rotationSpeed;

                transform.Rotate(
                    Vector3.up,
                    rotationAmount,
                    Space.World
                );
            }

            if (touch.phase == TouchPhase.Ended ||
                touch.phase == TouchPhase.Canceled)
            {
                isSelected = false;
            }
        }

        if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            if (touch0.phase == TouchPhase.Began)
            {
                isSelected = IsTouchingThisPlanet(touch0.position);
            }

            if (touch1.phase == TouchPhase.Began)
            {
                isSelected = IsTouchingThisPlanet(touch1.position);
            }

            if (!isSelected)
                return;

            Vector2 previousTouch0 =
                touch0.position - touch0.deltaPosition;

            Vector2 previousTouch1 =
                touch1.position - touch1.deltaPosition;

            float previousDistance = Vector2.Distance(
                previousTouch0, previousTouch1
            );

            float currentDistance = Vector2.Distance(
                touch0.position, touch1.position
            );

            float difference = currentDistance - previousDistance;
            float scaleChange = difference * scaleSpeed;

            Vector3 newScale =
                transform.localScale + Vector3.one * scaleChange;

            float clampedScale = Mathf.Clamp(
                newScale.x, minScale, maxScale
            );

            transform.localScale = Vector3.one * clampedScale;
        }

        if (Input.touchCount == 0)
        {
            isSelected = false;
        }
    }
}
