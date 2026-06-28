using UnityEngine;

public class ControladorLED : MonoBehaviour
{
    public Renderer ledRenderer;
    public Material materialEncendido;
    public Material materialApagado;

    void Awake()
    {
        // Esto busca automáticamente el renderer en los hijos si no está asignado
        if (ledRenderer == null)
            ledRenderer = GetComponentInChildren<Renderer>();
    }

    public void CambiarEstado(bool encendido)
    {
        if (ledRenderer == null) return;

        // Asignamos el material directamente
        ledRenderer.sharedMaterial = encendido ? materialEncendido : materialApagado;

        Debug.Log("Material aplicado a: " + ledRenderer.name);
    }
}