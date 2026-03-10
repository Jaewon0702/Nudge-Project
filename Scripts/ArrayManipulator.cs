using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrayManipulator : MonoBehaviour
{
    public void GetScripts() { }

    //Array Manipulator
    public int[] RandomizeArrayElements(int[] data, int seed)
    {
        Random.InitState(seed);
        for (int i = 0; i < data.Length; i++)
        {
            int swapPos = Random.Range(0, data.Length);
            int temp = data[swapPos];
            data[swapPos] = data[i];
            data[i] = temp;
        }

        return data;
    }

    //Array Manipulator
    public int[] RandomizeArrayElementsKeepBlocked(int[] data, int seed)
    {
        Random.InitState(seed);
        int TEMP = 999124999;
        //Swap all 
        for (int i = 0; i < data.Length; i++)
        {
            int leftData = data[i];
            int rightData = data[Random.Range(0, data.Length)];

            data = ReplaceDataInListWith(data, leftData, TEMP);
            data = ReplaceDataInListWith(data, rightData, leftData);
            data = ReplaceDataInListWith(data, TEMP, rightData);
        }
        return data;
    }

    //Array Manipulator
    public int[] ReplaceDataInListWith(int[] data, int dataToReplace, int replacementData)
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data[i] == dataToReplace)
            {
                data[i] = replacementData;
            }
        }
        return data;
    }

    //Array Manipulator
    public int[] RandomizeArrayElements(int[] data)
    {
        return RandomizeArrayElements(data, Random.Range(0, 1000));
    }
}
