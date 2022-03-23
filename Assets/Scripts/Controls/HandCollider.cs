﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class HandCollider : MonoBehaviour {
    [SerializeField]
    private bool IsExtendedHandCollider;
    
    private ObjectHighlight PreviousHighlight;

    private TriggerInteractableContainer container;
    private Collider handColl;

    private Vector3 closestPoint;

    private void Start() {
        closestPoint = Vector3.zero;
        container = gameObject.AddComponent<TriggerInteractableContainer>();
        container.IsExtendedCollider = IsExtendedHandCollider;
        container.OnExit = OnInteractableExit;

        handColl = GetComponent<Collider>();

        StartCoroutine(SetCollPos());
    }

    private IEnumerator SetCollPos() {

        while (true) {
            var type = GetModelType();

            yield return null;

            if (type == SteamVR_TrackedObject.EIndex.None) {
                continue;
            }

            break;
        }
    }

    private SteamVR_TrackedObject.EIndex GetModelType() {
        Transform model = transform.parent.Find("Model");
        SteamVR_RenderModel m = model.GetComponent<SteamVR_RenderModel>();
        return m.index;
    }


    private void OnInteractableExit(Interactable interactable) {
        if (interactable.Highlight == PreviousHighlight) UnhighlightPrevious();
    }

    public void Enable(bool enable) {
        handColl.enabled = enable;
        if (!enable) {
            container.ResetContainer();
        }
    }

    public void RemoveInteractable(Interactable interactable) {
        container.EnteredObjects.Remove(interactable);
    }

    #region Highlight
    public void HighlightClosestObject() {
        HighlightObject(GetClosestObject());
    }

    public void HighlightPointedObject(float maxAngle) {
        HighlightObject(GetPointedObject(maxAngle));
    }

    public void UnhighlightAll() {
        foreach (Interactable child in container.Objects) {
            child.Highlight.Unhighlight();
        }
    }

    private void HighlightObject(Interactable obj) {
        UnhighlightPrevious();

        if (container.Contains(obj) && !obj.DisableHighlighting) {
            PreviousHighlight = obj.Highlight;
            PreviousHighlight?.Highlight();
        }
    }
    #endregion

    public void UnhighlightPrevious() {
        PreviousHighlight?.Unhighlight();
        PreviousHighlight = null;
    }

    public Interactable GetClosestInteractable() {
        return GetClosestObject()?.GetComponent<Interactable>() ?? null;
    }

    public Interactable GetClosestObject() {
        float closestDistance = float.MaxValue;
        Interactable closest = null;

        foreach (Interactable rb in container.Objects) {
            if (IsExtendedHandCollider) {
                float distance = Vector3.Distance(transform.position, rb.transform.position);
                if (distance < closestDistance) {
                    closestDistance = distance;
                    closest = rb;
                }
            } else {
                if (!container.EnteredObjectPoints.ContainsKey(rb)) continue;
                foreach (Vector3 colPos in container.EnteredObjectPoints[rb].Values) {
                    float distance = Vector3.Distance(transform.position, colPos);
                    if (distance < closestDistance) {
                        closestDistance = distance;
                        closest = rb;
                        closestPoint = colPos;
                    }
                }
            }
        }
        
        return closest;
    }

    public void OnDrawGizmos() {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(closestPoint, 0.01f);
    }

    public Interactable GetPointedObject(float maxAngle) {
        float smallestAngle = float.MaxValue;
        Interactable closest = null;

        foreach (Interactable interactable in container.Objects) {

            if (interactable == null) {
                Logger.Print("interactable was null");
                continue;
            }

            float angle = Vector3.Angle(transform.forward, interactable.transform.position - transform.position);

            if (angle < smallestAngle && angle < maxAngle) {
                smallestAngle = angle;
                closest = interactable;
            }
        }

        return closest;
    }
}
