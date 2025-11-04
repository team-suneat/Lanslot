namespace TeamSuneat
{
    public class GameDefine
    {
        #region 기본 설정

        public static bool IS_EDITOR
        {
            get
            {
#if UNITY_EDITOR 
                return true;
#endif

                return false;
            }
        }


        public static bool IS_DEVELOPMENT_BUILD
        {
            get
            {
#if DEVELOPMENT_BUILD
                return true;
#endif

                return false;
            }
        }

        public static bool IS_EDITOR_OR_DEVELOPMENT_BUILD
        {
            get
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                return true;
#endif

                return false;
            }
        }

        public static int MAX_TREASURE_CLASS { get; internal set; }
        public static AreaNames FIRST_BATTLE_AREA_NAME { get; internal set; }
        public static AreaNames LAST_BATTLE_AREA_NAME { get; internal set; }
        public static int ITEM_LEVEL_RANGE_MIN_VALUE { get; internal set; }

        public const bool USE_ES3 = false;
        public const bool USE_AES_EDITOR = true;

        public const bool USE_DEBUG_LOG_ERROR = true;
        public const bool USE_DEBUG_LOG_WARNING = true;

        /// <summary> 기본 화면 너비 </summary>
        public const float DEFAULT_SCREEN_WIDTH = 1920;

        /// <summary> 기본 화면 높이 </summary>
        public const float DEFAULT_SCREEN_HEIGHT = 1080;

        #endregion 기본 설정

        #region UI 설정

        public const float DEFAULT_FADE_IN_UI = 0.3f;
        public const float DEFAULT_FADE_IN_START_DELAY = 0.1f;
        public const float INPUT_WAIT_TIME = 0.3f;

        #endregion UI 설정

        #region 게임 옵션 설정

        public const bool DEFAULT_GAME_OPTION_VIDEO_FULL_SCREEN = true;
        public const bool DEFAULT_GAME_OPTION_VIDEO_BORDERLESS = false;
        public const bool DEFAULT_GAME_OPTION_VIDEO_V_SYNC = true;

        #endregion 게임 옵션 설정

        #region 게임플레이 설정

        public const int CHARACTER_MAX_LEVEL = 99;
        public const bool USE_PLAYER_DAMAGE_HIT_STOP = true; // 플레이어 피격시 프레임 멈춤 기능 사용

        #endregion 게임플레이 설정

        #region 개발자 설정

        public static bool DEV_SCRIPTABLE_OBJECT_FORCE_REFRESH_ALL = false;

        #endregion 개발자 설정
    }
}