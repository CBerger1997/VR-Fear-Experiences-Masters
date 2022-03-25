using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RoomAllocation
{
    /// <summary>
    /// Allows for the creation of vector lists to define geometries
    /// </summary>
    public static class TestAreaCreator
    {
        /// <summary>
        /// Creates a test area object with the specified values.
        /// Also renders spheres for each geometry point if enabled.
        /// </summary>
        /// <param name="geometry">The points defining the geometry</param>
        /// <param name="areaType">The name of the area</param>
        /// <param name="width">The width of the area</param>
        /// <param name="height">The height of the area</param>
        /// <param name="renderPoints">If the test area should render test spheres for each geometry point</param>
        /// <returns></returns>
        private static List<Vector3> CreateAreaGeometry(List<Vector3> geometry, string areaType, float width, float height, bool renderPoints)
        {
            GameObject testArea = new GameObject("" + width + "x" + height + " " + areaType + " Test Area");
            testArea.tag = AllocationConstants.TESTAREA_TAG_NAME;
            if (renderPoints)
            {
                //Create test spheres for visualisation if enabled
                foreach (Vector3 vec in geometry)
                {
                    GameObject testSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    testSphere.transform.parent = testArea.transform;
                    testSphere.transform.position = vec;
                    testSphere.transform.localScale *= 0.1f;
                }
            }

            return geometry;
        }

        /// <summary>
        /// Creates a geometry in the shape of a sqaure
        /// </summary>
        /// <param name="numberOfPoints">How many geometry points need to be defined</param>
        /// <param name="origin">The origin vector of the area</param>
        /// <param name="size">The size of each side of the square</param>
        /// <param name="renderPoints">If the test area should render test spheres for each geometry point</param>
        /// <returns>A list of all generated geometry points</returns>
        public static List<Vector3> CreateSquareGeometry(int numberOfPoints, Vector3 origin, float size, bool renderPoints)
        {
            //Ensure that number of points is not 0 to prevent error
            if (numberOfPoints > 0)
            {
                List<Vector3> points = new List<Vector3>();
                Vector3 bottomLeft = new Vector3(origin.x, origin.y, origin.z + size);
                Vector3 bottomRight = new Vector3(origin.x + size, origin.y, origin.z + size);
                Vector3 topRight = new Vector3(origin.x + size, origin.y, origin.z);
                //Each side is equal in size
                int pointsPerSide = numberOfPoints / 4;
                float offset = size / pointsPerSide;
                points.AddRange(InterpolatePoints(pointsPerSide, origin, topRight, offset));
                points.AddRange(InterpolatePoints(pointsPerSide, topRight, bottomRight, offset));
                points.AddRange(InterpolatePoints(pointsPerSide, bottomRight, bottomLeft, offset));
                points.AddRange(InterpolatePoints(pointsPerSide, bottomLeft, origin, offset));
                //Create the area and render
                return CreateAreaGeometry(points, "Square", size, size, renderPoints);
            }
            else
            {
                //Incorrect number of points have been provided to the function
                Debug.LogError("Number of Points = 0 when creating Square Geometry");
                return null;
            }
        }

        /// <summary>
        /// Creates a geometry in the shape of a rectangle
        /// </summary>
        /// <param name="numberOfPoints">How many geometry points need to be defined</param>
        /// <param name="origin">The origin vector of the area</param>
        /// <param name="width">The width of the rectangle in x</param>
        /// <param name="height">The height of the rectangle in z</param>
        /// <param name="renderPoints">If the test area should render test spheres for each geometry point</param>
        /// <returns>A list of all generated geometry points</returns>
        public static List<Vector3> CreateRectGeometry(int numberOfPoints, Vector3 origin, float width, float height, bool renderPoints)
        {
            //Ensure that number of points is not 0 to prevent error
            if (numberOfPoints > 0)
            {
                List<Vector3> points = new List<Vector3>();
                Vector3 bottomLeft = new Vector3(origin.x, origin.y, origin.z + height);
                Vector3 bottomRight = new Vector3(origin.x + width, origin.y, origin.z + height);
                Vector3 topRight = new Vector3(origin.x + width, origin.y, origin.z);
                //Calculate side ratios to get number of points
                float widthRatio = width / (width + height);
                float heightRatio = height / (width + height);
                int pointsPerHorizontal = Mathf.RoundToInt((numberOfPoints / 2) * widthRatio);
                int pointsPerVertical = Mathf.RoundToInt((numberOfPoints / 2) * heightRatio);
                float wOffset = width / pointsPerHorizontal;
                float hOffset = height / pointsPerVertical;

                points.AddRange(InterpolatePoints(pointsPerHorizontal, origin, topRight, wOffset));
                points.AddRange(InterpolatePoints(pointsPerVertical, topRight, bottomRight, hOffset));
                points.AddRange(InterpolatePoints(pointsPerHorizontal, bottomRight, bottomLeft, wOffset));
                points.AddRange(InterpolatePoints(pointsPerVertical, bottomLeft, origin, hOffset));
                //Create the area and render

                return CreateAreaGeometry(points, "Rect", width, height, renderPoints);
            }
            else
            {
                //This should not occur - if it does, something has gone wrong
                Debug.LogError("Number of Points = 0 when creating Rect Geometry");
                return null;
            }
        }

        /// <summary>
        /// Creates a geometry in the shape of a triangle
        /// </summary>
        /// <param name="numberOfPoints">How many geometry points need to be defined</param>
        /// <param name="origin">The origin vector of the area</param>
        /// <param name="width">The width of the triangle's base, in x</param>
        /// <param name="topPoint">The position of the top point of the triangle</param>
        /// <param name="renderPoints">If the test area should render test spheres for each geometry point</param>
        /// <returns></returns>
        public static List<Vector3> CreateTriangleGeometry(int numberOfPoints, Vector3 origin, float width, Vector3 topPoint, bool renderPoints)
        {
            //Ensure that number of points is not 0 to prevent error
            if (numberOfPoints > 0)
            {
                List<Vector3> points = new List<Vector3>();
                //Obtain the point on the opposite side of the triangle base from the origin
                Vector3 rightPoint = new Vector3(origin.x + width, origin.y, origin.z);
                //Find the relative sizes of each side
                float leftSideSize = Vector3.Distance(origin, topPoint);
                float rightSideSize = Vector3.Distance(rightPoint, topPoint);
                float totalSize = width + leftSideSize + rightSideSize;

                //Calculate side ratios to get number of points
                float baseRatio = width / totalSize;
                float leftRatio = leftSideSize / totalSize;
                float rightRatio = rightSideSize / totalSize;

                int pointsOnBase = Mathf.RoundToInt(numberOfPoints * baseRatio);
                int pointsOnLeft = Mathf.RoundToInt(numberOfPoints * leftRatio);
                int pointsOnRight = Mathf.RoundToInt(numberOfPoints * rightRatio);

                float baseOffset = width / pointsOnBase;
                float leftOffset = leftSideSize / pointsOnLeft;
                float rightOffset = rightSideSize / pointsOnRight;

                //Interpolate the points to create each side's vectors
                points.AddRange(InterpolatePoints(pointsOnLeft, origin, topPoint, leftOffset));
                points.AddRange(InterpolatePoints(pointsOnRight, topPoint, rightPoint, rightOffset));
                points.AddRange(InterpolatePoints(pointsOnBase, rightPoint, origin, baseOffset));
                //Create the area and render
                return CreateAreaGeometry(points, "Triangle", width, Mathf.Abs(topPoint.z - origin.z), renderPoints);
            }
            else
            {
                //This should not occur - if it does, something has gone wrong
                Debug.LogError("Number of Points = 0 when creating Triangle Geometry");
                return null;
            }
        }

        /// <summary>
        /// Creates and returns a series of vectors between two points, returning numberOfPoints amount of vectors
        /// </summary>
        /// <param name="numberOfPoints">How many geometry points need to be defined</param>
        /// <param name="vecA">The first vector</param>
        /// <param name="vecB">The second vector</param>
        /// <param name="offset">How far to move across towards vecB from the current position</param>
        /// <returns>A vector array of all points generated</returns>
        private static Vector3[] InterpolatePoints(int numberOfPoints, Vector3 vecA, Vector3 vecB, float offset)
        {
            if (numberOfPoints > 0)
            {
                Vector3[] points = new Vector3[numberOfPoints];
                int currentPoint = 0;
                while (currentPoint < numberOfPoints)
                {
                    points[currentPoint] = vecA;
                    vecA = Vector3.MoveTowards(vecA, vecB, offset);
                    currentPoint++;
                }
                return points;
            }
            return new Vector3[0];
        }
    }
}