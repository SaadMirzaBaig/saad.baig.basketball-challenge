using UnityEngine;

public class BasketTriggerHandler : MonoBehaviour
{
    private BasketDetector basketDetector;

    //Initialize the basket detector
    public void Initialize(BasketDetector detector)
    {
        basketDetector = detector;
    }

    //On ball enter trigger
    private void OnTriggerEnter(Collider other)
    {
        if (basketDetector != null)
        {
            basketDetector.OnBallEnterDetection(other);
        }
    }
}
