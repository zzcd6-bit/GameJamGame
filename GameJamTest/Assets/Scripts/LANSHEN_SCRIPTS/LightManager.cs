using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

/*
 * Created By LANSHEN
 * 2025-10-4
 * This is for managing lights in the scene.
 */
namespace LANSHEN_SCRIPTS
{
    public class LightManager : MonoBehaviour
    {
        private Light _playerLight;
        public float rotationSpeed = 100f;
        public static GameObject instance = null;
        /*
        {
            get
            {
                if (Instance == null)
                {
                    Instance = GameObject.FindGameObjectWithTag("PlayerMainLight");
                }
                return Instance;
            }
            private set => Instance = value;
        }
        */
        void OnEnable()
        {
            if (instance == null)
            {
                instance = gameObject;
                DontDestroyOnLoad(instance);
            }
            _playerLight = instance.GetComponent<Light>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Keyboard.current!=null&&Keyboard.current.aKey.IsPressed())//上移
            {
                instance.transform.Translate(-10*Time.deltaTime,0,0);
            }
            else if (Keyboard.current!=null&&Keyboard.current.dKey.IsPressed())//上移
            {
                instance.transform.Translate(10*Time.deltaTime,0,0);
            }
            else if (Keyboard.current!=null&&Keyboard.current.sKey.IsPressed())//上移
            {
                instance.transform.Translate(0,-10*Time.deltaTime,0);
            }
            else if (Keyboard.current!=null&&Keyboard.current.wKey.IsPressed())//上移
            {
                instance.transform.Translate(0,10*Time.deltaTime,0);
            }
           
        }

        public bool InArea(Vector3 pos)
        {
            var dir = pos - transform.position;
            dir.Normalize();
            var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            return angle < _playerLight.spotAngle;
        }
    }
}
