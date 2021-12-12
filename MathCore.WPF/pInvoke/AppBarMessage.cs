﻿namespace MathCore.WPF.pInvoke;

public enum AppBarMessage : uint
{
    New = 0x00000000,
    Remove = 0x00000001,
    QueryPos = 0x00000002,
    SetPos = 0x00000003,
    GetState = 0x00000004,
    GetTaskbarPos = 0x00000005,
    Activate = 0x00000006,
    GetAutoHideBar = 0x00000007,
    SetAutoHideBar = 0x00000008,
    WindowPosChanged = 0x00000009,
    SetState = 0x0000000A,
}