using UnityEngine;
using System.Collections.Generic;

public class CircuitManager : MonoBehaviour
{
    public ControladorLED led;

    // Lista para rastrear qué está conectado con qué
    private Dictionary<string, string> conexiones = new Dictionary<string, string>();

    public void RegistrarConexion(string puntoA, string puntoB)
    {
        LimpiarConexionDePin(puntoA);
        LimpiarConexionDePin(puntoB);

        conexiones[puntoA] = puntoB;
        conexiones[puntoB] = puntoA;
        ValidarCircuito();
    }

    private void LimpiarConexionDePin(string pin)
    {
        if (conexiones.ContainsKey(pin))
        {
            string otroLado = conexiones[pin];
            conexiones.Remove(otroLado);
            conexiones.Remove(pin);
        }
    }

    private void ValidarCircuito()
    {
        foreach (var entry in conexiones)
        {
            Debug.Log("Clave: " + entry.Key + " | Valor: " + entry.Value);
        }

        // Verifica si el cable del Ánodo va a 5V/13 y el Cátodo a GND
        bool positivoConectado = conexiones.ContainsKey("LED_Positivo") && conexiones["LED_Positivo"] == "Pin_13";
        bool negativoConectado = conexiones.ContainsKey("LED_Negativo") && conexiones["LED_Negativo"] == "Pin_GND";

        if (positivoConectado && negativoConectado)
        {
            led.CambiarEstado(true);
        }
        else
        {
            led.CambiarEstado(false);
        }
    }

    public void ForzarDesconexion(string pinA, string pinB)
    {
        if (conexiones.ContainsKey(pinA)) conexiones.Remove(pinA);
        if (conexiones.ContainsKey(pinB)) conexiones.Remove(pinB);
        ValidarCircuito();
    }
}