//
// Copyright (c) David Wendland. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
//

using System.Windows;
using System.Windows.Controls.Primitives;

// ReSharper disable once CheckNamespace

namespace SniffCore.Buttons
{
    /// <summary>
    ///     The drop down toggle button placed in the <see cref="SplitButton" />.
    /// </summary>
    public class SplitToggleButton : ToggleButton
    {
        static SplitToggleButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SplitToggleButton), new FrameworkPropertyMetadata(typeof(SplitToggleButton)));
        }
    }
}