namespace Common.Util;

public static class FunctionalUtil
{
    /// <summary>
    ///  Chain actions.
    ///  Optionally do a preparation step before each.
    ///  Optionally wrap last action.
    /// </summary>
    public static void Chain(Action? beforeAction = null, Action<Action>? lastAction = null, params Action[] actions)
    {
        var last = actions.Last();
        foreach (var action in actions)
        {
            beforeAction?.Invoke();
            if (action.Equals(last) && lastAction != null) lastAction(action);
            else action();
        }
    }

    public static T2 Chain<T1, T2>(Func<T1> first, Func<T1, T2> second, Action? betweenAction = null, Action<Action>? lastAction = null)
    {
        var resultOfFirst = first();
        betweenAction?.Invoke();
        T2 resultOfSecond = default!;
        if (lastAction != null) lastAction?.Invoke(() => resultOfSecond = second(resultOfFirst));
        else resultOfSecond = second(resultOfFirst);
        return resultOfSecond;
    }

    public static T2 Chain<T1, T2>(T1 first, Func<T1, T2> second, Action? betweenAction = null, Action<Action>? lastAction = null)
    {
        betweenAction?.Invoke();
        T2 resultOfSecond = default!;
        if (lastAction != null) lastAction?.Invoke(() => resultOfSecond = second(first));
        else resultOfSecond = second(first);
        return resultOfSecond;
    }

    public static T3 Chain<T1, T2, T3>(Func<T1> first, Func<T1, T2> second, Func<T2, T3> third, Action? betweenAction = null, Action<Action>? lastAction = null)
        => Chain(() => Chain(first, second, betweenAction), third, betweenAction, lastAction);
}
