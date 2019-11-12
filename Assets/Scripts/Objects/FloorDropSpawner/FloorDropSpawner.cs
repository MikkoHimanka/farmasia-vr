﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorDropSpawner : MonoBehaviour {
    GameObject copy;
    GameObject currentObject;

    private void Start() {
        Events.SubscribeToEvent(Copy, EventType.ItemDroppedOnFloor);
    }

    public void SetCopyObject(GameObject gob) {
        copy = gob;
        currentObject = gob;
    }

    public void Copy(CallbackData data) {
        GeneralItem item = (GeneralItem) data.DataObject;
        if (item.gameObject == currentObject || currentObject == null) {
            currentObject = Instantiate(copy, transform.position, transform.rotation);
        } else {
        }

    }
}