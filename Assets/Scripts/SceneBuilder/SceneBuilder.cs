using UnityEngine;

namespace SceneBuilder
{
    public abstract class SceneBuilder : MonoBehaviour
    {
        public void Awake() => Build();

        protected abstract void Build();
    }
}