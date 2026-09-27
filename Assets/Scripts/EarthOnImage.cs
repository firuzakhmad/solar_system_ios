using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class EarthPlacement : MonoBehaviour
{
    [SerializeField]
    private ARTrackedImageManager imageManager;

    [SerializeField]
    private float height = 0.10f;

    private void OnEnable()
    {
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    private void OnTrackedImagesChanged(
        ARTrackedImagesChangedEventArgs args)
    {
        foreach (ARTrackedImage image in args.added)
        {
            PlaceEarth(image);
        }

        foreach (ARTrackedImage image in args.updated)
        {
            PlaceEarth(image);
        }
    }

    private void PlaceEarth(ARTrackedImage image)
    {
        if (image.trackingState != TrackingState.Tracking)
            return;

        transform.position =
            image.transform.position +
            image.transform.forward * height;
    }
}
