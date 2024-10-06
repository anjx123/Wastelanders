using UnityEngine;

namespace SceneBuilder
{
    public abstract class SceneBuilder : MonoBehaviour
    {
        private void Awake() => Build();

        protected abstract void Build();
    }
}