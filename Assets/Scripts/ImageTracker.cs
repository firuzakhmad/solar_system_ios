
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    private ARTrackedImageManager trackedImages;

    [SerializeField] private GameObject[] arPrefabs;
    [SerializeField] private float distanceFromImage = 0.07f;
    [SerializeField] private float rotationX = 0f;
    [SerializeField] private float rotationY = 0f;
    [SerializeField] private float rotationZ = 0f;

    private readonly Dictionary<string, GameObject> arObjects =
        new Dictionary<string, GameObject>();

    private readonly HashSet<string> pinnedObjects =
        new HashSet<string>();

    private readonly HashSet<string> dismissedObjects =
        new HashSet<string>();

    private void Awake()
    {
        trackedImages = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        if (trackedImages != null)
            trackedImages.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        if (trackedImages != null)
            trackedImages.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(
        ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            string imageName = trackedImage.referenceImage.name;

            foreach (GameObject arPrefab in arPrefabs)
            {
                if (imageName != arPrefab.name)
                    continue;

                // Avoid creating duplicate objects.
                if (arObjects.ContainsKey(imageName))
                    break;

                GameObject newObject =
                    Instantiate(arPrefab, trackedImage.transform);

                newObject.name = imageName;

                newObject.transform.localPosition =
                    Vector3.forward * distanceFromImage;

                newObject.transform.localRotation =
                    Quaternion.Euler(rotationX, rotationY, rotationZ);

                arObjects[imageName] = newObject;

                newObject.SetActive(
                    trackedImage.trackingState == TrackingState.Tracking
                );

                Debug.Log($"Created {imageName}");
                break;
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            string imageName = trackedImage.referenceImage.name;

            if (!arObjects.TryGetValue(
                    imageName, out GameObject arObject))
                continue;

            bool isTracking =
                trackedImage.trackingState == TrackingState.Tracking;

            if (!isTracking)
            {
                // Once tracking is lost, allow the user to select
                // this object again after the image is reacquired.
                dismissedObjects.Remove(imageName);
            }

            // Keep a selected object visible, even when tracking is lost.
            if (pinnedObjects.Contains(imageName))
            {
                arObject.SetActive(true);
            }
            else
            {
                arObject.SetActive(
                    isTracking &&
                    !dismissedObjects.Contains(imageName)
                );
            }
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.referenceImage.name;

            // Keep pinned objects even if AR Foundation removes
            // their tracked-image entry.
            if (pinnedObjects.Contains(imageName))
                continue;

            if (arObjects.TryGetValue(
                    imageName, out GameObject arObject))
            {
                Destroy(arObject);
                arObjects.Remove(imageName);
            }

            dismissedObjects.Remove(imageName);
        }
    }

    public void PinObject(string objectName)
    {
        if (!arObjects.TryGetValue(
                objectName, out GameObject arObject))
        {
            Debug.LogWarning(
                $"Cannot pin '{objectName}': object not found.");
            return;
        }

        dismissedObjects.Remove(objectName);
        pinnedObjects.Add(objectName);
        arObject.SetActive(true);
    }

    public void UnpinAndHideObject(string objectName)
    {
        pinnedObjects.Remove(objectName);
        dismissedObjects.Add(objectName);

        if (arObjects.TryGetValue(
                objectName, out GameObject arObject))
        {
            arObject.SetActive(false);
        }
    }
}
