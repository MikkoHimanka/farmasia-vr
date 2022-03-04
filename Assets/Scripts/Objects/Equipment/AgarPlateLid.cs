﻿
using UnityEngine;

public class AgarPlateLid : GeneralItem {

    public AgarPlateLidConnector Connector { get; private set; }

    [SerializeField]
    private GameObject BottomObject;

    protected override void Start() {
        base.Start();
        Type.On(InteractableType.Interactable, InteractableType.SmallObject);

        Connector = new AgarPlateLidConnector(this, transform.Find("Bottom Collider").gameObject);
        Connector.Subscribe();

        var Bottom = BottomObject.GetComponent<Interactable>();

        Connector.ConnectItem(Bottom);
    }
    public void ReleaseItem() {
        Connector.Connection?.Remove();
    }
}
