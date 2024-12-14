using UnityEngine;

namespace Core.Common.GameObject.Components
{
    public class Light : IComponent
    {
        public float Range;
        public Color Color;
        public float Intensity;
        public LightType Type = LightType.Point;
        public LightShadows Shadows = LightShadows.Soft;
    }
}