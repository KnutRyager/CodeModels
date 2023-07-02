﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common.DataStructures;

namespace Common.Algorithms.Search;

public static class BreathFirstSearch
{
    public static Node<T>? Search<T>(Node<T> root, T targetValue)
        where T : IEquatable<T>
    {
        if (root is null)
            return null;

        var queue = new Queue<Node<T>>();
        var seen = new HashSet<T>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (seen.Contains(current.Value)) continue;
            seen.Add(current.Value);
            if (current.Value.Equals(targetValue))
                return current;

            foreach (var child in current.Children)
            {
                queue.Enqueue(child);
            }
        }

        return null;
    }

    public static T? Search<T>(T root, Predicate<T> matchCriteria, Func<T, IEnumerable<T>> childGenerator)
        where T : IEquatable<T>
    {
        if (root is null)
            return default;

        var queue = new Queue<T>();
        var seen = new HashSet<T>();
        queue.Enqueue(root);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (seen.Contains(current)) continue;
            seen.Add(current);
            if (matchCriteria(current))
                return current;

            foreach (var child in childGenerator(current))
            {
                queue.Enqueue(child);
            }
        }

        return default;
    }

    public static List<T>? FindPath<T>(T root, Predicate<T> matchCriteria, Func<T, IEnumerable<T>> childGenerator)
        where T : IEquatable<T>
    {
        if (root is null)
            return default;

        var seen = new HashSet<T>();
        var queue = new Queue<List<T>>();
        var path = new List<T>() { root };
        queue.Enqueue(path);

        while (queue.Count > 0)
        {
            path = queue.Dequeue();
            var current = path[^1];
            if (seen.Contains(current)) continue;
            seen.Add(current);
            if (matchCriteria(current))
                return path;

            foreach (var child in childGenerator(current))
            {
                var newPath = new List<T>(path)
                {
                    child
                };
                queue.Enqueue(newPath);
            }
        }

        return default;
    }
}
