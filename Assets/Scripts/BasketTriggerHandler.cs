using UnityEngine;

public class BasketTriggerHandler : MonoBehaviour
{
    private BasketDetector basketDetector;

    public void Initialize(BasketDetector detector)
    {
        basketDetector = detector;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (basketDetector != null)
        {
            basketDetector.OnBallEnterDetection(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (basketDetector != null)
        {
            basketDetector.OnBallExitDetection(other);
        }
    }
}
