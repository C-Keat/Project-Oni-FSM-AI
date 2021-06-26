using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerVitals : MonoBehaviour
{
    [SerializeField] float health = 100;

    float curHealth = 100;

    // Start is called before the first frame update
    void Start()
    {
        curHealth = health;
    }

    private void Update()
    {
        if (curHealth == 0)
        {
            Destroy(this.gameObject);
            //load end scene saying that you are dead
            SceneManager.LoadScene(8);
        }
    }

    public float getcCurHealth()
    {
        return curHealth;
    }

    public void getHit(float damage)
    {
        Debug.Log(curHealth);
        curHealth -= damage;
    }
}
