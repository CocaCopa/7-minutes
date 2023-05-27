using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class CollectedItem : MonoBehaviour
{
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private GameObject maskLight;
    [SerializeField] private int collectibleId;

    private MeshRenderer meshRenderer;

    private void Awake() {

        meshRenderer = GetComponentInChildren<MeshRenderer>();
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
            maskLight.SetActive(true);
        }
    }
}
