using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class ImageTracker : MonoBehaviour
{
    private ARTrackedImageManager trackedImages;

    [SerializeField] private GameObject[] arPrefabs;

    private readonly Dictionary<string, GameObject> arObjects =
        new Dictionary<string, GameObject>();

    [SerializeField] private float distanceFromImage = 0.07f;

    [SerializeField] private float rotationX = 0f;
    [SerializeField] private float rotationY = 0f;
    [SerializeField] private float rotationZ = 0f;
    // [SerializeField] private float planetScale = 0.7f;

    private void Awake()
    {
        trackedImages = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        trackedImages.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        trackedImages.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(
        ARTrackedImagesChangedEventArgs eventArgs)
    {
        // Create objects for newly detected images
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            foreach (GameObject arPrefab in arPrefabs)
            {
                if (trackedImage.referenceImage.name == arPrefab.name)
                {
                    GameObject newObject =
                        Instantiate(arPrefab, trackedImage.transform);

                    // Give it the same name as the reference image
                    newObject.name = trackedImage.referenceImage.name;

                    // Position above the image
                    newObject.transform.localPosition =
                        Vector3.forward * distanceFromImage;

                    // Start with neutral local rotation
                    newObject.transform.localRotation =
                        Quaternion.identity;
                    newObject.transform.localRotation =
                        Quaternion.Euler(rotationX, rotationY, rotationZ);
                    // newObject.transform.localScale =
                    //     Vector3.one * planetScale;

                    newObject.SetActive(true);

                    arObjects[trackedImage.referenceImage.name] =
                        newObject;

                    Debug.Log(
                        $"Created {newObject.name}"
                    );

                    break;
                }
            }
        }

        // Update tracking
        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            string imageName = trackedImage.referenceImage.name;

            if (arObjects.TryGetValue(
                    imageName,
                    out GameObject arObject))
            {
                arObject.SetActive(
                    trackedImage.trackingState ==
                    TrackingState.Tracking
                );
            }
        }

        // Remove objects that AR Foundation removes
        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            string imageName = trackedImage.referenceImage.name;

            if (arObjects.TryGetValue(
                    imageName,
                    out GameObject arObject))
            {
                Destroy(arObject);
                arObjects.Remove(imageName);
            }
        }
    }
}