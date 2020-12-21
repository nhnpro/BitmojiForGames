using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SnapKitUserInfo
{
    public string displayName;
    public BitmojiInfo bitmoji;
}

[Serializable]
public class BitmojiInfo
{
    public string id;
    public string selfie;
}

[Serializable]
public class SnapKitResponse
{
    public SnapKitUser data;
}

[Serializable]
public class SnapKitUser
{
    public SnapKitUserInfo me;
}
