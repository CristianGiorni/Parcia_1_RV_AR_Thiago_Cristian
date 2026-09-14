using System;
using UnityEngine;

// El script hereda toda la lógica de colisiones de ARInteractableObject. 
public class Pikachu : ARInteractableObject
{
    // Referencia para controlar las animaciones del personaje
    private Animator _animator;

    [Header("Efectos del Rayo")]
    [SerializeField] private GameObject rayoPrefab;
    [SerializeField] private Transform puntoDeDisparo;
    [SerializeField] private Transform meowthObjetivo; // Se asignará solo por código

    // --- VARIABLES PARA ROTACIÓN ---
    [Header("Rotación Automática")]
    [SerializeField] private Transform modelo3D; // Aquí debe estar asignado el hijo (ej. pm0025_00_pikachu)
    private bool _estanCerca = false; // Interruptor para saber cuándo mirarse

    // Se ejecuta automáticamente al activarse el objeto en la escena
    private void OnEnable()
    {
        // Busca y obtiene el componente Animator que está en este mismo objeto
        _animator = GetComponent<Animator>();
    }

    // --- EL CÓDIGO QUE GIRA AL PERSONAJE EN TIEMPO REAL ---
    private void LateUpdate()
    {
        // Solo rotamos el cuerpo si las cartas están tocándose y encontró al enemigo
        if (_estanCerca && modelo3D != null && meowthObjetivo != null)
        {
            // Bloqueamos la altura (Y) para que Pikachu no se incline hacia el piso o el techo al mirar
            Vector3 posicionObjetivo = new Vector3(
                meowthObjetivo.position.x,
                modelo3D.position.y,
                meowthObjetivo.position.z
            );

            // Obligamos al modelo interno a mirar hacia ese punto de frente
            modelo3D.LookAt(posicionObjetivo);

            // (Rotación de 180 grados eliminada para que no le dé la espalda)
        }
    }

    // --- SE EJECUTA DESDE EL EVENTO DE ANIMACIÓN ---
    public void DispararRayo()
    {
        // Salvavidas: si por algún motivo perdió la referencia, lo busca de nuevo
        if (meowthObjetivo == null)
        {
            GameObject meowthEnEscena = GameObject.Find("Meowth");
            if (meowthEnEscena != null) meowthObjetivo = meowthEnEscena.transform;
        }

        if (rayoPrefab != null && puntoDeDisparo != null && meowthObjetivo != null)
        {
            // Instanciamos el rayo en el punto de disparo
            GameObject rayo = Instantiate(rayoPrefab, puntoDeDisparo.position, puntoDeDisparo.rotation);

            // Buscamos el script del rayo y le avisamos de dónde sale y hacia dónde va
            RayoPikachu scriptRayo = rayo.GetComponent<RayoPikachu>();
            if (scriptRayo != null)
            {
                scriptRayo.puntoOrigen = puntoDeDisparo;
                scriptRayo.puntoFinal = meowthObjetivo;
            }

            // El efecto de rayo durará 1.5 segundos en pantalla y luego se destruye
            Destroy(rayo, 1.5f);
        }
    }

    // --- SOBRESCRIBIMOS EL CAMBIO DE ESTADO ---
    protected override void SetState(State state)
    {
        base.SetState(state);

        switch (state)
        {
            case State.Active: // Cuando acercas las cartas
                _estanCerca = true; // Prendemos el seguimiento para que empiece a mirarlo

                // --- AUTO-APUNTADO ---
                // Pikachu busca a Meowth en la escena usando el nombre exacto
                if (meowthObjetivo == null)
                {
                    // IMPORTANTE: Asegúrate de que el objeto en la jerarquía se llame exactamente "Meowth"
                    GameObject meowthEnEscena = GameObject.Find("Meowth");

                    if (meowthEnEscena != null)
                    {
                        meowthObjetivo = meowthEnEscena.transform;
                    }
                    else
                    {
                        Debug.LogWarning("Pikachu no pudo encontrar a 'Meowth'. Revisa que el nombre sea el correcto en la escena.");
                    }
                }
                // ---------------------

                // 1. Limpiamos cualquier orden previa de reposo
                _animator.ResetTrigger("Idle");
                // 2. Mandamos la orden para que empiece el ataque
                _animator.SetTrigger("Attack");
                break;

            case State.Idle: // Cuando separas las cartas
                _estanCerca = false; // Apagamos el seguimiento visual

                // 1. Limpiamos cualquier orden de ataque que haya quedado
                _animator.ResetTrigger("Attack");
                // 2. Mandamos la orden para que vuelva a su pose de reposo
                _animator.SetTrigger("Idle");

                // Volvemos a poner a Pikachu mirando hacia adelante (su rotación original sobre la carta)
                if (modelo3D != null) modelo3D.localRotation = Quaternion.identity;
                break;
        }
    }
}