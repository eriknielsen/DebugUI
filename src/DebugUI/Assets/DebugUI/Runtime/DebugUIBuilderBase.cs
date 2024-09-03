using UnityEngine;
using UnityEngine.UIElements;

namespace DebugUI
{
    public abstract class DebugUIBuilderBase : MonoBehaviour
    {
        [SerializeField] UIDocument uiDocument;

        VisualElement selection;
        NavigationHelper navigationHelper;

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

            selection = uiDocument.rootVisualElement.Q<Toggle>();
            selection.Focus();

            navigationHelper = new NavigationHelper(uiDocument.rootVisualElement.Query(name: "unity-content-container").Build().First());
        }
        void OnKeyDown(KeyDownEvent ev)
        {
        }
        private void OnNavSubmitEvent(NavigationSubmitEvent evt)
        {
            Debug.Log($"OnNavSubmitEvent {evt.propagationPhase}");
        }

        private void OnNavMoveEvent(NavigationMoveEvent evt)
        {
            Debug.Log($"OnNavMoveEvent {evt.propagationPhase} - move {evt.move} - direction {evt.direction} - target {evt.target}");

            selection = navigationHelper.Navigate(selection, evt);
        }

        private void OnNavCancelEvent(NavigationCancelEvent evt)
        {
            SetVisible(false);
        }
        public void SetVisible(bool isVisible)
        {
            uiDocument.rootVisualElement.visible = isVisible;
            selection = uiDocument.rootVisualElement.Q<Toggle>();
            selection.Focus();
        }

        public bool GetVisible() => uiDocument.rootVisualElement.visible;
    }
}