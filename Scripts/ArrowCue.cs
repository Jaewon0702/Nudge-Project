using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowCue : TrialCue
{
    public override void CueUpdate(GameObject cueTargetObject)
    {
        Vector3 directionToTarget = cueTargetObject.transform.position - gameObject.transform.position;

        float angle = Mathf.Atan2(directionToTarget.y, directionToTarget.x);
        //To Degrees From Radians
        float angleOffset = 225;
        float degrees = (angle * Mathf.Rad2Deg) + angleOffset;

        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, degrees));
    }
}
