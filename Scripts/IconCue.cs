using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconCue : TrialCue
{
    public override void CueUpdate(GameObject cueTargetObject)
    {
        if(cueTargetObject.name.Equals("TrialManager"))
        {
            gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = null;
            return;
        }
        gameObject.transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = cueTargetObject.GetComponent<SpecialObject>().GetIcon();
    }
}