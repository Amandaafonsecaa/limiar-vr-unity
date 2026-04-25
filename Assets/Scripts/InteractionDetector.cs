using UnityEngine;

public class InteractionDetector : MonoBehaviour
{
    private InteractableHighlight currentHighlight;

    private void OnTriggerEnter(Collider other)
    {
        InteractableHighlight highlight = other.GetComponentInParent<InteractableHighlight>();

        if (highlight != null)
        {
            currentHighlight = highlight;
            currentHighlight.SetHighlight(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        InteractableHighlight highlight = other.GetComponentInParent<InteractableHighlight>();

        if (highlight != null)
        {
            highlight.SetHighlight(false);

            if (currentHighlight == highlight)
                currentHighlight = null;
        }
    }
}