public static class Utils
{
    public static int[] GetRandomIndexs(int maxCount)
    {
        int[] defaults = new int[maxCount];
        int[] results  = new int[maxCount];

        for(int i = 0; i < maxCount; i++)
        {
            defaults[i] = i;
        }

        for (int i = 0; i < maxCount; i++)
        {
            int index       = UnityEngine.Random.Range(0, maxCount);

            results[i]      = defaults[index];
            defaults[index] = defaults[maxCount - 1];
            maxCount--;
        }

        return results;
    }
}