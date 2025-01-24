using UnityEngine;

public class EmissionController : MonoBehaviour
{
    public Material materialRight;
    public Material materialLeft;
    public Color startColor;
    public Color endColor;
    public float maxIntensity = 1.0f;
    public float duration = 20.0f;
    public GameObject lightRight;
    public GameObject lightLeft;
    public ProgramManager programManager;

    private float elapsedTime = 0.0f;
    private bool isTimerRunning = false;
    private bool isRightHandIn = false;
    private bool isLeftHandIn = false;

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime <= duration)
            {
                float t = elapsedTime / duration;
                Color currentColor;
                float currentIntensity;

                if (t <= 0.5f) // First half of the duration
                {
                    float lerpT = t / 0.5f; // Normalize t to [0, 1] for the first half
                    currentColor = Color.Lerp(startColor, endColor, lerpT);
                    currentIntensity = Mathf.Lerp(0.0f, maxIntensity, lerpT);
                }
                else // Second half of the duration
                {
                    currentColor = endColor;
                    currentIntensity = maxIntensity;
                }

                Color finalColor = currentColor * Mathf.LinearToGammaSpace(currentIntensity);

                if (AreBothHandsIn())
                {
                    switch (programManager.GetCurrentPhase())
                    {
                        case Phase.RightOnLeftOff:
                            materialRight.SetColor("_EmissionColor", finalColor);
                            materialRight.EnableKeyword("_EMISSION");
                            lightRight.SetActive(isRightHandIn);
                            break;
                        case Phase.RightOffLeftOn:
                            materialLeft.SetColor("_EmissionColor", finalColor);
                            materialLeft.EnableKeyword("_EMISSION");
                            lightLeft.SetActive(isLeftHandIn);
                            break;
                        case Phase.RightOnLeftOn:
                            materialRight.SetColor("_EmissionColor", finalColor);
                            materialRight.EnableKeyword("_EMISSION");
                            materialLeft.SetColor("_EmissionColor", finalColor);
                            materialLeft.EnableKeyword("_EMISSION");
                            lightRight.SetActive(isRightHandIn);
                            lightLeft.SetActive(isLeftHandIn);
                            break;
                    }
                }
            }
            else
            {
                materialRight.SetColor("_EmissionColor", Color.black);
                materialRight.DisableKeyword("_EMISSION");
                materialLeft.SetColor("_EmissionColor", Color.black);
                materialLeft.DisableKeyword("_EMISSION");
                lightRight.SetActive(false);
                lightLeft.SetActive(false);
                isTimerRunning = false;
            }
        }
    }

    public void StartTimer()
    {
        if (isRightHandIn && isLeftHandIn)
        {
            isTimerRunning = true;
            elapsedTime = 0.0f;
        }
    }

    public void SetRightHandIn(bool isIn)
    {
        isRightHandIn = isIn;
        StartTimer();
    }

    public void SetLeftHandIn(bool isIn)
    {
        isLeftHandIn = isIn;
        StartTimer();
    }

    public bool AreBothHandsIn()
    {
        return isRightHandIn && isLeftHandIn;
    }

    public bool AreBothHandsOut()
    {
        return !isRightHandIn && !isLeftHandIn;
    }
}