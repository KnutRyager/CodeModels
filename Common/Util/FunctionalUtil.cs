using System;
using System.Linq;
using System.Linq.Expressions;

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

    public static Action Action<TResult>(Action action) => action;
    public static Action<T> Action<T>(Action<T> action) => action;
    public static Action<T1, T2> Action<T1, T2>(Action<T1, T2> action) => action;
    public static Action<T1, T2, T3> Action<T1, T2, T3>(Action<T1, T2, T3> action) => action;
    public static Action<T1, T2, T3, T4> Action<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action) => action;
    public static Action<T1, T2, T3, T4, T5> Action<T1, T2, T3, T4, T5>(Action<T1, T2, T3, T4, T5> action) => action;
    public static Action<T1, T2, T3, T4, T5, T6> Action<T1, T2, T3, T4, T5, T6>(Action<T1, T2, T3, T4, T5, T6> action) => action;

    public static Func<TResult> Lambda<TResult>(Func<TResult> func) => func;
    public static Func<T, TResult> Lambda<T, TResult>(Func<T, TResult> func) => func;
    public static Func<T1, T2, TResult> Lambda<T1, T2, TResult>(Func<T1, T2, TResult> func) => func;
    public static Func<T1, T2, T3, TResult> Lambda<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func) => func;
    public static Func<T1, T2, T3, T4, TResult> Lambda<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func) => func;
    public static Func<T1, T2, T3, T4, T5, TResult> Lambda<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func) => func;
    public static Func<T1, T2, T3, T4, T5, T6, TResult> Lambda<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func) => func;

    public static Expression<Action> ActionExpression(Expression<Action> action) => action;
    public static Expression<Action<T>> ActionExpression<T>(Expression<Action<T>> action) => action;
    public static Expression<Action<T1, T2>> ActionExpression<T1, T2>(Expression<Action<T1, T2>> action) => action;
    public static Expression<Action<T1, T2, T3>> ActionExpression<T1, T2, T3>(Expression<Action<T1, T2, T3>> action) => action;
    public static Expression<Action<T1, T2, T3, T4>> ActionExpression<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> action) => action;
    public static Expression<Action<T1, T2, T3, T4, T5>> ActionExpression<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> action) => action;
    public static Expression<Action<T1, T2, T3, T4, T5, T6>> ActionExpression<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>> action) => action;

    public static Expression<Func<TResult>> LambdaExpression<TResult>(Expression<Func<TResult>> func) => func;
    public static Expression<Func<T, TResult>> LambdaExpression<T, TResult>(Expression<Func<T, TResult>> func) => func;
    public static Expression<Func<T1, T2, TResult>> LambdaExpression<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> func) => func;
    public static Expression<Func<T1, T2, T3, TResult>> LambdaExpression<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> func) => func;
    public static Expression<Func<T1, T2, T3, T4, TResult>> LambdaExpression<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> func) => func;
    public static Expression<Func<T1, T2, T3, T4, T5, TResult>> LambdaExpression<T1, T2, T3, T4, T5, TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> func) => func;
    public static Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> LambdaExpression<T1, T2, T3, T4, T5, T6, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> func) => func;
}
