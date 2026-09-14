using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ARInteractableObject : MonoBehaviour
{
    // Lista de objetos interactuables con los que estoy interactuando actualmente
    private List<ARInteractableObject> _interactables = new List<ARInteractableObject>();

    // Estado actual de este objeto AR
    protected enum State
    {
        Idle, Active
    }
    protected State ARObjectState = State.Idle;

    // Detecta el objeto interactuable (ARInteractableObject) cuando entra al Trigger
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ARInteractableObject>(out var interactable))
        {
            AddInteractable(interactable);
        }
    }

    // Detecta cuando el objeto interactuable sale del Trigger
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ARInteractableObject>(out var interactable))
        {
            RemoveInteractable(interactable);
        }
    }

    protected void AddInteractable(ARInteractableObject interactable)
    {
        _interactables.Add(interactable);
        SetState(State.Active);
    }

    protected void RemoveInteractable(ARInteractableObject interactable)
    {
        _interactables.Remove(interactable);
        // Si ya no quedan objetos interactuando, volvemos al estado inactivo (Idle)
        if (_interactables.Count == 0) SetState(State.Idle);
    }

    // ¿Qué sucede cuando este objeto se desactiva/apaga?
    private void OnDisable()
    {
        foreach (var interactable in _interactables)
        {
            interactable.RemoveInteractable(this);
        }
        _interactables.Clear();
        SetState(State.Idle);
    }
    // Actualiza el estado de este objeto AR
    protected virtual void SetState(State state)
    {
        ARObjectState = state;
    }
}