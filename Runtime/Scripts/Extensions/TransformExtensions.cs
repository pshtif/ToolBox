/*
 *	Created by:  Peter @sHTiF Stefcek
 */

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace BinaryEgo.ToolBox
{
    public static class TransformExtensions
    {
        public static Vector2 FromToRectTransform(RectTransform p_from, RectTransform p_to)
        {
            Vector2 local;
            Vector2 fromPivotOffset= new Vector2(p_from.rect.width * 0.5f + p_from.rect.xMin, p_from.rect.height * 0.5f + p_from.rect.yMin);
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(null, p_from.position);
            screen += fromPivotOffset;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(p_to, screen, null, out local);
            Vector2 toPivotOffset = new Vector2(p_to.rect.width * 0.5f + p_to.rect.xMin, p_to.rect.height * 0.5f + p_to.rect.yMin);
            return p_to.anchoredPosition + local - toPivotOffset;
        }
        
        public static Bounds GetBounds(this Transform p_transform)
        {
            Bounds bounds = new Bounds();

            Renderer[] renderers = p_transform.GetComponentsInChildren<Renderer>().Where(r => r.enabled).ToArray();
            if (renderers.Length == 0)
                return bounds;

            for (int i = 1; i < renderers.Length; i++)
            {
                bounds.Encapsulate(renderers[i].bounds);
            }

            return bounds;
        }

        public static Transform GetChildByName(this Transform p_transform, string p_name)
        {
            foreach (Transform child in p_transform)
            {
                if (child.name == p_name)
                    return child;
            }

            return null;
        }
        
        public static int GetFirstActiveChildIndex(this Transform p_transform)
        {
            for (int i = 0; i < p_transform.childCount; i++)
            {
                if (p_transform.GetChild(i).gameObject.activeSelf)
                {
                    return i;
                }
            }

            return -1;
        }
        
        public static void GetAllChildren(this Transform p_transform, ref List<Transform> p_children)
        {
            foreach (Transform t in p_transform)
            {
                p_children.Add(t);
                GetAllChildren(t, ref p_children);
            }
        }
        
        public static void GetAllChildrenPaths(this Transform p_transform, ref List<string> p_childrenPaths, string p_path = "")
        {
            foreach (Transform t in p_transform)
            {
                p_childrenPaths.Add(p_path+t.name);
                GetAllChildrenPaths(t, ref p_childrenPaths, p_path+t.name+"/");
            }
        }

        public static string GetRelativePath(this Transform p_transform, Transform p_parent)
        {
            if (p_transform.parent == p_parent)
                return "/" + p_transform.name;
            return p_transform.parent.GetPath() + "/" + p_transform.name;
        }

        public static string GetPath(this Transform p_transform)
        {
            if (p_transform.parent == null)
                return "/" + p_transform.name;
            return p_transform.parent.GetPath() + "/" + p_transform.name;
        }
        
        public static Transform DeepFind(this Transform p_parent, string p_name)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(p_parent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == p_name)
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }

            return null;
        }

        public static Transform DeepFindAlt(this Transform p_parent, string p_name)
        {
            foreach (Transform child in p_parent)
            {
                if (child.name == p_name)
                    return child;
                var result = child.DeepFindAlt(p_name);
                if (result != null)
                    return result;
            }

            return null;
        }
        
        public static void DestroyChildren(this Transform p_parent)
        {
            foreach (Transform child in p_parent)
            {
                Object.Destroy(child.gameObject);
            }
        }
        
        public static void ForEachChild(this Transform p_parent, Action<Transform> p_action)
        {
            foreach (Transform child in p_parent)
            {
                p_action(child);
            }
        }
    }
}