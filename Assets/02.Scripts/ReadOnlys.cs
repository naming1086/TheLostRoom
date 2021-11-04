namespace VR.ReadOnlys
{
    public static class Defines
    {
        ///<summary>Unity Layers</summary>
        public enum Layer
        {
            Default = 0,
            TransparentFX = 1,
            IgnoreRaycast = 2,
            Water = 4,
            UI = 5,
            Grabbable = 29,

            // 추가 작성 
        }


        #region Tags
        ///<summary>플레이어 얼굴 콜라이더 태그</summary>
        public static string TAG_MainCamera = "MainCamera";

        ///<summary>검지 손가락 태그</summary>
        public static string TAG_INDEX = "INDEX";

        ///<summary>씨앗 태그</summary>
        public static string TAG_SEED = "SEED";
        #endregion



        #region ErrorMessages
        ///<summary></summary>
        public static string ERROR_NULL = "NULL";

        // 사운드 에러
        public static string ERROR_NO_AUDIOCLIPDATA = "해당 클립 이름에 해당하는 사운드 데이터가 없습니다. 사운드 매니저에 추가해 주세요.";
        public static string ERROR_MISSING_SOUNDCTRL = "사운드 컨트롤을 찾을 수 없습니다.";
        #endregion



        #region Animation Datas
        #endregion



        #region Time Types
        ///<summary>시간 유형</summaty>
        public enum TimeTyeps
        {
            DAWN = 0,
            MORING = 1,
            NOON = 2,
            EVENING = 3,
            NIGHT = 4
        }
        #endregion


        #region Planet Names
        #endregion
    }
}