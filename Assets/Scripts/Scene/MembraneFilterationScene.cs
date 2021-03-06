using System;
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

    public static byte[] SavedScoreState;

    protected override void Start() {
        base.Start();
        PlayFirstRoom(autoPlayStrength);
    }

    public void SaveProgress(bool overwrite = false) {
        if (SavedScoreState != null || overwrite) {
            SavedScoreState = DataSerializer.Serializer(G.Instance.Progress.Calculator);
        }
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

        Task.CreateGeneralMistake("Yleinen testivirhe", 2);

        Task.CreateTaskMistake(TaskType.MedicineToFilter, "Testivirhe lääkkeen lisäämisessä", 1);
        
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

        yield return Wait();

        // Unbottle everything
        new List<GameObject> { bottle0, bottle1, bottle2, bottle3, soycaseine, tioglykolate, peptonwater }.ForEach(g => {
            var cap = g.transform.GetComponentInChildren<BottleCap>();
            cap.Connector.Connection.Remove();
            DropAt(cap.transform, cap.transform.position + Vector3.forward * 0.2f);
        });

        // Fill bottles
        var things = new List<(Bottle, Bottle, Pipette)>() {
            (tioglygolateB, bottleT1, pipetteT),
            (tioglygolateB, bottleT2, pipetteT),
            (soycaseineB, bottleS1, pipetteS),
            (soycaseineB, bottleS2, pipetteS),
        };
        float fillSpeed = 0.4f;
        foreach(var stuff in things) {
            var (bigBottle, bottle, pipette) = stuff;

            yield return Wait(fillSpeed);
            DropAt(pipette.transform, bigBottle.transform.position + Vector3.up * 0.10f);
            pipette.transform.eulerAngles = new Vector3(-180, 0, 0);
            yield return Wait(fillSpeed);
            hand.transform.position = pipette.transform.position;
            yield return Wait(fillSpeed);
            hand.InteractWith(pipette);
            yield return Wait(fillSpeed);
            hand.transform.eulerAngles = Vector3.down;
            pipette.TakeMedicine();
            yield return Wait(fillSpeed);
            hand.Uninteract();
            yield return Wait(fillSpeed);
            DropAt(pipette.transform, bottle.transform.position + Vector3.up * 0.10f);
            pipette.transform.eulerAngles = new Vector3(-180, 0, 0);
            yield return Wait(fillSpeed);
            hand.InteractWith(pipette);
            yield return Wait(fillSpeed);
            pipette.SendMedicine();
            hand.Uninteract();
            yield return Wait();
        };
        
        if (autoPlay == AutoPlayStrength.FillBottles) {
            yield break;
        }


        // --- Try to connect pump filter ---

        Pump pumpBody = pump.GetComponent<Pump>();
        DropAt(pumpFilter.transform, pump.transform.position + Vector3.up * 0.3f);

        hand.transform.position = pumpFilter.transform.position;
        
        yield return Wait();

        pumpFilter.GetComponent<Cover>().OpenCover(hand);
        yield return Wait();
        hand.Uninteract();
        yield return Wait();
        scalpel.GetComponent<Cover>().OpenCover(hand);
        yield return Wait();
        hand.Uninteract();
        tweezers.GetComponent<Cover>().OpenCover(hand);
        yield return Wait();
        hand.Uninteract();

        yield return Wait();

        


        /*yield return Wait(1f);

        var assemblyFilterParts = GameObject.Find("Assembly filter parts");
        var filterBase = assemblyFilterParts.transform.GetChild(1);
        hand.transform.position = filterBase.transform.position;
        hand.InteractWith(filterBase.GetComponent<Interactable>());

        yield return Wait(1f);

        hand.Uninteract();

        if (autoPlay == AutoPlayStrength.PreparePump) {
            yield break;
        }
        
        /*
        */

        yield break;
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
