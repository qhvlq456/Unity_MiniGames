using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EAuthError
{
    None,
    NotEqual,
    Empty,
    EmptyConfirm,
    AlreadyEnter,
    Success,
    Fail,
    Create,
    NotCreate,
    Exist,
    NotExist,
    Logout
}
public enum EBoardGameError
{
    None,
    Fail,
    Wait,
    NextTurn
}
public class ErrorKind : MonoBehaviour
{
    public static Dictionary<EAuthError,string> authError = new Dictionary<EAuthError, string>()
    {
        {EAuthError.None,"None"},
        {EAuthError.NotEqual,"비밀번호 확인과 일치하지 않습니다 다시한번 확인해 주세요!"},
        {EAuthError.Empty,"닉네임을 입력해주세요."},
        {EAuthError.EmptyConfirm,"글자를 입력해주세요!"},
        {EAuthError.AlreadyEnter,"이미 접속중인 아이디입니다.."},
        {EAuthError.Success,"로그인에 성공하였습니다!!"},
        {EAuthError.Fail,"아이디를 확인해 주세요.."},
        {EAuthError.Create,"아이디를 성공적으로 만들었습니다"},
        {EAuthError.NotCreate,"아이디를 만들지 못하였습니다 다시한번 확인해 주세요.."},
        {EAuthError.Exist,"이미 존재합니다.."},
        {EAuthError.NotExist,"정상 확인 되었습니다!"},
        {EAuthError.Logout,"로그아웃 되었습니다."}
    };
    public static Dictionary<EBoardGameError,string> boardGameError = new Dictionary<EBoardGameError, string>()
    {
        {EBoardGameError.None,"None"},
        {EBoardGameError.Fail,"여기에 돌을 둘 수 없습니다.."},
        {EBoardGameError.Wait,"상대방이 돌을 두는 중입니다..."},
        {EBoardGameError.NextTurn,"다음 턴으로 바꿉니다"}
    };
}
