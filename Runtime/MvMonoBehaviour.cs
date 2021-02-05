using UnityEngine;

namespace Multiverse
{
    public class MvMonoBehaviour : MonoBehaviour
    {
        protected MvGameObject MvObject { get; private set; }

        private void Awake()
        {
            MvObject = GetComponent<MvGameObject>();
        }
    }
}