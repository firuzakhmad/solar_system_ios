using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class QuizManager : MonoBehaviour
{
    [Header("Questions")]
    [SerializeField] private QuizQuestion[] questions;

    [Header("Question UI")]
    [SerializeField] private TMP_Text questionNumberText;
    [SerializeField] private TMP_Text questionText;

    [Header("Answer Buttons")]
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TMP_Text[] answerTexts;

    [Header("Feedback UI")]
    [SerializeField] private GameObject explanationPanel;
    [SerializeField] private TMP_Text explanationText;
    [SerializeField] private Button nextButton;

    [Header("Button Colors")]
    [SerializeField] private Color normalColor = new Color(1.0f, 1.0f, 1.0f);
    [SerializeField] private Color correctColor = new Color(0.15f, 0.65f, 0.30f);
    [SerializeField] private Color incorrectColor = new Color(0.75f, 0.20f, 0.20f);

    [Header("Results UI")]
    [SerializeField] private GameObject resultsPanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text resultMessage;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button exitButton;

    private int currentQuestionIndex;
    private int score;

    private void Start()
    {
        currentQuestionIndex = 0;
        score = 0;

        nextButton.onClick.AddListener(NextQuestion);
        retryButton.onClick.AddListener(RestartQuiz);
        exitButton.onClick.AddListener(ExitQuizScene);

        ShowQuestion();
    }

    private void ShowQuestion()
    {
        QuizQuestion question = questions[currentQuestionIndex];

        questionNumberText.text =
            "Question " + (currentQuestionIndex + 1);

        questionText.text = question.questionText;

        // Hide feedback
        explanationPanel.SetActive(false);

        // Enable answer buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = true;

            answerButtons[i].image.color = normalColor;

            answerTexts[i].text = question.answers[i];

            int answerIndex = i;

            answerButtons[i].onClick.RemoveAllListeners();

            answerButtons[i].onClick.AddListener(
                () => SelectAnswer(answerIndex)
            );
        }
    }

    private void SelectAnswer(int answerIndex)
    {
        QuizQuestion question = questions[currentQuestionIndex];

        bool isCorrect =
            answerIndex == question.correctAnswerIndex;

        // Disable all answer buttons
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].interactable = false;
        }

        // Show correct answer in green
        answerButtons[question.correctAnswerIndex]
            .image.color = correctColor;

        // If the player selected the wrong answer, show it in red
        if (!isCorrect)
        {
            answerButtons[answerIndex]
                .image.color = incorrectColor;
        }
        else
        {
            score++;
        }

        // Show explanation
        explanationText.text = question.explanation;

        explanationPanel.SetActive(true);
    }

    private void NextQuestion()
    {
        currentQuestionIndex++;

        if (currentQuestionIndex >= questions.Length)
        {
            FinishQuiz();
            return;
        }

        ShowQuestion();
    }

    private void FinishQuiz()
    {
        Debug.Log(
            "Quiz finished! Score: " +
            score + "/" + questions.Length
        );

        // Hide quiz UI
        questionNumberText.gameObject.SetActive(false);
        questionText.gameObject.SetActive(false);

        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }

        explanationPanel.SetActive(false);

        // Show results
        resultsPanel.SetActive(true);

        // Display score
        scoreText.text = score + " / " + questions.Length;

        // Display message
        if (score == questions.Length)
        {
            resultMessage.text = "Perfect score!";
        }
        else if (score >= questions.Length * 0.7f)
        {
            resultMessage.text = "Great work!";
        }
        else if (score >= questions.Length * 0.5f)
        {
            resultMessage.text = "Good effort!";
        }
        else
        {
            resultMessage.text = "Keep practicing!";
        }
    }

    private void RestartQuiz()
    {
        currentQuestionIndex = 0;
        score = 0;

        resultsPanel.SetActive(false);

        questionNumberText.gameObject.SetActive(true);
        questionText.gameObject.SetActive(true);

        foreach (Button button in answerButtons)
        {
            button.gameObject.SetActive(true);
        }

        ShowQuestion();
    }

    private void ExitQuizScene()
    {
        SceneManager.LoadScene(0);
    }


}
