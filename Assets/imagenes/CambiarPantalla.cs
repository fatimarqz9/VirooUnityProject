using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject imagenCircuito;
    public GameObject tituloCircuito;
    public GameObject textoIntegrantes;

    public float tiempoCambio = 5f;

    private bool mostrandoCircuito = true;


    void Start()
    {
        MostrarCircuito();

        InvokeRepeating("CambiarPantallaVista", tiempoCambio, tiempoCambio);
    }


    void CambiarPantallaVista()
    {

        if(mostrandoCircuito)
        {
            MostrarIntegrantes();
        }
        else
        {
            MostrarCircuito();
        }

    }


    void MostrarCircuito()
    {
        imagenCircuito.SetActive(true);
        tituloCircuito.SetActive(true);

        textoIntegrantes.SetActive(false);

        mostrandoCircuito = true;
    }


    void MostrarIntegrantes()
    {
        imagenCircuito.SetActive(false);
        tituloCircuito.SetActive(false);

        textoIntegrantes.SetActive(true);

        mostrandoCircuito = false;
    }

}
