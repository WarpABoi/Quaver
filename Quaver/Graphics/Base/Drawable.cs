using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Quaver.Helpers;
using Quaver.Main;

namespace Quaver.Graphics.Base
{
    /// <summary>
    ///     This class is for any objects that will be drawn to the screen.
    /// </summary>
    internal abstract class Drawable
    {
        //Local variables
        internal bool Changed { get; set; } = true;
        private DrawRectangle _localRectangle = new DrawRectangle();
        private DrawRectangle _globalRectangle = new DrawRectangle();
        private Drawable _parent = null;
        internal UDim2D _position = new UDim2D();
        internal UDim2D _size = new UDim2D();

        /// <summary>
        ///     Position of this Object
        /// </summary>
        internal UDim2D Position
        {
            get => _position;
            set
            {
                _position = value;
                Changed = true;
            }
        }

        /// <summary>
        ///     X Position of this object
        /// </summary>
        internal float PosX
        {
            get => _position.X.Offset;
            set
            {
                _position.X.Offset = value;
                Changed = true;
            }
        }

        /// <summary>
        ///     Y Position of this object
        /// </summary>
        internal float PosY
        {
            get => _position.Y.Offset;
            set
            {
                _position.Y.Offset = value;
                Changed = true;
            }
        }

        /// <summary>
        ///     Size of this object
        /// </summary>
        internal UDim2D Size
        {
            get => _size;
            set
            {
                _size = value;
                Changed = true;
            }
        }

        /// <summary>
        ///     X Size of this object
        /// </summary>
        internal float SizeX
        {
            get => _size.X.Offset;
            set
            {
                _size.X.Offset = value;
                Changed = true;
            }
        }

        /// <summary>
        ///     Y Size of this object
        /// </summary>
        internal float SizeY
        {
            get => _size.Y.Offset;
            set
            {
                _size.Y.Offset = value;
                Changed = true;
            }
        }

        /// <summary>
        ///     X Scale of this object
        /// </summary>
        internal float ScaleX
        {
            get => _size.X.Scale;
            set
            {
                _size.X.Scale = value;
                Changed = true;
            }
        }

        /// <summary>
        ///     Y Scale of this object
        /// </summary>
        internal float ScaleY
        {
            get => _size.Y.Scale;
            set
            {
                _size.Y.Scale = value;
                Changed = true;
            }
        }

        /// <summary>
        ///     The alignment of the sprite relative to it's parent.
        /// </summary>
        internal Alignment Alignment { get; set; } = Alignment.TopLeft;

        /// <summary>
        ///     The children of this object that depend on this object's position/size.
        /// </summary>
        internal List<Drawable> Children { get; set; } = new List<Drawable>();

        /// <summary>
        ///     The parent of this object which it depends on for position/size.
        /// </summary>
        internal Drawable Parent
        {
            get => _parent;
            set
            {
                //Remove this object from its old parent's Children list
                if (_parent != null)
                {
                    var cIndex = Parent.Children.FindIndex(r => r == this);
                    Parent.Children.RemoveAt(cIndex);
                }

                //Add this object to its new parent's Children list
                if (value != null)
                    value.Children.Add(this);
                else
                {
                    // If we received null for the parent, that must mean we want to fully destroy
                    // the object, so delete all of its children as well
                    for (var i = Children.Count - 1; i >= 0; i--)
                    {
                        var drawable = Children[i];
                        drawable.Destroy();
                    }
                }

                //Assign parent in this object
                _parent = value;
                Changed = true;
            }
        }

        /// <summary>
        ///     (Read-only) Returns the Drawable's GlobalRect.
        /// </summary>
        internal DrawRectangle GlobalRectangle => _globalRectangle;

        /// <summary>
        ///     (Read-only) Returns the Drawable's LocalRect.
        /// </summary>
        internal DrawRectangle LocalRectangle => _localRectangle;

        /// <summary>
        ///     (Read-only) Absolute _size of this object
        /// </summary>
        internal Vector2 AbsoluteSize => new Vector2(_globalRectangle.Width, _globalRectangle.Height);

        /// <summary>
        ///     (Read-only) Absolute _position of this object
        /// </summary>
        internal Vector2 AbsolutePosition => new Vector2(_globalRectangle.X, _globalRectangle.Y);

        /// <summary>
        ///     Dictates whether or not we will be setting the children's visibility as well.
        /// </summary>
        internal bool SetChildrenVisibility { get; set; }

        /// <summary>
        ///     The total amount of objects drawn.
        /// </summary>
        internal static int TotalObjectsDrawn { get; set; }

        /// <summary>
        ///     The order at which things are drawn.
        /// </summary>
        internal int DrawOrder { get; private set; }

        /// <summary>
        ///     Determines if the Object is going to get drawn.
        /// </summary>
        private bool _visible = true;
        internal bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;

                if (SetChildrenVisibility)
                    Children.ForEach(x => x.Visible = value);
            }
        }

        /// <summary>
        ///     This method gets called every frame to update the object.
        /// </summary>
        /// <param name="dt"></param>
        internal virtual void Update(double dt)
        {
            //Animation logic
            if (Changed)
            {
                Changed = false;
                RecalculateRect();
            }

            //Update Children
            for (var i = Children.Count - 1; i >= 0; i--)
            {
                try
                {
                    Children[i].Update(dt);
                }
                catch (Exception e)
                {
                    break;
                }
            }
        }

        /// <summary>
        ///     This method gets called every frame to draw the object.
        /// </summary>
        internal virtual void Draw()
        {
            // Increase the total amount objects drawn and set the DrawOrder to this current object.
            TotalObjectsDrawn++;
            DrawOrder = TotalObjectsDrawn;

            if (!Visible)
                return;

            // Draw children and set their draw order.
            foreach (var drawable in Children)
            {
                drawable.Draw();

                TotalObjectsDrawn++;
                drawable.DrawOrder = TotalObjectsDrawn;
            }
        }

        /// <summary>
        ///     This method will be called everytime a property of this object gets updated.
        /// </summary>
        internal void RecalculateRect()
        {
            //Calculate Scale
            //todo: fix
            if (_parent != null)
            {
                _localRectangle.Width = _size.X.Offset + _parent.GlobalRectangle.Width * _size.X.Scale;
                _localRectangle.Height = _size.Y.Offset + _parent.GlobalRectangle.Height * _size.Y.Scale;
                _localRectangle.X = _position.X.Offset; //todo: implement scale
                _localRectangle.Y = _position.Y.Offset; //todo: implement scale
            }
            else
            {
                _localRectangle.Width = _size.X.Offset + GameBase.WindowRectangle.Width * _size.X.Scale;
                _localRectangle.Height = _size.Y.Offset + GameBase.WindowRectangle.Height * _size.Y.Scale;
                _localRectangle.X = _position.X.Offset; //todo: implement scale
                _localRectangle.Y = _position.Y.Offset; //todo: implement scale
            }

            //Update Global Rect
            if (_parent != null)
                _globalRectangle = GraphicsHelper.AlignRect(Alignment, _localRectangle, Parent.GlobalRectangle);
            else
                _globalRectangle = GraphicsHelper.AlignRect(Alignment, _localRectangle, GameBase.WindowRectangle);

            Children.ForEach(x => x.Changed = true);
            Children.ForEach(x => x.RecalculateRect());
        }

        /// <summary>
        ///     This method is called when the object will be removed from memory.
        /// </summary>
        internal virtual void Destroy() => Parent = null;
    }
}
