using UnityEngine;
using System.Collections;

namespace Level
{
    public class Obstacle : MonoBehaviour
    {
        public MeshRenderer mainRenderer;
        private void Awake()
        {
            mainRenderer = GetComponent<MeshRenderer>();
            if (!mainRenderer)
                GetComponentInChildren<MeshRenderer>();
        }
    }
}