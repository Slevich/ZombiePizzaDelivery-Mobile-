using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public static class AxesSelector
{
    public static float ReturnPositionByAxis (Axes axis, Vector3 position)
    {
        switch (axis)
        {
            case Axes.X:
            return position.x;

            case Axes.Y:
            return position.y;

            case Axes.Z:
            return position.z;

            default:
            return 0;
        }
    }

    public static Vector3 ReturnVectorWithPositionByAxis (Axes axis, Vector3 mainVector, Vector3 vectorWithAxisPosition)
    {
        switch (axis)
        {
            case Axes.X:
                return new Vector3(vectorWithAxisPosition.x, mainVector.y, mainVector.z);

            case Axes.Y:
                return new Vector3(mainVector.x, vectorWithAxisPosition.y, mainVector.z);

            case Axes.Z:
                return new Vector3(mainVector.x, mainVector.y, vectorWithAxisPosition.z);

            default:
                return Vector3.zero;
        }
    }

    public static Vector3 ReturnVector (Axes axis, (Vector3 onXSelection, Vector3 onYSelection, Vector3 onZSelection) vectors)
    {
        switch (axis)
        {
            case Axes.X:
                return vectors.onXSelection;

            case Axes.Y:
                return vectors.onYSelection;

            case Axes.Z:
                return vectors.onZSelection;

            default:
                return Vector3.zero;
        }
    }

    public static Vector3 ReturnVectorWithPlusPositionByAxis (Axes axis, Vector3 mainVector, float PlusPosition)
    {
        switch (axis)
        {
            case Axes.X:
                return new Vector3(mainVector.x + PlusPosition, mainVector.y, mainVector.z);

            case Axes.Y:
                return new Vector3(mainVector.x, mainVector.y + PlusPosition, mainVector.z);

            case Axes.Z:
                return new Vector3(mainVector.x, mainVector.y, mainVector.z + PlusPosition);

            default:
                return Vector3.zero;
        }
    }
}