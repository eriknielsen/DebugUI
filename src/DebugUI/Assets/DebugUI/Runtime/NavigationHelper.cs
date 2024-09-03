using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DebugUI
{
    public static class NavigationHelper
    {
        public static void GoToParentFoldout(this VisualElement target, NavigationCancelEvent evt)
        {
            void Recurse(VisualElement ve)
            {
                if (ve is Foldout foldout || ve.parent == null)
                {
                    var toggle = ve.Q<Toggle>();
                    toggle.value = false;
                    toggle.Focus();
                }
                else
                {
                    Recurse(ve.parent);
                }
            }

            Recurse(target.parent);
            evt.StopPropagation();
        }
    }
}
