using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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
        public bool freezed = false;
        private float _g = 10f;
        public static PlayerManager instance = null;
        //用于检测是否碰撞到阴影
        private List<GameObject> _detector = new List<GameObject>();
        private readonly Vector3[] _offset =
        {
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, -1f, 0f),
            new Vector3(-1f, 0f, 0f),
            new Vector3(1f, 0f, 0f),
        };
        //记录初始位置。
        private Vector3 _position;
        //记录运动方向
        private Vector3 _speed;

        [Header("跳跃高度")]
        public float jumpHeight = 1f;
        
        [Header("水平移动速度")]
        public float speed = 5f;
        float getJumpSpeed()
        {
            return Mathf.Sqrt(2f * jumpHeight *_g);
        }
        
        Animator amimator;
        void OnEnable()
        {
            _speed = Vector3.zero;
            img = gameObject.GetComponent<Image>();
            amimator = gameObject.GetComponent<Animator>();
            if (instance != null) return;
            instance = this;
            for (var i = 0; i < 4; i++)
            {
                //var s = instance.transform.GetComponent<Image>().sprite.bounds.size;
                //s /= 80;
                var s = instance.GetComponent<RectTransform>().sizeDelta;
                s /= 2;
                var obj  = new GameObject
                {
                    
                    transform =
                    {
                        parent = instance.transform,
                        position = instance.transform.position+new Vector3(s.x*_offset[i].x,s.y*_offset[i].y,0f),
                    },
                    name = $"{i}"
                };
                _detector.Add(obj);
            }
            var hits = Physics.RaycastAll(transform.position, transform.forward, Mathf.Infinity);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == gameObject) continue;
                transform.position = hit.point;
                break;
            }
            //DontDestroyOnLoad(instance);
        }

        private Image img;
        void Update()
        {
            if (freezed)
            {
                return;
            }
            if (special != 0)
            {
                return;
            }
            _speed.x = 0;
            if (Keyboard.current != null && Keyboard.current.upArrowKey.wasPressedThisFrame)
            {
                if (_speed.y == 0)
                {
                    _speed.y = getJumpSpeed();
                    //Debug.Log(_speed);
                }
            }
            _speed.y-=_g*Time.deltaTime;
            if (Keyboard.current != null && Keyboard.current.leftArrowKey.isPressed)
            {
                _speed.x = -speed;
            }
            else if (Keyboard.current != null && Keyboard.current.rightArrowKey.isPressed)
            {
                _speed.x = speed;
            }
            var allow = CheckMove();
            /*if (allow == AllowMove.All)
            {
                
            }
            else if (allow == AllowMove.None)
            {
                _speed = Vector3.zero;
            }
            else if(allow == AllowMove.X)
            {
                _speed.y = 0;
            }
            else if(allow == AllowMove.Y)
            {
                _speed.x = 0;
            }*/

            var a = amimator.GetCurrentAnimatorClipInfo(0)[0].clip;
            if (_speed.y != 0)
            {
                if (a.name != "jump")
                {
                    amimator.Play("jump");
                }
            }
            else if (_speed.x != 0)
            {
                if (a.name != "walk")
                {
                    amimator.Play("walk");
                }
                
            }
            else
            {
                if (a.name != "idle")
                {
                    amimator.Play("idle");
                }
            }
            if (_speed.x > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if (_speed.x < 0)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            transform.position += _speed*Time.deltaTime;
        }

        

        private void LateUpdate()
        {
            if (freezed)
            {
                return;
            }
            if (!LightManager.instance.InArea())
            {
                return;
            }
            int i = 0;
            foreach (var detector in _detector)
            {
                var sPos = detector.transform.position;
                var dir = LightManager.instance.transform.position - sPos;
                var dist = dir.magnitude;
                dir.Normalize();
                Debug.DrawRay(sPos, dir, Color.red);
                if(Physics.Raycast(sPos, dir, out RaycastHit hit, dist))
                {
                    int j = 0;
                    var index = int.Parse(detector.name);
                    switch (index)
                    {
                        case 0:
                        {
                            j = 1;
                            break;
                        }
                        case 1:
                        {
                            j = 0;
                            break;
                        }
                        case 2:
                        {
                            j = 3;
                            break;
                        }
                        case 3:
                        {
                            j = 2;
                            break;
                        }
                    }
                    transform.position = _detector[j].transform.position;
                    return;
                }
                
            }
        }

        enum AllowMove
        {
            All,
            X,
            Y,
            None
        }
        AllowMove CheckMove()
        {
            if (transform.position.y < 0&&_speed.y<0)
            {
                _speed.y = 0f;
            }

            foreach (var detector in _detector)
            {
                if (detector.transform.position.y < 0&&_speed.y<0)
                {
                    _speed.y = 0f;
                }
            }

            for (int t = 0; t < 3; t++)
            {
                var s = _speed;
                switch (t)
                {
                    case 1:
                        s.y = 0;
                        break;
                    case 2:
                        s.x = 0;
                        break;
                }
                var flag = false;
                foreach (var detector in _detector)
                {
                    var sPos = detector.transform.position;
                    sPos+=s*Time.deltaTime;
                    var dir = LightManager.instance.transform.position - sPos;
                    var dist = dir.magnitude;
                    dir.Normalize();
                    Debug.DrawRay(sPos, dir, Color.red);
                    if (!LightManager.instance.InArea(sPos))
                    {
                        flag = true;
                    }
                    
                    if(Physics.Raycast(sPos, dir, out RaycastHit hit, dist))
                    {
                        var interaction = hit.collider.GetComponent<InteractionBaseObject>();
                        if (interaction != null)
                        {
                            interaction.Interaction();
                        }
                        flag = true;
                    }
                }
                if (flag == false)
                {
                    _speed = s;
                    return (AllowMove)t;
                }
            }
            _speed = Vector3.zero;
            return AllowMove.None;
        }
        private void OnDisable()
        {
            instance = null;
        }

        private int special = 0;

        public float PlaySpecial(string name)
        {
            special = 1;
            amimator.Play(name);
            var time = 1.1f;
            special = 1;
            Invoke(nameof(wait),time);
            return time;
        }

        void wait()
        {
            special = 0;
        }

    }
    
    
}
