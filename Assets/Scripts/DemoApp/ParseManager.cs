using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Parse;
using Parse.Infrastructure;
using System.IO;
using Parse.LiveQuery;

namespace Immersal.Samples.DemoApp
{
    public class ParseManager : MonoBehaviour
    {
		private static ParseManager instance = null;

        [SerializeField]
        private string m_AppId = "LBJl45CsHxQMqVeSMJlOJdYsC5qOxN5aLLCSk3dh";
//        private string m_AppId = "LBJl45CsHxQMqVeSMJlOJdYsC5qOxN5aLLCSk3dh";
        [SerializeField]
        private string m_Server = "https://immersalar.back4app.io";
  //      private string m_Server = "https://parseapi.back4app.com";
        [SerializeField]
        private string m_DotNetKey = "TigMW2KtuNAH3Uy9WrKqoSxNMBmMG2EdkGPTRBY2";
//        private string m_DotNetKey = "TigMW2KtuNAH3Uy9WrKqoSxNMBmMG2EdkGPTRBY2";
        [SerializeField]
//        private string m_LiveServer = "wss://immersalar.back4app.io";
        private string m_LiveServer = "wss://immersalar.back4app.io";
        [SerializeField]
        private string m_Username = "User";
        [SerializeField]
        private string m_Password = "DemoApp";

        /// <summary>
        /// A reference to the Parse Client
        /// </summary>
        ParseClient m_ParseClient;

        /// <summary>
        /// A reference to the parse Live Client
        /// </summary>
        private ParseLiveQueryClient m_ParseLiveClient;

        public ParseClient parseClient { get { return m_ParseClient; } }
        public ParseLiveQueryClient parseLiveClient { get { return m_ParseLiveClient; } }

        public static ParseManager Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null && !Application.isPlaying)
                {
                    instance = UnityEngine.Object.FindObjectOfType<ParseManager>();
                }
#endif
                if (instance == null)
                {
                    Debug.LogError("No ParseManager instance found. Ensure one exists in the scene.");
                }
                return instance;
            }
        }

        void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            if (instance != this)
            {
                Debug.LogError("There must be only one ParseManager object in a scene.");
                UnityEngine.Object.DestroyImmediate(this);
                return;
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            // Make the normal client
            m_ParseClient = new ParseClient(
                new ServerConnectionData
                {
                    ApplicationID = m_AppId,
                    ServerURI = m_Server,
                    Key = m_DotNetKey, // This is unnecessary if a value for MasterKey is specified.
                    Headers = new Dictionary<string, string>
                    {
                    }
                },
                new LateInitializedMutableServiceHub { },
                new MetadataMutator
                {
                    EnvironmentData = new EnvironmentData { OSVersion = SystemInfo.operatingSystem, Platform = $"Unity {Application.unityVersion} on {SystemInfo.operatingSystemFamily}", TimeZone = System.TimeZoneInfo.Local.StandardName },
                    HostManifestData = new HostManifestData { Name = Application.productName, Identifier = Application.productName, ShortVersion = Application.version, Version = Application.version }
                },
                new AbsoluteCacheLocationMutator
                {
                    CustomAbsoluteCacheFilePath = $"{Application.persistentDataPath.Replace('/', Path.DirectorySeparatorChar)}{Path.DirectorySeparatorChar}Parse.cache"
                }
            );

            // Setup the instance
            m_ParseClient.Publicize();

            // Make the live client
            m_ParseLiveClient = new ParseLiveQueryClient(new System.Uri(m_LiveServer)); //This is the URI to your live server

            // Lets listen to some events for the lifecycle of the Live Query
            m_ParseLiveClient.OnConnected += ParseLiveClient_OnConnected;
            m_ParseLiveClient.OnDisconnected += ParseLiveClient_OnDisconnected;
            m_ParseLiveClient.OnLiveQueryError += ParseLiveClient_OnLiveQueryError;
            m_ParseLiveClient.OnSocketException += ParseLiveClient_OnSocketException;

            // Setup the instance
            m_ParseLiveClient.Publicize();
        }

        /// <summary>
        /// You must disconnect when unity closes
        /// </summary>
        private void OnDestroy()
        {
            if (m_ParseLiveClient != null)
            {
                m_ParseLiveClient.Disconnect();
            }
        }

        private void ParseLiveClient_OnSocketException(System.Exception obj)
        {
            Debug.LogError("ParseLiveClient_OnSocketException");
            Debug.LogException(obj);
        }

        private void ParseLiveClient_OnLiveQueryError(System.Exception obj)
        {
            Debug.LogError("ParseLiveClient_OnLiveQueryError");
            Debug.LogException(obj);
        }

        private void ParseLiveClient_OnDisconnected()
        {
            Debug.Log("ParseLiveClient_OnDisconnected");
        }

        private void ParseLiveClient_OnConnected()
        {
            Debug.Log("ParseLiveClient_OnConnected");
        }
        
        /// <summary>
        /// This creates and Auths a User
        /// </summary>
        public async Task<ParseUser> AuthUser()
        {
            // Create a user, save it, and authenticate with it.
            //await parseClient.SignUpAsync(username: m_Username, password: m_Password);  //You only need to do this once

            // Authenticate the user.
            ParseUser user = await m_ParseClient.LogInAsync(username: m_Username, password: m_Password);
            Debug.Log(m_ParseClient.GetCurrentUser().SessionToken);
            return user;
        }

        public async void LogoutUser()
        {
            await m_ParseClient.LogOutAsync();
        }
    }
}
