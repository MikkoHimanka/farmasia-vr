﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;


class MembraneFilterationScene : SceneScript {

    public enum AutoPlayStrength {
        None = 0,
        ItemsToPassThrough,
        WorkspaceRoom,
        ItemsToWorkspace,
        WriteItems,
        OpenAgarPlates,
        FillBottles,
        PreparePump,
    }

    [SerializeField]
    private bool CleanEquipment = true;

    [SerializeField]
    public AutoPlayStrength autoPlayStrength;

    [SerializeField]
    private GameObject pipette0, pipette1, pipette2,
        bottle0, bottle1, bottle2, bottle3,
        plate0, plate1, plate2, plate3,
        soycaseine, tioglykolate, peptonwater,
        tweezers, scalpel,
        pumpFilter, pump,
        sterileBag,
        cleaningBottle,
        writingPen
        ;

    [SerializeField]
    private Transform cleaningPosition;

    [Tooltip("Scene items")]
    [SerializeField]
    private Transform correctPositions;

    [SerializeField]
    private Transform correctPositionsWorkspace;

    [SerializeField]
    private Interactable teleportDoorKnob;

    private bool played;
    public bool IsAutoPlaying { get; private set; }


    protected override void Start() {
        base.Start();
        PlayFirstRoom(autoPlayStrength);
    }

    public void PlayFirstRoom(AutoPlayStrength strength = AutoPlayStrength.None) {

        if (IsAutoPlaying || played || strength == 0) {
            return;
        }
        played = true;
        IsAutoPlaying = true;

        CoroutineUtils.StartThrowingCoroutine(
            this,
            PlayCoroutine(strength),
            e => {
                if (e != null)
                    Logger.Error(e);
                Logger.Print("Autoplay finished");
            }
        );
    }

