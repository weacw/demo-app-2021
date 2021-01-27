using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

namespace Immersal.Util
{
	public class RemoteTexture: MonoBehaviour {

		public string url;// = "http://tech.velmont.net/files/2009/04/lenna-lg.jpg";
		public Renderer[] targets;
		public bool shouldLoadRemoteTexture = true;

		private Vector3 origScale = Vector3.one;

		void Start()
		{
			origScale = gameObject.transform.localScale;
		}

		void Update() {
			if (shouldLoadRemoteTexture) {
				shouldLoadRemoteTexture = false;
				if (url.Length > 0)
					LoadTexture(url, this);
			}
		}

		public class RemoteTextureLoader : MonoBehaviour
		{
			private static RemoteTextureLoader theInstance = null;

			public static RemoteTextureLoader Instance {
				get {
					if (theInstance == null) {
						GameObject go = new GameObject("_RemoteTextureLoader");
						theInstance = go.AddComponent<RemoteTextureLoader>();
					}
					return theInstance;
				}
			}
		}

		public class CachedTexture
		{
			public string url;
			public Texture2D tex = null;
			public bool isLoaded = false;
			public List<RemoteTexture> remoteTextures = new List<RemoteTexture>();
		}

		private static Dictionary<string, CachedTexture> s_TexturePool = new Dictionary<string, CachedTexture>();

		public void LoadTexture(string url, RemoteTexture component)
		{
			if (!s_TexturePool.ContainsKey(url)) {
				Debug.Log("load texture[new]: " + url);
				CachedTexture p = new CachedTexture();
				p.url = url;
				p.remoteTextures.Add(this);
				s_TexturePool[url] = p;
				StartCoroutine(GetTexture(p));
			} else {
				CachedTexture p = s_TexturePool[url];
				p.remoteTextures.Add(this);
				if (p.isLoaded)
				{
					Debug.Log("load texture[loaded]: " + url);
					UpdateMaterials(p);
				} else
				{
					Debug.Log("load texture[pending]: " + url);
				}
			}
		}

		public void UpdateMaterials(CachedTexture p) {
			if (targets.Length > 0) {
				foreach (Renderer r in targets)
				{
					r.materials[0].mainTexture = p.tex;
				}
			}
			else {
				gameObject.GetComponent<Renderer>().materials[0].mainTexture = p.tex;
			}

			Vector3 newScale = new Vector3(origScale.x, origScale.y, origScale.z * ((float)p.tex.height / (float)p.tex.width));
			gameObject.transform.localScale = newScale;
		}

		IEnumerator GetTexture(CachedTexture item)
		{
            using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(item.url, true))
            {
                yield return request.SendWebRequest();

                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
					item.tex = DownloadHandlerTexture.GetContent(request);
					foreach (RemoteTexture r in item.remoteTextures)
						r.UpdateMaterials(item);
					item.isLoaded = true;
					Debug.Log("loaded(" + item.tex.format + ") " + item.tex.width + "x" + item.tex.height + " " + item.url);
                }
            }
		}
	}
}
