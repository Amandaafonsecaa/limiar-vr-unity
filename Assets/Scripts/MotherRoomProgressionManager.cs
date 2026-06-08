using UnityEngine;

public class MotherRoomProgressionManager : MonoBehaviour
{
    [Header("Progression")]
    [SerializeField] private bool prescriptionRead;
    [SerializeField] private bool medicineSeen;

    public void MarkPrescriptionRead()
    {
        if (prescriptionRead)
            return;

        prescriptionRead = true;
        Debug.Log("Atestado/receita lido.");
    }

    public void MarkMedicineSeen()
    {
        if (medicineSeen)
            return;

        medicineSeen = true;
        Debug.Log("Remédios vistos.");
    }

    public bool CanCollectAnchor()
    {
        return prescriptionRead && medicineSeen;
    }
}