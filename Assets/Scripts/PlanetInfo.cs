using UnityEngine;

public class PlanetInfo : MonoBehaviour
{
    [Header("Object Information")]

    [SerializeField] private string objectName;

    [TextArea(3, 10)]
    [SerializeField] private string description;

    public string ObjectName => objectName;
    public string Description => description;
}