using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllControl : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public class GameManager
    {
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new GameManager();
                }
                return _instance;
            }
        }

        public int cherries = 0;
        public int strawberries = 0;
        public int apples = 0;

    }
}
