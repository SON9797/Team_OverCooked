using UnityEngine;

namespace Overcooked
{
    public class PlayerToolVisualController : MonoBehaviour
    {
        [Header("도구 오브젝트")]
        [SerializeField] private GameObject _cleaver;
        [SerializeField] private GameObject _knife;
        [SerializeField] private GameObject _props;

        private void Awake()
        {
            Debug.Log($"[ToolVisual] Awake : {gameObject.name}", this);
            HideAllTools();
        }

        public void HideAllTools()
        {
            Debug.Log($"[ToolVisual] HideAllTools : {gameObject.name}", this);

            SetActiveSafe(_cleaver, false, "Cleaver");
            SetActiveSafe(_knife, false, "Knife");
            SetActiveSafe(_props, false, "Props");
        }

        public void ShowCleaverOnly()
        {
            Debug.Log($"[ToolVisual] ShowCleaverOnly : {gameObject.name}", this);

            SetActiveSafe(_cleaver, true, "Cleaver");
            SetActiveSafe(_knife, false, "Knife");
            SetActiveSafe(_props, false, "Props");
        }

        public void ShowKnifeOnly()
        {
            Debug.Log($"[ToolVisual] ShowKnifeOnly : {gameObject.name}", this);

            SetActiveSafe(_cleaver, false, "Cleaver");
            SetActiveSafe(_knife, true, "Knife");
            SetActiveSafe(_props, false, "Props");
        }

        public void ShowPropsOnly()
        {
            Debug.Log($"[ToolVisual] ShowPropsOnly : {gameObject.name}", this);

            SetActiveSafe(_cleaver, false, "Cleaver");
            SetActiveSafe(_knife, false, "Knife");
            SetActiveSafe(_props, true, "Props");
        }

        private void SetActiveSafe(GameObject target, bool value, string label)
        {
            if (target == null)
            {
                Debug.LogWarning($"[ToolVisual] {label} is null", this);
                return;
            }

            target.SetActive(value);

            Debug.Log(
                $"[ToolVisual] {label} -> {value} / activeSelf:{target.activeSelf} / activeInHierarchy:{target.activeInHierarchy} / target:{target.name}",
                target);
        }
    }
}