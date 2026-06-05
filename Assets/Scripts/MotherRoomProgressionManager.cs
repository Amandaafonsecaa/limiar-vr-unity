using UnityEngine;

public class MotherRoomProgressionManager : MonoBehaviour
{
    [Header("Required Objects")]
    [SerializeField] private bool prescriptionRead;
    [SerializeField] private bool medicineSeen;

    [Header("Final Event")]
    [SerializeField] private MotherRoomFinalEvent finalEvent;

    private bool finalEventTriggered;

    public void MarkPrescriptionRead()
    {
        prescriptionRead = true;
        Debug.Log("Receita/atestado lido.");
        CheckCompletion();
    }

    public void MarkMedicineSeen()
    {
        medicineSeen = true;
        Debug.Log("Remédios vistos.");
        CheckCompletion();
    }

    private void CheckCompletion()
    {
        if (finalEventTriggered)
            return;

        if (prescriptionRead && medicineSeen)
        {
            finalEventTriggered = true;
            TriggerFinalEvent();
        }
    }

    private void TriggerFinalEvent()
    {
        Debug.Log("Receita e remédios encontrados. Evento final do quarto ativado.");

        if (finalEvent != null)
            finalEvent.PlayEvent();
    }
}