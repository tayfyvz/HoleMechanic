using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallCollider : MonoBehaviour
{
    public int collectiblesCount = 0;
    public int collectiblesMaxCount = 5;

    public int goldCount = 0;
    public int silverCount = 0;
    public int crystalCount = 0;

    public Slider progress;
    public static FallCollider Instance;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.transform.tag == "gold" )
        {
            float newValue = progress.value + 1;
            progress.value = newValue;


            collectiblesCount++;
            goldCount++;

            Destroy(other.gameObject);
        }
        
        else if(other.transform.tag == "silver" )
        {
            float newValue = progress.value + 1;
            progress.value = newValue;


            collectiblesCount++;
            silverCount++;

            Destroy(other.gameObject);
        }
        
        else if(other.transform.tag == "crystal" )
        {
            float newValue = progress.value + 1;
            progress.value = newValue;


            collectiblesCount++;
            crystalCount++;

            Destroy(other.gameObject);
        }
    }

    

}
