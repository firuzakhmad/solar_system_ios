
using System;
using UnityEngine;

public class SolarSystemSimulation : MonoBehaviour
{
    [Serializable]
    public class OrbitBody
    {
        public string bodyName;
        public Transform orbitPivot;

        [Tooltip("Orbital speed in degrees per second. Negative reverses direction.")]
        public float orbitSpeed = 10f;
    }

    [Header("Solar System")]
    [SerializeField] private Transform sun;
    [SerializeField] private OrbitBody[] orbitBodies;

    [Header("Simulation Controls")]
    [SerializeField] private bool simulationRunning = true;

    [Tooltip("1 = normal configured speed, 2 = twice as fast.")]
    [SerializeField, Min(0f)]
    private float simulationSpeed = 1f;

    [Header("Optional Sun Rotation")]
    [SerializeField] private bool rotateSun = true;
    [SerializeField] private float sunRotationSpeed = 5f;

    private void Update()
    {
        if (!simulationRunning)
            return;

        float delta = Time.deltaTime * simulationSpeed;

        // Rotate the Sun around its own axis.
        if (rotateSun && sun != null)
        {
            sun.Rotate(
                Vector3.up,
                sunRotationSpeed * delta,
                Space.Self
            );
        }

        // Rotate each orbit pivot around the solar system center.
        if (orbitBodies == null)
            return;

        foreach (OrbitBody body in orbitBodies)
        {
            if (body == null || body.orbitPivot == null)
                continue;

            body.orbitPivot.Rotate(
                Vector3.up,
                body.orbitSpeed * delta,
                Space.Self
            );
        }
    }

    public void PlaySimulation()
    {
        simulationRunning = true;
    }

    public void PauseSimulation()
    {
        simulationRunning = false;
    }

    public void SetSimulationSpeed(float speed)
    {
        simulationSpeed = Mathf.Max(0f, speed);
    }

    public void ResetSimulation()
    {
        if (orbitBodies == null)
            return;

        foreach (OrbitBody body in orbitBodies)
        {
            if (body == null || body.orbitPivot == null)
                continue;

            body.orbitPivot.localRotation = Quaternion.identity;
        }
    }
}
