using UnityEngine;

namespace Immersal.Samples.DemoApp.UI
{
    public class CanvasManager : MonoBehaviour
    {
        [SerializeField]
        private Canvas canvas;
            
        private void Start()
        {
            if (canvas == null)
                canvas = this.GetComponent<Canvas>() as Canvas;

            SetCamera();
        }

        private void SetCamera()
        {
            Camera cam = Camera.main;
            canvas.worldCamera = cam;
        }

        private void PointToCamera()
        {
            if (Camera.main != null)
                transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
                //transform.rotation = Camera.main.transform.rotation;

        }

        private void Update()
        {
            PointToCamera();
        }
    }
}
