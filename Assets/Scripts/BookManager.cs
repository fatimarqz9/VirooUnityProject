using UnityEngine;
using System.Collections.Generic;

public class BookManager : MonoBehaviour
{
    public List<GameObject> paginas;
    public GameObject botonVolverTeoria;
    public GameObject botonArmarCircuito; 

    private int paginaActual = 0;
    private int paginaInicioCircuito = 10;

    void Start()
    {
        ActualizarPagina();
    }

    public void SiguientePagina()
    {
        if (paginaActual < paginas.Count - 1)
        {
            paginaActual++;
            ActualizarPagina();
        }
    }

    public void PaginaAnterior()
    {
        if (paginaActual > 0)
        {
            paginaActual--;
            ActualizarPagina();
        }
    }

    void ActualizarPagina()
    {
        // 1. Enciende solo la página correcta
        for (int i = 0; i < paginas.Count; i++)
        {
            paginas[i].SetActive(i == paginaActual);
        }

        // 2. Muestra "Volver a la Teoría" solo en la zona de práctica
        if (botonVolverTeoria != null)
        {
            botonVolverTeoria.SetActive(paginaActual >= paginaInicioCircuito);
        }

        // 3. Muestra "Armar Circuito" SOLO en la página de inicio (índice 0)
        if (botonArmarCircuito != null)
        {
            botonArmarCircuito.SetActive(paginaActual == 0);
        }
    }

    public void SaltarAlCircuito()
    {
        paginaActual = paginaInicioCircuito;
        ActualizarPagina();
    }

    public void VolverALaTeoria()
    {
        paginaActual = 0;
        ActualizarPagina();
    }
}