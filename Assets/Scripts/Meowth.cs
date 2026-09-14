using System;
using UnityEngine;

// El script hereda toda la lógica de colisiones de ARInteractableObject.
public class Meowth : ARInteractableObject
{
    // Referencia para controlar las animaciones del personaje
    private Animator _animator;

    // --- VARIABLES PARA ROTACIÓN ---
    [Header("Rotación Automática")]
    [SerializeField] private Transform modelo3D; // Aquí arrastraremos la malla interna de Meowth
    private Transform pikachuObjetivo; // Se asignará solo por código
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
        // Solo rotamos el cuerpo si las cartas están tocándose y encontró a Pikachu
        if (_estanCerca && modelo3D != null && pikachuObjetivo != null)
        {
            // Bloqueamos la altura (Y) para que Meowth no se incline hacia el piso o el techo
            Vector3 posicionObjetivo = new Vector3(
                pikachuObjetivo.position.x, 
                modelo3D.position.y, 
                pikachuObjetivo.position.z
            );

            // Obligamos al modelo interno a mirar hacia Pikachu de frente
            modelo3D.LookAt(posicionObjetivo);
        }
    }

    // Sobrescribimos el cambio de estado para decirle qué animaciones usar
    protected override void SetState(State state)
    {
        base.SetState(state);

        switch (state)
        {
            case State.Active:
                _estanCerca = true; // Prendemos el seguimiento visual

                // --- AUTO-APUNTADO ---
                // Meowth busca a Pikachu en la escena automáticamente
                if (pikachuObjetivo == null)
                {
                    // IMPORTANTE: Por una de tus capturas anteriores vi que tu objeto se llamaba "PikachuF". 
                    // Si se llama distinto, cambia este nombre entre las comillas.
                    GameObject pikachuEnEscena = GameObject.Find("PikachuF"); 
                    
                    if (pikachuEnEscena != null)
                    {
                        pikachuObjetivo = pikachuEnEscena.transform;
                    }
                    else
                    {
                        Debug.LogWarning("Meowth no pudo encontrar a 'PikachuF'. Revisa el nombre exacto en la jerarquía.");
                    }
                }
                // ---------------------

                _animator.ResetTrigger("Idle");
                _animator.SetTrigger("Death"); // Muere al juntar las cartas
                break;

            case State.Idle:
                _estanCerca = false; // Apagamos el seguimiento visual

                _animator.ResetTrigger("Death");
                _animator.SetTrigger("Idle"); // Vuelve a respirar al separar las cartas

                // Volvemos a poner a Meowth mirando hacia adelante (su rotación original sobre la carta)
                if (modelo3D != null) modelo3D.localRotation = Quaternion.identity;
                break;
        }
    }
}