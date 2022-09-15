using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System.Linq;

namespace Unity1Week
{
    public class UISceneLoader : MonoBehaviour
    {
        [SerializeField]
        private string initialSceneName;

        [SerializeField]
        private string resultSceneName;

        [SerializeField]
        private Camera mainCamera;

        static UISceneLoader _instance;

        void Start()
        {
            StartCoroutine(LoadSceneAdditive(initialSceneName));
        }

        void Awake()
        {
            _instance = this;
        }

        void OnDestroy()
        {
            _instance = null;
        }

        public void LoadResultScene()
        {
            // スコア表示などの UI を非表示にする
            SceneManager.UnloadSceneAsync(initialSceneName);

            StartCoroutine(LoadSceneAdditive(resultSceneName));
        }

        public static void LoadUIScene(string sceneNameToLoad)
        {
            if (_instance != null)
            {
                _instance.StartCoroutine(_instance.LoadSceneAdditive(sceneNameToLoad));
            }
        }

        private IEnumerator LoadSceneAdditive(string sceneName)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            yield return op;

            var scene = SceneManager.GetSceneByName(sceneName);
            var rootObjects = scene.GetRootGameObjects();

            // UI シーンのカメラを削除し、Canvas のカメラを差し替える
            {
                HashSet<Camera> camerasToDestory = new HashSet<Camera>();

                var canvasObjects = rootObjects.Where(obj => obj.TryGetComponent(out Canvas canvas));
                foreach (var canvasObj in canvasObjects)
                {
                    var uiCanvas = canvasObj.GetComponent<Canvas>();
                    if (uiCanvas.worldCamera != null)
                    {
                        camerasToDestory.Add(uiCanvas.worldCamera);
                        uiCanvas.worldCamera = mainCamera;
                    }
                }

                foreach (var cam in camerasToDestory)
                {
                    Destroy(cam.gameObject);
                }
            }

            // UI シーンの EventSystem を削除する
            var eventSystemObj = rootObjects
                .FirstOrDefault(obj => obj.TryGetComponent(out EventSystem eventSystem));

            if (eventSystemObj != null)
            {
                Destroy(eventSystemObj);
            }
        }
    }
}
