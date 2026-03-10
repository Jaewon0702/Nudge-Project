using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighPriorityObject : MonoBehaviour
{
    float width = 17.0f;
    float height = 4.25f;

    public Sprite[] sprites;

    //REMEMBER THESE ARE SET BY THE PREFAB
    public float maxDistanceFromObj = 5.0f;
    public float minDistanceFromObj = 1.0f;

    public void Collected()
    {
        gameObject.SetActive(false);
    }

    public void RandomPosition(Vector3 targetObjectPosition)
    {
        transform.position = new Vector3(Random.Range(-width, width), Random.Range(-height, height));

        float distance = Vector3.Distance(targetObjectPosition, transform.position);
        int tries = 0;
        while(distance > maxDistanceFromObj || distance < minDistanceFromObj)
        {
            tries++;
            transform.position = new Vector3(Random.Range(-width, width), Random.Range(-height, height));
            distance = Vector3.Distance(targetObjectPosition, transform.position);
            if(tries >= (int.MaxValue - 1) / 5 )
            {
                Debug.LogError("Could Not Find Appropriate Location For the High Priority Object");
                break;
            }
        }

        RandomTexture();
    }

    void RandomTexture()
    {
        gameObject.GetComponent<SpriteRenderer>().sprite = sprites[Random.Range(0, sprites.Length)];
    }
}
