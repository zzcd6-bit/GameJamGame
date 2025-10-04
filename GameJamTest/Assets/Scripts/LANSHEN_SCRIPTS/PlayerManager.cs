using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
 * Created By LANSHEN
 * 2025-10-4
 * This is for managing player behavior in the scene.
 */
namespace LANSHEN_SCRIPTS
{
    public class PlayerManager : MonoBehaviour
    {
        private float _g = 10f;
        public static GameObject instance = null;
        //用于检测是否碰撞到阴影
        private List<GameObject> _detector;
        private Vector3[] offset =
        {
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, -1f, 0f),
            new Vector3(-1f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
        };
        //记录初始位置。
        private Vector3 _position;
        //记录运动方向
        private Vector3 speed;
        void Start()
        {
            speed = Vector3.zero;
            if (instance != null) return;
            instance = gameObject;
            for (var i = 0; i < 4; i++)
            {
                var obj  = new GameObject
                {
                    transform =
                    {
                        parent = instance.transform,
                        position = instance.transform.position+(offset[i]*instance.transform.GetComponent<Image>().sprite.bounds.size.x),
                    },
                    name = $"detector{i}"
                };
            }
            var hits = Physics.RaycastAll(transform.position, transform.forward, Mathf.Infinity);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == gameObject) continue;
                transform.position = hit.point;
                break;
            }
            DontDestroyOnLoad(instance);
        }
        void Update()
        {
            speed.y+=_g*Time.deltaTime;
        }

        void CheckMove()
        {
            
        }
    }
}
