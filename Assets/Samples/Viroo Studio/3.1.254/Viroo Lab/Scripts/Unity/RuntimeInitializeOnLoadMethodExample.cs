#nullable enable
using UnityEngine;
using VirooLab.Actions;

namespace VirooLab
{
    public class RuntimeInitializeOnLoadMethodExample : MonoBehaviour
    {
        private static bool s_executeAction;

        [RuntimeInitializeOnLoadMethod]
        internal static void TestMethod()
        {
            s_executeAction = true;
        }

#pragma warning disable CA1822 // Mark members as static
#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        protected void Start()
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
#pragma warning restore CA1822 // Mark members as static
        {
            if (s_executeAction)
            {
                RandomColorAction randomColorAction = GameObject.Find("RandomColorAction").GetComponent<RandomColorAction>();
                randomColorAction.LocalExecute(string.Empty);
            }
        }

        protected void OnDrawGizmos()
        {
            gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
    }
}
