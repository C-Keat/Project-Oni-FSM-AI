using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ENDGAMESCRIPT : MonoBehaviour
{
    [SerializeField] private KeyCode StartagainKey;

    private void Update()
    {
        if (Input.GetKeyDown(StartagainKey))
        {
            SceneManager.LoadScene(0);
        }
    }
}
