using UnityEngine;
using TMPro;

public class PinInfoManager : MonoBehaviour
{
    // Ahora tenemos dos referencias en el Canvas
    public TextMeshProUGUI tituloText;
    public TextMeshProUGUI cuerpoText;

    public void MostrarInfo(string datosConcatenados)
    {
        // El script busca el símbolo '|' y divide el texto en dos partes
        string[] partes = datosConcatenados.Split('|');

        if (partes.Length >= 2)
        {
            tituloText.text = partes[0]; // Lo que está antes del '|'
            cuerpoText.text = partes[1]; // Lo que está después del '|'
        }
        else
        {
            // Por si acaso olvidas poner el '|' en algún pin
            tituloText.text = "Información";
            cuerpoText.text = datosConcatenados;
        }
    }

   
}