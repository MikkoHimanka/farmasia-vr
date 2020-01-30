﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaminarCabinetTable : MonoBehaviour {

    #region fields
    private static float ContaminateTime = 1.5f;

    private TriggerInteractableContainer safeZone;
    private TriggerInteractableContainer contaminateZone;
    #endregion

    private void Start() {
        safeZone = transform.GetChild(0).gameObject.AddComponent<TriggerInteractableContainer>();
        contaminateZone = transform.GetChild(1).gameObject.AddComponent<TriggerInteractableContainer>();
    }

    #region Collision events
    private void OnCollisionEnter(Collision coll) {
        if (Interactable.GetInteractable(coll.gameObject.transform) as GeneralItem is var i && i != null) {
            ContaminateItem(i);
        }
    }
    #endregion

    private void ContaminateItem(GeneralItem item) {

        IEnumerator Wait() {
            yield return new WaitForSeconds(ContaminateTime);
            
            if (!safeZone.Contains(item) && contaminateZone.Contains(item)) {
                UISystem.Instance.CreatePopup("Työvälineet eivät saisi koskea laminaarikaapin pintaa.", MsgType.Mistake);
                G.Instance.Progress.AddMistake("Esine koski laminaarikaapin pintaa");
                item.Contamination = GeneralItem.ContaminateState.Contaminated;
            }
        }

        StartCoroutine(Wait());
    }
}