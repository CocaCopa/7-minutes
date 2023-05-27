using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectedItem : MonoBehaviour
{
    [SerializeField] private Material materialWhenHidden;
    [SerializeField] private int collectibleId;

    private PlayerInventory inventory;
    private MeshRenderer meshRenderer;
    private Material defaultMaterial;

    private void Awake() {

        inventory = FindObjectOfType<PlayerInventory>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        defaultMaterial = meshRenderer.material;

        meshRenderer.material = materialWhenHidden;
    }

    private void Start() {

        MaskItem[] masks = FindObjectsOfType<MaskItem>();

        foreach (var mask in masks) {

            mask.OnMaskCollected += Mask_OnMaskCollected;
        }
    }

    private void Mask_OnMaskCollected(object sender, MaskItem.OnMaskCollectedEventArgs e) {

        if (collectibleId == e.id) {

            meshRenderer.material = defaultMaterial;
        }
    }
}
