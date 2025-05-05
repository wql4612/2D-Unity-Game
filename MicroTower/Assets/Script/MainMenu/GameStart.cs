using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

    public class GameStart : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public void OnButtonClick()
        {
            Debug.Log("Button Clicked");
            string sceneName = "Level1"; // 修改为游戏场景的名字
            // 检查是否已存在相同的场景
            Scene existingScene = SceneManager.GetSceneByName(sceneName);
            if (existingScene.IsValid())
            {
                // 如果存在，则卸载重复场景
                SceneManager.UnloadSceneAsync(existingScene);
            }

            // 加载Level1场景
            SceneManager.LoadScene(sceneName, LoadSceneMode.Single);

            // 如果需要返回菜单场景的功能，可以保留原有的OnButtonClick逻辑，并添加新的逻辑
            // 例如，加载Level1场景后，卸载Menu场景
            string menuSceneName = "Menu";
            Scene menuExistingScene = SceneManager.GetSceneByName(menuSceneName);
            if (menuExistingScene.IsValid())
            {
                SceneManager.UnloadSceneAsync(menuExistingScene);
            }
        }
    }
