using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WireManager : MonoBehaviour
{
    public Material materialCable;
    public float grosorCable = 0.005f;
    public CircuitManager circuitManager;
    private Transform pinDeInicio = null;
    private GameObject cableEnCurso = null;

    public void TocarPinSimple(SelectEnterEventArgs args)
    {
        Transform pinSeleccionado = args.interactableObject.transform;
        Transform manoJugador = args.interactorObject.transform;
        EjecutarLogicaConexion(pinSeleccionado, manoJugador);
    }

    private void EjecutarLogicaConexion(Transform pinSeleccionado, Transform manoJugador)
    {
        if (pinDeInicio == null)
        {
            // primero buscamos si el pin que toca tiene un cable viejo
            GameObject cableExistente = BuscarCableEnPin(pinSeleccionado);
            if (cableExistente != null)
            {
                CableDinamico datosCable = cableExistente.GetComponent<CableDinamico>();
                if (circuitManager != null)
                    circuitManager.ForzarDesconexion(datosCable.pinA.tag, datosCable.pinB.tag);
                Destroy(cableExistente);
            }

            // despues empezamos el nuevo cable
            pinDeInicio = pinSeleccionado;
            cableEnCurso = new GameObject("CableEnCurso");

            LineRenderer lr = cableEnCurso.AddComponent<LineRenderer>();
            CableDinamico dinamico = cableEnCurso.AddComponent<CableDinamico>();

            dinamico.pinA = pinDeInicio;
            dinamico.Inicializar(materialCable, grosorCable);
            dinamico.ConfigurarCable(pinDeInicio, manoJugador);
        }
        else
        {
            if (pinDeInicio != pinSeleccionado)
            {
                cableEnCurso.GetComponent<CableDinamico>().ConfigurarCable(pinDeInicio, pinSeleccionado);
                cableEnCurso.GetComponent<CableDinamico>().pinB = pinSeleccionado;
                if (circuitManager != null)
                    circuitManager.RegistrarConexion(pinDeInicio.tag, pinSeleccionado.tag);
            }
            else { Destroy(cableEnCurso); }
            pinDeInicio = null;
            cableEnCurso = null;
        }
    }

    private GameObject BuscarCableEnPin(Transform pin)
    {
        CableDinamico[] cablesExistentes = FindObjectsByType<CableDinamico>(FindObjectsSortMode.None);
        foreach (var cable in cablesExistentes)
        {
            if (cable.pinA == pin || cable.pinB == pin)
                return cable.gameObject;
        }
        return null;
    }
}