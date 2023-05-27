using System;
using UnityEngine;

public class MaskItem : CollectibleItem {

    public class OnMaskCollectedEventArgs {
        public int id;
    }
    public event EventHandler<OnMaskCollectedEventArgs> OnMaskCollected;

    [SerializeField] private int maskId;

    public override void Interact() {
        base.Interact();

        OnMaskCollected?.Invoke(this, new OnMaskCollectedEventArgs {
            id = maskId
        });
    }
}
