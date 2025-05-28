using System;
using UnityEngine;
using UnityEngine.UI;

namespace ui.windows
{
    public class ThemeSettings : MonoBehaviour
    {
        public event Action<int> BackgroundStyleChanged;

        public WindowAnimation window;

        public Toggle[] simpleBackToggles;
        public Toggle[] customBackToggles;
        public Toggle[] bgTypeToggles;
        public Toggle[] cardStyleToggles;

        public BackPreviewController previewCtrl;
        public DialogController dialog;

        public delegate void ThemeChanged();

        public static event ThemeChanged CardBackChanged;
        public static event ThemeChanged CardStyleChanged;

        private bool shown = false;
        private const float selectedToggleScale = 1.1f;
        private float themeIconScale = 1.3f;

        /// <summary>
        /// Subscribe to all controllers events
        /// </summary>
        private void EnableListeners()
        {
            foreach (var t in simpleBackToggles)
            {
                t.onValueChanged.AddListener((res) => { OnSimpleBackClicked(res); });
            }

            foreach (var t in customBackToggles)
            {
                t.onValueChanged.AddListener((res) => { OnCustomBackClicked(res); });
            }

            foreach (var t in bgTypeToggles)
            {
                t.onValueChanged.AddListener((res) => { OnBgChanged(res); });
            }

            foreach (var t in cardStyleToggles)
            {
                t.onValueChanged.AddListener((res) => { OnDeckStyleChanged(res); });
            }
        }

        /// <summary>
        /// Unsubscribe from all controllers events
        /// </summary>
        private void DisableListeners()
        {
            foreach (var t in simpleBackToggles)
            {
                t.onValueChanged.RemoveAllListeners();
            }

            foreach (var t in customBackToggles)
            {
                t.onValueChanged.RemoveAllListeners();
            }

            foreach (var t in bgTypeToggles)
            {
                t.onValueChanged.RemoveAllListeners();
            }

            foreach (var t in cardStyleToggles)
            {
                t.onValueChanged.RemoveAllListeners();
            }
        }

        public void Show()
        {
            if (shown)
                return;

            //disable game during theme settings menu
            KlondikeController.Instance.PauseGame();
            KlondikeController.Instance.SetObjectsVisibility(false);

            EnableListeners();

            RefreshBackPreviews();
            RefreshCustomBackPreviews();

            //apply settings data to controllers
            SelectBgToggle(GameSettings.Instance.Data.Background);
            SelectCardStyleToggle(GameSettings.Instance.Data.CardStyle);
            SelectBackToggle(GameSettings.Instance.Data.CardBack);

            BackgroundStyleChanged?.Invoke(GameSettings.Instance.Data.Background);

            gameObject.SetActive(true);
            window.Show();
            shown = true;
        }

        /// <summary>
        /// Refreshes custom back previews.
        /// </summary>
        private void RefreshCustomBackPreviews()
        {
            for (int i = 0; i < 4; i++)
            {
                RefreshCustomBackPreview(i);
            }
        }

        /// <summary>
        /// Apply sprite with given index to custom back toggle
        /// </summary>
        /// <param name="i">The sprite index.</param>
        private void RefreshCustomBackPreview(int i)
        {
            //if there is no custom back with given index - do nothing
            if (ThemeManager.GetCustomSprite(i) != null)
                customBackToggles[i].image.sprite = ThemeManager.GetCustomSprite(i);
        }

        /// <summary>
        /// Applies all simple back sprites to toggles
        /// </summary>
        private void RefreshBackPreviews()
        {
            foreach (var v in Enum.GetValues(typeof(CardsBackStyle))) 
                simpleBackToggles[(int)v].image.sprite = ThemeManager.GetSimpleSprite((int)v);
        }

        /// <summary>
        /// Selects the card style toggle and deselect all the others.
        /// </summary>
        /// <param name="index">Index.</param>
        private void SelectCardStyleToggle(int index)
        {
            foreach (var t in cardStyleToggles)
            {
                t.transform.localScale = Vector2.one;
            }

            DisableListeners();

            cardStyleToggles[index].isOn = true;
            cardStyleToggles[index].transform.localScale = Vector3.one * selectedToggleScale;

            EnableListeners();
        }

        private void SelectBgToggle(int index)
        {
            //set default scale to all the toggles
            foreach (var t in bgTypeToggles)
            {
                t.transform.localScale = Vector2.one;
            }

            DisableListeners();

            //select and highlight toggle with given index
            bgTypeToggles[index].isOn = true;
            bgTypeToggles[index].transform.localScale = Vector3.one * themeIconScale;

            EnableListeners();
        }

        /// <summary>
        /// Highlight background toggle with given index and set to default others
        /// </summary>
        /// <param name="index">Index.</param>
        private void SelectBackToggle(int index)
        {
            // set all simple back toggles to default
            foreach (var t in simpleBackToggles)
            {
                t.transform.localScale = Vector2.one;
            }

            // set all custom back toggles to default
            foreach (var t in customBackToggles)
            {
                t.transform.localScale = Vector2.one;
            }

            Toggle target;

            //choose from which array we select toggle
            if (index >= simpleBackToggles.Length)
            {
                target = customBackToggles[index - simpleBackToggles.Length];
            }
            else
            {
                target = simpleBackToggles[index];
            }

            DisableListeners();

            //select and highlight toggle with given index
            target.isOn = true;
            target.transform.localScale = Vector2.one * selectedToggleScale;

            EnableListeners();
        }


