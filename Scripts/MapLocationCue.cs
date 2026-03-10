using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapLocationCue : TrialCue
{
    //MAP SETTINGS
    float roomToWorkWith = 1.125f; //X and Y
    float totalWidth = 5;
    float totalHeight = 5;

    public GameObject dotPrefab;
    public GameObject highlightPrefab;
    GameObject[] dots;

    Vector3[] cachedWorldPositions; // last known world positions per object

    public GameObject objectsParent;
    public GameObject cueCenter;
    GameObject dotsParent;
    GameObject highlight;

    public Color dimColor;
    public Color brightColor;

    public bool showImage;
    Sprite defaultDotSprite;
    public Sprite highPriorityObjectSprite;

    public void Awake()
    {
        dots = new GameObject[objectsParent.transform.childCount];
        cachedWorldPositions = new Vector3[objectsParent.transform.childCount];
        for (int i = 0; i < objectsParent.transform.childCount; i++)
        {
            cachedWorldPositions[i] = objectsParent.transform.GetChild(i).position;
        }
        dotsParent = gameObject.transform.GetChild(1).gameObject;
        defaultDotSprite = dotPrefab.GetComponent<SpriteRenderer>().sprite;
        InitDots();
    }

    void InitDots()
    {
        if (dots != null && dots.Length > 0)
        {
            for (int i = 0; i < dots.Length; i++)
            {
                if (dots[i])
                {
                    Destroy(dots[i]);
                }
            }
        }else
        {
            //Here becuse unity is dumb and for some reason awake just not doing its thing
            dotsParent = gameObject.transform.GetChild(1).gameObject;
            defaultDotSprite = dotPrefab.GetComponent<SpriteRenderer>().sprite;
        }
        dots = new GameObject[objectsParent.transform.childCount];
        for (int i = 0; i < objectsParent.transform.childCount; i++)
        {
            if (dots[i])
            {
                Destroy(dots[i]);
            }
            GameObject dot = Instantiate(dotPrefab, dotsParent.transform);
            dots[i] = dot;
        }
        if (highlight)
            Destroy(highlight);
        highlight = Instantiate(highlightPrefab, dotsParent.transform);
    }

    void UpdateDotsPositions(GameObject cueTargetObject)
    {
        if (!highlight)
        {
            InitDots();
        }
        highlight.GetComponent<SpriteRenderer>().color = Color.yellow;
        highlight.GetComponent<SpriteRenderer>().sprite = dotPrefab.GetComponent<SpriteRenderer>().sprite;
        highlight.transform.localScale = new Vector3(1.25f, 1.25f, 1.25f);
        highlight.transform.position = Vector3.zero + transform.position;

        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].GetComponent<SpriteRenderer>().color = (objectsParent.transform.GetChild(i).name.Equals(cueTargetObject.name)) ? brightColor : dimColor;
            dots[i].GetComponent<SpriteRenderer>().sortingOrder = i;
            if(objectsParent.transform.GetChild(i).name.Equals(cueTargetObject.name))
            {
                highlight.GetComponent<SpriteRenderer>().color = Color.yellow;
                highlight.GetComponent<SpriteRenderer>().sortingOrder = -10;
                highlight.transform.position = GetObjectRelativeLocation(objectsParent.transform.GetChild(i).gameObject) + transform.position;
                if(showImage)
                {
                    highlight.GetComponent<SpriteRenderer>().sprite = objectsParent.transform.GetChild(i).GetComponent<SpecialObject>().GetIcon();
                    float scale = 0.4f;
                    highlight.transform.localScale = new Vector3(scale, scale, scale);
                }else
                {
                    highlight.transform.localScale = new Vector3(1, 1, 1);
                }
                highlight.transform.localScale *= 1.2f; // THIS CHANGES THE HIGHLIGHT SIZE
                highlight.GetComponent<SpriteRenderer>().enabled = (objectsParent.transform.GetChild(i).gameObject.activeSelf);
            }

            dots[i].SetActive(objectsParent.transform.GetChild(i).gameObject.activeSelf);

            dots[i].transform.position = GetObjectRelativeLocation(objectsParent.transform.GetChild(i).gameObject) + transform.position;

            if(showImage)
            {
                SpecialObject objectRef = objectsParent.transform.GetChild(i).GetComponent<SpecialObject>();
                if(objectRef != null)
                {
                    dots[i].GetComponent<SpriteRenderer>().sprite = objectsParent.transform.GetChild(i).GetComponent<SpecialObject>().GetIcon();
                    dots[i].GetComponent<SpriteRenderer>().color = Color.white;
                    float scale = 0.4f; //changes sprite size on minimap
                    dots[i].transform.localScale = new Vector3(scale, scale, scale);
                }else
                {
                    //Special Object
                    dots[i].GetComponent<SpriteRenderer>().sprite = objectsParent.transform.GetChild(i).GetComponent<SpriteRenderer>().sprite;
                    dots[i].GetComponent<SpriteRenderer>().color = Color.white;
                    float scale = 0.4f;//changes sprite size on minimap
                    dots[i].transform.localScale = new Vector3(scale, scale, scale);
                }
            }
            else
            {
                dots[i].GetComponent<SpriteRenderer>().sprite = defaultDotSprite;
                dots[i].transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    GameObject prevObject ;
    public float widthDivision = 600;
    public float heightDivision = 400;
    public override void CueUpdate(GameObject cueTargetObject)
    {

        //Update
        totalWidth = float.Parse(Screen.currentResolution.ToString().Substring(0, 4)) / (widthDivision * transform.localScale.x);
        totalHeight = float.Parse(Screen.currentResolution.ToString().Substring(7, 4)) / (heightDivision * transform.localScale.y);

        if(prevObject != null && !prevObject.Equals(cueTargetObject))
        {
            InitDots();
        }
        prevObject = cueTargetObject;
        UpdateDotsPositions(cueTargetObject);
        cueCenter.transform.localPosition = GetObjectRelativeLocation(gameObject);

        //LocatorBounce();
    }

    public Vector3 GetObjectRelativeLocation(GameObject targetObject)
    {
        float targetX = targetObject.transform.position.x + totalWidth;
        float targetY = targetObject.transform.position.y + totalHeight;

        float localizedTargetX = targetX / totalWidth;
        float localizedTargetY = targetY / totalHeight;

        float aproxPosX = localizedTargetX * roomToWorkWith; //X and Y
        float aproxPosY = localizedTargetY * roomToWorkWith; //X and Y

        return new Vector3(aproxPosX - roomToWorkWith, aproxPosY - roomToWorkWith, 0);
    }

    public void LocatorBounce()
    {
        //Locator BOUNCE
        if(gameObject.transform.childCount >= 3)
        {
            GameObject bounce = gameObject.transform.GetChild(2).gameObject;
            float multiplier = 4;
            bounce.transform.localScale += new Vector3(multiplier * Time.deltaTime, multiplier * Time.deltaTime, 0);
            if (bounce.transform.localScale.x > 10)
            {
                bounce.transform.localScale = Vector3.zero;
            }
        }
    }
}
