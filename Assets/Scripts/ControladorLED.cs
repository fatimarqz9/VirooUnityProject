using UnityEngine;

public class ControladorLED : MonoBehaviour
{
    public Renderer ledRenderer;
    public Material materialEncendido;
    public Material materialApagado;
    public Material materialQuemado; 

    private bool estaQuemado = false; 
    void Awake()
    {
       
        if (ledRenderer == null)
            ledRenderer = GetComponentInChildren<Renderer>();
    }

    public void CambiarEstado(bool encendido)
    {
      
        if (ledRenderer == null || estaQuemado) return;

        
        ledRenderer.sharedMaterial = encendido ? materialEncendido : materialApagado;
        Debug.Log("Estado del LED cambiado a: " + (encendido ? "Encendido" : "Apagado"));
    }

    public void QuemarLED()
    {
        if (estaQuemado || ledRenderer == null) return; 

        estaQuemado = true;
        ledRenderer.sharedMaterial = materialQuemado;
        Debug.Log("¡PUM! Cortocircuito. El LED recibió corriente directa y se quemó.");
    }
}