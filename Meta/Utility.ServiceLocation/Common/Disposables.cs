// Copyright (c) 2025 ReactiveUI. All rights reserved.
// Licensed to ReactiveUI under one or more agreements.
// ReactiveUI licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace Utility.ServiceLocation;

/// <summary>
/// A disposable which will call the specified action.
/// </summary>
internal sealed class ActionDisposable(Action block) : IDisposable
{
    private Action _block = block;

    /// <summary>
    /// Gets a action disposable which does nothing.
    /// </summary>
    public static IDisposable Empty => new ActionDisposable(() => { });

    /// <inheritdoc />
    public void Dispose() => Interlocked.Exchange(ref _block, () => { })();
}

internal sealed class BooleanDisposable() : IDisposable
{
    private volatile bool _isDisposed;

    /// <summary>
    /// Gets a value indicating whether the object is disposed.
    /// </summary>
    public bool IsDisposed => _isDisposed;

    /// <summary>
    /// Sets the status to disposed, which can be observer through the <see cref="IsDisposed"/> property.
    /// </summary>
    public void Dispose() => _isDisposed = true;
}
