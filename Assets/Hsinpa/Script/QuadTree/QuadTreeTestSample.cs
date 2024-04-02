using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Hsinpa.Algorithm.Sample
{
    public class QuadTreeTestSample : MonoBehaviour
    {
        int width = 100;
        int height = 100;

        [SerializeField]
        private Vector2 mouse_size;

        QuadTree _rootQuadTree;

        private Camera _camera;
        private Vector3 _mousePosition;
        private QuadTreeUti.QuadRect _mouseRect;
        private List<QuadTreeUti.Point> _findPoints = new List<QuadTreeUti.Point>();

        void Start()
        {
            _camera = Camera.main;
            this._mouseRect = new QuadTreeUti.QuadRect();
            this._mouseRect.extends = mouse_size * 0.5f;
            float2 extend = new float2(width * 0.5f, height * 0.5f);
            int samples = 100;

            this._rootQuadTree = new QuadTree(new QuadTreeUti.QuadRect(extend.x, extend.y, extend));
            Unity.Mathematics.Random rand = new Unity.Mathematics.Random((uint)System.DateTime.Now.Millisecond);

            for (int i = 0; i < samples; i++) {
                QuadTreeUti.Point point = new QuadTreeUti.Point();
                point.x = rand.NextFloat(width) ;
                point.y = rand.NextFloat(height);
                point.id = i;

                this._rootQuadTree.Insert(point);
            }
        }

        private void Update()
        {
            _findPoints.Clear();

            this._mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

            this._mouseRect.x = this._mousePosition.x;
            this._mouseRect.y = this._mousePosition.z;

            this._rootQuadTree.QueryRect(this._mouseRect, ref _findPoints);
        }

        private int GetQuadUniqueID(QuadTree quadTree) {
            return Mathf.RoundToInt(quadTree.Boundary.x + (width * quadTree.Boundary.y));
        }

        private void OnDrawGizmos()
        {
            if (this._rootQuadTree == null) return;

            //Draw QuadTree
            Queue<QuadTree> quadTreeOpens = new Queue<QuadTree>();

            quadTreeOpens.Enqueue(this._rootQuadTree);
            while (quadTreeOpens.Count > 0) {
                QuadTree dequeueQuadTree = quadTreeOpens.Dequeue();

                Gizmos.color = Color.black;
                Gizmos.DrawWireCube(new Vector3(dequeueQuadTree.Boundary.x, 0, dequeueQuadTree.Boundary.y), 
                                new Vector3(dequeueQuadTree.Boundary.size.x, 0.05f, dequeueQuadTree.Boundary.size.y));

                if (dequeueQuadTree.NorthEastBranch != null)
                {
                    quadTreeOpens.Enqueue(dequeueQuadTree.NorthEastBranch);
                    quadTreeOpens.Enqueue(dequeueQuadTree.NorthWestBranch);
                    quadTreeOpens.Enqueue(dequeueQuadTree.SouthEastBranch);
                    quadTreeOpens.Enqueue(dequeueQuadTree.SouthWestBranch);
                }
                else {
                    Gizmos.color = Color.red;
                    int count = 0;
                    foreach (var point in dequeueQuadTree.Points) {
                        count++;

                        if (count > dequeueQuadTree.Size) break;

                        Gizmos.DrawSphere(new Vector3(point.x, 0, point.y), 0.4f);
                    }
                }
            }

            //Mouse Rect
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(new Vector3(_mousePosition.x, 0f, _mousePosition.z),
                new Vector3(mouse_size.x, 0.05f, mouse_size.y));
        }



    }
}