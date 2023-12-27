using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using System.Linq;

namespace u1w202312
{
    public class RailroadUISceneLoader : MonoBehaviour
    {
        [SerializeField]
        private string resultSceneName;

        [SerializeField]
        private Camera mainCamera;

        // リザルト画面表示時に非表示にするキャンバス
        [SerializeField]
        private Canvas canvasToHideOnResult;

        [SerializeField]
        private List<GameObject> objectsToHideOnResult;

        private RailroadGameController _controller;

        void Start()
        {
            var go = GameObject.FindGameObjectWithTag("GameController");
            Debug.Assert(go != null);
            go.TryGetComponent<RailroadGameController>(out _controller);
            Debug.Assert(_controller != null);

            // ゲームオーバーでリザルト画面を表示する
            _controller.OnPlayerDied.AddListener(() =>
            {
                if (canvasToHideOnResult != null)
                {
                    canvasToHideOnResult.gameObject.SetActive(false);
                    objectsToHideOnResult.ForEach(obj => obj.SetActive(false));
                }

                LoadResultScene();
            });
        }

        public void LoadResultScene()
        {
            StartCoroutine(LoadSceneAdditive(resultSceneName));
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
