using UnityEngine;

public static class MathUtilities {
    public static Vector3 Random(Vector3 min, Vector3 max) {
       return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
    }

    public static float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return 180f - Mathf.Atan2(a.z - b.z, a.x - b.x) * Mathf.Rad2Deg;
    }

    public static Vector3 RandomOffset(Vector3 startPos, Vector3 offset) {
        float randomX = UnityEngine.Random.Range(startPos.x - offset.x, startPos.x + offset.x);
        float randomY = UnityEngine.Random.Range(startPos.y - offset.y, startPos.y + offset.y);
        float randomZ = UnityEngine.Random.Range(startPos.z - offset.z, startPos.z + offset.z);
        return new Vector3(randomX, randomY, randomZ);
    }
}
