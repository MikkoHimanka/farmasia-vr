﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThroughCabinet : MonoBehaviour {
    List<GameObject> objectsInsideArea;
    // Start is called before the first frame update
    void Start() {
        objectsInsideArea = new List<GameObject>();
        
    }

    private void OnTriggerEnter(Collider other) {
        GameObject foundObject = other.transform.gameObject;
        if (foundObject.GetComponent<GeneralItem>() == null) {
            return;
        }
        
        if (!objectsInsideArea.Contains(foundObject)) {
            objectsInsideArea.Add(foundObject);
            Events.FireEvent(EventType.ArrangeItems, CallbackData.String("" + objectsInsideArea.Count));
        }
        
        //UISystem.Instance.CreatePopup("Added object to Area!", MessageType.Notify);
    }

    private void OnTriggerExit(Collider other) {
        GameObject foundObject = other.transform.gameObject;
        if (foundObject.GetComponent<GeneralItem>() == null) {
            return;
        }
        objectsInsideArea.Remove(other.transform.gameObject);
        Events.FireEvent(EventType.ArrangeItems, CallbackData.String("" + objectsInsideArea.Count));
        //UISystem.Instance.CreatePopup("Removed Object From Area!", MessageType.Notify);
    }

    // Update is called once per frame
    void Update() {
        
    }
}