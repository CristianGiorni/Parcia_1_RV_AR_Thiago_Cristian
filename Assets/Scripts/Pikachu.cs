using System;
using UnityEngine;

// El script GirlUTN hereda toda la lógica de colisiones de ARInteractableObject. Para el script de KayaUTN reemplazar el nombre de la public class por KayaUTN. Ambos cumplen la misma funcion.
public class Pikachu : ARInteractableObject
{
    // Referencia para controlar las animaciones del personaje
    private Animator _animator;

    // Se ejecuta automáticamente al activarse el objeto en la escena
    private void OnEnable()
    {
        // Busca y obtiene el componente Animator que está en este mismo objeto
        _animator = GetComponent<Animator>();
    }

    // Sobrescribimos el cambio de estado para decirle qué animaciones usar
    protected override void SetState(State state)
    {
        base.SetState(state);

        switch (state)
        {
            case State.Active: // Cuando acercas las cartas
                // 1. Limpiamos cualquier orden previa de reposo
                _animator.ResetTrigger("Idle");
                // 2. Mandamos la orden para que empiece el ataque
                _animator.SetTrigger("Attack");
                break;

            case State.Idle: // Cuando separas las cartas
                // 1. Limpiamos cualquier orden de ataque (o baile) que haya quedado
                _animator.ResetTrigger("Attack");
                // 2. Mandamos la orden para que vuelva a su pose de reposo
                _animator.SetTrigger("Idle");
                break;
        }
    }
}