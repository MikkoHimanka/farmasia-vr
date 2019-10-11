using System;
using UnityEngine;

public class LockedDoor : MonoBehaviour {

    private void Start() {
    }

    public void CheckExitPermission() {
        GameObject gm = GameObject.FindWithTag("PassThroughCabinet");
        Events.FireEvent(EventType.CorrectItemsInThroughput, CallbackData.Object(gm.GetComponent<PassThroughCabinet>().objectsInsideArea));
        if (String.Equals(G.Instance.Progress.currentPackage.name, "Workspace")) {
            Events.FireEvent(EventType.CorrectLayoutInThroughput, CallbackData.String("" + gm.GetComponent<PassThroughCabinet>().objectsInsideArea.Count));
            //move to second room
        } else {
            UISystem.Instance.CreatePopup(gm.GetComponent<PassThroughCabinet>().GetMissingItems(), MessageType.Notify);
        }
    }
}