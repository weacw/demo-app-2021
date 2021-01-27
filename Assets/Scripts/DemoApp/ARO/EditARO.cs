using UnityEngine;

namespace Immersal.Samples.DemoApp.ARO
{
    [RequireComponent(typeof(ARODataHandler), typeof(Collider))]
    public class EditARO : MonoBehaviour
    {
        [SerializeField]
        private float m_ClickHoldTime = 1f;
        private float m_timeHold = 0f;

        private Transform m_CameraTransform;
        private float m_MovePlaneDistance;

        private bool m_MovingARO = false;
        private ARODataHandler m_ARODataHandler = null;
        private Collider m_Collider = null;

        private Vector3 originalPos = Vector3.zero;

        [SerializeField]
        private float doubleTapMaxTimeBetween = 0.5f;
        private bool touched = false;
        private float touchTime = 0f;
        

        private void Start()
        {
            m_CameraTransform = Camera.main.transform;
            m_ARODataHandler = GetComponent<ARODataHandler>();
            m_Collider = GetComponent<Collider>();
        }
        private void Update()
        {
            // reset if we got locked while moving
            if (m_ARODataHandler.IsLocked())
            {
                m_MovingARO = false;
                transform.position = originalPos;
            }

            if (m_MovingARO)
            {
                Vector3 projection = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_MovePlaneDistance));
                transform.position = projection;
            }
        }

        private void Reset()
        {
            m_timeHold = 0f;
            m_MovingARO = false;
        }

        private void OnMouseDrag()
        {
            ContentPlacementManager.isEditingARO = true;
            
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider == m_Collider)
                {
                    m_timeHold += Time.deltaTime;
                    touched = true;
                }
                else
                {
                    m_timeHold = 0f;
                }
            }

            if (m_timeHold >= m_ClickHoldTime && !m_MovingARO)
            {
                m_MovePlaneDistance = Vector3.Dot(transform.position - m_CameraTransform.position, m_CameraTransform.forward) / m_CameraTransform.forward.sqrMagnitude;
                m_MovingARO = true;
                m_timeHold = 0f;
                originalPos = transform.position;
            }
        }

        private void OnMouseUp()
        {
            if (touched)
            {
                if (Time.time - touchTime <= doubleTapMaxTimeBetween)
                {
                    m_ARODataHandler.TryToRemoveARO();
                }
                
                touchTime = Time.time;
                touched = false;
            }
            
            if (m_MovingARO && !m_ARODataHandler.IsLocked())
            {
                Vector3 projection = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, m_MovePlaneDistance));
                transform.position = projection;
                m_ARODataHandler.MoveTo(new Pose(transform.position, transform.rotation));
            }
            else if (m_MovingARO)
            {
                transform.position = originalPos;
            }

            m_MovingARO = false;
            m_timeHold = 0f;

            ContentPlacementManager.isEditingARO = false;
        }
    }
}
