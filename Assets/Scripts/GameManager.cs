using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // SINGLETON
    public static GameManager instance;

     private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    
    // EVENTS
    public UnityEvent ui_selectStartGame;
    public UnityEvent ui_selectRestartGame;
    public UnityEvent ui_selectCredits;
    public UnityEvent loseGame;
}
