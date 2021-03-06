using UnityEngine;
using UnityEngine.Assertions;

public class DisinfectingCloth : GeneralItem {

    protected override void Start() {
        base.Start();

        ObjectType = ObjectType.DisinfectingCloth;
        Type.On(InteractableType.Interactable);
    }

    protected override void OnCollisionEnter(Collision other) {
        base.OnCollisionEnter(other);

        GameObject foundObject = GetInteractableObject(other.transform);
        GeneralItem item = foundObject?.GetComponent<GeneralItem>();
        if (item == null) {
            return;
        }
        if ((item.ObjectType == ObjectType.Bottle || item.ObjectType == ObjectType.Medicine) && this.IsClean) {
            Bottle bottle = item as Bottle;
            if (!bottle.IsClean) {
                bottle.Contamination = ContaminateState.Clean;
                UISystem.Instance.CreatePopup("Lääkepullon korkki puhdistettu.", MsgType.Done);
                Events.FireEvent(EventType.BottleDisinfect, CallbackData.Object(bottle));
            }
        }
    }
}