using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemsToSterileBag : TaskBase {

    #region Fields
    public enum Conditions { SyringesPut }
    private int smallSyringesCount;
    private string description = "Viimeistele ruiskujen kanssa työskentely.";
    private string hint = "Laita molemmat käyttämäsi ruiskut steriiliin pussiin.";
    #endregion

    #region Constructor
    ///  <summary>
    ///  Constructor for ItemsToSterileBag task.
    ///  Is removed when finished and doesn't require previous task completion.
    ///  </summary>
    public ItemsToSterileBag() : base(TaskType.ItemsToSterileBag, true, false) {
        smallSyringesCount = 0;
        Subscribe();
        AddConditions((int[]) Enum.GetValues(typeof(Conditions)));
        points = 1;
    }
    #endregion

    #region Event Subscriptions
    /// <summary>
    /// Subscribes to required Events.
    /// </summary>
    public override void Subscribe() {
        base.SubscribeEvent(PutToBag, EventType.SterileBag);
    }
    /// <summary>
    /// Once fired by an event, checks how many syringe objects are put to bag object.
    /// Sets corresponding condition to be true.
    /// </summary>
    /// <param name="data">"Refers to the data returned by the trigger."</param>
    private void PutToBag(CallbackData data) {
        GameObject gm = GameObject.FindWithTag("Bag");
        SterileBag sterileBag = gm.GetComponent<SterileBag>();
        if (!sterileBag.IsClosed) {
            UISystem.Instance.CreatePopup("Sulje steriili pussi.", MessageType.Notify);
            return;
        }

        List<GameObject> inBag = data.DataObject as List<GameObject>;
        if (inBag == null) {
            return;
        }
        foreach(GameObject value in inBag) {
            GeneralItem item = value.GetComponent<GeneralItem>();
            ObjectType type = item.ObjectType;
            if (type == ObjectType.Syringe) {
                Syringe syringe = item as Syringe;
                //should be 0,15ml
                if (syringe.Container.Capacity == 1 && syringe.Container.Amount == 15) {
                    smallSyringesCount++;
                }
            }
        }

        /*GameObject lc = GameObject.FindWithTag("LaminarCabinet");
        LaminarCabinet laminar = lc.GetComponent<LaminarCabinet>();
        List<GameObject> inLaminar = laminar.objectsInsideArea;
        int usedSmallSyringes = 0;
        foreach(GameObject value in inLaminar) {
            GeneralItem item = value.GetComponent<GeneralItem>();
            ObjectType type = item.ObjectType;
            if (type == ObjectType.Syringe) {
                Syringe syringe = item as Syringe;
                //should be 0,15ml
                if (syringe.Container.Capacity == 1 && syringe.Container.Amount != 0) {
                    usedSmallSyringes++;
                }
            }
        }*/

        
        if (smallSyringesCount == 6) {
            EnableCondition(Conditions.SyringesPut);
        }

        CheckClearConditions(true);
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Once all conditions are true, this method is called.
    /// </summary>
    public override void FinishTask() {
        UISystem.Instance.CreatePopup("Ruiskut laitettiin steriiliin pussiin.", MessageType.Done);
        base.FinishTask();
    }
    
    /// <summary>
    /// Used for getting the task's description.
    /// </summary>
    /// <returns>"Returns a String presentation of the description."</returns>
    public override string GetDescription() {
        return description;
    }

    /// <summary>
    /// Used for getting the hint for this task.
    /// </summary>
    /// <returns>"Returns a String presentation of the hint."</returns>
    public override string GetHint() {
        return hint;
    }
    #endregion
}