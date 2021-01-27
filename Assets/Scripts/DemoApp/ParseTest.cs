using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Immersal.Samples.DemoApp;
using Immersal.Samples.DemoApp.ARO;
using Parse;
using Parse.LiveQuery;

public class ParseTest : MonoBehaviour
{
    private ParseClient m_ParseClient;
    private ParseLiveQueryClient m_ParseLiveClient;
    private RealtimeQuery<ParseObject> m_RealtimeQuery;
    private ParseObject m_CurrentScene;
    private ParseManager m_ParseManager;
    private AROManager m_AROManager;

    // Start is called before the first frame update
    void Start()
    {
        m_ParseManager = ParseManager.Instance;
        m_AROManager = AROManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void OnConnect()
    {
        Debug.Log("Try auth...");
        ParseUser user = await ParseManager.Instance.AuthUser();
        
        m_ParseClient = m_ParseManager.parseClient;
        m_ParseLiveClient = m_ParseManager.parseLiveClient;
        Debug.Log(m_ParseClient);
        Debug.Log(m_ParseLiveClient);

        int serverMapId = 12455;
        m_CurrentScene = await m_AROManager.GetSceneByMapId(serverMapId);

        if (m_CurrentScene == null)
        {
            m_CurrentScene = await m_AROManager.AddScene(serverMapId);
        }
        Debug.Log("currentScene: " + m_CurrentScene.ObjectId);

        StartRealtimeQuery();
    }

    public void StartRealtimeQuery()
    {
        ParseQuery<ParseObject> query = m_ParseClient.GetQuery("ARO").WhereEqualTo("sceneId", m_CurrentScene.ObjectId);

        m_RealtimeQuery = new RealtimeQuery<ParseObject>(query, slowAndSafe: true);
        /*m_RealtimeQuery.OnCreate += ARO_OnCreate;
        m_RealtimeQuery.OnDelete += ARO_OnDelete;
        m_RealtimeQuery.OnEnter += ARO_OnEnter;
        m_RealtimeQuery.OnLeave += ARO_OnLeave;
        m_RealtimeQuery.OnUpdate += ARO_OnUpdate;*/
    }
}
