using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.AssetImporters;
using UnityEngine;
using UnityEngine.UIElements;

namespace DebugUI
{
    public class NavigationHelper
    {
        VisualElement root;
        public NavigationHelper(VisualElement navigationRoot)
        {
            root = navigationRoot;
            if(root == null)
            {
                Debug.LogError("Root can't be null");

            }
        }
        /*
        public void GoToParentFoldout(NavigationCancelEvent evt)
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
        */
        Foldout GetParentFoldout(VisualElement element)
        {
            if(element is Foldout foldout)
            {
                return foldout;
            }
            else
            {
                return GetParentFoldout(element.parent);
            }
        }
        public VisualElement Navigate(VisualElement target, NavigationMoveEvent evt)
        {
            VisualElement FindFoldoutInParent(VisualElement ve, VisualElement ignore = null)
            {
                Debug.Log($"Find FoldoutParent for {ve}, ignore {ignore}");
                if (ve != ignore && (ve is Foldout foldout || ve.parent == null))
                {
                    var toggle = ve.Q<Toggle>();
                    toggle.Focus();
                    Debug.Log($"found foldout {ve} with toggle {toggle}");
                    return toggle;
                }
                else
                {
                    Debug.Log("Go next praent");
                    FindFoldoutInParent(ve.parent, ignore);
                }
                return null;
            }
            VisualElement FindFoldoutInParent2(VisualElement ve, VisualElement ignore = null)
            {
                if (ve != ignore && (ve is Foldout foldout || ve.parent == null))
                {
                    return ve;
                }
                else
                {
                    return FindFoldoutInParent2(ve.parent, ignore);
                }
            }
            VisualElement ChildRecurse(VisualElement ve)
            {
                //Debug.Log($"Is focusable? {ve.focusable} - childCount: {ve.childCount} - {ve}");
                if(ve is Foldout)
                {
                    return ve.Q<Toggle>();
                    
                }
                else if(ve.focusable)
                {
                    return ve;
                }
                else
                {
                    foreach(var child in ve.Children())
                    {
                        return ChildRecurse(child);

                    }
                }
                throw new System.Exception("Aaa found no child wä");
            }
            void ChildRecursePrint(VisualElement ve)
            {
                if (ve.focusable)
                {
                    Debug.Log("Found a focusable");
                }
                else
                {
                    foreach (var child in ve.Children())
                    {
                        ChildRecursePrint(child);
                    }
                }
            }
            int GetChildIndex(VisualElement child, VisualElement parent)
            {
                var children = parent.Children();
                for (int i = 0; i < children.Count(); i++)
                {
                    if (children.ElementAt(i) == child)
                    {
                        return i;
                    }
                    else
                    {
                        return GetChildIndex(child, children.ElementAt(i));
                    }
                }
                return -1;
            }

            bool IsChildOfParent(VisualElement child, VisualElement parent)
            {
                var children = parent.Children();
                for (int i = 0; i < children.Count(); i++)
                {
                    if (children.ElementAt(i) == child)
                    {
                        return true;
                    }
                    else
                    {
                        return IsChildOfParent(child, children.ElementAt(i));
                    }
                }
                return false;
            }

            void PrintHierarchy(VisualElement ve)
            {
                for (int i = 0; i < ve.hierarchy.childCount; i++)
                {
                    PrintHierarchy(ve.hierarchy[i]);
                }
            }
            void FindChild(VisualElement ve, VisualElement searchFor, ref bool foundIt)
            {
                //Debug.Log($"Is {ve} == {searchFor}");
                if(ve == searchFor)
                {
                    foundIt = true;
                }
                for (int i = 0; i < ve.hierarchy.childCount; i++)
                {
                    FindChild(ve.hierarchy[i], searchFor, ref foundIt);
                }
            }
            if (evt.direction == NavigationMoveEvent.Direction.Up)
            {
                bool isFirst = target.parent.Children().First() == target;
                if(target is Toggle && target.parent is Foldout)
                {
                    isFirst = target.parent.parent.Children().First() == target.parent;
                }
                if (isFirst)
                {
                    // if we are the first child, focus the parent instead
                    Debug.Log("omg we are first child, go to parent by recursion");
                    VisualElement newSelection = FindFoldoutInParent(target.parent);
                    evt.PreventDefault();
                    evt.StopImmediatePropagation();
                    if (newSelection == null)
                    {
                        throw new System.Exception("Aa recurse didnt find antyhing");
                    }
                    return newSelection;
                }
                else
                {
                    // go to child below
                    VisualElement parentToLookThrough = target.parent;
                    VisualElement elementToCompareWith = target;
                    if (target is Toggle && target.parent is Foldout)
                    {
                        parentToLookThrough = target.parent.parent;
                        elementToCompareWith = target.parent;
                    }
                    var children = parentToLookThrough.Children();
                    int childIdxToSelect = -1;
                    for (int i = 0; i < children.Count(); i++)
                    {
                        if (elementToCompareWith == children.ElementAt(i))
                        {
                            childIdxToSelect = i - 1;
                        }
                    }
                    if (childIdxToSelect > -1)
                    {
                        VisualElement child = children.ElementAt(childIdxToSelect);
                        Debug.Log("Found child above us to focus");
                        if (child is Foldout)
                        {
                            child = child.Q<Toggle>();
                            child.Focus();
                        }
                        else
                        {
                            child.Focus();
                        }


                        evt.PreventDefault();
                        evt.StopImmediatePropagation();
                        return child;
                    }
                }

            }
            else if (evt.direction == NavigationMoveEvent.Direction.Down)
            {
                bool isLast = target.parent.Children().Last() == target;
                if(target is Toggle)
                {
                    isLast = target.parent.parent.Children().Last() == target.parent;
                }
                if (isLast)
                {
                    // if we are the last child, focus the parent's sibling instead
                    Debug.Log("we are last child, go to parent's sibling by recursion");
                    VisualElement parentWithSiblingToFocus = null;
                    if (target is Toggle && target.parent is Foldout)
                    {
                        parentWithSiblingToFocus = target.parent.parent;//FindFoldoutInParent2(target.parent.parent, ignore: GetParentFoldout(target));
                    }
                    else
                    {
                        parentWithSiblingToFocus = target.parent;// FindFoldoutInParent2(target.parent, ignore: GetParentFoldout(target));
                    }
                    parentWithSiblingToFocus = root;
                    Debug.Log($"parent to change sibling in: {parentWithSiblingToFocus}");
                    // get which index we are in so we can increment it
                    int childIdx = -1;
                    for (int i = 0; i < parentWithSiblingToFocus.childCount; i++)
                    {
                        bool foundIt = false;
                        FindChild(parentWithSiblingToFocus.hierarchy[i], target, ref foundIt);
                        if(foundIt)
                        {
                            childIdx = i;
                            break;
                        }
                        if(IsChildOfParent(target, parentWithSiblingToFocus.Children().ElementAt(i)))
                        {
                            childIdx = i;
                            break;
                        }

                    }
                    Debug.Log(childIdx);
                    VisualElement newSelection = null;
                    if(childIdx > -1)
                    {
                        var childrenInParentWithSiblingToFocus = parentWithSiblingToFocus.Children();
                        childIdx = (childIdx + 1) % childrenInParentWithSiblingToFocus.Count();
                        Debug.Log($"wow we found something: {childIdx}");

                        //newSelection = parentWithSiblingToFocus.Query<Foldout>().AtIndex(childIdx).Q<Toggle>();
                        //newSelection = ChildRecurse(childrenInParentWithSiblingToFocus.ElementAt(childIdx));
                        var child = childrenInParentWithSiblingToFocus.ElementAt(childIdx);
                        if(child is Foldout)
                        {
                            child = child.Q<Toggle>();
                        }
                        newSelection = child;
                        newSelection.Focus();
                    }
                    else
                    {
                        throw new System.Exception("nope didnt find anyhint");
                    }
                    evt.PreventDefault();
                    evt.StopImmediatePropagation();
                    return newSelection;
                }
                else
                {
                    // go to child below
                    VisualElement parentToLookThrough = target.parent;
                    VisualElement elementToCompareWith = target;
                    if (target is Toggle foldoutToggle && target.parent is Foldout && !foldoutToggle.value)
                    {
                        parentToLookThrough = target.parent.parent;
                        elementToCompareWith = target.parent;
                    }
                    var children = parentToLookThrough.Children();
                    int childIdxToSelect = -1;
                    for (int i = 0; i < children.Count(); i++)
                    {
                        if (elementToCompareWith == children.ElementAt(i))
                        {
                            childIdxToSelect = i + 1;
                        }
                    }
                    if (childIdxToSelect > -1 && childIdxToSelect < children.Count())
                    {
                        VisualElement child = children.ElementAt(childIdxToSelect);
                        Debug.Log("Found child below us to focus");
                        if (child is Foldout)
                        {
                            child = child.Q<Toggle>();
                            child.Focus();
                        }
                        else
                        {
                            child.Focus();
                        }


                        evt.PreventDefault();
                        evt.StopImmediatePropagation();
                        return child;
                    }
                    else if(target.childCount > 0)
                    {
                        // if there was no child in the parent, check if we have children to go to

                        VisualElement child = null;

                        foreach (var c in target.parent.Children())
                        {
                            Debug.Log(c);
                        }
                        VisualElement parentToSearchThrough = target;
                        if (target is Toggle && target.parent is Foldout)
                        {
                            // HACK: Since we looking at a toggle, avoid the Toggle's focusable children and instead search through the foldouts other children
                            parentToSearchThrough = target.parent;
                        }


                        foreach (var c in parentToSearchThrough.Children())
                        {
                            ChildRecursePrint(c);
                        }
                        foreach (var c in parentToSearchThrough.Children())
                        {
                            child = ChildRecurse(c);
                            if (child != null)
                            {
                                break;
                            }
                        }
                        Debug.Log("Found our child to focus" + child);
                        if (child is Foldout)
                        {
                            child = child.Q<Toggle>();
                            child.Focus();
                        }
                        else
                        {
                            child.Focus();
                        }


                        evt.PreventDefault();
                        evt.StopImmediatePropagation();
                        return child;
                    }
                }
            }

            throw new System.Exception("no support yet");
        }
    }
}
