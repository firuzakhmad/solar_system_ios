using UnityEngine;

public class InstructionStepManager : MonoBehaviour
{
    private InstructionStepUI currentOpenStep;

    public void OpenStep(InstructionStepUI step)
    {
        // Clicking the currently open step closes it
        if (currentOpenStep == step)
        {
            step.SetOpen(false);
            currentOpenStep = null;
            return;
        }

        // Close previous step
        if (currentOpenStep != null)
        {
            currentOpenStep.SetOpen(false);
        }

        // Open new step
        step.SetOpen(true);

        currentOpenStep = step;
    }
}
