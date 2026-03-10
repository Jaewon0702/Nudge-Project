using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeGuidanceCue : TrialCue
{
    LineRenderer lr;

    void Start()
    {
        lr = gameObject.GetComponent<LineRenderer>();
        lr.positionCount = 2;
    }

    public override void CueUpdate(GameObject cueTargetObject)
    {
        lr = gameObject.GetComponent<LineRenderer>();
        Vector3 startPos = transform.position;
        Vector3 endPoint = cueTargetObject.transform.position;
        if(startPos.Equals(endPoint))
        {
            startPos += new Vector3(0, 0.1f);
            endPoint -= new Vector3(0, 0.1f);
        }

        //To make the line not directly touch the object
        float distanceAway = GetSpecificDistanceAway(cueTargetObject);
        Vector3 directionNormalized = (cueTargetObject.transform.position - transform.position).normalized;

        endPoint -= (directionNormalized * distanceAway);

        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPoint);

        lr.enabled = cueTargetObject.activeSelf;
    }

    public float GetSpecificDistanceAway(GameObject cueTargetObject)
    {
        string spriteName;
        if (cueTargetObject.GetComponent< SpriteRenderer>() == null)
        {
            spriteName = "NoObject";
        }
        else
        {
            spriteName = cueTargetObject.GetComponent<SpriteRenderer>().sprite.name;
        }
        switch(spriteName)
        {
            //EXAMPLE OF HOW TO DO IT WITH OBJECT
            case "TestObject":
                return 1.0f;
            default:
                return 0.5f;
        }
    }
}
