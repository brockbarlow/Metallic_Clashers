﻿using UnityEngine;

namespace Combat
{
    using System.Collections;

    using UnityEngine.UI;

    public class ResultsScreen : MonoBehaviour
    {
        [SerializeField]
        private Text m_ResultText;
        [SerializeField]
        private Image m_ExpForgroundImage;
        [SerializeField]
        private Image m_ExpMidgroundImage;
        private RectTransform m_RectTransform;
        private Button m_EndSceneButton;

        [Space]

        public float animationTime;
        public float barFillTime;

        private void Awake()
        {

            //Test Area

            GameManager.self.playerData.playerLevelSystem.IsLeveledUp(5000);

            ////////////

            m_RectTransform = GetComponent<RectTransform>();
            m_RectTransform.anchoredPosition -= new Vector2(0, 1000);
            m_ExpForgroundImage.fillAmount = 0f;
            SetMidground(0);
            m_EndSceneButton = GetComponentInChildren<Button>();
            m_EndSceneButton.onClick.AddListener(() => { CombatManager.self.onCombatEnd.Invoke();});
        }

        [ContextMenu("Start animation")]
        private void ResultsScreenBegin()
        {
            StartCoroutine(ResultsScreenEnumerator(true));
        }

        private void SetMidground(float fillAmount)
        {
            m_ExpMidgroundImage.rectTransform.anchorMin = new Vector2(
                m_ExpForgroundImage.fillAmount, m_ExpMidgroundImage.rectTransform.anchorMin.y);
            m_ExpMidgroundImage.rectTransform.anchorMax = new Vector2(
                m_ExpMidgroundImage.rectTransform.anchorMin.x + 1, 
                m_ExpMidgroundImage.rectTransform.anchorMax.y);
            m_ExpMidgroundImage.fillAmount = fillAmount;
        }

        private IEnumerator ResultsScreenEnumerator(bool result)
        {   // Set Win Text
            m_ResultText.text = result ? "You Win!" : "You Lose...";

            var animationFraction = 1000/animationTime;

            // Animate the pannel in
            while (m_RectTransform.anchoredPosition.y < 0)
            {
                m_RectTransform.anchoredPosition += new Vector2(0, Time.deltaTime * animationFraction);
                yield return null;
            }
            m_RectTransform.anchoredPosition = Vector2.zero;    // Make it all good

            // Check to see how much of the bar needs to be filled.
            var forgroundFillAmount = (float)
                GameManager.self.playerData.playerLevelSystem.playerLevelInfo.currentExperience /
                GameManager.self.playerData.playerLevelSystem.playerLevelInfo.experienceRequired;

            // Find out how much needs to be filled per second
            var fillFraction = 1/animationTime;

            // Animate Bar filling
            while (m_ExpForgroundImage.fillAmount < forgroundFillAmount)
            {
                m_ExpForgroundImage.fillAmount += Time.deltaTime*fillFraction;
                yield return null;
            }

            // Get some experience
            GameManager.self.playerData.playerLevelSystem.IsLeveledUp(200);

            // Check to see how much the total bar needs 
            var midgroundFillAmount = (float)
                GameManager.self.playerData.playerLevelSystem.playerLevelInfo.currentExperience /
                GameManager.self.playerData.playerLevelSystem.playerLevelInfo.experienceRequired;

            var finalFill = midgroundFillAmount;
            // Take off how much is already filled in
            midgroundFillAmount -= forgroundFillAmount;

            fillFraction = midgroundFillAmount/barFillTime;

            // As long as the bar is in range and not greater than one, fill.
            float setamount = 0;
            while (m_ExpMidgroundImage.fillAmount < midgroundFillAmount &&
                m_ExpForgroundImage.fillAmount + m_ExpMidgroundImage.fillAmount < 1)
            {
                setamount += Time.deltaTime*fillFraction;
                SetMidground(setamount);
                yield return null;
            }

            while (m_ExpForgroundImage.fillAmount < finalFill)
            {
                m_ExpForgroundImage.fillAmount += Time.deltaTime*fillFraction;
                yield return null;
            }
    
        }

    }
}
