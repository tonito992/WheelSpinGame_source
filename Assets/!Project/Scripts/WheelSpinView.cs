using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WheelSpinGame
{
    public class SpinWheelView : MonoBehaviour
    {
        [SerializeField] private RectTransform wheelRect;
        [SerializeField] private int numberOfSections = 5;
        [SerializeField] private float constantSpeed = 200f;
        [SerializeField] private float slowDownSpeed = 20f;
        [SerializeField] private Button spinButton;
        [SerializeField] private TMP_Text textSpinsLeft;
        [SerializeField] private TMP_Text textCoins;
        [SerializeField] private Reward[] rewards;

        private bool isSpinning;
        private float targetAngle;
        private int coins;
        private int spinsLeft;
        private float totalRotation;

        private void Setup()
        {
            this.spinsLeft = 5;  // Players start with 5 spins
            this.coins = 0;
            this.spinButton.gameObject.SetActive(true);
            this.UpdateUi();
            this.StartCoroutine(this.GrantFreeSpin());  // Start the coroutine to grant free spins every minute
        }

        private void UpdateUi()
        {
            this.textCoins.SetText($"Coins: {this.coins}");
            this.textSpinsLeft.SetText($"Spins: {this.spinsLeft}");
        }

        private IEnumerator Spin()
        {
            this.isSpinning = true;
            var currentSpeed = this.constantSpeed;

            while (true)
            {
                var rotationStep = Time.deltaTime * currentSpeed;
                this.wheelRect.Rotate(0, 0, -rotationStep);
                this.totalRotation -= rotationStep;

                if (this.totalRotation - this.targetAngle < (360f / this.numberOfSections) * 2)
                {
                    currentSpeed = this.slowDownSpeed;
                }

                if (this.totalRotation - this.targetAngle < 7.5f)
                {
                    currentSpeed = this.slowDownSpeed * .1f;
                }

                if (this.totalRotation - this.targetAngle < .25f)
                {
                    break;
                }

                yield return null;
            }

            this.FinaliseSpin();
        }

        private void FinaliseSpin()
        {
            this.isSpinning = false;
            var rewardIndex = this.GetCurrentWheelIndex();
            this.coins += this.rewards[rewardIndex].coinAmount;
            this.UpdateUi();
        }

        private int GetCurrentWheelIndex()
        {
            var degreesPerSection = 360f / this.numberOfSections;
            var index = Mathf.FloorToInt(NormalizeAngle(this.wheelRect.eulerAngles.z) / degreesPerSection) % this.numberOfSections;
            return index;
        }

        public void OnTapSpinButton()
        {
            if (this.isSpinning) return;

            if (this.spinsLeft > 0)
            {
                this.spinsLeft--;
            }
            else if (this.coins >= 100)
            {
                this.coins -= 100;
            }
            else
            {
                Debug.Log("Not enough spins or coins!");
                return;
            }

            var rewardIndex = RewardController.GetWeightedRandomIndex(this.rewards);
            var wheelTargetIndex = this.numberOfSections - rewardIndex + this.GetCurrentWheelIndex();

            var degreesPerSection = 360f / this.numberOfSections;
            this.totalRotation = this.wheelRect.eulerAngles.z;
            this.targetAngle = this.totalRotation - (3 * 360f) - (degreesPerSection * wheelTargetIndex);
            this.StartCoroutine(this.Spin());
            this.UpdateUi();
        }

        private static float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle < 0) angle += 360f;
            return angle;
        }

        private void Awake()
        {
            this.Setup();
        }

        private IEnumerator GrantFreeSpin()
        {
            while (true)
            {
                yield return new WaitForSeconds(60);
                this.spinsLeft++;
                this.UpdateUi();
            }
        }
    }
}
