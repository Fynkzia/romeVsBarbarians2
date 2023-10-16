using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptions : MonoBehaviour {
    public float maxLineLength;
    public static GameOptions Instance { get; private set; }
    private void Awake() {
        Instance = this;
    }
}