    private IEnumerator PlayCoroutine(AutoPlayStrength autoPlay) {
        
        // Create objects from prefabs and store in a list. They must be in the correct order here!
        List<GameObject> gameObjects = new List<GameObject>() {
            pipette0, pipette1, pipette2,
            bottle0, bottle1, bottle2, bottle3,
            plate0, plate1, plate2, plate3,
            soycaseine, tioglykolate, peptonwater,
            tweezers, scalpel,
            pumpFilter, pump,
            sterileBag
        };

        List<Transform> transforms = gameObjects.Select(SelectTransform).ToList();

        yield return Wait();

        Hand hand = VRInput.Hands[0].Hand;

        // --- Set to correct positions in throughput cabinet ---

        for (int i = 0; i < transforms.Count; i++) { // -1 because no pen
            yield return Wait();
            if (CleanEquipment) { 
                DropAt(transforms[i], cleaningPosition);
                yield return Wait();
                cleaningBottle.GetComponent<CleaningBottle>().Clean();
            }
            DropAt(transforms[i], correctPositions.GetChild(i).transform);
        }

        yield return Wait();

        if (autoPlay == AutoPlayStrength.ItemsToPassThrough) {
            yield break;
        }

        yield return Wait();

        // --- Go to workspace ---

        hand.InteractWith(teleportDoorKnob);

        yield return Wait();

        hand.Uninteract();

        if (autoPlay == AutoPlayStrength.WorkspaceRoom) {
            yield break;
        }

        yield return Wait();

        // --- Set to correct positions in workspace ---

        for (int i = 0; i < transforms.Count; i++) {
            yield return Wait();
            DropAt(transforms[i], correctPositionsWorkspace.GetChild(i).transform);
        }

        if (autoPlay == AutoPlayStrength.ItemsToWorkspace) {
            yield break;
        }

        yield return Wait();
        

        WritingPen pen = ToInteractable(writingPen) as WritingPen;
        AgarPlateLid plateS1Lid = plate0.GetComponentInChildren<AgarPlateLid>();
        AgarPlateLid plateS2Lid = plate1.GetComponentInChildren<AgarPlateLid>();
        AgarPlateLid plateS3Lid = plate2.GetComponentInChildren<AgarPlateLid>();
        AgarPlateLid plateSDLid = plate3.GetComponentInChildren<AgarPlateLid>();
        Pipette pipetteS = ToInteractable(pipette0) as Pipette;
        Pipette pipetteT = ToInteractable(pipette1) as Pipette;
        Pipette pipetteP = ToInteractable(pipette2) as Pipette;
        Bottle bottleT1 = bottle0.GetComponentInChildren<Bottle>();
        Bottle bottleT2 = bottle1.GetComponentInChildren<Bottle>();
        Bottle bottleS1 = bottle2.GetComponentInChildren<Bottle>();
        Bottle bottleS2 = bottle3.GetComponentInChildren<Bottle>();
        Bottle soycaseineB = soycaseine.GetComponentInChildren<Bottle>();
        Bottle tioglygolateB = tioglykolate.GetComponentInChildren<Bottle>();

        // Write
        var writing = new Dictionary<WritingType, string>() {
            {WritingType.Name, "Oma nimi"},
            {WritingType.Date, "pvm"},
            {WritingType.Time, "klonaika"},
        };
        pen.SubmitWriting(plateS1Lid.GetComponent<Writable>(), plateS1Lid.gameObject, writing);

        writing = new Dictionary<WritingType, string>() {
            {WritingType.Name, "Oma nimi"},
            {WritingType.Date, "pvm"},
            {WritingType.Time, "klonaika"},
        };
        pen.SubmitWriting(plateSDLid.GetComponent<Writable>(), plateSDLid.gameObject, writing);

        writing = new Dictionary<WritingType, string>() {
            {WritingType.Name, "Oma nimi"},
            {WritingType.Date, "pvm"},
            {WritingType.RightHand, "oikea"},
            {WritingType.Time, "klonaika"}
        };
        pen.SubmitWriting(plateS2Lid.GetComponent<Writable>(), plateS2Lid.gameObject, writing);

        writing = new Dictionary<WritingType, string>() {
            {WritingType.Name, "Oma nimi"},
            {WritingType.Date, "pvm"},
            {WritingType.LeftHand, "vasen"},
            {WritingType.Time, "klonaika"}
        };
        pen.SubmitWriting(plateS3Lid.GetComponent<Writable>(), plateS3Lid.gameObject, writing);


        writing = new Dictionary<WritingType, string>() {
            {WritingType.Name, "Oma nimi"},
            {WritingType.Date, "pvm"},
            {WritingType.SoyCaseine, "soijakaseiini"},
        };
        pen.SubmitWriting(bottleS1.GetComponent<Writable>(), bottleS1.gameObject, writing);

        writing = new Dictionary<WritingType, string>() {
            {WritingType.Name, "Oma nimi"},
            {WritingType.Date, "pvm"},
            {WritingType.SoyCaseine, "soijakaseiini"},
        };
        pen.SubmitWriting(bottleS2.GetComponent<Writable>(), bottleS2.gameObject, writing);

        writing = new Dictionary<WritingType, string>() {
            {WritingType.Name, "Oma nimi"},
            {WritingType.Date, "pvm"},
            {WritingType.Tioglygolate, "tioglykolaatti"},
        };
        pen.SubmitWriting(bottleT1.GetComponent<Writable>(), bottleT1.gameObject, writing);

        writing = new Dictionary<WritingType, string>() {
            {WritingType.Name, "Oma nimi"},
            {WritingType.Date, "pvm"},
            {WritingType.Tioglygolate, "tioglykolaatti"},
        };
        pen.SubmitWriting(bottleT2.GetComponent<Writable>(), bottleT2.gameObject, writing);

        if (autoPlay == AutoPlayStrength.WriteItems) {
            yield break;
        }

        yield return Wait();

        plateS1Lid.ReleaseItem();
        plateSDLid.ReleaseItem();

        yield return Wait();

        DropAt(plateS1Lid.transform, plateS1Lid.transform.position + Vector3.up * 0.5f);
        DropAt(plateSDLid.transform, plateS1Lid.transform.position + Vector3.up * 0.5f);
        plateS1Lid.transform.Rotate(new Vector3(180, 0, 0));
        plateSDLid.transform.Rotate(new Vector3(180, 0, 0));

        if (autoPlay == AutoPlayStrength.OpenAgarPlates) {
            yield break;
        }

        yield break;
        /*

        // Fill bottles
        // tioglygolate 1
        yield return Wait();
        DropAt(pipette.transform, tioglygolate.transform.position + Vector3.up * 0.12f);
        pipette.transform.eulerAngles = new Vector3(-180, 0, 0);
        yield return Wait();
        hand.transform.position = pipette.transform.position;
        yield return Wait();
        hand.InteractWith(pipette);
        yield return Wait();
        hand.transform.eulerAngles = Vector3.down;
        pipette.TakeMedicine();
        yield return Wait();
        hand.Uninteract();
        yield return Wait();
        DropAt(pipette.transform, bottleT1.transform.position + Vector3.up * 0.1f);
        pipette.transform.eulerAngles = new Vector3(-180, 0, 0);
        yield return Wait();
        hand.InteractWith(pipette);
        yield return Wait();
        pipette.SendMedicine();
        hand.Uninteract();

        // tioglygolate 2
        yield return Wait(0.5f);
        tioglygolate.transform.eulerAngles *= 0f;
        DropAt(pipette.transform, tioglygolate.transform.position + Vector3.up * 0.12f);
        pipette.transform.eulerAngles = new Vector3(-180, 0, 0);
        hand.transform.position = pipette.transform.position;
        hand.transform.eulerAngles = Vector3.down;
        hand.InteractWith(pipette);
        yield return Wait(0.5f);
        pipette.TakeMedicine();
        yield return Wait();
        hand.Uninteract();
        yield return Wait();
        DropAt(pipette.transform, bottleT2.transform.position + Vector3.up * 0.1f);
        pipette.transform.eulerAngles = new Vector3(-180, 0, 0);
        yield return Wait();
        hand.InteractWith(pipette);
        yield return Wait();
        pipette.SendMedicine();
        hand.Uninteract();

        // soycaseine 1
        yield return Wait(0.5f);
        tioglygolate.transform.eulerAngles *= 0f;
        yield return Wait();
        DropAt(pipette2.transform, soycaseine.transform.position + Vector3.up * 0.2f);
        yield return Wait();
        pipette2.transform.eulerAngles = new Vector3(-180,0,0);
        hand.transform.position = pipette2.transform.position;
        yield return Wait();
        hand.transform.eulerAngles = Vector3.down;;
        hand.InteractWith(pipette2);
        yield return Wait(0.5f);
        pipette2.TakeMedicine();
        yield return Wait();
        hand.Uninteract();
        yield return Wait();
        DropAt(pipette2.transform, bottleS1.transform.position + Vector3.up * 0.1f);
        pipette2.transform.eulerAngles = new Vector3(-180,0,0);
        yield return Wait();
        hand.InteractWith(pipette2);
        yield return Wait();
        pipette2.SendMedicine();
        hand.Uninteract();

        // soycaseine 2
        yield return Wait(0.5f);
        tioglygolate.transform.eulerAngles *= 0f;
        DropAt(pipette2.transform, soycaseine.transform.position + Vector3.up * 0.2f);
        pipette2.transform.eulerAngles = new Vector3(-180,0,0);
        hand.transform.position = pipette2.transform.position;
        hand.transform.eulerAngles = Vector3.down;
        hand.InteractWith(pipette2);
        yield return Wait(0.5f);
        pipette2.TakeMedicine();
        yield return Wait();
        hand.Uninteract();
        yield return Wait();
        DropAt(pipette2.transform, bottleS2.transform.position + Vector3.up * 0.1f);
        pipette2.transform.eulerAngles = new Vector3(-180,0,0);
        yield return Wait();
        hand.InteractWith(pipette2);
        yield return Wait();
        pipette2.SendMedicine();
        hand.Uninteract();

        if (autoPlay == AutoPlayStrength.FillBottles) {
            yield break;
        }

        // --- Try to connect pump filter ---

        Pump pump = ToInteractable(gameObjects[14]) as Pump;
        PumpFilter filter = ToInteractable(gameObjects[15]) as PumpFilter;

        yield return Wait();

        DropAt(filter.transform, pump.transform.position + Vector3.up * 0.12f);

        yield return Wait();

        hand.InteractWith(filter);

        yield return Wait();

        hand.transform.position -= Vector3.up * 0.04f;

        yield return Wait();

        hand.Uninteract();

        yield return Wait();

        if (filter.Connector.AttachedInteractable == null) {
            filter.Connector.ConnectItem(pump);
            Logger.Print("Autoplay forced pump filter connection");
        }

        if (autoPlay == AutoPlayStrength.PreparePump) {
            yield break;
        }
        

        yield break;
        */
    }

