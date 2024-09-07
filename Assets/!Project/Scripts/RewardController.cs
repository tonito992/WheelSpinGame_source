using UnityEngine;

namespace WheelSpinGame
{
    public class RewardController : MonoBehaviour
    {
        public static int GetWeightedRandomIndex(Reward[] rewards)
        {
            if (rewards == null || rewards.Length == 0)
                return -1;

            var totalProbability = 0f;
            var cumulative = new float[rewards.Length];

            for (var i = 0; i < rewards.Length; i++)
            {
                totalProbability += rewards[i].probability;
                cumulative[i] = totalProbability;
            }

            var randomValue = Random.Range(0f, totalProbability);

            for (var i = 0; i < cumulative.Length; i++)
            {
                if (randomValue < cumulative[i])
                {
                    return i;
                }
            }

            return -1;
        }
    }
}