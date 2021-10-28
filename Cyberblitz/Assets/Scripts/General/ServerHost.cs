using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerHost : MonoBehaviour
{
    public static ServerHost Instance;

    private void Awake() {
        if(Instance != null) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }
}
