using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LocalCue : TrialCue
{
    public override void CueUpdate(GameObject cueTargetObject)
    {
         Vector3 offset = GetSpecificOffset(cueTargetObject);
         gameObject.transform.position = cueTargetObject.transform.position + offset;

        gameObject.GetComponent<SpriteRenderer>().enabled = cueTargetObject.activeSelf;
    }

    public Vector3 GetSpecificOffset(GameObject cueTargetObject)
    {
        string spriteName;
        if (cueTargetObject.GetComponent<SpriteRenderer>() == null)
        {
            spriteName = "NoObject";
        }
        else
        {
            spriteName = cueTargetObject.GetComponent<SpriteRenderer>().sprite.name;
        }
        switch (spriteName)
        {
            //EXAMPLE OF HOW TO DO IT WITH OBJECT
            case "TestObject":
                return new Vector3(0, 1.5f, 0);
            default:
                return new Vector3(0, 0.75f, 0);
        }
    }
}
