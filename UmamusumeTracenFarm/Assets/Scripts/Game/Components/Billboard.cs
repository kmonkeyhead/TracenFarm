using UnityEngine;

namespace Game.Components
{
    public sealed class Billboard : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private void Awake()
        {
            if (_camera == null)
            {
                _camera = Camera.main;
            }
        }

        private void LateUpdate()
        {
            if (_camera == null)
            {
                return;
            }

            transform.rotation = _camera.transform.rotation;
        }
    }
}