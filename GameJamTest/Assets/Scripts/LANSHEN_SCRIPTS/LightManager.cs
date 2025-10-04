using System;
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
        public GameObject leftDownAxis;
        public GameObject rightTopAxis;
        public float rotationSpeed = 100f;
        public static LightManager instance = null;
        void OnEnable()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(instance);
            }
            _playerLight = instance.GetComponent<Light>();
        }

        // Update is called once per frame
        void Update()
        {
            var pos = gameObject.transform.position;
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
            ClampPos();
            if (!InArea())
            {
                transform.position = pos;
            }
        }

        private void ClampPos()
        {
            var pos = gameObject.transform.position;
            pos.x = Mathf.Clamp(pos.x, leftDownAxis.transform.position.x, rightTopAxis.transform.position.x);
            pos.y = Mathf.Clamp(pos.y, leftDownAxis.transform.position.y, rightTopAxis.transform.position.y);
            gameObject.transform.position = pos;
        }

        public bool InArea()
        {
            var player = PlayerManager.instance.gameObject;
            foreach (Transform decorator in player.transform)
            {
                var pos = decorator.transform.position;
                var dir = pos - transform.position;
                var angle = Vector3.Angle(dir,transform.forward);
                if (angle <= _playerLight.spotAngle / 2f)
                {
                    return true;
                }
            }
            return false;
        }

        public bool InArea(Vector3 pos)
        {
            var dir = pos - transform.position;
            var angle = Vector3.Angle(dir,transform.forward);
            if (angle <= _playerLight.spotAngle / 2f)
            {
                return true;
            }
            return false;
        }
    }
}
