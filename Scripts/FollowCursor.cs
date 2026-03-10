using UnityEngine;

public class FollowCursor : MonoBehaviour
{
    TrialManager manager;

    public GameObject objectParent;

    public float distanceFromObjectToSelect;

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("TrialManager").GetComponent<TrialManager>();
    }

    void Update()
    {
        if (manager.GetTrialStarted())
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = 10;

            transform.position = Camera.main.ScreenToWorldPoint(mousePosition);

            if(Input.GetButtonDown("SelectObject"))
            {
                GameObject closestObject = objectParent.transform.GetChild(0).gameObject;
                float closest = float.MaxValue;
                for(int i = 0; i < objectParent.transform.childCount; i++)
                {
                    GameObject currChild = objectParent.transform.GetChild(i).gameObject;
                    float distance = Vector3.Distance(transform.position, currChild.transform.position);
                    if (closest > distance && currChild.activeSelf)
                    {
                        closest = distance;
                        closestObject = currChild;
                    }
                }

                if(closest <= distanceFromObjectToSelect)
                {
                    if(closestObject.CompareTag("HighPriority"))
                    {
                        if(manager.SelectedObject(closestObject))
                        {
                            closestObject.GetComponent<HighPriorityObject>().Collected();
                            manager.GotHighPriorityObject();
                        }
                    }else
                    {
                        closestObject.GetComponent<SpecialObject>().SelectedObject();
                    }
                }
            }
        }
    }
}