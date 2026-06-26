using UnityEngine;

public class ProximityPanel : MonoBehaviour
{
    public GameObject panelInfo; // Arrastra aquí tu Canvas

    void OnTriggerEnter(Collider other)
    {
        // Si detecta la mano (o cualquier cosa con tag "Player"), enciende el panel
        if (other.CompareTag("Player"))
        {
            panelInfo.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        // Al alejarse, lo apaga
        if (other.CompareTag("Player"))
        {
            panelInfo.SetActive(false);
        }
    }
}