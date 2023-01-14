using SmokGnu.SpriteSheetRenderer.Utils.TMUtilsEcs;
using Unity.Mathematics;
using UnityEngine;

namespace Utils
{
    public static class MathUtils
    {
        public const float Epsilon = 1.1920929E-06F;

        public static float3 Project(Ray ray, Plane plane)
        {
            plane.Raycast(ray, out float rayIntersectDistance);
            return ray.GetPoint(rayIntersectDistance);
        }

        public static bool MoveTowards(Transform transform, Vector3 to, float step)
        {
            bool isReached;
            (transform.position, isReached) = MoveTowards(transform.position, to, step);
            return isReached;
        }

        public static bool MoveTowardsLookAt(Transform transform, Vector3 to, float step)
        {
            bool isReached = MoveTowards(transform, to, step);
            transform.rotation = Quaternion.LookRotation(to - transform.position);
            return isReached;
        }

        public static Vector3 MoveTowardsXZ(Vector3 from, Vector3 to, float step)
        {
            Vector3 result = Vector2.MoveTowards(from.To2D(), to.To2D(), step).To3D();
            result.y = from.y;
            return result;
        }

        public static (Vector3, bool) MoveTowardsXZCheckIfArrived(Vector3 from, Vector3 to, float step)
        {
            Vector3 result = MoveTowardsXZ(from, to, step);
            float distToDestination = DistanceXZ(result, to);

            bool isReached = distToDestination <= step;
            return (result, isReached);
        }

        public static (Vector3, bool) MoveTowards(Vector3 from, Vector3 to, float step)
        {
            Vector3 result = Vector3.MoveTowards(from, to, step);
            float distToDestination = Vector3.Distance(result, to);

            bool isReached = distToDestination <= step;
            return (result, isReached);
        }

        public static (float, bool) RotateAroundYTowards(float from, float to, float step)
        {
            float delta = Mathf.DeltaAngle(from, to);
            int sign = delta is < 0f or > 180f ? -1 : 1;
            float target = from + delta;
        
            if (-step < delta && delta < step)
                return (target, true);
        
            bool isReached = math.abs(delta) <= step;
            float result = from + sign * step;
            return (result, isReached);
        }

        public static float DistanceXZ(Vector3 from, Vector3 to) => Vector2.Distance(from.To2D(), to.To2D());

        public static float Distance(float from, float to) => Mathf.Abs(to - from);

        public static float FloorSymmetrical(float x) =>
            x > 0 ? math.floor(x) : math.ceil(x);

        public static int FloorToIntSymmetrical(float x) => (int) FloorSymmetrical(x);

        public static int2 FloorToIntSymmetrical(float2 v) => new int2(FloorToIntSymmetrical(v.x), FloorToIntSymmetrical(v.y));

        public static float SignedCounterClockwizeToUnsigned(float angle)
        {
            if (angle < 0)
                angle += 360;
            return angle;
        }

        // clockwise
        public static float2 RotateDegrees(this float2 v, float delta)
        {
            delta = -math.radians(delta);
            math.sincos(delta, out float sin, out float cos);

            return new float2(
                v.x * cos - v.y * sin,
                v.x * sin + v.y * cos
            );
        }

        public static int2 RotateDegrees(this int2 v, float delta)
        {
            var resultFloat = new float2(v).RotateDegrees(delta);
            return Round(resultFloat);
        }

        //public static float GameObjectRotationToCounterClockwise(float rotation) =>
        

        public static bool Approximately(float f1, float f2, float epsilon = Epsilon) =>
            math.abs(f1 - f2) < epsilon;

        public static bool Approximately(this float2 v1, float2 v2, float epsilon = Epsilon) =>
            Approximately(v1.x, v2.x, epsilon) && Approximately(v1.y, v2.y, epsilon);

        public static bool Approximately(this float3 v1, float3 v2, float epsilon = Epsilon) =>
            Approximately(v1.x, v2.x, epsilon) &&
            Approximately(v1.y, v2.y, epsilon) &&
            Approximately(v1.z, v2.z, epsilon);

        public static int2 Round(float2 v) => new int2(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));

        public static int Length(this int2 v) => Mathf.RoundToInt(math.length(new float2(v)));

        public static int2 Normalized(this int2 v) => new int2(math.normalize(new float2(v)));


        public static float4x4 TranslationMatrix(float3 translation) => float4x4.TRS(translation, quaternion.identity, Float3V.One);

        public static float3 GetScale(this in float4x4 matrix) => new float3(
            math.length(matrix.c0.xyz),
            math.length(matrix.c1.xyz),
            math.length(matrix.c2.xyz));
        
        public static float3 GetScale(this in float3x3 matrix) => new float3(
            math.length(matrix.c0.xyz),
            math.length(matrix.c1.xyz),
            math.length(matrix.c2.xyz));
    
        public static float3 GetPosition(this in float4x4 trs) =>
            new float3(trs.c3.x, trs.c3.y, trs.c3.z);

        public static float3 EulerAngles(this in quaternion quat) 
            => ((Quaternion) quat).eulerAngles;
    }


    public static class VectorConversions
    {
        // unity-style (Y for top)
        public static Vector3 To3D(this Vector2 xz, float y = 0f) => new Vector3(xz.x, y, xz.y);

        public static int3 To3D(this Vector2Int xz, int y = 0) => new int3(xz.x, y, xz.y);


        public static Vector2 To2D(this Vector3 xyz) => new Vector2(xyz.x, xyz.z);

        public static Vector2Int To2D(this int3 xyz) => new Vector2Int(xyz.x, xyz.z);

        // z-top
        public static float3 To3DZTop(this float2 xy, int z = 0) => new float3(xy.x, xy.y, z);
        public static int3 To3DZTop(this Vector2Int xy, int z = 0) => new int3(xy.x, xy.y, z);

        // converting to/from unity axis alignment
        public static int3 SwapYZ(this int3 v) => new int3(v.x, v.z, v.y);

        public static Vector3 SwapYZ(this Vector3 v) => new Vector3(v.x, v.z, v.y);

        public static Vector3 WithX(this Vector3 v, float x)
        {
            v.x = x;
            return v;
        }

        public static Vector3 WithY(this Vector3 v, float y)
        {
            v.y = y;
            return v;
        }

        public static Vector3 WithZ(this Vector3 v, float z)
        {
            v.z = z;
            return v;
        }
    }
}