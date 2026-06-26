using UnityEngine;

public class WireManager : MonoBehaviour
{
    [Header("Configuración del Cable")]
    public Material materialCable;
    public float grosorCable = 0.005f; // 5 milímetros de grosor

    // Guardame el primer pin que el usuario toque
    private Transform pinDeInicio = null;

    // Esta función la llamará cada pin cuando el usuario lo toque
    public void TocarPin(Transform pinSeleccionado)
    {
        if (pinDeInicio == null)
        {
            // Es el primer toque: Guardamos este pin como inicio
            pinDeInicio = pinSeleccionado;
            Debug.Log("Primer pin seleccionado: " + pinSeleccionado.name);
        }
        else
        {
            // Es el segundo toque: Verificamos que no sea el mismo pin
            if (pinDeInicio != pinSeleccionado)
            {
                CrearCableVisual(pinDeInicio, pinSeleccionado);
                Debug.Log("Conectando " + pinDeInicio.name + " con " + pinSeleccionado.name);
            }

            // Limpiamos la memoria para que pueda hacer otro cable nuevo
            pinDeInicio = null;
        }
    }

    private void CrearCableVisual(Transform inicio, Transform fin)
    {
        // Creamos un objeto vacío nuevo para el cable
        GameObject nuevoCable = new GameObject("Cable_" + inicio.name + "_A_" + fin.name);

        // Le añadimos la herramienta de dibujo de Unity
        LineRenderer lr = nuevoCable.AddComponent<LineRenderer>();

        // Lo configuramos para que parezca un cable real
        lr.material = materialCable;
        lr.startWidth = grosorCable;
        lr.endWidth = grosorCable;
        lr.positionCount = 2; // Solo tiene 2 puntos (inicio y fin)

        // Trazamos la línea
        lr.SetPosition(0, inicio.position);
        lr.SetPosition(1, fin.position);
    }
}