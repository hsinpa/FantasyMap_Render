using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Hsinpa.Algorithm
{
    public class QuadTreeUti
    {

        public struct Point {
            public float x;
            public float y;
            public int id;
        }

        public struct QuadRect {
            public float x;
            public float y;
            public float2 extends;

            public float2 size => extends * 2;
            public float2 minPoints => new float2(x - extends.x, y - extends.y);
            public float2 maxPoints => new float2(x + extends.x, y + extends.y);

            public QuadRect(float p_x, float p_y, float2 p_extends) {
                this.x = p_x;
                this.y = p_y;
                this.extends = p_extends;
            }

            public bool Intersect(QuadRect otherQuadRect) {
                return (
                    this.x - extends.x <= otherQuadRect.x + otherQuadRect.extends.x && //Check self left to other right
                    this.x + extends.x >= otherQuadRect.x - otherQuadRect.extends.x &&//Check self right to other left

                    this.y - extends.y <= otherQuadRect.y + otherQuadRect.extends.y && //Check bottom to other top
                    this.y + extends.y >= otherQuadRect.y - otherQuadRect.extends.y
                );
            }

            public bool ContainPoint(float p_x, float p_y) {
                return (
                    p_x >= (this.x - extends.x) && //Left
                    p_x  <= (this.x + extends.x) && //Right
                    p_y >= (this.y - extends.y) && //Bottom
                    p_y <= (this.y + extends.y) // Top
                );
            }
        }

    }
}