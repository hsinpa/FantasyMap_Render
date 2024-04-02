using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;


namespace Hsinpa.Algorithm {
    public class QuadTree
    {
        public const int CAPACITY = 5;

        private QuadTreeUti.QuadRect _boundary;
        public QuadTreeUti.QuadRect Boundary => _boundary;

        QuadTreeUti.Point[] points = new QuadTreeUti.Point[CAPACITY];
        public IReadOnlyCollection<QuadTreeUti.Point> Points => points;

        private int _size;
        public int Size => _size;

        //Children
        private QuadTree _northWestBranch;
        public QuadTree NorthWestBranch => _northWestBranch;

        private QuadTree _northEastBranch;
        public QuadTree NorthEastBranch => _northEastBranch;

        private QuadTree _southWestBranch;
        public QuadTree SouthWestBranch => _southWestBranch;

        private QuadTree _southEastBranch;
        public QuadTree SouthEastBranch => _southEastBranch;

        public QuadTree(QuadTreeUti.QuadRect boundary) {
            _size = 0;
            _boundary = boundary;

            //Debug.Log($"boundary {boundary.x} {boundary.y}, size ex {boundary.extends}");
        }

        public bool Insert(QuadTreeUti.Point point) {
            if (!this._boundary.ContainPoint(point.x, point.y)) {
                return false;
            }

            // If there is space in this quad tree and if doesn't have subdivisions, add the object here
            if (_size < CAPACITY && _northWestBranch == null) {
                points[_size] = point;
                _size++;
                return true;
            }

            if (_northWestBranch == null) {
                Subdivide();

                _size = 0;
                //points = null;
            }

            bool pushSuccess = PushPointToChild(point);

            return pushSuccess;
        }

        public void Subdivide() {

            float2 childExtend = _boundary.extends * 0.5f;

            _northWestBranch = new QuadTree(new QuadTreeUti.QuadRect(_boundary.x - childExtend.x, _boundary.y + childExtend.y, childExtend));
            _northEastBranch = new QuadTree(new QuadTreeUti.QuadRect(_boundary.x + childExtend.x, _boundary.y + childExtend.y, childExtend));

            _southWestBranch = new QuadTree(new QuadTreeUti.QuadRect(_boundary.x - childExtend.x, _boundary.y - childExtend.y, childExtend));
            _southEastBranch = new QuadTree(new QuadTreeUti.QuadRect(_boundary.x + childExtend.x, _boundary.y - childExtend.y, childExtend));

            //Insert current hcild to subdivide node
            if (_size == 0) return;

            for (int i = 0; i < _size; i++)
                PushPointToChild(this.points[i]);
        }

        public void QueryRect(QuadTreeUti.QuadRect quadRect, ref List<QuadTreeUti.Point> points) {
            if (!this._boundary.Intersect(quadRect))
                return;

            for (int i = 0; i < this._size; i++) {
                if (quadRect.ContainPoint(this.points[i].x, this.points[i].y)) {
                    points.Add(this.points[i]);
                }
            }

            if (_northWestBranch == null || _northEastBranch == null || _southWestBranch == null || _southEastBranch == null) 
                return;

            if (points != null) _northWestBranch.QueryRect(quadRect, ref points);
            if (points != null) _northEastBranch.QueryRect(quadRect, ref points);
            if (points != null) _southWestBranch.QueryRect(quadRect, ref points);
            if (points != null) _southEastBranch.QueryRect(quadRect, ref points);
        }

        public void QueryRadius(float2 point, float radius)
        {

        }

        public void Dispose() {
            this._size = 0;
            this._northWestBranch = null;
            this._northEastBranch = null;
            this._southWestBranch = null;
            this._southEastBranch = null;
        }

        private bool PushPointToChild(QuadTreeUti.Point point) {
            if (this._northWestBranch.Insert(point) ) return true;
            if (this._northEastBranch.Insert(point) ) return true;
            if (this._southWestBranch.Insert(point) ) return true;
            if (this._southEastBranch.Insert(point) ) return true;

            return false;
        }

    }
}
