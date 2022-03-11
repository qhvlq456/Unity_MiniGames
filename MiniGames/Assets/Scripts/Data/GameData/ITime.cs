using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ITime
{
    long GetBinaryLastPlayTime(); // string -> long으로 변환 왜냐 계산 해야됨;;
    DateTime GetDateTimeLastPlayTime(); // 이거 바꿔야 됨!! 이 밑으로 전부~
}
