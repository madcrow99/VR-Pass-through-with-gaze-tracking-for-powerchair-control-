using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Oculus.SampleFramework.Usage.Passthrough.Scripts
{

    [RequireComponent(typeof(LineRenderer))]
    public class EyeTrackingRay : MonoBehaviour
    {
        [SerializeField]
        private float rayDistance = 0.25f;

        [SerializeField]
        private float rayWidth = 0.01f;

        [SerializeField]
        private LayerMask layersToInclude;

        [SerializeField]
        private Color rayColorDefaultState = Color.yellow;

        [SerializeField]
        private Color rayColorHoverState = Color.red;

        private LineRenderer lineRenderer;

        private List<EyeInteractable> eyeInteractables = new List<EyeInteractable>();

        // Start is called before the first frame update
        void Start()
        {
            lineRenderer = GetComponent<LineRenderer>();
            SetupRay();
        }

        private void SetupRay()
        {
            lineRenderer.useWorldSpace = false;
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = rayWidth;
            lineRenderer.endWidth = rayWidth;
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, transform.position.z + rayDistance));
        }

        void FixedUpdate()
        {
            RaycastHit hit;
            Vector3 rayDirection = transform.TransformDirection(Vector3.forward) * rayDistance;

            //has the interactable froze
            if (Time.time - PowerChairClient.Instance.restartTimer > PowerChairClient.Instance.restartTimeLimit)
            {
               // SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            // Does the ray intersect any objects excluding the player layer
            //if (Physics.Raycast(transform.position, rayDirection, out hit, Mathf.Infinity, layersToInclude))
            if (Physics.Raycast(transform.position, rayDirection, out hit, 1.1f, layersToInclude))
            {
                UnSelect();
                lineRenderer.startColor = rayColorHoverState;
                lineRenderer.endColor = rayColorHoverState;
                var eyeInteractable = hit.transform.GetComponent<EyeInteractable>();
                eyeInteractables.Add(eyeInteractable);
                eyeInteractable.IsHovered = true;
                //reset the scene crash timer
                PowerChairClient.Instance.restartTimer = Time.time;
                
            }
            else
            {
                lineRenderer.startColor = rayColorDefaultState;
                lineRenderer.endColor = rayColorDefaultState;
                
                UnSelect(true);
            }
        }

        private void UnSelect(bool clear = false)
        {
            foreach (var interactable in eyeInteractables)
            {
                interactable.IsHovered = false;
            }
            if (clear)
                eyeInteractables.Clear();
        }
    }
}
