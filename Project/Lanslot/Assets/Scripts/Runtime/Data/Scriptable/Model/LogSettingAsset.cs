using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace TeamSuneat
{
    [CreateAssetMenu(fileName = "LogSetting", menuName = "TeamSuneat/Scriptable/LogSetting")]
    public class LogSettingAsset : ScriptableObject
    {
        [SuffixLabel("애널리틱스")] public bool Analytics;

        [FoldoutGroup("[캐릭터]")][SuffixLabel("캐릭터")] public bool Character;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("플레이어 캐릭터")] public bool Player;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("몬스터 캐릭터")] public bool Monster;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("엘리트 캐릭터")] public bool Elite;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("보스캐릭터")] public bool Boss;

        [FoldoutGroup("[캐릭터]")][SuffixLabel("캐릭터 생성")] public bool CharacterSpawn;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("캐릭터 웨이브")] public bool CharacterWave;
        [FoldoutGroup("[캐릭터]")][SuffixLabel("캐릭터 능력")] public bool Ability;

        [FoldoutGroup("[캐릭터 랜더러]")][SuffixLabel("애니메이션")] public bool Animation;
        [FoldoutGroup("[캐릭터 랜더러]")][SuffixLabel("랜더러")] public bool Renderer;

        [FoldoutGroup("[전투]")][SuffixLabel("공격")] public bool Attack;
        [FoldoutGroup("[전투]")][SuffixLabel("공격(목표 공격)")] public bool Attack_Target;
        [FoldoutGroup("[전투]")][SuffixLabel("피해량 계산")] public bool Damage;
        [FoldoutGroup("[전투]")][SuffixLabel("패시브")] public bool Passive;
        [FoldoutGroup("[전투]")][SuffixLabel("패시브 발동")] public bool PassiveTrigger;
        [FoldoutGroup("[전투]")][SuffixLabel("버프")] public bool Buff;
        [FoldoutGroup("[전투]")][SuffixLabel("상태")] public bool State;
        [FoldoutGroup("[전투]")][SuffixLabel("능력치")] public bool Stat;
        [FoldoutGroup("[전투]")][SuffixLabel("전투자원")] public bool BattleResource;
        [FoldoutGroup("[전투]")][SuffixLabel("캐릭터 전투자원")] public bool Vital;
        [FoldoutGroup("[전투]")][SuffixLabel("탐지")] public bool Detect;

        [FoldoutGroup("[기술]")][SuffixLabel("기술")] public bool Skill;

        [FoldoutGroup("[아이템]")][SuffixLabel("아이템")] public bool Item;
        [FoldoutGroup("[아이템 종류]")][SuffixLabel("무기")] public bool Weapon;
        [FoldoutGroup("[아이템 종류]")][SuffixLabel("물약")] public bool Potion;
        [FoldoutGroup("[아이템]")][SuffixLabel("드랍 오브젝트")] public bool DropObject;

        [FoldoutGroup("[해금]")][SuffixLabel("퀘스트")] public bool Quest;
        [FoldoutGroup("[해금]")][SuffixLabel("퀘스트 조건")] public bool QuestCondition;
        [FoldoutGroup("[해금]")][SuffixLabel("튜토리얼")] public bool Tutorial;
        [FoldoutGroup("[해금]")][SuffixLabel("난이도")] public bool Difficulty;

        [FoldoutGroup("[인게임]")][SuffixLabel("이펙트")] public bool Effect;
        [FoldoutGroup("[인게임]")][SuffixLabel("타임라인")] public bool Timeline;
        [FoldoutGroup("[인게임]")][SuffixLabel("포지션 그룹")] public bool PositionGroup;

        [FoldoutGroup("[데이터]")][SuffixLabel("데이터")] public bool Data;
        [FoldoutGroup("[데이터]")][SuffixLabel("Json 데이터")] public bool JsonData;
        [FoldoutGroup("[데이터]")][SuffixLabel("스크립터블 데이터")] public bool ScriptableData;
        [FoldoutGroup("[데이터]")][SuffixLabel("임시 데이터")] public bool GamePref;
        [FoldoutGroup("[데이터]")][SuffixLabel("리소스")] public bool Resource;
        [FoldoutGroup("[데이터]")][SuffixLabel("경로")] public bool Path;

        [FoldoutGroup("[세이브 데이터]")][SuffixLabel("게임 데이터")] public bool GameData;
        [FoldoutGroup("[세이브 데이터]")][SuffixLabel("게임 데이터 - 캐릭터")] public bool GameData_Character;
        [FoldoutGroup("[세이브 데이터]")][SuffixLabel("게임 데이터 - 기술")] public bool GameData_Skill;
        [FoldoutGroup("[세이브 데이터]")][SuffixLabel("게임 데이터 - 무기")] public bool GameData_Weapon;

        [FoldoutGroup("[세이브 데이터 - 인벤토리]")][SuffixLabel("재화")] public bool Currency;
        [FoldoutGroup("[세이브 데이터 - 인벤토리]")][SuffixLabel("소지품")] public bool Inventory;
        [FoldoutGroup("[세이브 데이터 - 인벤토리]")][SuffixLabel("창고")] public bool Storage;

        [FoldoutGroup("[게임 설정]")][SuffixLabel("설정")] public bool Setting;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("음향")] public bool Audio;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("비디오")] public bool Video;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("카메라")] public bool Camera;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("입력")] public bool Input;
        [FoldoutGroup("[게임 설정]")][SuffixLabel("마우스 커서")] public bool Cursor;

        [FoldoutGroup("[지역]")][SuffixLabel("스테이지")] public bool Stage;
        [FoldoutGroup("[지역]")][SuffixLabel("스테이지 몬스터")] public bool Stage_Monster;
        [FoldoutGroup("[지역]")][SuffixLabel("타일")] public bool Tile;
        [FoldoutGroup("[지역]")][SuffixLabel("턴 관리")] public bool Turn;

        [FoldoutGroup("[글로벌]")][SuffixLabel("글로벌 이벤트")] public bool Global;
        [FoldoutGroup("[글로벌]")][SuffixLabel("스트링 텍스트")] public bool String;
        [FoldoutGroup("[글로벌]")][SuffixLabel("개발용")] public bool Develop;

        [FoldoutGroup("[UI]")] public bool UI;
        [FoldoutGroup("[UI]")][SuffixLabel("버튼")] public bool UI_Button;
        [FoldoutGroup("[UI]")][SuffixLabel("게이지")] public bool UI_Gauge;
        [FoldoutGroup("[UI]")][SuffixLabel("팝업")] public bool UI_Popup;
        [FoldoutGroup("[UI]")][SuffixLabel("상세정보")] public bool UI_Details;
        [FoldoutGroup("[UI]")][SuffixLabel("소지품")] public bool UI_Inventory;        
        [FoldoutGroup("[UI]")][SuffixLabel("선택 이벤트")] public bool UI_SelectEvent;
        [FoldoutGroup("[UI]")][SuffixLabel("탭")] public bool UI_Tab;
        [FoldoutGroup("[UI]")][SuffixLabel("기술")] public bool UI_Skill;
        [FoldoutGroup("[UI]")][SuffixLabel("슬롯머신")] public bool UI_SlotMachine;

        public bool LoadString;

        #region FOR EDITOR

        [FoldoutGroup("#Button")]
        [Button("Switch On All", ButtonSizes.Large)]
        public void ExternSwitchOnAll()
        {
            SwitchOnAll();
        }

        [FoldoutGroup("#Button")]
        [Button("Switch Off All", ButtonSizes.Large)]
        public void ExternSwitchOffAll()
        {
            SwitchOffAll();
        }

        #endregion FOR EDITOR

        private void SwitchOnAll()
        {
            Analytics = true;
            Ability = true;

            Animation = true;
            Renderer = true;

            Attack = true;
            Attack_Target = true;
            Audio = true;
            Video = true;

            Buff = true;

            Camera = true;
            Character = true;
            Player = true;
            Monster = true;
            Elite = true;
            Boss = true;

            CharacterSpawn = true;
            CharacterWave = true;

            Damage = true;
            Detect = true;
            Data = true;
            GamePref = true;

            GameData = true;
            GameData_Character = true;
            Quest = true;
            QuestCondition = true;
            Tutorial = true;
            Storage = true;
            Inventory = true;
            GameData_Skill = true;
            GameData_Weapon = true;

            Develop = true;
            Currency = true;

            Effect = true;
            Global = true;

            Item = true;
            DropObject = true;

            Cursor = true;
            Input = true;
            JsonData = true;
            ScriptableData = true;

            Passive = true;
            PassiveTrigger = true;
            Potion = true;
            Resource = true;
            Path = true;

            Setting = true;
            Skill = true;

            Stage = true;
            Stage_Monster = true;
            Turn = true;

            Difficulty = true;

            Stat = true;
            State = true;
            String = true;

            Tile = true;
            Timeline = true;
            PositionGroup = true;

            UI = true;
            UI_Button = true;
            UI_Gauge = true;
            UI_Popup = true;
            UI_Details = true;
            UI_Inventory = true;
            UI_SelectEvent = true;
            UI_Tab = true;

            Vital = true;
            BattleResource = true;
            Weapon = true;

            UI_Skill = true;
            UI_SlotMachine = true;
        }

        private void SwitchOffAll()
        {
            Analytics = false;
            Ability = false;

            Animation = false;
            Renderer = false;

            Attack = false;
            Attack_Target = false;
            Audio = false;
            Video = false;

            Buff = false;

            Camera = false;
            Character = false;
            Player = false;
            Monster = false;
            Elite = false;
            Boss = false;
            CharacterSpawn = false;
            CharacterWave = false;

            Damage = false;
            Detect = false;
            Data = false;

            GamePref = false;

            GameData = false;
            GameData_Character = false;
            Quest = false;
            QuestCondition = false;
            Tutorial = false;
            Storage = false;
            Inventory = false;

            GameData_Skill = false;
            GameData_Weapon = false;

            Difficulty = false;

            Develop = false;
            Currency = false;

            Effect = false;
            Global = false;

            Item = false;
            DropObject = false;

            Cursor = false;
            Input = false;
            JsonData = false;
            ScriptableData = false;

            Passive = false;
            PassiveTrigger = false;
            Potion = false;
            Resource = false;
            Path = false;

            Setting = false;
            Skill = false;

            Stage = false;
            Stage_Monster = false;
            Turn = false;

            Stat = false;
            State = false;
            String = false;

            Tile = false;
            Timeline = false;
            PositionGroup = false;

            UI = false;
            UI_Button = false;
            UI_Gauge = false;
            UI_Popup = false;
            UI_Details = false;
            UI_Inventory = false;
            UI_SelectEvent = false;
            UI_Tab = false;

            Vital = false;
            BattleResource = false;
            Weapon = false;

            UI_Skill = false;
            UI_SlotMachine = false;
        }

        public void OnLoadData()
        {
#if !UNITY_EDITOR
            SwitchOnAll();
#endif
        }

        public bool Find(LogTags logTag)
        {
            return logTag switch
            {
                LogTags.Analytics => Analytics,

                LogTags.Ability => Ability,

                LogTags.Animation => Animation,
                LogTags.Renderer => Renderer,

                LogTags.Attack => Attack,
                LogTags.Attack_Target => Attack_Target,
                LogTags.Audio => Audio,
                LogTags.Video => Video,

                LogTags.Buff => Buff,

                LogTags.Camera => Camera,
                LogTags.Character => Character,
                LogTags.Player => Player,
                LogTags.Monster => Monster,
                LogTags.Elite => Elite,
                LogTags.Boss => Boss,

                LogTags.CharacterSpawn => CharacterSpawn,
                LogTags.CharacterWave => CharacterWave,
                LogTags.Damage => Damage,
                LogTags.Detect => Detect,

                LogTags.Data => Data,
                LogTags.JsonData => JsonData,
                LogTags.ScriptableData => ScriptableData,
                LogTags.GamePref => GamePref,

                LogTags.GameData => GameData,
                LogTags.GameData_Character => GameData_Character,
                LogTags.Quest => Quest,
                LogTags.QuestCondition => QuestCondition,
                LogTags.Tutorial => Tutorial,
                LogTags.Storage => Storage,
                LogTags.Inventory => Inventory,

                LogTags.GameData_Skill => GameData_Skill,
                LogTags.GameData_Weapon => GameData_Weapon,

                LogTags.Difficulty => Difficulty,

                LogTags.Develop => Develop,
                LogTags.Effect => Effect,
                LogTags.Global => Global,

                LogTags.Currency => Currency,
                LogTags.Item => Item,
                LogTags.DropObject => DropObject,

                LogTags.Cursor => Cursor,
                LogTags.Input => Input,

                LogTags.Passive => Passive,
                LogTags.PassiveTrigger => PassiveTrigger,
                LogTags.Potion => Potion,
                LogTags.Resource => Resource,
                LogTags.Path => Path,

                LogTags.Setting => Setting,
                LogTags.Skill => Skill,

                LogTags.Stage => Stage,
                LogTags.Stage_Monster => Stage_Monster,
                LogTags.Turn => Turn,

                LogTags.Stat => Stat,
                LogTags.State => State,
                LogTags.String => String,
                LogTags.Tile => Tile,
                LogTags.Timeline => Timeline,
                LogTags.PositionGroup => PositionGroup,

                LogTags.UI => UI,
                LogTags.UI_Button => UI_Button,
                LogTags.UI_Gauge => UI_Gauge,
                LogTags.UI_Popup => UI_Popup,
                LogTags.UI_Details => UI_Details,
                LogTags.UI_Inventory => UI_Inventory,
                LogTags.UI_SelectEvent => UI_SelectEvent,
                LogTags.UI_Tab => UI_Tab,

                LogTags.Vital => Vital,
                LogTags.BattleResource => BattleResource,
                LogTags.Weapon => Weapon,

                LogTags.UI_Skill => UI_Skill,
                LogTags.UI_SlotMachine => UI_SlotMachine,

                _ => false,
            };
        }

        public void SwitchOn(LogTags logTag)
        {
            switch (logTag)
            {
                case LogTags.Analytics: { Analytics = true; } break;
                case LogTags.Ability: { Ability = true; } break;

                case LogTags.Animation: { Animation = true; } break;
                case LogTags.Renderer: { Renderer = true; } break;

                case LogTags.Attack: { Attack = true; } break;
                case LogTags.Attack_Target: { Attack_Target = true; } break;
                case LogTags.Audio: { Audio = true; } break;
                case LogTags.Video: { Video = true; } break;

                case LogTags.Buff: { Buff = true; } break;

                case LogTags.Camera: { Camera = true; } break;
                case LogTags.Character: { Character = true; } break;
                case LogTags.Player: { Player = true; } break;
                case LogTags.Monster: { Monster = true; } break;
                case LogTags.Elite: { Elite = true; } break;
                case LogTags.Boss: { Boss = true; } break;

                case LogTags.CharacterSpawn: { CharacterSpawn = true; } break;
                case LogTags.CharacterWave: { CharacterWave = true; } break;

                case LogTags.Damage: { Damage = true; } break;
                case LogTags.Detect: { Detect = true; } break;

                case LogTags.Data: { Data = true; } break;
                case LogTags.JsonData: { JsonData = true; } break;
                case LogTags.ScriptableData: { ScriptableData = true; } break;
                case LogTags.GamePref: { GamePref = true; } break;

                case LogTags.GameData: { GameData = true; } break;
                case LogTags.GameData_Character: { GameData_Character = true; } break;
                case LogTags.Quest: { Quest = true; } break;
                case LogTags.QuestCondition: { QuestCondition = true; } break;
                case LogTags.Tutorial: { Tutorial = true; } break;
                case LogTags.Storage: { Storage = true; } break;
                case LogTags.Inventory: { Inventory = true; } break;

                case LogTags.GameData_Skill: { GameData_Skill = true; } break;
                case LogTags.GameData_Weapon: { GameData_Weapon = true; } break;

                case LogTags.Difficulty: { Difficulty = true; } break;

                case LogTags.Develop: { Develop = true; } break;

                case LogTags.Effect: { Effect = true; } break;
                case LogTags.Global: { Global = true; } break;
                case LogTags.Currency: { Currency = true; } break;
                case LogTags.Item: { Item = true; } break;
                case LogTags.DropObject: { DropObject = true; } break;
                case LogTags.Cursor: { Cursor = true; } break;
                case LogTags.Input: { Input = true; } break;
                case LogTags.Passive: { Passive = true; } break;
                case LogTags.PassiveTrigger: { PassiveTrigger = true; } break;
                case LogTags.Potion: { Potion = true; } break;
                case LogTags.Resource: { Resource = true; } break;
                case LogTags.Path: { Path = true; } break;
                case LogTags.Setting: { Setting = true; } break;
                case LogTags.Skill: { Skill = true; } break;
                case LogTags.Stage: { Stage = true; } break;
                case LogTags.Stage_Monster: { Stage_Monster = true; } break;
                case LogTags.Turn: { Turn = true; } break;
                case LogTags.Stat: { Stat = true; } break;
                case LogTags.State: { State = true; } break;
                case LogTags.String: { String = true; } break;
                case LogTags.Tile: { Tile = true; } break;                
                case LogTags.Timeline: { Timeline = true; } break;
                case LogTags.PositionGroup: { PositionGroup = true; } break;
                case LogTags.UI: { UI = true; } break;
                case LogTags.UI_Button: { UI_Button = true; } break;
                case LogTags.UI_Gauge: { UI_Gauge = true; } break;
                case LogTags.UI_Popup: { UI_Popup = true; } break;
                case LogTags.UI_Details: { UI_Details = true; } break;
                case LogTags.UI_Inventory: { UI_Inventory = true; } break;                
                case LogTags.UI_SelectEvent: { UI_SelectEvent = true; } break;
                case LogTags.UI_Tab: { UI_Tab = true; } break;
                case LogTags.Vital: { Vital = true; } break;
                case LogTags.BattleResource: { BattleResource = true; } break;
                case LogTags.Weapon: { Weapon = true; } break;
                case LogTags.UI_Skill: { UI_Skill = true; } break;
                case LogTags.UI_SlotMachine: { UI_SlotMachine = true; } break;
            }
            ;
        }

        public void SwitchOff(LogTags logTag)
        {
            switch (logTag)
            {
                case LogTags.Analytics: { Analytics = false; } break;
                case LogTags.Ability: { Ability = false; } break;

                case LogTags.Animation: { Animation = false; } break;
                case LogTags.Renderer: { Renderer = false; } break;

                case LogTags.Attack: { Attack = false; } break;
                case LogTags.Attack_Target: { Attack_Target = false; } break;
                case LogTags.Audio: { Audio = false; } break;
                case LogTags.Video: { Video = false; } break;

                case LogTags.Buff: { Buff = false; } break;

                case LogTags.Camera: { Camera = false; } break;
                case LogTags.Character: { Character = false; } break;
                case LogTags.Player: { Player = false; } break;
                case LogTags.Monster: { Monster = false; } break;
                case LogTags.Elite: { Elite = false; } break;
                case LogTags.Boss: { Boss = false; } break;
                case LogTags.CharacterSpawn: { CharacterSpawn = false; } break;
                case LogTags.CharacterWave: { CharacterWave = false; } break;

                case LogTags.Damage: { Damage = false; } break;
                case LogTags.Detect: { Detect = false; } break;

                case LogTags.Data: { Data = false; } break;
                case LogTags.JsonData: { JsonData = false; } break;
                case LogTags.ScriptableData: { ScriptableData = false; } break;
                case LogTags.GamePref: { GamePref = false; } break;

                case LogTags.GameData: { GameData = false; } break;
                case LogTags.GameData_Character: { GameData_Character = false; } break;
                case LogTags.Quest: { Quest = false; } break;
                case LogTags.QuestCondition: { QuestCondition = false; } break;
                case LogTags.Tutorial: { Tutorial = false; } break;
                case LogTags.Storage: { Storage = false; } break;
                case LogTags.Inventory: { Inventory = false; } break;

                case LogTags.GameData_Skill: { GameData_Skill = false; } break;
                case LogTags.GameData_Weapon: { GameData_Weapon = false; } break;

                case LogTags.Difficulty: { Difficulty = false; } break;

                case LogTags.Develop: { Develop = false; } break;
                case LogTags.Effect: { Effect = false; } break;

                case LogTags.Global: { Global = false; } break;

                case LogTags.Currency: { Currency = false; } break;
                case LogTags.Item: { Item = false; } break;
                case LogTags.DropObject: { DropObject = false; } break;

                case LogTags.Cursor: { Cursor = false; } break;
                case LogTags.Input: { Input = false; } break;

                case LogTags.Passive: { Passive = false; } break;
                case LogTags.PassiveTrigger: { PassiveTrigger = false; } break;
                case LogTags.Potion: { Potion = false; } break;
                case LogTags.Resource: { Resource = false; } break;
                case LogTags.Path: { Path = false; } break;

                case LogTags.Setting: { Setting = false; } break;
                case LogTags.Skill: { Skill = false; } break;

                case LogTags.Stage: { Stage = false; } break;
                case LogTags.Stage_Monster: { Stage_Monster = false; } break;
                case LogTags.Turn: { Turn = false; } break;

                case LogTags.Stat: { Stat = false; } break;
                case LogTags.State: { State = false; } break;
                case LogTags.String: { String = false; } break;

                case LogTags.Tile: { Tile = false; } break;
                case LogTags.Timeline: { Timeline = false; } break;
                case LogTags.PositionGroup: { PositionGroup = false; } break;

                case LogTags.UI: { UI = false; } break;
                case LogTags.UI_Button: { UI_Button = false; } break;
                case LogTags.UI_Gauge: { UI_Gauge = false; } break;
                case LogTags.UI_Popup: { UI_Popup = false; } break;
                case LogTags.UI_Details: { UI_Details = false; } break;
                case LogTags.UI_Inventory: { UI_Inventory = false; } break;
                case LogTags.UI_SelectEvent: { UI_SelectEvent = false; } break;
                case LogTags.UI_Tab: { UI_Tab = false; } break;

                case LogTags.Vital: { Vital = false; } break;
                case LogTags.BattleResource: { BattleResource = false; } break;
                case LogTags.Weapon: { Weapon = false; } break;

                case LogTags.UI_Skill: { UI_Skill = false; } break;
                case LogTags.UI_SlotMachine: { UI_SlotMachine = false; } break;
            }
            ;
        }

        public void Refresh()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
    }
}