using TeamSuneat.Data;
using TeamSuneat.Setting;
using TeamSuneat.UserInterface;
using UnityEngine;

namespace TeamSuneat
{
    /// <summary>
    /// ResourcesManager의 UI 관련 메서드들
    /// </summary>
    public partial class ResourcesManager
    {
        #region Floaty Text

        public static UIFloatyText SpawnFloatyText(string content, bool isFacingRight, Transform follow)
        {
            if (GameSetting.Instance.Play.HideUserInterface) { return null; }
            if (string.IsNullOrEmpty(content)) { return null; }

            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.IngameWorldSpace);
            if (canvasOrder == null) { return null; }

            GameObject spawnedObject = SpawnPrefab("UIFloatyText", canvasOrder.transform);
            if (spawnedObject == null) { return null; }

            UIFollowObject followObject = spawnedObject.GetComponent<UIFollowObject>();
            if (followObject != null)
            {
                followObject.IsWorldSpaceCanvas = true;
                followObject.Setup(follow);
            }

            UIFloatyText component = spawnedObject.GetComponent<UIFloatyText>();
            if (component != null)
            {
                component.Setup(content, UIFloatyMoveNames.Content);
                component.SetFacingRight(isFacingRight);
                component.StartMove();
            }

            return component;
        }

        public static UIFloatyText SpawnFloatyText(string content, UIFloatyMoveNames moveName, Transform follow)
        {
            if (GameSetting.Instance.Play.HideUserInterface) { return null; }
            if (string.IsNullOrEmpty(content)) { return null; }

            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.IngameWorldSpace);
            if (canvasOrder == null) { return null; }

            GameObject spawnedObject = SpawnPrefab("UIFloatyText", canvasOrder.transform);
            if (spawnedObject == null) { return null; }

            UIFollowObject followObject = spawnedObject.GetComponent<UIFollowObject>();
            if (followObject != null)
            {
                followObject.IsWorldSpaceCanvas = true;
                followObject.Setup(follow);
            }

            UIFloatyText component = spawnedObject.GetComponent<UIFloatyText>();
            if (component != null)
            {
                component.Setup(content, moveName);
                component.StartMove();
            }

            return component;
        }

        public static UIFloatyText SpawnCurrencyFloatyText(CurrencyNames currencyName, int amount, Transform follow)
        {
            if (GameSetting.Instance.Play.HideUserInterface) { return null; }
            if (currencyName == CurrencyNames.None) { return null; }

            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.IngameWorldSpace);
            if (canvasOrder == null) { return null; }

            GameObject spawnedObject = SpawnPrefab("UIFloatyText", canvasOrder.transform);
            if (spawnedObject == null) { return null; }

            UIFollowObject followObject = spawnedObject.GetComponent<UIFollowObject>();
            if (followObject != null)
            {
                followObject.IsWorldSpaceCanvas = true;
                followObject.Setup(follow);
            }

            UIFloatyText component = spawnedObject.GetComponent<UIFloatyText>();
            if (component != null)
            {
                string format = JsonDataManager.FindStringClone($"Currency_Format_{currencyName}");
                string content = string.Format(format, amount);

                UIFloatyMoveNames moveName = UIFloatyMoveNames.Content;
                if (currencyName == CurrencyNames.Gold) moveName = UIFloatyMoveNames.Gold;
                else if (currencyName == CurrencyNames.Gem) moveName = UIFloatyMoveNames.Gem;

                component.Setup(content, moveName);
                component.StartMove();
            }

            return component;
        }

        public static UIFloatyText SpawnUIFloatyText(string content, UIFloatyMoveNames moveName)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.Notice);
            if (canvasOrder == null)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIFloatyText", canvasOrder.transform);

            if (spawnedObject == null)
            {
                return null;
            }

            UIFollowObject followObject = spawnedObject.GetComponent<UIFollowObject>();
            if (followObject != null)
            {
                followObject.IsWorldSpaceCanvas = false;
            }

            UIFloatyText component = spawnedObject.GetComponent<UIFloatyText>();
            if (component != null)
            {
                component.Setup(content, moveName);
            }

            component.rectTransform.anchoredPosition3D = new Vector3(GameDefine.DEFAULT_SCREEN_WIDTH * 0.5f, GameDefine.DEFAULT_SCREEN_HEIGHT * 0.5f);

            return component;
        }

        #endregion Floaty Text

        #region Notice

        public static UIStageTitleNotice SpawnStageTitleNotice(StageNames stageName)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }

            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.Notice);
            if (canvasOrder == null)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIStageTitleNotice", canvasOrder.transform);
            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UIStageTitleNotice notice = spawnedObject.GetComponent<UIStageTitleNotice>();
            notice?.Show(stageName);

            return notice;
        }

        public static UIStageTitleNotice SpawnStageTitleNotice(string content)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }

            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.Notice);
            if (canvasOrder == null)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIStageTitleNotice", canvasOrder.transform);
            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UIStageTitleNotice notice = spawnedObject.GetComponent<UIStageTitleNotice>();
            notice?.Show(content);

            return notice;
        }

        public static UITurnNotice SpawnTurnNotice(TurnNoticeOwner owner)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }

            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.Notice);
            if (canvasOrder == null)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UITurnNotice", canvasOrder.transform);
            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();

            UITurnNotice notice = spawnedObject.GetComponent<UITurnNotice>();
            notice?.Show(owner);

            return notice;
        }

        #endregion Notice

        public static EnemyHealthShieldView SpawnMonsterGauge(Vital vital)
        {
            if (GameSetting.Instance.Play.HideUserInterface)
            {
                return null;
            }
            CanvasOrder canvasOrder = UIManager.Instance?.GetCanvas(CanvasOrderNames.IngameWorldSpace);
            if (canvasOrder == null)
            {
                return null;
            }

            GameObject spawnedObject = SpawnPrefab("UIMonsterGauge", canvasOrder.transform);
            if (spawnedObject == null)
            {
                return null;
            }

            return spawnedObject.GetComponent<EnemyHealthShieldView>();
        }

        internal static UIDetails SpawnDetails(UIDetailsNames detailsName, Transform parent)
        {
            return null;
        }

        internal static void SpawnDropPotion(Vector3 position)
        {
        }

        internal static UIDetails SpawnIngameDetails(UIDetailsNames detailsName, Transform toFollow)
        {
            return null;
        }

        public static UIPopup SpawnPopup(UIPopupNames popupName)
        {
            CanvasOrder popupCanvas = UIManager.Instance.GetCanvas(CanvasOrderNames.Popup);
            string prefabName = "UI" + popupName.ToString() + "Popup";
            GameObject spawnedObject = SpawnPrefab(prefabName, popupCanvas.transform);

            if (spawnedObject == null)
            {
                return null;
            }

            spawnedObject.ResetLocalTransform();
            return spawnedObject.GetComponent<UIPopup>();
        }
    }
}