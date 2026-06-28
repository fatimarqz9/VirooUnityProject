using UnityEngine;

public class CableDinamico : MonoBehaviour
{
    private LineRenderer lr;
    private Transform inicio;
    private Transform fin;
    public Transform pinA;
    public Transform pinB;

    void Awake()
    {
        // Buscamos el LineRenderer en el mismo objeto
        lr = GetComponent<LineRenderer>();

        // Si por algún motivo no lo encuentra, lo crea 
        if (lr == null)
            lr = gameObject.AddComponent<LineRenderer>();

        lr.positionCount = 2;
    }

    public void Inicializar(Material mat, float grosor)
    {
        lr.material = mat;
        lr.startWidth = grosor;
        lr.endWidth = grosor;
    }

    public void ConfigurarCable(Transform inicioTransform, Transform finTransform)
    {
        inicio = inicioTransform;
        fin = finTransform;
    }

    void Update()
    {
        if (inicio != null && fin != null)
        {
            lr.SetPosition(0, inicio.position);
            lr.SetPosition(1, fin.position);
        }
    }
}