    private Transform SelectTransform(GameObject gameObject) {
        if (gameObject.GetComponent<Rigidbody>() != null) {
            return gameObject.transform;
        } else {
            return gameObject.GetComponentInChildren<Rigidbody>().transform;
        }
    }

    private void DropAt(Transform theObject, Transform position) {
        DropAt(theObject, position.position);
    }

    private void DropAt(Transform theObject, Vector3 position) {
        theObject.position = position;
        theObject.eulerAngles = Vector3.up;
        var rigidBody = theObject.gameObject.GetComponent<Rigidbody>();
        if (rigidBody == null) {
            rigidBody = theObject.gameObject.GetComponentInChildren<Rigidbody>();
        }
        // rigidBody.velocity *= 0f;
    }

    private WaitForSeconds Wait() {
        return Wait(0.1f);
    }

    private WaitForSeconds Wait(float seconds) {
        return new WaitForSeconds(seconds);
    }

    private void AllowCollisionsBetween(List<Transform> items, bool allow) {
        for (int i = 0; i < items.Count; i++) {
            for (int j = 0; j < items.Count; j++) {
                if (i != j) {
                    CollisionIgnore.IgnoreCollisions(items[i], items[j], !allow);
                }
            }
        }
    }

    private Interactable ToInteractable(GameObject g) {
        var interactable = Interactable.GetInteractable(g.transform);
        if (interactable == null) {
            Logger.Warning(g.name + " converted to interactable was null");
        }
        return interactable;
    }
}
