using UnityEngine;

public class MotherRoomProgressionManager : MonoBehaviour
{
    [Header("Progression")]
    [SerializeField] private bool prescriptionRead;
    [SerializeField] private bool medicineSeen;

    [Header("Final Event")]
    [SerializeField] private MotherRoomFinalEvent finalEvent;

    private bool finalEventTriggered;

    public void MarkPrescriptionRead()
    {
        if (prescriptionRead)
            return;

        prescriptionRead = true;
        Debug.Log("Atestado/receita lido.");

        CheckCompletion();
    }

    public void MarkMedicineSeen()
    {
        if (medicineSeen)
            return;

        medicineSeen = true;
        Debug.Log("Remédios vistos.");

        CheckCompletion();
    }

    private void CheckCompletion()
    {
        Debug.Log("Checando conclusão do quarto da mãe.");

        if (finalEventTriggered)
            return;

        if (!prescriptionRead || !medicineSeen)
            return;

        finalEventTriggered = true;

        if (finalEvent != null)
        {
            finalEvent.PlayEvent();
            Debug.Log("Evento final do quarto da mãe chamado.");
        }
        else
        {
            Debug.LogWarning("Final Event não está conectado.");
        }
    }

    [ContextMenu("Test Final Event")]
    private void TestFinalEvent()
    {
        prescriptionRead = true;
        medicineSeen = true;

        CheckCompletion();
    }
}