using System.Collections;
using LANSHEN_SCRIPTS;
using UnityEngine;
using UnityEngine.UI;

public class GoalScript : MonoBehaviour
{
    public bool hide = false;
    public float radius = 0.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var hits = Physics.RaycastAll(transform.position, transform.forward, Mathf.Infinity);
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject == gameObject) continue;
            transform.position = hit.point;
            break;
        }
        radius = gameObject.GetComponent<Image>().sprite.bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (hide)
        {
            return;
        }
        var pos =  PlayerManager.instance.gameObject.transform.position;
        Vector2 ppos = new Vector2(pos.x, pos.y);
        Vector2 gpos = new Vector2(transform.position.x, transform.position.y);
        if (Vector2.Distance(ppos, gpos) < radius)
        {
            hide = true;
            StartCoroutine(SetHide());
        }
    }

    IEnumerator SetHide()
    {
        yield return null;
        gameObject.SetActive(false);
    }
}
