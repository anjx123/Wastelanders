using UnityEngine;

namespace Initializers
{
    public class SceneInitializer : MonoBehaviour
    {
        [SerializeField] internal GameObject[] managers;

        public void Awake()
        {
            foreach (var manager in managers)
            {
                Instantiate(manager);
            }
        }
    }
}