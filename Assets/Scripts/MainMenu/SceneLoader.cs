﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public Animator animator;
    private string scene;
    public void SwapScene(SceneTypes type) {
        switch (type) {
            case SceneTypes.MainMenu:
                ChangeScene("MainMenu");
                break;
            case SceneTypes.MedicinePreparation:
                ChangeScene("MedicinePreparation");
                break;
            case SceneTypes.MembraneFilteration:
                break;
        }
    }
    private void ChangeScene(string name) {
        scene = name;
        FadeOutScene();

    }

    public void FadeOutScene() {
        animator.SetTrigger("FadeOut");
    }

    public void OnFadeComplete() {
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        Resources.UnloadUnusedAssets();
        SceneManager.LoadScene(scene);
    }


}