        /// <summary>
        /// Handles click on custom back toggle
        /// </summary>
        /// <param name="value">If set to <c>true</c> value.</param>
        private void OnCustomBackClicked(bool value)
        {
            if (!value)
                return;

            int foundId;

            if (GameSettings.GetSelectedToggleId(customBackToggles, out foundId))
            {
                if (ThemeManager.GetCustomSprite(foundId) == null)
                {
                    //if custom back sprite with given index is not created yet( its null) then create it
                    CreateBack(foundId);
                }
                else if (foundId + simpleBackToggles.Length != GameSettings.Instance.Data.CardBack)
                {
                    //if sprite already created and set we ask user if he wants to override it or just select
                    dialog.Show(LocalizationManager.Instance.GetText("replaceQuestion"), DialogType.YesNo, (yes) =>
                    {
                        if (yes)
                            //override current sprite
                            CreateBack(foundId);
                        else
                            // just select curret sprite
                            ApplyBackSetting(foundId + simpleBackToggles.Length);
                    }, LocalizationManager.Instance.GetText("replace"));
                }
            }
        }

        /// <summary>
        /// Handles click on simple back toggle
        /// </summary>
        /// <param name="value">If set to <c>true</c> value.</param>
        private void OnSimpleBackClicked(bool value)
        {
            if (!value)
                return;

            int foundId;

            if (GameSettings.GetSelectedToggleId(simpleBackToggles, out foundId))
            {
                ApplyBackSetting(foundId);
            }
        }

        /// <summary>
        /// Handles click on deck style toggle
        /// </summary>
        /// <param name="value">If set to <c>true</c> value.</param>
        private void OnDeckStyleChanged(bool value)
        {
            if (!value)
                return;

            int foundId;

            if (GameSettings.GetSelectedToggleId(cardStyleToggles, out foundId))
            {
                ApplyCardStyleSetting(foundId);
            }
        }

        private void OnBgChanged(bool value)
        {
            int foundId;
            if (GameSettings.GetSelectedToggleId(bgTypeToggles, out foundId))
            {
                if (foundId != GameSettings.Instance.Data.Background)
                {
                    GameSettings.Instance.TryToSetBg(foundId, (ok) =>
                    {
                        if (ok)
                        {
                            SelectBgToggle(foundId);
                        }
                        else
                            SelectBgToggle(GameSettings.Instance.Data.Background);
                    });
                }
                else
                {
                    //return prev value
                    SelectBgToggle(GameSettings.Instance.Data.Background);
                }
            }

            BackgroundStyleChanged?.Invoke(foundId);
        }

        /// <summary>
        /// Runs the process of creating new custom card back sprite and applies it, if creation was successful
        /// </summary>
        /// <param name="slotId">Slot identifier.</param>
        private void CreateBack(int slotId)
        {
            previewCtrl.CreateBack(
                (success, tex) =>
                {
                    if (success)
                    {
                        //if we successfully created a new card back sprite we must apply new texture to ThemeManager
                        // to make it available to other classes
                        ApplyCustomBackTexture(slotId, tex);

                        //apply new settings value 
                        ApplyBackSetting(slotId + simpleBackToggles.Length);
                    }
                    else
                    {
                        //return prev value
                        SelectBackToggle(GameSettings.Instance.Data.CardBack);
                    }
                }
            );
        }

        /// <summary>
        /// Tries to apply new card back setting
        /// </summary>
        /// <param name="i">Custom back sprite index.</param>
        /// <param name="tex">New texture.</param>
        private void ApplyCustomBackTexture(int i, Texture2D tex)
        {
            //save image to persistent path
            string path = System.IO.Path.Combine(Application.persistentDataPath,
                string.Format(ThemeManager.CUSTOM_BACK_TEMPL, i));
            System.IO.File.WriteAllBytes(path, tex.EncodeToPNG());

            //apply texture to ThemeManager
            ThemeManager.SetCustomSprite(i,
                Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2f, 180, 0,
                    SpriteMeshType.FullRect));

            //refresh toggle button
            RefreshCustomBackPreview(i);
        }

        /// <summary>
        /// Tries to apply new background style setting
        /// </summary>
        /// <param name="i">The index.</param>
        private void ApplyBackSetting(int i)
        {
            //check if new value can be set 
            GameSettings.Instance.TryToSetCardBack(i, (success) =>
            {
                if (success)
                {
                    //only if success we change UI controller state
                    SelectBackToggle(i);
                    //if yes raise an event to notify subscribers about settings change
                    RaiseEvent(CardBackChanged);
                }
                else
                    //return prev value
                    SelectBackToggle(GameSettings.Instance.Data.CardBack);
            });
        }

        /// <summary>
        /// Tries to apply new card style setting
        /// </summary>
        /// <param name="i">The index.</param>
        private void ApplyCardStyleSetting(int i)
        {
            //check if new value can be set 
            GameSettings.Instance.TryToSetCardStyle(i, (success) =>
            {
                if (success)
                {
                    //only if success we change UI controller state
                    SelectCardStyleToggle(i);
                    //if yes raise an event to notify subscribers about settings change
                    RaiseEvent(CardStyleChanged);
                }
                else
                    //return prev value
                    SelectCardStyleToggle(GameSettings.Instance.Data.CardStyle);
            });
        }

        private bool RaiseEvent(ThemeChanged _delegate)
        {
            if (_delegate == null)
                return false;

            _delegate();
            return true;
        }

        public void Hide()
        {
            if (!shown)
                return;

            DisableListeners();

            KlondikeController.Instance.ResumeGame();
            KlondikeController.Instance.SetObjectsVisibility(true);

            GameSettings.Instance.Save();

            window.Hide(() => { gameObject.SetActive(false); });
            shown = false;
        }
    }
}