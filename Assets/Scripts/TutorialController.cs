using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialController : MonoBehaviour
{
    // --- Referencias a los elementos UI ---
    public GameObject tutorialUIContainer;
    public TextMeshProUGUI tutorialInstructionText;
    [TextArea(3, 10)]
    public string[] tutorialSteps;
    public float timePerStep = 5f; 
    private int currentStepIndex = 0;
    private Coroutine tutorialCoroutine;

    void Start()
    {
        // Comprobaciones de seguridad
        if (tutorialUIContainer == null || tutorialInstructionText == null)
        {
            enabled = false;
            return;
        }

        tutorialUIContainer.SetActive(true);

        if (tutorialSteps.Length > 0)
        {
            currentStepIndex = 0;
            tutorialCoroutine = StartCoroutine(RunTutorialSequence());
        }
        else
        {
            tutorialUIContainer.SetActive(false);
            enabled = false;
        }
    }
    IEnumerator RunTutorialSequence()
    {
        while (currentStepIndex < tutorialSteps.Length)
        {
            tutorialInstructionText.text = tutorialSteps[currentStepIndex];
            yield return new WaitForSeconds(timePerStep); 

            currentStepIndex++;
        }
        FinishTutorial();
    }

    public void FinishTutorial()
    {
        if (tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
            tutorialCoroutine = null;
        }

        tutorialUIContainer.SetActive(false);

    }


    [ContextMenu("Reset Non-Intrusive Tutorial Progress (Editor Only)")]
    void ResetTutorialProgress()
    {
        PlayerPrefs.Save();
    }
}
