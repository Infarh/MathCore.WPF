﻿using System.Windows.Media;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Windows;

public static class VisualTreeHelperExtensions
{
    public static T? FindVisualParent<T>(this DependencyObject? obj) where T : class
    {
        if (obj is null) return null;
        var target = obj;
        do target = VisualTreeHelper.GetParent(target);
        while (target != null && target is not T);
        return target as T;
    }

    public static DependencyObject? FindLogicalRoot(this DependencyObject? obj)
    {
        if (obj is null) return null;
        do
        {
            if (LogicalTreeHelper.GetParent(obj) is not { } parent) return obj;
            obj = parent;
        } while (true);
    }

    public static DependencyObject? FindVisualRoot(this DependencyObject? obj)
    {
        if (obj is null) return null;
        do
        {
            var parent = VisualTreeHelper.GetParent(obj);
            if (parent is null) return obj;
            obj = parent;
        } while (true);
    }

    public static T? FindLogicalParent<T>(this DependencyObject? obj) where T : class
    {
        if (obj is null) return null;
        var target = obj;
        do { target = LogicalTreeHelper.GetParent(target); } while (target != null && target is not T);
        return target as T;
    }

    public static IEnumerable<DependencyObject> GetAllVisualChilds(this DependencyObject? obj)
    {
        if (obj is null) yield break;
        var to_process = new Stack<DependencyObject>(obj.GetVisualChilds());
        do
        {
            obj = to_process.Pop();
            yield return obj;
            obj.GetVisualChilds().Foreach(to_process.Push);
        } while (to_process.Count > 0);
    }

    public static IEnumerable<DependencyObject> GetVisualChilds(this DependencyObject? obj)
    {
        if (obj is null) yield break;
        for (int i = 0, count = VisualTreeHelper.GetChildrenCount(obj); i < count; i++)
            yield return VisualTreeHelper.GetChild(obj, i);
    }
    //Child
    public static IEnumerable<DependencyObject> GetAllLogicalChilds(this DependencyObject? obj)
    {
        if (obj is null) yield break;
        var to_process = new Stack<DependencyObject>(obj.GetLogicalChilds());
        do
        {
            obj = to_process.Pop();
            yield return obj;
            obj.GetLogicalChilds().Foreach(to_process.Push);
        } while (to_process.Count > 0);
    }

    public static IEnumerable<DependencyObject> GetLogicalChilds(this DependencyObject? obj) => obj is null
        ? []
        : LogicalTreeHelper.GetChildren(obj).OfType<DependencyObject>();

    public static IEnumerable<DependencyObject> GetLogicalParents(this DependencyObject? obj)
    {
        if (obj is null) yield break;

        var current = obj;
        do
        {
            current = LogicalTreeHelper.GetParent(current);
            if (current != null)
                yield return current;
        }
        while (current != null);
    }

    public static IEnumerable<DependencyObject> GetVisualParents(this DependencyObject? obj)
    {
        if (obj is null) yield break;

        var current = obj;
        do
        {
            current = VisualTreeHelper.GetParent(current);
            if (current != null)
                yield return current;
        }
        while (current != null);
    }
}