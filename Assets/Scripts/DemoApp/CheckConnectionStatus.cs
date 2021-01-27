using UnityEngine;
using Parse.LiveQuery;
using UnityEngine.UI;

namespace Immersal.Samples.DemoApp
{
    [RequireComponent(typeof(Text))]
    public class CheckConnectionStatus : MonoBehaviour
    {
        private ParseLiveQueryClient m_ParseLiveClient;

        private Text m_Text;

        // Start is called before the first frame update
        void Start()
        {
            m_Text = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_ParseLiveClient == null)
            {
                m_ParseLiveClient = ParseManager.Instance.parseLiveClient;
            }

            if (m_ParseLiveClient != null)
            {
                m_Text.text = m_ParseLiveClient.IsConnected() ? "Connected" : "Disconnected";
            }
        }
    }
}
