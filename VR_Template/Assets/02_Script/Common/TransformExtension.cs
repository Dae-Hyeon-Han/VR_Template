using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Object 찾기 위한 확장 버전.
/// </summary>
public static class TransformExtension
{
    public static Transform FindDeepChild(this Transform parent, string name)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == name)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }

    public static IEnumerable<Transform> GetChildren(this Transform parent, int depth)
    {
        Queue<(int, Transform)> queue = new Queue<(int, Transform)>();
        queue.Enqueue((0, parent));
        while (queue.Count > 0)
        {
            (var d, var c) = queue.Dequeue();

            if (d == depth)
            {
                yield return c;
            }

            if (d <= depth)
            {
                foreach (Transform t in c)
                    queue.Enqueue((d + 1, t));
            }
        }
    }

    public static IEnumerable<T> GetAllComponentsInChildren<T>(this Transform parent)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(parent);
        while (queue.Count > 0)
        {
            var transform = queue.Dequeue();
            var components = transform.GetComponents<T>();
            foreach (var component in components)
            {
                yield return component;
            }
            foreach (Transform child in transform)
                queue.Enqueue(child);
        }
    }

    public static IEnumerable<TargetObject> FindTargetObjects(this Transform transform, string targetObjects)
    {
        if (string.IsNullOrEmpty(targetObjects) == false)
        {
            foreach (var t in targetObjects.Split(','))
            {
                var targetname = t.Trim();

                if (targetname.Contains("/"))  // path find
                {
                    var obj = transform.FindPathObject(targetname);
                    if (obj != null)
                    {
                        var targetObject = obj.GetComponent<TargetObject>();
                        if (targetObject == null)
                        {
                            Debug.LogError($"{targetname} not found");
                        }
                        else
                        {
                            yield return targetObject;
                        }
                    }
                }
                else
                {
                    foreach (var component in transform.GetComponentsInChildren<TargetObject>(true))
                    {
                        if (component.gameObject.name == targetname)
                        {
                            yield return component;
                        }
                    }
                }
            }
        }
    }

    private static GameObject FindPathObject(this Transform transform, string targetPath)
    {
        Transform found = transform;

        foreach (var path in targetPath.Split('/'))
        {
            found = found.Find(path);
            if (found == null)
            {
                Debug.LogError($"[TargetPath Error] {targetPath} => {path} not found");
                return null;
            }
        }

        return found.gameObject;
    }

    public static T GetChildComponentByName<T>(this Transform transform, string name) where T : Component
    {
        foreach (T component in transform.GetComponentsInChildren<T>(true))
        {
            if (component.gameObject.name == name)
            {
                return component;
            }
        }
        return null;
    }
}