using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteractUI : MonoBehaviour {

    [SerializeField] private GameObject containerGameObject;
    [SerializeField] private TextMeshProUGUI interactableTMProText;
    
    private PlayerController playerController;

    private void Awake() {
        
        playerController = FindObjectOfType<PlayerController>();
    }

    private void Update() {

        if (playerController.GetInteractObject() != null) {

            Show(playerController.GetInteractObject());
        }
        else {

            Hide();
        }
    }


    private void Show(IInteractable interactable) {

        containerGameObject.SetActive(true);
        interactableTMProText.text = interactable.GetInteractableText();
    }

    private void Hide() {

        containerGameObject.SetActive(false);
    }
}
