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
    
    private Coroutine _coroutine;

    // Update is called once per frame
    void Update()
    {
        if (_coroutine != null)
        {
            return;
        }
        if (hide)
        {
            return;
        }

        if (PlayerManager.instance != null)
        {
            var pos =  PlayerManager.instance.gameObject.transform.position;
            Vector2 ppos = new Vector2(pos.x, pos.y);
            Vector2 gpos = new Vector2(transform.position.x, transform.position.y);
            if (Vector2.Distance(ppos, gpos) < radius)
            {
                _coroutine = StartCoroutine(SetHide());
            }  
        }
        
    }

    IEnumerator SetHide()
    {
        AudioManager.PlaySound("Wipe");
        yield return new WaitForSeconds(PlayerManager.instance.PlaySpecial("collection"));
        Debug.Log(1);
        hide = true;
        gameObject.SetActive(false);
    }
}
