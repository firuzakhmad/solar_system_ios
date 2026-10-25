using UnityEngine;

[CreateAssetMenu(
    fileName = "New Celestial Object",
    menuName = "Solar System/Celestial Object"
)]
public class CelestialObjectData : ScriptableObject
{
    public string objectName;

    public Sprite thumbnail;

    public Sprite detailImage;

    [TextArea(3, 8)]
    public string description;

    [TextArea(2, 5)]
    public string interestingFact;

    public CelestialObjectType objectType;
}

public enum CelestialObjectType
{
    Planet,
    Moon,
    Star,
    DwarfPlanet,
    Galaxy,
    Nebula,
    Asteroid,
    Comet,
    Other
}