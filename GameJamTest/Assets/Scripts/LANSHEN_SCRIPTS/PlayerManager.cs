using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEditor;
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
        
        private AudioSource _audioSource;
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
        private bool IsTrigger = false;
        void OnEnable()
        {
            _speed = Vector3.zero;
            img = gameObject.GetComponent<Image>();
            amimator = gameObject.GetComponent<Animator>();
            if (instance != null) return;
            _audioSource = gameObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = true;
            _audioSource.loop = true;
            _audioSource.clip = Resources.Load<AudioClip>("audio/Walk");
            _audioSource.Stop();
            AudioManager.PlayBGM("BGM");
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
                    AudioManager.PlaySound("Jump");
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
                _audioSource.Stop();
            }
            else if (_speed.x != 0)
            {
                if (a.name != "walk")
                {
                    amimator.Play("walk");
                }

                if (!_audioSource.isPlaying)
                {
                    _audioSource.Play();
                }
                    
            }
            else
            {
                if (a.name != "idle")
                {
                    amimator.Play("idle");
                }
                _audioSource.Stop();
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



        private int GetDirection(int index)
        {
            var j = 0;
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
            return j;
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
            //Debug.Log("late");
            int i = 0;
            List<int> hiting = new List<int>();
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
                    //Debug.Log(hit.collider.gameObject.name);

                    TryTriggerObjectInteraction(hit.collider.gameObject);

                    /*if (_speed == Vector3.zero)
                    {
                        transform.position = _detector[j].transform.position;
                        return;
                    }*/
                    //
                    //transform.position +=_offset[j]*Time.deltaTime;
                    hiting.Add(index);
                    //return;
                }
            }
            //Debug.Log(hiting.Count);
            if (hiting.Count == 1)
            {
                if (_speed == Vector3.zero)
                {
                    transform.position =_detector[GetDirection(hiting[0])].transform.position;
                }
                else
                {
                    transform.position +=_offset[GetDirection(hiting[0])]*Time.deltaTime*speed;
                }
                
            }
            else if (hiting.Count == 2)
            {
                if (hiting[0] + hiting[1] == 1) //上下
                {
                    transform.position +=_offset[2]*Time.deltaTime*speed;
                }
                else if (hiting[0] + hiting[1] == 5) //左右
                {
                    if (_speed == Vector3.zero)
                    {
                        transform.position = _detector[0].transform.position;
                    }
                    else
                    {
                        transform.position +=_offset[0]*Time.deltaTime*speed;
                    }
                    
                }
                else
                {
                    foreach (var item in hiting)
                    {
                        transform.position +=_offset[GetDirection(item)]*Time.deltaTime*speed;
                    }
                }
            }
            else if(hiting.Count >= 3 ||transform.position.y < 0)
            {
                Debug.Log("failure");
                //在这里判断卡住失败？？？
            }
            
            /*if (hiting.Count == 3)
            {
                for (int n = 0; n < 4; n++)
                {
                    if (!hiting.Contains(n))
                    {
                        transform.position = _detector[n].transform.position;
                    }
                }
            }*/
            
        }

        void TryTriggerObjectInteraction(GameObject hitObject)
        {
            var dialogueInteraction = hitObject.GetComponent<DialogueInteractionObject>();
            if (dialogueInteraction != null && IsTrigger == false)
            {
                IsTrigger = true;
                dialogueInteraction.Interaction(); // 调用子类的方法
                return;
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
                    var n = s;
                    int index = int.Parse(detector.name);
                    switch (index)
                    {
                        case 0:
                            n.x = 0;
                            n.y = n.y<0 ? 0 : n.y;
                            break;
                        case 1:
                            n.x = 0;
                            n.y = n.y>0 ? 0 : n.y;
                            break;
                        case 2:
                            n.y = 0;
                            n.x = n.x<0 ? 0 : n.x;
                            break;
                        case 3:
                            n.y = 0;
                            n.x = n.x>0 ? 0 : n.x;
                            break;
                    }
                    var sPos = detector.transform.position;
                    sPos+=n*Time.deltaTime;
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
                        if (t == 2)
                        {
                            //Debug.Log(hit.collider.gameObject.name);
                        }
                        
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
