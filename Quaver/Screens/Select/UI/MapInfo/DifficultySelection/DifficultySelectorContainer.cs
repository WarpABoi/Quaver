﻿using Microsoft.Xna.Framework;
using Quaver.Assets;
using Quaver.Graphics;
using Wobble.Assets;
using Wobble.Graphics;
using Wobble.Graphics.Sprites;

namespace Quaver.Screens.Select.UI.MapInfo.DifficultySelection
{
    public class DifficultySelectorContainer : HeaderedContainer
    {
        /// <summary>
        ///     Reference to select screen
        /// </summary>
        public SelectScreen Screen { get; }

        /// <summary>
        ///     Reference to select screenview.
        /// </summary>
        public SelectScreenView View { get; }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public DifficultySelectorContainer(SelectScreen screen, SelectScreenView view)
            : base(new Vector2(608, 300), "SELECT MAP", Fonts.Exo2Regular24, 0.50f, Alignment.MidCenter, 30, Colors.DarkGray)
        {
            Screen = screen;
            View = view;

            X = 8;
            Header.Tint = Colors.DarkGray;

            CreateContent();
        }

        /// <inheritdoc />
        /// <summary>
        /// /
        /// </summary>
        /// <returns></returns>
        protected sealed override Sprite CreateContent()
        {
            var content = new Sprite()
            {
                Parent = this,
                Y = Header.Height,
                X = 5,
                Size = new ScalableVector2(Header.Width - 45, Height - Header.Height),
                Alignment = Alignment.TopCenter,
                Alpha = 0
            };

            // Make the actual DifficultySelector a parent of this.
            // It is originally created in MapsetContainer, so that it can be used to select the first difficulty
            var diffSelector = View.MapsetContainer.DifficultySelector;
            // diffSelector.Parent = content;

            return content;
        }
    }
}