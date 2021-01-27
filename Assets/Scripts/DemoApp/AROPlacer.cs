using UnityEngine;

namespace Immersal.Samples.DemoApp
{
    public class AROPlacer : MonoBehaviour
    {
        [SerializeField]
        private bool useGazer = true;

        [SerializeField]
        private Gazer gazer;

        [SerializeField]
        private Vector3 nonGazeOffset = new Vector3(0, 0, 1.5f);

        [SerializeField]
        private GameObject[] ghosts;

        [SerializeField]
        private GameObject currentGhost;

        [SerializeField]
        private Canvas canvas;

        public System.Action<string, Pose> PlacementCompleted;

        public enum AROPlacerState
        {
            Off,
            Placing
        };

        private AROPlacerState currentState = AROPlacerState.Off;
        public AROPlacerState CurrentState { get => currentState; }

        private string currentAROuid = null;

        public void Start()
        {
            if (useGazer && gazer == null)
            {
                Debug.LogError("[AROPlacer] Set Gazer in the inspector!");
                gameObject.SetActive(false);
            }         

            if (currentGhost == null)
                currentGhost = transform.Find("TicketGhost").gameObject;

            if (canvas == null)
                canvas = GetComponentInChildren<Canvas>() as Canvas;

            Reset();
        }

        public void Reset()
        {
            currentState = AROPlacerState.Off;
            currentAROuid = null;
            currentGhost.SetActive(false);
            canvas.gameObject.SetActive(false);
        }

        public void StartPlacing(string aroUid, int ghostIndex = 0)
        {
            currentState = AROPlacerState.Placing;
            currentAROuid = aroUid;

            if (useGazer)
                gazer?.PrepareTargeting();

            SetupGhost(ghostIndex);
            SetupCanvas();
        }

        private void Update()
        {
            if (useGazer && currentState == AROPlacerState.Placing)
            {
                Pose gazeResult = default;

                if (gazer.Gaze(out gazeResult))
                {
                    currentGhost.transform.position = gazeResult.position;
                    currentGhost.transform.rotation = gazeResult.rotation;
                }
                else
                {
                    currentGhost.transform.localPosition = nonGazeOffset;
                    currentGhost.transform.localRotation = Quaternion.identity;
                }
            }
        }

        public void FinishPlacement()
        {
            PlacementCompleted?.Invoke(currentAROuid, new Pose(currentGhost.transform.position, currentGhost.transform.rotation));
            Reset();
        }

        private void SetupGhost(int ghostIndex = 0)
        {
            currentGhost = ghosts[ghostIndex];

            currentGhost.transform.SetParent(Camera.main.transform, false);
            currentGhost.transform.localPosition = nonGazeOffset;
            currentGhost.transform.localRotation = Quaternion.identity;
            currentGhost.SetActive(true);
        }

        private void SetupCanvas()
        {
            canvas.gameObject.SetActive(true);
        }
    }
}
