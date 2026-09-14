using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RayoPikachu : MonoBehaviour
{
    [Header("Conexiones del Rayo")]
    public Transform puntoOrigen; // NUEVO: De dónde sale (Pikachu)
    public Transform puntoFinal;  // Hacia dónde viaja (Meowth)

    [Header("Configuración visual")]
    public int cantidadPuntos = 10;
    public float dispersion = 0.5f;
    public float tiempoRefresco = 0.05f;

    private LineRenderer _lineRenderer;
    private float _temporizador;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        _temporizador -= Time.deltaTime;
        if (_temporizador <= 0f)
        {
            ActualizarPuntos();
            _temporizador = tiempoRefresco;
        }
    }

    void ActualizarPuntos()
    {
        // Si falta alguno de los dos puntos, no dibujamos nada
        if (puntoOrigen == null || puntoFinal == null) return;

        _lineRenderer.positionCount = cantidadPuntos;

        // El punto 0 (inicio) ahora es exactamente el punto de origen
        _lineRenderer.SetPosition(0, puntoOrigen.position);

        // El último punto es Meowth
        _lineRenderer.SetPosition(cantidadPuntos - 1, puntoFinal.position);

        // Puntos intermedios con tembleque
        for (int i = 1; i < cantidadPuntos - 1; i++)
        {
            Vector3 puntoBase = Vector3.Lerp(puntoOrigen.position, puntoFinal.position, (float)i / (cantidadPuntos - 1));

            Vector3 desfase = new Vector3(
                Random.Range(-dispersion, dispersion),
                Random.Range(-dispersion, dispersion),
                Random.Range(-dispersion, dispersion)
            );

            _lineRenderer.SetPosition(i, puntoBase + desfase);
        }
    }
}