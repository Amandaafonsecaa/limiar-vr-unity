using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Collider))]
public class LightSwitch : MonoBehaviour
{
    [Header("Switch Animation")]
    [SerializeField] private Animator switchAnimator;
    [SerializeField] private string switchUpAnimationName = "SwitchUp";
    [SerializeField] private string switchDownAnimationName = "SwitchDown";

    [Header("Lights")]
    [SerializeField] private Light[] targetLights;

    [Header("Audio")]
    [SerializeField] private AudioSource clickAudio;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable interactable;
    private bool isOn;

    private void Awake()
    {
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

        if (targetLights != null && targetLights.Length > 0 && targetLights[0] != null)
            isOn = targetLights[0].enabled;

        UpdateSwitchVisual();
    }

    private void OnEnable()
    {
        if (interactable != null)
            interactable.selectEntered.AddListener(OnXRSelected);
    }

    private void OnDisable()
    {
        if (interactable != null)
            interactable.selectEntered.RemoveListener(OnXRSelected);
    }

    private void OnXRSelected(SelectEnterEventArgs args)
    {
        ToggleSwitch();
    }

    private void OnMouseDown()
    {
        ToggleSwitch();
    }

    public void ToggleSwitch()
    {
        isOn = !isOn;

        foreach (Light light in targetLights)
        {
            if (light != null)
                light.enabled = isOn;
        }

        UpdateSwitchVisual();

        if (clickAudio != null)
            clickAudio.Play();

        Debug.Log("Interruptor acionado: " + (isOn ? "Ligado" : "Desligado"));
    }

    private void UpdateSwitchVisual()
    {
        if (switchAnimator == null)
            return;

        if (isOn)
            switchAnimator.Play(switchUpAnimationName, 0, 0f);
        else
            switchAnimator.Play(switchDownAnimationName, 0, 0f);
    }

    [ContextMenu("Test Toggle Switch")]
    private void TestToggleSwitch()
    {
        ToggleSwitch();
    }
}