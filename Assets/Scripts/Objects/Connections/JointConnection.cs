﻿using UnityEngine;

public class JointConnection : ItemConnection {

    #region Fields
    protected override ItemConnector Connector { get; set; }
    private Rigidbody rb;
    private Rigidbody target;
    private Transform transformTarget;
    private Interactable interactable;
    private Joint joint;

    #endregion

    private void Awake() {
        interactable = GetComponent<Interactable>();
        rb = interactable.Rigidbody;
    }

    private void SetJoint() {
        joint = JointConfiguration.AddJoint(gameObject, rb.mass);
        joint.connectedBody = target;
    }

    private void OnJointBreak(float force) {
        OnRemoveConnection();
    }


    protected override void Update() {
        base.Update();

        transform.rotation = Quaternion.Lerp(transform.rotation, transformTarget.rotation, 0.5f);
    }

    protected override void OnRemoveConnection() {

        Logger.Print("Removing SpringJointConnection from " + transform.name);

        SetVelocity();

        Destroy(joint);
    }

    private void SetVelocity() {
       if (Connector as HandConnector is var handConnector && handConnector != null) {
            rb.velocity = VRInput.Skeleton(handConnector.Hand.HandType).velocity;
            rb.angularVelocity = VRInput.Skeleton(handConnector.Hand.HandType).angularVelocity;
        }
    }

    public static JointConnection Configuration(ItemConnector connector, Transform target, Interactable addTo) {
        Rigidbody targetRB = GetTargetRigidbody(target);

        JointConnection conn = addTo.gameObject.AddComponent<JointConnection>();

        conn.Connector = connector;
        conn.target = targetRB;
        conn.transformTarget = target;

        conn.SetJoint();

        Logger.Print("Creating JointConnection to " + conn.transform.name + ", attached to " + targetRB.name);

        return conn;
    }

    private static Rigidbody GetTargetRigidbody(Transform target) {

        Rigidbody rb = Interactable.GetInteractable(target)?.Rigidbody;

        if (rb != null) {
            return rb;
        }

        if (target.parent != null && target.parent.GetComponent<Hand>() != null) {
            return target.parent.GetComponent<Rigidbody>();
        }

        if (target.GetComponent<HandSmoother>() != null) {
            return target.GetComponent<Rigidbody>();
        }

        throw new System.Exception("No target rigidbody found: " + target.name);
    }
}
