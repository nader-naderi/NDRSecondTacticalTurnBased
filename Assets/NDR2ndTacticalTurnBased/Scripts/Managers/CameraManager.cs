using UnityEngine;
using System.Collections;

namespace NDR2ndTTB
{
    public class CameraManager : MonoBehaviour
    {

        #region Singleton
        public static CameraManager instance;
        private void Awake()
        {
            instance = this;
        }

        #endregion

        

        [SerializeField]
        float speed = 0.1f;
        [SerializeField]
        float rotateSpeed = 5;

        public void Init ()
        {
            Vector3 targetPos = GridBase.instance.GetWorldCoordinatesFromNode(8, 10, -3);
            transform.position = targetPos;
        }

        private void Update()
        {
            Movement();
            Rotation();
        }

        private void Rotation()
        {
            bool rotateLeft = Input.GetKey(KeyCode.Q);
            bool rotateRight = Input.GetKey(KeyCode.E);

            if (rotateLeft || rotateRight)
            {
                float value = rotateSpeed;
                if (rotateLeft)
                    value = -value;

                Vector3 euler = transform.localEulerAngles;
                euler.y += value * Time.deltaTime;
                transform.localEulerAngles = euler;

            }
        }

        private void Movement()
        {
            float hor = Input.GetAxis("Horizontal");
            float ver = Input.GetAxis("Vertical");

            Vector3 targetPosition = Vector3.zero;

            if (hor != 0)
                targetPosition += hor * transform.right;
            if (ver != 0)
                targetPosition += ver * transform.forward;

            transform.position += targetPosition * speed * Time.deltaTime;
        }
    }
}