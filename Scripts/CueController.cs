using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CueController : MonoBehaviour
{
    ObjectController objController;
    TrialDataCreator trialDataCreator;
    TrialManager manager;
    ArrayManipulator arrayManipulator;

    public GameObject[] cues;
    public TrialCue arrowCue;
    public TrialCue miniMapCue;
    public TrialCue radarCue;
    public TrialCue localCue;
    public TrialCue iconCue;
    public TrialCue highlightCue;
    public TrialCue gazeCue;

    public Vector2[] possibleCueLocations = { new Vector2(0, 0) };

    private GameObject cueTargetObject;

    // Randomize order of blocks
    private void Start()
    {
        // for(int i = 0; i < Random.Range(0, cues.Length * 3); i++) {
        //     if(Random.Range(0, 1) == 0) {
        //         continue;
        //     }
        //     GameObject temp = cues[(i+1)%cues.Length];
        //     cues[(i+1)%cues.Length] = cues[i];
        //     cues[i] = temp;
        // }
        int[] cuesIndex = new int[cues.Length];
        for(int i = 0; i < cuesIndex.Length; i++)
        {
            cuesIndex[i] = i;
        }
        cuesIndex = arrayManipulator.RandomizeArrayElements(cuesIndex);
        GameObject[] cuesCopy = new GameObject[cues.Length];
        for(int i = 0; i < cues.Length; i++)
        {
            cuesCopy[i] = cues[cuesIndex[i]];
        }
        cues = cuesCopy;
    }    

    public void GetScripts()
    {
        arrayManipulator = gameObject.GetComponent<ArrayManipulator>();
        trialDataCreator = gameObject.GetComponent<TrialDataCreator>();
        //screenManager = gameObject.GetComponent<ScreenManager>();
        objController = gameObject.GetComponent<ObjectController>();
        manager = gameObject.GetComponent<TrialManager>();
        //cueController = gameObject.GetComponent<CueController>();
    }

    public GameObject GetCueTargetObject()
    {
        return cueTargetObject;
    }

    public void SetCueTargetObject(GameObject obj)
    {
        cueTargetObject = obj;
    }

    public int GetLastErrorType()
    {
        return lastErrorType;
    }

    public void SetCuesInactive()
    {
        arrowCue.gameObject.SetActive(false);
        miniMapCue.gameObject.SetActive(false);
        radarCue.gameObject.SetActive(false);
        localCue.gameObject.SetActive(false);
        iconCue.gameObject.SetActive(false);
        highlightCue.gameObject.SetActive(false);
        gazeCue.gameObject.SetActive(false);
    }

    //CUE CONTROLLER
    int testTotal = 0;
    int testFail = 0;
    int lastErrorType = 0;
    public GameObject CreateCueTargetObject(GameObject targetObject)
    {
        GameObject[] objects = objController.GetObjects();

        //THIS MEANS TARGET IS ABSENT
        //So we find a target that is active as to fool the user
        if (targetObject.Equals(gameObject))
        {
            //This actually finds target that is for sure active and makes sure new target isn't hidden by minimap or radar
            targetObject = objects[Random.Range(0, objects.Length)];
            while (!targetObject.activeSelf || ( (GetCueName().Contains("MiniMap") || GetCueName().Contains("RadarCue") || GetCueName().Contains("Icon")) && 
                                        (targetObject.transform.position.x < getMinimapTransform().localScale.x + 2 && targetObject.transform.position.x > -1 * getMinimapTransform().localScale.x - 2
                                         && targetObject.transform.position.y < getMinimapTransform().localScale.y + 2 && targetObject.transform.position.y > -1 * getMinimapTransform().localScale.y - 2) ) )
            {
                targetObject = objects[Random.Range(0, objects.Length)];
            }
        }

        //Fail if the chance is a 1
        if (trialDataCreator.whenToFail[manager.GetCurrentTrial()] == 0)
        {
            testTotal++;
            lastErrorType = 3;
            return targetObject;
        }
        testFail++;
        lastErrorType = manager.GetReliabilityError() - 1;
        //ERROR
        if (lastErrorType == 0)
        {
            //Point to Wrong Object
            GameObject obj = targetObject;
            while (obj.Equals(targetObject) || !obj.activeSelf || ( (GetCueName().Contains("MiniMap") || GetCueName().Contains("RadarCue") || GetCueName().Contains("Icon")) && 
                                        (obj.transform.position.x < getMinimapTransform().localScale.x + 2 && obj.transform.position.x > -1 * getMinimapTransform().localScale.x - 2
                                         && obj.transform.position.y < getMinimapTransform().localScale.y + 2 && obj.transform.position.y > -1 * getMinimapTransform().localScale.y - 2) ))
            {
                obj = objects[Random.Range(0, objects.Length)];
            }
            return obj;
        }
        else if (lastErrorType == 1)
        {
            //Point to inactive object, if none just do the 
            GameObject obj = targetObject;
            while (obj.Equals(targetObject) || obj.activeSelf || ( (GetCueName().Contains("MiniMap") || GetCueName().Contains("RadarCue") || GetCueName().Contains("Icon")) && 
                                        (obj.transform.position.x < getMinimapTransform().localScale.x + 2 && obj.transform.position.x > -1 * getMinimapTransform().localScale.x - 2
                                         && obj.transform.position.y < getMinimapTransform().localScale.y + 2 && obj.transform.position.y > -1 * getMinimapTransform().localScale.y - 2) ))
            {
                obj = objects[Random.Range(0, objects.Length)];
            }
            return obj;
        }
        else
        {
            //Error Out
            return gameObject;
        }
    }

    //CUE CONTROLLER
    void UpdateCuePosition(TrialCue cueController)
    {
        Vector2 cueOffSet = possibleCueLocations[manager.GetTrialDataAtNum(3)];
        if (GetCueCanBeMoved())
        {
            if (cueOffSet.x > 0)
            {
                cueController.MovePositionRight(cueOffSet.x * 1);
            }
            else if (cueOffSet.x < 0)
            {
                cueController.MovePositionLeft(cueOffSet.x * -1);
            }
            else if (cueOffSet.y > 0)
            {
                cueController.MovePositionUp(cueOffSet.y * 1);
            }
            else if (cueOffSet.y < 0)
            {
                cueController.MovePositionDown(cueOffSet.y * -1);
            }
            else
            {
                cueController.MovePositionDown(0);
            }
        }
    }

    //CUE CONTROLLER
    public void UpdateCue()
    {
        int cueType = manager.GetTrialDataAtNum(2);
        //0-Arrow 1-Minimap 2-Radar 3-Local 4-Icon
        if (cueType >= cues.Length) return;
        GameObject cue = cues[cueType];
        if (!cueTargetObject)
        {
            cueTargetObject = gameObject;
        }
        if (cue.name.Equals("ArrowCue"))
        {
            //ARROW
            cue.SetActive(true);
            cue.GetComponent<TrialCue>().CueUpdate(cueTargetObject);
            UpdateCuePosition(arrowCue);
        }
        else if (cue.name.Equals("MiniMapCue"))
        {
            //MINIMAP
            cue.SetActive(true);
            cue.GetComponent<TrialCue>().CueUpdate(cueTargetObject);
            UpdateCuePosition(miniMapCue);
        }
        else if (cue.name.Equals("RadarCue"))
        {
            //RADAR
            cue.SetActive(true);
            cue.GetComponent<TrialCue>().CueUpdate(cueTargetObject);
            UpdateCuePosition(radarCue);
        }
        else if (cue.name.Equals("LocalCue"))
        {
            //LOCAL
            cue.SetActive(true);
            cue.GetComponent<TrialCue>().CueUpdate(cueTargetObject);
        }
        else if (cue.name.Equals("IconCue"))
        {
            //ICON
            cue.SetActive(true);
            cue.GetComponent<TrialCue>().CueUpdate(cueTargetObject);
            UpdateCuePosition(iconCue);
        }
        else if (cue.name.Equals("GazeGuidanceCue"))
        {
            //GAZE GUIDANCE
            cue.SetActive(true);
            cue.GetComponent<TrialCue>().CueUpdate(cueTargetObject);
        }
        else if (cue.name.Equals("HighlightCue"))
        {
            //HIGHLIGHT
            cue.SetActive(true);
            cue.GetComponent<TrialCue>().CueUpdate(cueTargetObject);
        }
        else if (cue.name.Equals("IconComboCueLocal"))
        {
            for (int i = 0; i < cues.Length; i++)
            {
                if (cues[i].Equals(iconCue.gameObject) || cues[i].Equals(localCue.gameObject))
                {
                    cues[i].SetActive(true);
                }
            }
            iconCue.CueUpdate(cueTargetObject);
            localCue.CueUpdate(cueTargetObject);
            iconCue.gameObject.SetActive(true);
            localCue.gameObject.SetActive(true);
            UpdateCuePosition(iconCue);
        }
        else if (cue.name.Equals("IconComboCueHighlight"))
        {
            for (int i = 0; i < cues.Length; i++)
            {
                if (cues[i].Equals(iconCue.gameObject) || cues[i].Equals(highlightCue.gameObject))
                {
                    cues[i].SetActive(true);
                }
            }
            iconCue.CueUpdate(cueTargetObject);
            highlightCue.CueUpdate(cueTargetObject);
            iconCue.gameObject.SetActive(true);
            highlightCue.gameObject.SetActive(true);
            UpdateCuePosition(iconCue);
        }
        else if (cue.name.Equals("IconComboCueGaze"))
        {
            for (int i = 0; i < cues.Length; i++)
            {
                if (cues[i].Equals(iconCue.gameObject) || cues[i].Equals(gazeCue.gameObject))
                {
                    cues[i].SetActive(true);
                }
            }
            iconCue.CueUpdate(cueTargetObject);
            gazeCue.CueUpdate(cueTargetObject);
            iconCue.gameObject.SetActive(true);
            gazeCue.gameObject.SetActive(true);
            UpdateCuePosition(iconCue);
        }
        else if (cue.name.Equals("MiniMapComboCueLocal"))
        {
            for (int i = 0; i < cues.Length; i++)
            {
                if (cues[i].Equals(miniMapCue.gameObject) || cues[i].Equals(localCue.gameObject))
                {
                    cues[i].SetActive(true);
                }
            }
            miniMapCue.CueUpdate(cueTargetObject);
            localCue.CueUpdate(cueTargetObject);
            miniMapCue.gameObject.SetActive(true);
            localCue.gameObject.SetActive(true);
            UpdateCuePosition(miniMapCue);
        }
        else if (cue.name.Equals("MiniMapComboCueHighlight"))
        {
            for (int i = 0; i < cues.Length; i++)
            {
                if (cues[i].Equals(miniMapCue.gameObject) || cues[i].Equals(highlightCue.gameObject))
                {
                    cues[i].SetActive(true);
                }
            }
            miniMapCue.CueUpdate(cueTargetObject);
            highlightCue.CueUpdate(cueTargetObject);
            miniMapCue.gameObject.SetActive(true);
            highlightCue.gameObject.SetActive(true);
            UpdateCuePosition(miniMapCue);
        }
        else if (cue.name.Equals("MiniMapComboCueGaze"))
        {
            for (int i = 0; i < cues.Length; i++)
            {
                if (cues[i].Equals(miniMapCue.gameObject) || cues[i].Equals(gazeCue.gameObject))
                {
                    cues[i].SetActive(true);
                }
            }
            miniMapCue.CueUpdate(cueTargetObject);
            gazeCue.CueUpdate(cueTargetObject);
            miniMapCue.gameObject.SetActive(true);
            gazeCue.gameObject.SetActive(true);
            UpdateCuePosition(miniMapCue);
        }
        else if (cue.name.Equals("ArrowComboCueLocal"))
        {
            for (int i = 0; i < cues.Length; i++)
            {
                if (cues[i].Equals(arrowCue.gameObject) || cues[i].Equals(localCue.gameObject))
                {
                    cues[i].SetActive(true);
                }
            }
            arrowCue.CueUpdate(cueTargetObject);
            localCue.CueUpdate(cueTargetObject);
            arrowCue.gameObject.SetActive(true);
            localCue.gameObject.SetActive(true);
            UpdateCuePosition(arrowCue);
        }
        else if (cue.name.Equals("ArrowComboCueHighlight"))
        {
            for (int i = 0; i < cues.Length; i++)
            {
                if (cues[i].Equals(arrowCue.gameObject) || cues[i].Equals(highlightCue.gameObject))
                {
                    cues[i].SetActive(true);
                }
            }
            arrowCue.CueUpdate(cueTargetObject);
            highlightCue.CueUpdate(cueTargetObject);
            arrowCue.gameObject.SetActive(true);
            highlightCue.gameObject.SetActive(true);
            UpdateCuePosition(arrowCue);
        }
        else if (cue.name.Equals("ArrowComboCueGaze"))
        {
            for (int i = 0; i < cues.Length; i++)
            {
                if (cues[i].Equals(arrowCue.gameObject) || cues[i].Equals(gazeCue.gameObject))
                {
                    cues[i].SetActive(true);
                }
            }
            arrowCue.CueUpdate(cueTargetObject);
            arrowCue.gameObject.SetActive(true);
            gazeCue.gameObject.SetActive(true);
            gazeCue.CueUpdate(cueTargetObject);
            UpdateCuePosition(arrowCue);
        }
        else if (cue.name.Equals("NoCue"))
        {
            //No Cue
        }
        else
        {
            //No Cue
        }
    }

    // Get the collision2d component of the border around the minimap to ensure the target isn't behind it
    public Transform getMinimapTransform() {
        return miniMapCue.gameObject.transform;
    }

    //CUE CONTROLLER
    public string GetCueName()
    {
        int cueType = manager.GetTrialDataAtNum(2);
        if (cueType >= cues.Length)
        {
            return "No Cue";
        }
        else
        {
            return cues[cueType].name;
        }
    }

    //CUE CONTROLLER
    public string GetCueIsLocal()
    {
        int cueType = manager.GetTrialDataAtNum(2);
        //0-Arrow 1-Minimap 2-Radar 3-Local 4-Icon
        if (cueType >= cues.Length) return "None";
        GameObject cue = cues[manager.GetTrialDataAtNum(2)];
        //NOT USED IN DESKTOP VERSION
        if (cue.name.Equals("ArrowCue"))
        {
            return "G";
        }
        else if (cue.name.Equals("MiniMapCue"))
        {
            return "G";
        }
        else if (cue.name.Equals("RadarCue"))
        {
            return "G";
        }
        else if (cue.name.Equals("LocalCue"))
        {
            return "L";
        }
        else if (cue.name.Equals("IconCue"))
        {
            return "I";
        }
        else if (cue.name.Equals("GazeGuidanceCue"))
        {
            return "L";
        }
        else if (cue.name.Equals("HighlightCue"))
        {
            return "L";
        }
        else if (cue.name.Equals("IconComboCueLocal"))
        {
            return "I-L";
        }
        else if (cue.name.Equals("IconComboCueHighlight"))
        {
            return "I-L";
        }
        else if (cue.name.Equals("IconComboCueGaze"))
        {
            return "I-L";
        }
        else if (cue.name.Equals("MiniMapComboCueLocal"))
        {
            return "G-L";
        }
        else if (cue.name.Equals("MiniMapComboCueHighlight"))
        {
            return "G-L";
        }
        else if (cue.name.Equals("MiniMapComboCueGaze"))
        {
            return "G-L";
        }
        else if (cue.name.Equals("ArrowComboCueLocal"))
        {
            return "G-L";
        }
        else if (cue.name.Equals("ArrowComboCueHighlight"))
        {
            return "G-L";
        }
        else if (cue.name.Equals("ArrowComboCueGaze"))
        {
            return "G-L";
        }
        return "WIP";
    }

    //CUE CONTROLLER
    public string GetCueIsConformal()
    {
        int cueType = manager.GetTrialDataAtNum(2);
        //0-Arrow 1-Minimap 2-Radar 3-Local 4-Icon
        if (cueType >= cues.Length) return "None";
        GameObject cue = cues[manager.GetTrialDataAtNum(2)];
        //NOT USED IN DESKTOP VERSION
        if (cue.name.Equals("ArrowCue"))
        {
            return "conformal";
        }
        else if (cue.name.Equals("MiniMapCue"))
        {
            return "partially conformal";
        }
        if (cue.name.Equals("RadarCue"))
        {
            return "partially conformal";
        }
        if (cue.name.Equals("LocalCue"))
        {
            return "partially conformal";
        }
        if (cue.name.Equals("IconCue"))
        {
            return "non conformal";
        }
        if (cue.name.Equals("GazeGuidanceCue"))
        {
            return "conformal";
        }
        if (cue.name.Equals("HighlightCue"))
        {
            return "conformal";
        }
        else if (cue.name.Equals("IconComboCueLocal"))
        {
            return "non conformal-partially conformal";
        }
        else if (cue.name.Equals("IconComboCueHighlight"))
        {
            return "non conformal-conformal";
        }
        else if (cue.name.Equals("IconComboCueGaze"))
        {
            return "non conformal-conformal";
        }
        else if (cue.name.Equals("MiniMapComboCueLocal"))
        {
            return "partially conformal-partially conformal";
        }
        else if (cue.name.Equals("MiniMapComboCueHighlight"))
        {
            return "partially conformal-conformal";
        }
        else if (cue.name.Equals("MiniMapComboCueGaze"))
        {
            return "partially conformal-conformal";
        }
        else if (cue.name.Equals("ArrowComboCueLocal"))
        {
            return "conformal-partially conformal";
        }
        else if (cue.name.Equals("ArrowComboCueHighlight"))
        {
            return "conformal-conformal";
        }
        else if (cue.name.Equals("ArrowComboCueGaze"))
        {
            return "conformal-conformal";
        }
        return "WIP";
    }

    //CUE CONTROLLER
    public bool GetCueCanBeMoved()
    {
        //Maybe switch to name as well
        int cueType = manager.GetTrialDataAtNum(2);
        //0-Arrow 1-Minimap 2-Radar 3-Local 4-Icon
        if (cueType >= cues.Length) return false;
        GameObject cue = cues[cueType];
        //NOT USED IN DESKTOP VERSION
        if (cue.name.Equals("ArrowCue"))
        {
            return true;
        }
        else if (cue.name.Equals("MiniMapCue"))
        {
            return true;
        }
        if (cue.name.Equals("RadarCue"))
        {
            return true;
        }
        if (cue.name.Equals("LocalCue"))
        {
            return false;
        }
        if (cue.name.Equals("IconCue"))
        {
            return true;
        }
        if (cue.name.Equals("GazeGuidanceCue"))
        {
            return false;
        }
        if (cue.name.Equals("HighlightCue"))
        {
            return false;
        }
        else if (cue.name.Equals("IconComboCueLocal"))
        {
            return true;
        }
        else if (cue.name.Equals("IconComboCueHighlight"))
        {
            return true;
        }
        else if (cue.name.Equals("IconComboCueGaze"))
        {
            return true;
        }
        else if (cue.name.Equals("MiniMapComboCueLocal"))
        {
            return true;
        }
        else if (cue.name.Equals("MiniMapComboCueHighlight"))
        {
            return true;
        }
        else if (cue.name.Equals("MiniMapComboCueGaze"))
        {
            return true;
        }
        else if (cue.name.Equals("ArrowComboCueLocal"))
        {
            return true;
        }
        else if (cue.name.Equals("ArrowComboCueHighlight"))
        {
            return true;
        }
        else if (cue.name.Equals("ArrowComboCueGaze"))
        {
            return true;
        }
        return false;
    }

    //CUE CONTROLLER
    public string GetCuePosition(Vector2 cueOffSet)
    {
        if (GetCueCanBeMoved())
        {
            if (cueOffSet.x > 0)
            {
                return "rightward" + cueOffSet.x;
            }
            else if (cueOffSet.x < 0)
            {
                return "leftward" + (cueOffSet.x * -1);
            }
            else if (cueOffSet.y > 0)
            {
                return "upward" + cueOffSet.y;
            }
            else if (cueOffSet.y < 0)
            {
                return "downward" + (cueOffSet.y * -1);
            }
        }
        return "center";
    }
}
