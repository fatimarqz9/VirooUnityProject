using UnityEngine;
using System.Collections.Generic;

public class CircuitManager : MonoBehaviour
{
    public ControladorLED led;
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
        bool tierraConectada = conexiones.ContainsKey("LED_Negativo") && conexiones["LED_Negativo"] == "Pin_GND";

        if (!tierraConectada)
        {
            led.CambiarEstado(false);
            return;
        }

        if (conexiones.ContainsKey("LED_Positivo"))
        {
            string componenteEnPositivo = conexiones["LED_Positivo"];

            if (componenteEnPositivo == "Pin_13" || componenteEnPositivo == "Pin_5V")
            {
                led.QuemarLED();
                return;
            }

            if (componenteEnPositivo == "R220_A" || componenteEnPositivo == "R220_B")
            {
                string pataContraria = (componenteEnPositivo == "R220_A") ? "R220_B" : "R220_A";

                if (conexiones.ContainsKey(pataContraria) &&
                   (conexiones[pataContraria] == "Pin_13" || conexiones[pataContraria] == "Pin_5V"))
                {
                    led.CambiarEstado(true);
                    return;
                }
            }

            if (componenteEnPositivo == "R1K_A" || componenteEnPositivo == "R1K_B" ||
                componenteEnPositivo == "R4.7_A" || componenteEnPositivo == "R4.7_B" ||
                componenteEnPositivo == "R10K_A" || componenteEnPositivo == "R10K_B")
            {
                string pataContraria = "";

                if (componenteEnPositivo == "R1K_A") pataContraria = "R1K_B";
                else if (componenteEnPositivo == "R1K_B") pataContraria = "R1K_A";
                else if (componenteEnPositivo == "R4.7_A") pataContraria = "R4.7_B";
                else if (componenteEnPositivo == "R4.7_B") pataContraria = "R4.7_A";
                else if (componenteEnPositivo == "R10K_A") pataContraria = "R10K_B";
                else if (componenteEnPositivo == "R10K_B") pataContraria = "R10K_A";

                if (conexiones.ContainsKey(pataContraria) &&
                   (conexiones[pataContraria] == "Pin_13" || conexiones[pataContraria] == "Pin_5V"))
                {
                    led.CambiarEstado(false);
                    return;
                }
            }
        }

        led.CambiarEstado(false);
    }

    public void ForzarDesconexion(string pinA, string pinB)
    {
        if (conexiones.ContainsKey(pinA)) conexiones.Remove(pinA);
        if (conexiones.ContainsKey(pinB)) conexiones.Remove(pinB);
        ValidarCircuito();
    }
}