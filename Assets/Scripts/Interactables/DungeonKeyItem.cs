using System;

public class DungeonKeyItem : KeyItem {

    public event EventHandler OnDungeonKeyPickedUp;

    public override void Interact() {
        base.Interact();

        OnDungeonKeyPickedUp?.Invoke(this, EventArgs.Empty);
    }
}
