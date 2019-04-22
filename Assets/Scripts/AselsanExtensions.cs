using System;
using System.Collections.Generic;
using UnityEngine;

public static class AselsanExtensions
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public static Vector3 GetGroupCenterPosition(this Transform transform) {
        Vector3 totalPos = Vector3.zero;
        int totalChild = 0;
        foreach (Transform child in transform) {
            totalChild++;
            totalPos += (child.transform.position) + (child.transform.right*5) + (child.transform.up*5)/* + (child.transform.forward)*/;
        }
        return totalPos / totalChild;
    }
    public static void DestroyAllChildren(this Transform transform)
    {
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        children.ForEach(child => GameObject.Destroy(child));
    }

    public static List<T> GetAllChildren<T>(this Transform transform)
    {
        var children = new List<T>();
        foreach (Transform child in transform)
        {
            T component = child.GetComponent<T>();
            if (component != null) {
                children.Add(component);
            }
        }
        return children;
    }

    public static T GetRandom<T>(this List<T> myList)
    {
        return myList[UnityEngine.Random.Range(0, myList.Count)];
    }

    public static long ToTimestamp(this DateTime time)
    {
        return (long)time.ToUniversalTime()
            .Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc))
            .TotalMilliseconds;
    }

    public static DateTime ToDateTime(this long timestamp)
    {
        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(timestamp);
    }

    public static string ToPercentage(this float ratio)
    {
        return String.Format("{0:P2}", ratio);
    }

    public static string GetTimerText(this float seconds)
    {
        TimeSpan time = TimeSpan.FromSeconds(Mathf.CeilToInt(seconds));            
        return time.GetFormettedTimeSpan();
    }

    public static string GetFormettedTimeSpan(this TimeSpan ts)
    {

        if (ts.Days > 0)
        {
            return string.Format("{0} d {1} h {2} m {3} s", ts.Days, ts.Hours, ts.Minutes, ts.Seconds);
        }
        else if (ts.Hours > 0)
        {
            return string.Format("{0} h {1} m {2} s", ts.Hours, ts.Minutes, ts.Seconds);
        }
        else if (ts.Minutes > 0)
        {
            return string.Format("{0} m {1} s", ts.Minutes, ts.Seconds);
        }
        else
        {
            return string.Format("{0} s", ts.Seconds);
        }

    }
}

