using System;
using System.Collections;
using System.Collections.Generic;
using LANSHEN_SCRIPTS;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScript : MonoBehaviour
{
    [Serializable]
    public class Data
    {
        //相机跟踪的物体
        public GameObject target;
        //跟踪到物体时展示信息
        public GameObject UI;
        //相机偏移量
        public Vector3 offset;
        //相机类型
        public DataType dataType;

        
        public enum DataType
        {
            Perspective,
            Orthographic
        }
        
    }
    
    [SerializeField]
    public List<Data> data;

    
    private Camera cam;
    private Coroutine coroutine;
    private bool guide = false;
    IEnumerator CameraRoutineAnimation()
    {
        PlayerManager.instance.freezed = true;
        LightManager.instance.freezed = true;
        for (int i = 0; i < data.Count;i++)
        {
            //先移动相机
            var d = data[i];
            var t = GetObjectBoundsCenter(d.target);
            var pos = t + d.offset;
            var rot = Quaternion.LookRotation(-d.offset);
            var rotation = transform.rotation;
            var position = transform.position;
            float a = 0;
            cam.orthographic = d.dataType==Data.DataType.Orthographic;
            //插值
            while (a < 1)
            {
                a+=Time.deltaTime*2;
                a = Mathf.Clamp01(a);
                transform.rotation = Quaternion.Slerp(rotation, rot, a);
                transform.position = Vector3.Lerp(position, pos, a);
                yield return null;
            }
            yield return null;
            //再展示UI
            if (d.UI != null)
            {
               d.UI?.SetActive(true); 
            }
            
            yield return null;
            //循环等待按键触发
            while (true)
            {
                if ((Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
                    ||(Mouse.current != null&&Mouse.current.leftButton.wasPressedThisFrame))
                {
                    if (d.UI != null)
                    {
                        d.UI?.SetActive(false); 
                    }
                    break;
                }
                yield return null;
            }
        }
        guide = true;
        coroutine = null;
        StartCoroutine(EnterPlayMode());
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        coroutine = StartCoroutine(CameraRoutineAnimation());
    }

    public void SkipGuide()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }
        foreach (var d in data)
        {
            if (d.UI != null)
            {
                d.UI?.SetActive(false);
            }
            var t = GetObjectBoundsCenter(d.target);
            var pos = t + d.offset;
            var rot = Quaternion.LookRotation(-d.offset);
            cam.orthographic = d.dataType==Data.DataType.Orthographic;
            transform.position = pos;
            transform.rotation = rot;
        }
        guide = true;
        StartCoroutine(EnterPlayMode());
    }

    //墙的位置
    public GameObject wall;
    //进入游戏阶段
    IEnumerator EnterPlayMode()
    {
        guide = false;
        var pos = transform.position;
        var offset = new Vector3(0f, 1.7f, -8f);
        var dest = wall.transform.position+offset;
        var rot = transform.rotation;
        var destination = Quaternion.LookRotation(new Vector3(0,0,1));
        //if (data.Count > 0)
        {
            var timer = 0f;
            var duration = 0.2f;
            while (true)
            {
                timer += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, timer / duration); // 使用SmoothStep实现平滑
                cam.orthographic = false; // 过渡期间强制为透视
                cam.fieldOfView = Mathf.Lerp(60, 75, t);
                transform.position = Vector3.Lerp(pos, dest, t);
                transform.rotation = Quaternion.Slerp(rot, destination, t);
                if (t >= 1)
                {
                    break;
                }
                yield return null;
            }
            //cam.orthographic = true;
        }
        PlayerManager.instance.freezed = false;
        LightManager.instance.freezed = false;
        //else
        {
            yield return null;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (guide == false)
        {
            return;
        }
        /*if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
        {
            StartCoroutine(EnterPlayMode());
        }*/
    }

    private void LateUpdate()
    {
        if (guide == true)
        {
            return;
        }
        if (coroutine == null&&PlayerManager.instance)
        {
            transform.position = new Vector3(PlayerManager.instance.transform.position.x,transform.position.y,transform.position.z);
        }
    }


    public static Vector3 GetObjectBoundsCenter(GameObject obj)
    {
        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return obj.transform.position;
        }
        Bounds totalBounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            totalBounds.Encapsulate(renderer.bounds);
        }
        return totalBounds.center;
    }
}
