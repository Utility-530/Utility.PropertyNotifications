//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System.Windows;
using System.Windows.Controls;

// ReSharper disable once CheckNamespace

namespace SniffCore.Buttons
{
    /// <summary>
    ///     The main button placed in the <see cref="SplitButton" />.
    /// </summary>
    public class SplitMainButton : Button
    {
        static SplitMainButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitMainButton), new FrameworkPropertyMetadata(typeof(SplitMainButton)));
        }
    }
}