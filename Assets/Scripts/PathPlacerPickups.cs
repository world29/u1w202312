using PathCreation;
using UnityEngine;

namespace u1w202312
{
    [ExecuteInEditMode]
    public class PathPlacerPickups : PathCreation.Examples.PathSceneTool
    {

        public GameObject prefab;
        public GameObject holder;
        public float spacing = 3;
        public int maxCount = 5;
        // x: 最初の要素が生成される位置
        // y,z: 各要素を生成する座標にかかるオフセット
        public Vector3 offset = Vector3.zero;

        const float minSpacing = .1f;

        void Generate()
        {
            if (pathCreator != null && prefab != null && holder != null)
            {
                DestroyObjects();

                VertexPath path = pathCreator.path;

                spacing = Mathf.Max(minSpacing, spacing);
                float dst = offset.x;

                int count = 0;
                while (dst < path.length)
                {
                    Vector3 point = path.GetPointAtDistance(dst);
                    Quaternion rot = path.GetRotationAtDistance(dst);
                    Instantiate(prefab, point + new Vector3(0, offset.y, offset.z), rot, holder.transform);
                    dst += spacing;

                    ++count;
                    if (maxCount <= count)
                    {
                        break;
                    }
                }
            }
        }

        void DestroyObjects()
        {
            int numChildren = holder.transform.childCount;
            for (int i = numChildren - 1; i >= 0; i--)
            {
                DestroyImmediate(holder.transform.GetChild(i).gameObject, false);
            }
        }

        protected override void PathUpdated()
        {
            if (pathCreator != null)
            {
                Generate();
            }
        }
    }
}