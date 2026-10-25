
using UnityEngine;

public class MoonOrbit : MonoBehaviour
{
    [SerializeField] private float orbitSpeed = 25f;
    [SerializeField] private Vector3 orbitAxis = Vector3.up;

    private void Update()
    {
        transform.Rotate(
            orbitAxis.normalized,
            orbitSpeed * Time.deltaTime,
            Space.Self
        );
    }
}
