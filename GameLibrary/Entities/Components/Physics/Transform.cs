﻿using Microsoft.Xna.Framework;
using System;

namespace GameLibrary.Entities.Components
{
    public struct Transform : ITransform
    {
        public Transform(Vector2 position, float rotation)
        {
            _Position = position;
            _Rotation = rotation;
        }

        public Microsoft.Xna.Framework.Vector2 Position
        {
            get
            {
                return _Position;
            }
            set
            {
                _Position = value;
            }
        }

        private Vector2 _Position;

        public float Rotation
        {
            get
            {
                return _Rotation;
            }
            set
            {
                _Rotation = value;
            }
        }

        private float _Rotation;

        public void RotateTo(Vector2 direction)
        {
            Rotation = (float)Math.Atan2(direction.Y, direction.X);
        }
    }
}