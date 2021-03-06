﻿using GameLibrary.Dependencies.Entities;

namespace GameLibrary.Entities.Components
{
    /// <summary>
    /// The animation type enum.
    /// </summary>
    public enum AnimationType
    {
        None, //Keeps the sprite at its current sprite frame.
        Loop, //Loops the the sprite incrementally
        ReverseLoop, //Loops the sprite decrementally
        Increment, //Increments the sprites frame once and then sets its animation state to none.
        Decrement, //Decrements the sprites frame once and then sets its animation state to none.
        Bounce, //Animate back and forth through frames
        Once //Animates once and stops
    }

    public class Animation : Component
    {
        /// <summary>
        /// Default constructor for animation component
        /// </summary>
        /// <param name="rate">The rate at which the animation updates (miliseconds).</param>
        public Animation(AnimationType type, int rate)
        {
            FrameRate = rate;
            Type = type;
            _Tick = 0;
        }

        public Animation(AnimationType type)
            : this(type, 1)
        {
        }

        #region Properties

        /// <summary>
        /// The frame rate in miliseconds of the animation
        /// </summary>
        public int FrameRate { set; get; }

        public int FrameInc
        {
            get { return frameInc; }
            set { frameInc = value; }
        }

        /// <summary>
        /// The type of animation. (See AnimationType)
        /// </summary>
        public AnimationType Type { set; get; }

        #endregion Properties

        #region Fields

        internal int _Tick;
        private int frameInc = 1;

        #endregion Fields
    }
}