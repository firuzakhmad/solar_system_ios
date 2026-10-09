using UnityEngine;

[CreateAssetMenu(fileName = "NewQuestion", menuName = "Quiz/Question")]
public class QuizQuestion : ScriptableObject
{
    [TextArea(2, 4)]
    public string questionText;

    public string[] answers = new string[4];

    [Range(0, 3)]
    public int correctAnswerIndex;

    [TextArea(2, 4)]
    public string explanation;
}
