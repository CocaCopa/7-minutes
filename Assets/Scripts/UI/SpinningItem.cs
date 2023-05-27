using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningItem : MonoBehaviour
{
    [SerializeField] private float spinSpeed;

    private void Update() {

        transform.Rotate(0, spinSpeed * Time.deltaTime, 0, Space.Self);
    }
}
