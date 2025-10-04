using System.Collections;
using UnityEngine;

public class CheckWin : MonoBehaviour
{
    Coroutine routine;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        routine = null;
    }

    
    // Update is called once per frame
    void Update()
    {
        if (routine != null)
        {
            return;
        }
        foreach (Transform sub in gameObject.transform)
        {
            var obj = sub.gameObject;
            var goal = obj.GetComponent<GoalScript>();
            if (goal != null)
            {
                if (goal.hide == false)
                {
                    return;
                }
            }
        }

        if (routine == null)
        {
            StartCoroutine(ShowWin());
        }
    }

    IEnumerator ShowWin()
    {
        yield return null;
        var obj = GameObject.Find("UI");
        foreach (Transform sub in obj.transform)
        {
            if (sub.name == "通关")
            {
                sub.gameObject.SetActive(true);
                yield break;
            }
        }
    }
}
