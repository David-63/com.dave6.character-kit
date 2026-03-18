namespace Dave6.CharacterKit.Handler.Combat
{
    public enum EActionExitReason
    {
        None,
        LeaseExpired,   // 유지 조건 만료
        InputCancelled, // 입력 끊김
        Chained,        // 다른 액션으로 연계
    }

}