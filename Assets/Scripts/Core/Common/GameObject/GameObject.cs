﻿using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Core.Common.GameObject
{
    public class GameObject
    {
        public string Name;
        
        private GameObject _parent;

        public GameObject Parent
        {
            get => _parent;

            set
            {
                _parent?._children?.Remove(this);
                _parent = value;
                _parent?._children?.Add(this);
            }
        }
        
        private readonly List<GameObject> _children = new();
        
        public IReadOnlyList<GameObject> Children => _children;
        
        public readonly List<IComponent> Components = new();
        
        public float3 Position = Vector3.zero;
        
        public quaternion Rotation = Quaternion.identity;
        
        public float3 LocalScale = Vector3.one;
        
        public GameObject(string name, GameObject parent)
        {
            Name = name;
            Parent = parent;
        }
        
        public GameObject(string name)
        {
            Name = name;
        }
    }
}