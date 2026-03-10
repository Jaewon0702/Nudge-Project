using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightCue : TrialCue
{
    SpriteRenderer sr;
    public Sprite defSprite;
    Vector3 scaleOffset = new Vector3(0.25f, 0.25f);
    public override void CueUpdate(GameObject cueTargetObject)
    {
        if(sr == null) { sr = gameObject.GetComponent<SpriteRenderer>(); }
        if(cueTargetObject.name.Equals("TrialManager"))
        {
            transform.position = Vector3.zero;
            sr.sprite = defSprite;
            return;
        }
        transform.position = cueTargetObject.transform.position;
        transform.localScale = cueTargetObject.transform.localScale + scaleOffset;
        sr.sprite = cueTargetObject.GetComponent<SpriteRenderer>().sprite;
        sr.enabled = (cueTargetObject.activeSelf);
    }
}