using System;
using UnityEngine;

// El script GirlUTN hereda toda la lógica de colisiones de ARInteractableObject. Para el script de KayaUTN reemplazar el nombre de la public class por KayaUTN. Ambos cumplen la misma funcion.
public class Meowth : ARInteractableObject
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
            case State.Active:
                _animator.ResetTrigger("Idle");
                _animator.SetTrigger("Death"); // Muere al juntar las cartas
                break;

            case State.Idle:
                _animator.ResetTrigger("Death");
                _animator.SetTrigger("Idle"); // Vuelve a respirar al separar las cartas
                break;
        }
    }
}