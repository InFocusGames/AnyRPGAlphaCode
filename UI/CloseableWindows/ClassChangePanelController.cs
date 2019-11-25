using AnyRPG;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace AnyRPG {
    public class ClassChangePanelController : WindowContentController {

        public event System.Action OnConfirmAction = delegate { };
        public override event Action<ICloseableWindowContents> OnCloseWindow = delegate { };

        [SerializeField]
        private GameObject rewardIconPrefab;

        [SerializeField]
        private CharacterClassButton characterClassButton;

        [SerializeField]
        private GameObject abilitiesArea;

        [SerializeField]
        private GameObject abilityIconsArea;

        [SerializeField]
        private GameObject traitsArea;

        [SerializeField]
        private GameObject traitIconsArea;

        private List<RewardButton> abilityRewardIcons = new List<RewardButton>();

        private List<RewardButton> traitRewardIcons = new List<RewardButton>();

        private string characterClassName;

        public void Setup(string newCharacterClassName) {
            //Debug.Log("ClassChangePanelController.Setup(" + newClassName + ")");
            characterClassName = newCharacterClassName;
            characterClassButton.AddCharacterClass(characterClassName);
            PopupWindowManager.MyInstance.classChangeWindow.SetWindowTitle(characterClassName);
            ShowAbilityRewards();
            ShowTraitRewards();
            PopupWindowManager.MyInstance.classChangeWindow.OpenWindow();
        }

        public void ShowTraitRewards() {
            //Debug.Log("ClassChangePanelController.ShowTraitRewards()");

            ClearTraitRewardIcons();
            // show trait rewards
            CharacterClass characterClass = SystemCharacterClassManager.MyInstance.GetResource(characterClassName);
            if (characterClass.MyTraitList.Count > 0) {
                traitsArea.gameObject.SetActive(true);
            } else {
                traitsArea.gameObject.SetActive(false);
            }
            for (int i = 0; i < characterClass.MyTraitList.Count; i++) {
                if (characterClass.MyTraitList[i] != null && characterClass.MyTraitList[i] != string.Empty) {
                    AbilityEffect abilityEffect = SystemAbilityEffectManager.MyInstance.GetResource(characterClass.MyTraitList[i]);
                    if (abilityEffect != null) {
                        RewardButton rewardIcon = Instantiate(rewardIconPrefab, traitIconsArea.transform).GetComponent<RewardButton>();
                        rewardIcon.SetDescribable(abilityEffect);
                        traitRewardIcons.Add(rewardIcon);
                        if ((SystemAbilityEffectManager.MyInstance.GetResource(characterClass.MyTraitList[i]) as StatusEffect).MyRequiredLevel > PlayerManager.MyInstance.MyCharacter.MyCharacterStats.MyLevel) {
                            rewardIcon.MyStackSizeText.text = "Level\n" + (SystemAbilityEffectManager.MyInstance.GetResource(characterClass.MyTraitList[i]) as StatusEffect).MyRequiredLevel;
                            rewardIcon.MyHighlightIcon.color = new Color32(255, 255, 255, 80);
                        }
                    }
                }
            }
        }

        public void ShowAbilityRewards() {
            //Debug.Log("ClassChangePanelController.ShowAbilityRewards()");

            ClearRewardIcons();
            // show ability rewards
            CharacterClass characterClass = SystemCharacterClassManager.MyInstance.GetResource(characterClassName);
            if (characterClass.MyAbilityList.Count > 0) {
                abilitiesArea.gameObject.SetActive(true);
            } else {
                abilitiesArea.gameObject.SetActive(false);
            }
            for (int i = 0; i < characterClass.MyAbilityList.Count; i++) {
                if (characterClass.MyAbilityList[i] != null && characterClass.MyAbilityList[i] != string.Empty) {
                    BaseAbility baseAbility = SystemAbilityManager.MyInstance.GetResource(characterClass.MyAbilityList[i]);
                    if (baseAbility != null) {
                        RewardButton rewardIcon = Instantiate(rewardIconPrefab, abilityIconsArea.transform).GetComponent<RewardButton>();
                        rewardIcon.SetDescribable(baseAbility);
                        abilityRewardIcons.Add(rewardIcon);
                        if (SystemAbilityManager.MyInstance.GetResource(characterClass.MyAbilityList[i]).MyRequiredLevel > PlayerManager.MyInstance.MyCharacter.MyCharacterStats.MyLevel) {
                            rewardIcon.MyStackSizeText.text = "Level\n" + SystemAbilityManager.MyInstance.GetResource(characterClass.MyAbilityList[i]).MyRequiredLevel;
                            rewardIcon.MyHighlightIcon.color = new Color32(255, 255, 255, 80);
                        }
                    }
                }
            }
        }

        private void ClearTraitRewardIcons() {
            //Debug.Log("ClassChangePanelController.ClearRewardIcons()");

            foreach (RewardButton rewardIcon in traitRewardIcons) {
                Destroy(rewardIcon.gameObject);
            }
            traitRewardIcons.Clear();
        }

        private void ClearRewardIcons() {
            //Debug.Log("ClassChangePanelController.ClearRewardIcons()");

            foreach (RewardButton rewardIcon in abilityRewardIcons) {
                Destroy(rewardIcon.gameObject);
            }
            abilityRewardIcons.Clear();
        }

        public void CancelAction() {
            //Debug.Log("ClassChangePanelController.CancelAction()");
            PopupWindowManager.MyInstance.classChangeWindow.CloseWindow();
        }

        public void ConfirmAction() {
            //Debug.Log("ClassChangePanelController.ConfirmAction()");
            PlayerManager.MyInstance.SetPlayerCharacterClass(characterClassName);
            OnConfirmAction();
            PopupWindowManager.MyInstance.classChangeWindow.CloseWindow();
        }

        public override void ReceiveOpenWindowNotification() {
            //Debug.Log("ClassChangePanelController.OnOpenWindow()");
            base.ReceiveOpenWindowNotification();
        }

        public override void RecieveClosedWindowNotification() {
            //Debug.Log("ClassChangePanelController.OnCloseWindow()");
            base.RecieveClosedWindowNotification();
            OnCloseWindow(this);
        }
    }

}