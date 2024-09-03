using UnityEngine;
using UnityEngine.UIElements;

namespace DebugUI
{
    public abstract class DebugUIBuilderBase : MonoBehaviour
    {
        [SerializeField] UIDocument uiDocument;

        protected abstract void Configure(IDebugUIBuilder builder);

        protected virtual void Awake()
        {
            var builder = new DebugUIBuilder();
            builder.ConfigureWindowOptions(options =>
            {
                options.Title = GetType().Name;
            });

            Configure(builder);
            builder.BuildWith(uiDocument);

            uiDocument.rootVisualElement.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
            uiDocument.rootVisualElement.RegisterCallback<NavigationCancelEvent>(OnNavCancelEvent);
            uiDocument.rootVisualElement.RegisterCallback<NavigationMoveEvent>(OnNavMoveEvent);
            uiDocument.rootVisualElement.RegisterCallback<NavigationSubmitEvent>(OnNavSubmitEvent);
        }
        void OnKeyDown(KeyDownEvent ev)
        {
            Debug.Log("KeyDown:" + ev.keyCode);
            Debug.Log("KeyDown:" + ev.character);
            Debug.Log("KeyDown:" + ev.modifiers);
        }
        private void OnNavSubmitEvent(NavigationSubmitEvent evt)
        {
            Debug.Log($"OnNavSubmitEvent {evt.propagationPhase}");
        }

        private void OnNavMoveEvent(NavigationMoveEvent evt)
        {
            Debug.Log($"OnNavMoveEvent {evt.propagationPhase} - move {evt.move} - direction {evt.direction}");
        }

        private void OnNavCancelEvent(NavigationCancelEvent evt)
        {
            SetVisible(false);
        }
        public void SetVisible(bool isVisible)
        {
            uiDocument.rootVisualElement.visible = isVisible;
        }

        public bool GetVisible() => uiDocument.rootVisualElement.visible;
    }
}