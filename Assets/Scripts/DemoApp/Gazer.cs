using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace Immersal.Samples.DemoApp
{
    [RequireComponent(typeof(ARRaycastManager))]
    [RequireComponent(typeof(ARPlaneManager))]
    public class Gazer : MonoBehaviour
    {
        ARRaycastManager m_RaycastManager;
        ARPlaneManager m_PlaneManager;

        static List<ARRaycastHit> s_Hits = new List<ARRaycastHit>();

        public enum Mode
        {
            Off,
            Targeting,
            Preparing
        };

        private Mode m_CurrentMode = Mode.Preparing;

        public Camera m_TargetingCamera;
        public bool m_EnablePlanesWhenTargeting = true;

        void Awake()
        {
            m_RaycastManager = GetComponent<ARRaycastManager>();
            m_PlaneManager = GetComponent<ARPlaneManager>();
        }

        public bool Gaze(out Pose pose)
        {
            pose = default;

            if (m_CurrentMode == Mode.Targeting)
            {
                Ray gazeRay = m_TargetingCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

                if (m_RaycastManager.Raycast(gazeRay, s_Hits, TrackableType.PlaneWithinPolygon))
                {
                    ARRaycastHit hit = s_Hits[0];
                    pose = hit.pose;
                    ARPlane plane = m_PlaneManager.GetPlane(hit.trackableId);

                    switch (plane.alignment)
                    {
                        case PlaneAlignment.None:
                        case PlaneAlignment.NotAxisAligned:
                            return false;
                        case PlaneAlignment.Vertical:
                            Vector3 forward = pose.position - (pose.position + Vector3.down);
                            pose.rotation = Quaternion.LookRotation(forward, plane.normal);
                            break;
                        default:
                            break;
                    }

                    return true;
                }
            }

            return false;
        }

        bool TryGetTouchPosition(out Vector2 touchPosition)
        {
    #if UNITY_EDITOR
            if (Input.GetMouseButton(0))
            {
                var mousePosition = Input.mousePosition;
                touchPosition = new Vector2(mousePosition.x, mousePosition.y);
                return true;
            }
    #else
            if (Input.touchCount > 0)
            {
                touchPosition = Input.GetTouch(0).position;
                return true;
            }
    #endif

            touchPosition = default;
            return false;
        }

        void LateUpdate()
        {
            if (m_CurrentMode == Mode.Preparing)
                EnableTargeting();
        }
        
        public bool IsOnAPlane(out PlaneAlignment alignment)
        {
            alignment = PlaneAlignment.None;

            if (!TryGetTouchPosition(out Vector2 touchPosition))
                return false;
            
            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                ARRaycastHit hit = s_Hits[0];
                ARPlane plane = m_PlaneManager.GetPlane(hit.trackableId);
                alignment = plane.alignment;
                return true;
            }

            return false;
        }
        
        public bool TryGetVerticalHitPose(out Pose pose)
        {
            if (!TryGetTouchPosition(out Vector2 touchPosition))
            {
                pose = default;
                return false;
            }
            
            if (m_RaycastManager.Raycast(touchPosition, s_Hits, TrackableType.PlaneWithinPolygon))
            {
                ARRaycastHit hit = s_Hits[0];
                Pose hitPose = s_Hits[0].pose;

                ARPlane plane = m_PlaneManager.GetPlane(hit.trackableId);
                if (plane.alignment == PlaneAlignment.Vertical)
                {
                    pose = hit.pose;
                    return true;
                }
            }

            pose = default;
            return false;
        }

        public void PrepareTargeting()
        {
            m_CurrentMode = Mode.Preparing;
        }

        public void EnableTargeting()
        {
            m_CurrentMode = Mode.Targeting;
            if (m_EnablePlanesWhenTargeting)
            {
                //PlaneDetectionController pdc = gameObject.GetComponent<PlaneDetectionController>();
                //pdc?.EnablePlaneDetection();
            }
        }
    }
}
