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

        void Start()
        {
            StartCoroutine(LoadSceneAdditive(initialSceneName));
        }

        public void LoadResultScene()
        {
            StartCoroutine(LoadSceneAdditive(resultSceneName));
        }

        private IEnumerator LoadSceneAdditive(string sceneName)
        {
            var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

            yield return op;

            // UI シーンのカメラを削除し、Canvas のカメラを差し替える
            var scene = SceneManager.GetSceneByName(sceneName);
            var rootObjects = scene.GetRootGameObjects();

            var canvasObj = rootObjects
                .FirstOrDefault(obj => obj.TryGetComponent(out Canvas canvas));

            var uiCanvas = canvasObj.GetComponent<Canvas>();
            var cameraToDestroy = uiCanvas.worldCamera;
            uiCanvas.worldCamera = mainCamera;

            Destroy(cameraToDestroy.gameObject);

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
