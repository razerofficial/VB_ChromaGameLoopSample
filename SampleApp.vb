REM // When true, the sample will set Chroma effects directly from Arrays
REM // When false, the sample will use dynamic animations that set Chroma effects
REM // using the first frame of the dynamic animation.
#Const USE_ARRAY_EFFECTS = True

Imports VB_ChromaGameLoopSample.ChromaSDK
Imports System
Imports System.Collections.Generic
Imports System.Threading

Class SampleApp

#Region "Init/Uninit"

    Private _mResult As Integer = 0
    Private _mRandom As Random = New Random()

    Private _mShortCode As String = ChromaSDK.Stream._Default.Shortcode
    Private _mLenShortCode As Byte = 0

    Private _mStreamId As String = ChromaSDK.Stream._Default.StreamId
    Private _mLenStreamId As Byte = 0

    Private _mStreamKey As String = ChromaSDK.Stream._Default.StreamKey
    Private _mLenStreamKey As Byte = 0

    Private _mStreamFocus As String = ChromaSDK.Stream._Default.StreamFocus
    Private _mLenStreamFocus As Byte = 0
    Private _mStreamFocusGuid As String = "UnitTest-" + Guid.NewGuid().ToString()

    Public Function GetInitResult() As Integer
        Return _mResult
    End Function

    Public Function GetShortcode() As String
        If _mLenShortCode.Equals(0) Then
            Return "NOT_SET"
        Else
            Return _mShortCode
        End If
    End Function


    Public Function GetStreamId() As String
        If _mLenStreamId.Equals(0) Then
            Return "NOT_SET"
        Else
            Return _mStreamId
        End If
    End Function

    Public Function GetStreamKey() As String
        If _mLenStreamKey.Equals(0) Then
            Return "NOT_SET"
        Else
            Return _mStreamKey
        End If
    End Function


    Public Function GetStreamFocus() As String
        If _mLenStreamFocus.Equals(0) Then
            Return "NOT_SET"
        Else
            Return _mStreamFocus
        End If
    End Function

    Public Function Start()
        Dim appInfo As ChromaSDK.APPINFOTYPE = New APPINFOTYPE()
        appInfo.Title = "Razer Chroma VB Game Loop Sample Application"
        appInfo.Description = "A sample application using Razer Chroma SDK"

        appInfo.Author_Name = "Razer"
        appInfo.Author_Contact = "https://developer.razer.com/chroma"

        REM //appInfo.SupportedDevice = 
        REM //    0x01 | // Keyboards
        REM //    0x02 | // Mice
        REM //    0x04 | // Headset
        REM //    0x08 | // Mousepads
        REM //    0x10 | // Keypads
        REM //    0x20   // ChromaLink devices
        appInfo.SupportedDevice = (&H1 Or &H2 Or &H4 Or &H8 Or &H10 Or &H20)
        REM //    0x01 | // Utility. (To specifiy this Is an utility application)
        REM //    0x02   // Game. (To specifiy this Is a game)
        appInfo.Category = 1
        _mResult = ChromaAnimationAPI.InitSDK(appInfo)
        Select Case (_mResult)
            Case RazerErrors.RZRESULT_DLL_NOT_FOUND
                Console.Error.WriteLine("Chroma DLL is not found! {0}", RazerErrors.GetResultString(_mResult))
                Return Nothing
            Case RazerErrors.RZRESULT_DLL_INVALID_SIGNATURE
                Console.Error.WriteLine("Chroma DLL has an invalid signature! {0}", RazerErrors.GetResultString(_mResult))
                Return Nothing
            Case RazerErrors.RZRESULT_SUCCESS
                Thread.Sleep(100)
            Case Else
                Console.Error.WriteLine("Failed to initialize Chroma! {0}", RazerErrors.GetResultString(_mResult))
                Return Nothing
        End Select

        REM // setup scene
        _mScene = New FChromaSDKScene()

        Dim effect As FChromaSDKSceneEffect = New FChromaSDKSceneEffect()
        effect._mAnimation = "Animations/Landscape"
        effect._mSpeed = 1
        effect._mBlend = EChromaSDKSceneBlend.SB_None
        effect._mState = False
        effect._mMode = EChromaSDKSceneMode.SM_Add
        _mScene._mEffects.Add(effect)
        _mIndexLandscape = _mScene._mEffects.Count - 1

        effect = New FChromaSDKSceneEffect()
        effect._mAnimation = "Animations/Fire"
        effect._mSpeed = 1
        effect._mBlend = EChromaSDKSceneBlend.SB_None
        effect._mState = False
        effect._mMode = EChromaSDKSceneMode.SM_Add
        _mScene._mEffects.Add(effect)
        _mIndexFire = _mScene._mEffects.Count - 1

        effect = New FChromaSDKSceneEffect()
        effect._mAnimation = "Animations/Rainbow"
        effect._mSpeed = 1
        effect._mBlend = EChromaSDKSceneBlend.SB_None
        effect._mState = True
        effect._mMode = EChromaSDKSceneMode.SM_Add
        _mScene._mEffects.Add(effect)
        _mIndexRainbow = _mScene._mEffects.Count - 1

        effect = New FChromaSDKSceneEffect()
        effect._mAnimation = "Animations/Spiral"
        effect._mSpeed = 1
        effect._mBlend = EChromaSDKSceneBlend.SB_None
        effect._mState = False
        effect._mMode = EChromaSDKSceneMode.SM_Add
        _mScene._mEffects.Add(effect)
        _mIndexSpiral = _mScene._mEffects.Count - 1

        Return Nothing
    End Function

    Public Function OnApplicationQuit()
        _mWaitForExit = False
        ChromaAnimationAPI.StopAll()
        ChromaAnimationAPI.CloseAll()
        ChromaAnimationAPI.Uninit()
        Return Nothing
    End Function

    Public Function GetEffectName(Index As Integer) As String
        Select Case (Index)
            Case -9
                Return "Request Shortcode" & ControlChars.Tab
            Case -8
                Return "Request StreamId" & ControlChars.Tab
            Case -7
                Return "Request StreamKey" & ControlChars.Tab
            Case -6
                Return "Release Shortcode" & vbCrLf
            Case -5
                Return "Broadcast" & ControlChars.Tab & ControlChars.Tab
            Case -4
                Return "BroadcastEnd" & vbCrLf
            Case -3
                Return "Watch" & ControlChars.Tab & ControlChars.Tab
            Case -2
                Return "WatchEnd" & vbCrLf
            Case -1
                Return "GetFocus" & ControlChars.Tab & ControlChars.Tab
            Case 0
                Return "SetFocus" & vbCrLf
            Case Else
                Return String.Format("Effect{0}", Index)
        End Select
    End Function

#End Region

#If USE_ARRAY_EFFECTS <> True Then
    REM // This final animation will have a single frame
    REM // Any color changes will immediately display in the next frame update.
    Private ANIMATION_FINAL_CHROMA_LINK As String = "Dynamic\\Final_ChromaLink.chroma"
    Private ANIMATION_FINAL_HEADSET As String = "Dynamic\\Final_Headset.chroma"
    Private ANIMATION_FINAL_KEYBOARD As String = "Dynamic\\Final_Keyboard.chroma"
    Private ANIMATION_FINAL_KEYPAD As String = "Dynamic\\Final_Keypad.chroma"
    Private ANIMATION_FINAL_MOUSE As String = "Dynamic\\Final_Mouse.chroma"
    Private ANIMATION_FINAL_MOUSEPAD As String = "Dynamic\\Final_Mousepad.chroma"
#End If

    Private _mWaitForExit As Boolean = True
    Private _mHotkeys As Boolean = True
    Private _mAmmo As Boolean = True
    Private _mIndexLandscape As Integer = -1
    Private _mIndexFire As Integer = -1
    Private _mIndexRainbow As Integer = -1
    Private _mIndexSpiral As Integer = -1

    Private _mScene As FChromaSDKScene = Nothing

    Function HIBYTE(a As Integer) As Integer
        Return (a And &HFF00) >> 8
    End Function

    Function LOBYTE(a As Integer) As Integer
        Return (a And &HFF)
    End Function

    Function GetKeyColorIndex(row As Integer, column As Integer) As Integer
        Return ChromaAnimationAPI.GetMaxColumn(Device2D.Keyboard) * row + column
    End Function

    Function SetKeyColor(colors As Integer(), rzkey As Integer, color As Integer)
        Dim row As Integer = HIBYTE(rzkey)
        Dim column As Integer = LOBYTE(rzkey)
        colors(GetKeyColorIndex(row, column)) = color
        Return Nothing
    End Function

    Function SetKeyColorRGB(colors As Integer(), rzkey As Integer, red As Integer, green As Integer, blue As Integer)
        SetKeyColor(colors, rzkey, ChromaAnimationAPI.GetRGB(red, green, blue))
        Return Nothing
    End Function

    Function GetColorArraySize1D(device As Device1D) As Integer
        Dim maxLeds As Integer = ChromaAnimationAPI.GetMaxLeds(device)
        Return maxLeds
    End Function

    Function GetColorArraySize2D(device As Device2D) As Integer
        Dim maxRow As Integer = ChromaAnimationAPI.GetMaxRow(device)
        Dim maxColumn As Integer = ChromaAnimationAPI.GetMaxColumn(device)
        Return maxRow * maxColumn
    End Function

#If USE_ARRAY_EFFECTS <> True Then

    Function SetupAnimation1D(path As String, device As Device1D)
        Dim animationId As Integer = ChromaAnimationAPI.GetAnimation(path)
        If animationId.Equals(-1) Then
            animationId = ChromaAnimationAPI.CreateAnimationInMemory(DeviceType.DE_1D, device)
            ChromaAnimationAPI.CopyAnimation(animationId, path)
            ChromaAnimationAPI.CloseAnimation(animationId)
            ChromaAnimationAPI.MakeBlankFramesName(path, 1, 0.1F, 0)
        End If
        Return Nothing
    End Function

    Function SetupAnimation2D(path As String, device As Device2D)
        Dim animationId As Integer = ChromaAnimationAPI.GetAnimation(path)
        If animationId.Equals(-1) Then
            animationId = ChromaAnimationAPI.CreateAnimationInMemory(DeviceType.DE_2D, device)
            ChromaAnimationAPI.CopyAnimation(animationId, path)
            ChromaAnimationAPI.CloseAnimation(animationId)
            ChromaAnimationAPI.MakeBlankFramesName(path, 1, 0.1F, 0)
        End If
        Return Nothing
    End Function

#End If

    Function SetAmbientColor1D(device As Device1D, colors As Integer(), ambientColor As Integer)
        Dim size As Integer = GetColorArraySize1D(device)
        For i = 1 To size Step 1
            If colors(i - 1).Equals(0) Then
                colors(i - 1) = ambientColor
            End If
        Next
        Return Nothing
    End Function

    Function SetAmbientColor2D(device As Device2D, colors As Integer(), ambientColor As Integer)
        Dim size As Integer = GetColorArraySize2D(device)
        For i = 1 To size Step 1
            If colors(i - 1).Equals(0) Then
                colors(i - 1) = ambientColor
            End If
        Next
        Return Nothing
    End Function

    Function SetAmbientColor(ambientColor As Integer,
            colorsChromaLink As Integer(),
            colorsHeadset As Integer(),
            colorsKeyboard As Integer(),
            colorsKeypad As Integer(),
            colorsMouse As Integer(),
            colorsMousepad As Integer())
        REM // Set ambient color
        For d = Device.ChromaLink To Device.MAX Step 1
            Select Case (d)
                Case Device.ChromaLink
                    SetAmbientColor1D(Device1D.ChromaLink, colorsChromaLink, ambientColor)
                Case Device.Headset
                    SetAmbientColor1D(Device1D.Headset, colorsHeadset, ambientColor)
                Case Device.Keyboard
                    SetAmbientColor2D(Device2D.Keyboard, colorsKeyboard, ambientColor)
                Case Device.Keypad
                    SetAmbientColor2D(Device2D.Keypad, colorsKeypad, ambientColor)
                Case Device.Mouse
                    SetAmbientColor2D(Device2D.Mouse, colorsMouse, ambientColor)
                Case Device.Mousepad
                    SetAmbientColor1D(Device1D.Mousepad, colorsMousepad, ambientColor)
            End Select
        Next
        Return Nothing
    End Function

    Function MultiplyColor(color1 As Integer, color2 As Integer) As Integer
        Dim redColor1 As Integer = color1 And &HFF
        Dim greenColor1 As Integer = (color1 >> 8) And &HFF
        Dim blueColor1 As Integer = (color1 >> 16) And &HFF

        Dim redColor2 As Integer = color2 And &HFF
        Dim greenColor2 As Integer = (color2 >> 8) And &HFF
        Dim blueColor2 As Integer = (color2 >> 16) And &HFF

        Dim red As Integer = Convert.ToInt32(Math.Floor(255 * ((redColor1 / 255.0F) * (redColor2 / 255.0F))))
        Dim green As Integer = Convert.ToInt32(Math.Floor(255 * ((greenColor1 / 255.0F) * (greenColor2 / 255.0F))))
        Dim blue As Integer = Convert.ToInt32(Math.Floor(255 * ((blueColor1 / 255.0F) * (blueColor2 / 255.0F))))

        Return ChromaAnimationAPI.GetRGB(red, green, blue)
    End Function

    Function AverageColor(color1 As Integer, color2 As Integer) As Integer
        Return ChromaAnimationAPI.LerpColor(color1, color2, 0.5F)
    End Function

    Function AddColor(color1 As Integer, color2 As Integer) As Integer
        Dim redColor1 As Integer = color1 And &HFF
        Dim greenColor1 As Integer = (color1 >> 8) And &HFF
        Dim blueColor1 As Integer = (color1 >> 16) And &HFF

        Dim redColor2 As Integer = color2 And &HFF
        Dim greenColor2 As Integer = (color2 >> 8) And &HFF
        Dim blueColor2 As Integer = (color2 >> 16) And &HFF

        Dim red As Integer = Math.Min(redColor1 + redColor2, 255) And &HFF
        Dim green As Integer = Math.Min(greenColor1 + greenColor2, 255) And &HFF
        Dim blue As Integer = Math.Min(blueColor1 + blueColor2, 255) And &HFF

        Return ChromaAnimationAPI.GetRGB(red, green, blue)
    End Function

    Function SubtractColor(color1 As Integer, color2 As Integer) As Integer
        Dim redColor1 As Integer = color1 And &HFF
        Dim greenColor1 As Integer = (color1 >> 8) And &HFF
        Dim blueColor1 As Integer = (color1 >> 16) And &HFF

        Dim redColor2 As Integer = color2 And &HFF
        Dim greenColor2 As Integer = (color2 >> 8) And &HFF
        Dim blueColor2 As Integer = (color2 >> 16) And &HFF

        Dim red As Integer = Math.Max(redColor1 - redColor2, 0) And &HFF
        Dim green As Integer = Math.Max(greenColor1 - greenColor2, 0) And &HFF
        Dim blue As Integer = Math.Max(blueColor1 - blueColor2, 0) And &HFF

        Return ChromaAnimationAPI.GetRGB(red, green, blue)
    End Function

    Function MaxColor(color1 As Integer, color2 As Integer) As Integer
        Dim redColor1 As Integer = color1 And &HFF
        Dim greenColor1 As Integer = (color1 >> 8) And &HFF
        Dim blueColor1 As Integer = (color1 >> 16) And &HFF

        Dim redColor2 As Integer = color2 And &HFF
        Dim greenColor2 As Integer = (color2 >> 8) And &HFF
        Dim blueColor2 As Integer = (color2 >> 16) And &HFF

        Dim red As Integer = Math.Max(redColor1, redColor2) And &HFF
        Dim green As Integer = Math.Max(greenColor1, greenColor2) And &HFF
        Dim blue As Integer = Math.Max(blueColor1, blueColor2) And &HFF

        Return ChromaAnimationAPI.GetRGB(red, green, blue)
    End Function

    Function MinColor(color1 As Integer, color2 As Integer) As Integer
        Dim redColor1 As Integer = color1 And &HFF
        Dim greenColor1 As Integer = (color1 >> 8) And &HFF
        Dim blueColor1 As Integer = (color1 >> 16) And &HFF

        Dim redColor2 As Integer = color2 And &HFF
        Dim greenColor2 As Integer = (color2 >> 8) And &HFF
        Dim blueColor2 As Integer = (color2 >> 16) And &HFF

        Dim red As Integer = Math.Min(redColor1, redColor2) And &HFF
        Dim green As Integer = Math.Min(greenColor1, greenColor2) And &HFF
        Dim blue As Integer = Math.Min(blueColor1, blueColor2) And &HFF

        Return ChromaAnimationAPI.GetRGB(red, green, blue)
    End Function

    Function InvertColor(color As Integer) As Integer
        Dim red As Integer = 255 - (color And &HFF)
        Dim green As Integer = 255 - ((color >> 8) And &HFF)
        Dim blue As Integer = 255 - ((color >> 16) And &HFF)

        Return ChromaAnimationAPI.GetRGB(red, green, blue)
    End Function

    Function MultiplyNonZeroTargetColorLerp(color1 As Integer, color2 As Integer, inputColor As Integer) As Integer
        If inputColor.Equals(0) Then
            Return inputColor
        End If
        Dim red As Single = (inputColor And &HFF) / 255.0F
        Dim green As Single = ((inputColor And &HFF00) >> 8) / 255.0F
        Dim blue As Single = ((inputColor And &HFF0000) >> 16) / 255.0F
        Dim t As Single = (red + green + blue) / 3.0F
        Return ChromaAnimationAPI.LerpColor(color1, color2, t)
    End Function

    Function Thresh(color1 As Integer, color2 As Integer, inputColor As Integer) As Integer
        Dim red As Single = (inputColor And &HFF) / 255.0F
        Dim green As Single = ((inputColor And &HFF00) >> 8) / 255.0F
        Dim blue As Single = ((inputColor And &HFF0000) >> 16) / 255.0F
        Dim t As Single = (red + green + blue) / 3.0F
        If t.Equals(0.0) Then
            Return 0
        End If
        If t < 0.5 Then
            Return color1
        Else
            Return color2
        End If
    End Function

    Function BlendAnimation1D(effect As FChromaSDKSceneEffect, deviceFrameIndex As FChromaSDKDeviceFrameIndex, device As Integer, device1d As Device1D, animationName As String, colors As Integer(), tempColors As Integer())
        Dim size As Integer = GetColorArraySize1D(device1d)
        Dim frameId As Integer = deviceFrameIndex._mFrameIndex(device)
        Dim frameCount As Integer = ChromaAnimationAPI.GetFrameCountName(animationName)
        If frameId < frameCount Then
            REM //cout << animationName << ": " << (1 + frameId) << " of " << frameCount << endl
            Dim duration As Single
            Dim animationId As Integer = ChromaAnimationAPI.GetAnimation(animationName)
            ChromaAnimationAPI.GetFrame(animationId, frameId, duration, tempColors, size)
            For i = 1 To size Step 1
                Dim color1 As Integer = colors(i - 1) REM // target
                Dim tempColor As Integer = tempColors(i - 1) REM // source

                REM // BLEND
                Dim color2 As Integer
                Select Case effect._mBlend
                    Case EChromaSDKSceneBlend.SB_None
                        color2 = tempColor REM // source
                    Case EChromaSDKSceneBlend.SB_Invert
                        If tempColor <> 0 Then REM // source Then                    							{
                            color2 = InvertColor(tempColor) REM // source inverted
                        Else
                            color2 = 0
                        End If
                    Case EChromaSDKSceneBlend.SB_Threshold
                        color2 = Thresh(effect._mPrimaryColor, effect._mSecondaryColor, tempColor) REM // source
                    Case EChromaSDKSceneBlend.SB_Lerp
                        color2 = MultiplyNonZeroTargetColorLerp(effect._mPrimaryColor, effect._mSecondaryColor, tempColor) REM // source
                    Case Else
                        color2 = MultiplyNonZeroTargetColorLerp(effect._mPrimaryColor, effect._mSecondaryColor, tempColor) REM // source
                End Select

                REM // MODE
                Select Case effect._mMode
                    Case EChromaSDKSceneMode.SM_Max
                        colors(i - 1) = MaxColor(color1, color2)
                    Case EChromaSDKSceneMode.SM_Min
                        colors(i - 1) = MinColor(color1, color2)
                    Case EChromaSDKSceneMode.SM_Average
                        colors(i - 1) = AverageColor(color1, color2)
                    Case EChromaSDKSceneMode.SM_Multiply
                        colors(i - 1) = MultiplyColor(color1, color2)
                    Case EChromaSDKSceneMode.SM_Add
                        colors(i - 1) = AddColor(color1, color2)
                    Case EChromaSDKSceneMode.SM_Subtract
                        colors(i - 1) = SubtractColor(color1, color2)
                    Case EChromaSDKSceneMode.SM_Replace
                        If color2 <> 0 Then
                            colors(i - 1) = color2
                        End If
                    Case Else
                        If color2 <> 0 Then
                            colors(i - 1) = color2
                        End If
                End Select
                deviceFrameIndex._mFrameIndex(device) = (frameId + frameCount + effect._mSpeed) Mod frameCount
            Next
        End If
        Return Nothing
    End Function

    Function BlendAnimation2D(effect As FChromaSDKSceneEffect, deviceFrameIndex As FChromaSDKDeviceFrameIndex, device As Integer, device2D As Device2D, animationName As String, colors As Integer(), tempColors As Integer())
        Dim size As Integer = GetColorArraySize2D(device2D)
        Dim frameId As Integer = deviceFrameIndex._mFrameIndex(device)
        Dim frameCount As Integer = ChromaAnimationAPI.GetFrameCountName(animationName)
        If frameId < frameCount Then
            REM //cout << animationName << ": " << (1 + frameId) << " of " << frameCount << endl
            Dim duration As Single
            Dim animationId As Integer = ChromaAnimationAPI.GetAnimation(animationName)
            ChromaAnimationAPI.GetFrame(animationId, frameId, duration, tempColors, size)
            For i = 1 To size Step 1
                Dim color1 As Integer = colors(i - 1) REM // target
                Dim tempColor As Integer = tempColors(i - 1) REM //source

                REM // BLEND
                Dim color2 As Integer
                Select Case effect._mBlend
                    Case EChromaSDKSceneBlend.SB_None
                        color2 = tempColor REM // source
                    Case EChromaSDKSceneBlend.SB_Invert
                        If tempColor <> 0 Then REM // source
                            color2 = InvertColor(tempColor) REM // source inverted
                        Else
                            color2 = 0
                        End If
                    Case EChromaSDKSceneBlend.SB_Threshold
                        color2 = Thresh(effect._mPrimaryColor, effect._mSecondaryColor, tempColor) REM // source
                    Case EChromaSDKSceneBlend.SB_Lerp
                        color2 = MultiplyNonZeroTargetColorLerp(effect._mPrimaryColor, effect._mSecondaryColor, tempColor) REM // source
                    Case Else
                        color2 = MultiplyNonZeroTargetColorLerp(effect._mPrimaryColor, effect._mSecondaryColor, tempColor) REM // source
                End Select

                REM // MODE
                Select Case effect._mMode
                    Case EChromaSDKSceneMode.SM_Max
                        colors(i - 1) = MaxColor(color1, color2)
                    Case EChromaSDKSceneMode.SM_Min
                        colors(i - 1) = MinColor(color1, color2)
                    Case EChromaSDKSceneMode.SM_Average
                        colors(i - 1) = AverageColor(color1, color2)
                    Case EChromaSDKSceneMode.SM_Multiply
                        colors(i - 1) = MultiplyColor(color1, color2)
                    Case EChromaSDKSceneMode.SM_Add
                        colors(i - 1) = AddColor(color1, color2)
                    Case EChromaSDKSceneMode.SM_Subtract
                        colors(i - 1) = SubtractColor(color1, color2)
                    Case EChromaSDKSceneMode.SM_Replace
                        If color2 <> 0 Then
                            colors(i - 1) = color2
                        End If
                    Case Else
                        If color2 <> 0 Then
                            colors(i - 1) = color2
                        End If
                End Select
            Next
            deviceFrameIndex._mFrameIndex(device) = (frameId + frameCount + effect._mSpeed) Mod frameCount
        End If
        Return Nothing
    End Function

    Function BlendAnimations(scene As FChromaSDKScene,
            colorsChromaLink As Integer(), tempColorsChromaLink As Integer(),
            colorsHeadset As Integer(), tempColorsHeadset As Integer(),
            colorsKeyboard As Integer(), tempColorsKeyboard As Integer(),
            colorsKeypad As Integer(), tempColorsKeypad As Integer(),
            colorsMouse As Integer(), tempColorsMouse As Integer(),
            colorsMousepad As Integer(), tempColorsMousepad As Integer())

        REM // blend active animations
        Dim effects As List(Of FChromaSDKSceneEffect) = scene._mEffects
        For Each effect As FChromaSDKSceneEffect In effects
            If effect._mState Then
                Dim deviceFrameIndex As FChromaSDKDeviceFrameIndex = effect._mFrameIndex
                REM //iterate all device types
                For d = Device.ChromaLink To Device.MAX Step 1
                    Dim animationName As String = effect._mAnimation

                    Select Case (d)
                        Case Device.ChromaLink
                            animationName += "_ChromaLink.chroma"
                            BlendAnimation1D(effect, deviceFrameIndex, d, Device1D.ChromaLink, animationName, colorsChromaLink, tempColorsChromaLink)
                        Case Device.Headset
                            animationName += "_Headset.chroma"
                            BlendAnimation1D(effect, deviceFrameIndex, d, Device1D.Headset, animationName, colorsHeadset, tempColorsHeadset)
                        Case Device.Keyboard
                            animationName += "_Keyboard.chroma"
                            BlendAnimation2D(effect, deviceFrameIndex, d, Device2D.Keyboard, animationName, colorsKeyboard, tempColorsKeyboard)
                        Case Device.Keypad
                            animationName += "_Keypad.chroma"
                            BlendAnimation2D(effect, deviceFrameIndex, d, Device2D.Keypad, animationName, colorsKeypad, tempColorsKeypad)
                        Case Device.Mouse
                            animationName += "_Mouse.chroma"
                            BlendAnimation2D(effect, deviceFrameIndex, d, Device2D.Mouse, animationName, colorsMouse, tempColorsMouse)
                        Case Device.Mousepad
                            animationName += "_Mousepad.chroma"
                            BlendAnimation1D(effect, deviceFrameIndex, d, Device1D.Mousepad, animationName, colorsMousepad, tempColorsMousepad)
                    End Select
                Next
            End If
        Next
        Return Nothing
    End Function

    Public Function GameLoop()
        Dim sizeChromaLink As Integer = GetColorArraySize1D(Device1D.ChromaLink)
        Dim sizeHeadset As Integer = GetColorArraySize1D(Device1D.Headset)
        Dim sizeKeyboard As Integer = GetColorArraySize2D(Device2D.Keyboard)
        Dim sizeKeypad As Integer = GetColorArraySize2D(Device2D.Keypad)
        Dim sizeMouse As Integer = GetColorArraySize2D(Device2D.Mouse)
        Dim sizeMousepad As Integer = GetColorArraySize1D(Device1D.Mousepad)

        Dim colorsChromaLink As Integer() = New Integer(sizeChromaLink) {}
        Dim colorsHeadset As Integer() = New Integer(sizeHeadset) {}
        Dim colorsKeyboard As Integer() = New Integer(sizeKeyboard) {}
        Dim colorsKeypad As Integer() = New Integer(sizeKeypad) {}
        Dim colorsMouse As Integer() = New Integer(sizeMouse) {}
        Dim colorsMousepad As Integer() = New Integer(sizeMousepad) {}

        Dim tempColorsChromaLink As Integer() = New Integer(sizeChromaLink) {}
        Dim tempColorsHeadset As Integer() = New Integer(sizeHeadset) {}
        Dim tempColorsKeyboard As Integer() = New Integer(sizeKeyboard) {}
        Dim tempColorsKeypad As Integer() = New Integer(sizeKeypad) {}
        Dim tempColorsMouse As Integer() = New Integer(sizeMouse) {}
        Dim tempColorsMousepad As Integer() = New Integer(sizeMousepad) {}

        Dim timeMS As UInteger = 0

        While _mWaitForExit
            REM // start with a blank frame
            Array.Clear(colorsChromaLink, 0, sizeChromaLink)
            Array.Clear(colorsHeadset, 0, sizeHeadset)
            Array.Clear(colorsKeyboard, 0, sizeKeyboard)
            Array.Clear(colorsKeypad, 0, sizeKeypad)
            Array.Clear(colorsMouse, 0, sizeMouse)
            Array.Clear(colorsMousepad, 0, sizeMousepad)

#If USE_ARRAY_EFFECTS <> True Then

            SetupAnimation1D(ANIMATION_FINAL_CHROMA_LINK, Device1D.ChromaLink)
			SetupAnimation1D(ANIMATION_FINAL_HEADSET, Device1D.Headset)
			SetupAnimation2D(ANIMATION_FINAL_KEYBOARD, Device2D.Keyboard)
			SetupAnimation2D(ANIMATION_FINAL_KEYPAD, Device2D.Keypad)
			SetupAnimation2D(ANIMATION_FINAL_MOUSE, Device2D.Mouse)
			SetupAnimation1D(ANIMATION_FINAL_MOUSEPAD, Device1D.Mousepad)

#End If
            BlendAnimations(_mScene,
                colorsChromaLink, tempColorsChromaLink,
                colorsHeadset, tempColorsHeadset,
                colorsKeyboard, tempColorsKeyboard,
                colorsKeypad, tempColorsKeypad,
                colorsMouse, tempColorsMouse,
                colorsMousepad, tempColorsMousepad)

            If _mAmmo Then
                REM // Show health animation
                Dim keys As Integer() = {
                            Keyboard.RZKEY.RZKEY_F1,
                            Keyboard.RZKEY.RZKEY_F2,
                            Keyboard.RZKEY.RZKEY_F3,
                            Keyboard.RZKEY.RZKEY_F4,
                            Keyboard.RZKEY.RZKEY_F5,
                            Keyboard.RZKEY.RZKEY_F6
                        }
                Dim keysLength As Integer = keys.Length

                Dim t As Single = timeMS * 0.002F
                Dim hp As Single = Math.Abs(Math.Cos(Math.PI / 2.0F + t))
                For i = 1 To keysLength Step 1
                    Dim color As Integer
                    If (i / Convert.ToSingle(keysLength + 1)) < hp Then
                        color = ChromaAnimationAPI.GetRGB(0, 255, 0)
                    Else
                        color = ChromaAnimationAPI.GetRGB(0, 100, 0)
                    End If
                    Dim key As Integer = keys(i - 1)
                    SetKeyColor(colorsKeyboard, key, color)
                Next

                REM // Show ammo animation
                keys = {
                            Keyboard.RZKEY.RZKEY_F7,
                            Keyboard.RZKEY.RZKEY_F8,
                            Keyboard.RZKEY.RZKEY_F9,
                            Keyboard.RZKEY.RZKEY_F10,
                            Keyboard.RZKEY.RZKEY_F11,
                            Keyboard.RZKEY.RZKEY_F12
                        }
                keysLength = keys.Length

                t = timeMS * 0.001F
                hp = Math.Abs(Math.Cos(Math.PI / 2.0F + t))
                For i = 1 To keysLength Step 1
                    Dim color As Integer
                    If (i / Convert.ToSingle(keysLength + 1)) < hp Then
                        color = ChromaAnimationAPI.GetRGB(255, 255, 0)
                    Else
                        color = ChromaAnimationAPI.GetRGB(100, 100, 0)
                    End If
                    Dim key As Integer = keys(i - 1)
                    SetKeyColor(colorsKeyboard, key, color)
                Next
            End If

            If _mHotkeys Then
                REM // Show hotkeys
                SetKeyColorRGB(colorsKeyboard, Keyboard.RZKEY.RZKEY_ESC, 255, 255, 0)
                SetKeyColorRGB(colorsKeyboard, Keyboard.RZKEY.RZKEY_W, 255, 0, 0)
                SetKeyColorRGB(colorsKeyboard, Keyboard.RZKEY.RZKEY_A, 255, 0, 0)
                SetKeyColorRGB(colorsKeyboard, Keyboard.RZKEY.RZKEY_S, 255, 0, 0)
                SetKeyColorRGB(colorsKeyboard, Keyboard.RZKEY.RZKEY_D, 255, 0, 0)

                If _mAmmo Then
                    SetKeyColorRGB(colorsKeyboard, Keyboard.RZKEY.RZKEY_A, 0, 255, 0)
                End If

                REM // Highlight R if rainbow Is active
                If _mScene._mEffects(_mIndexRainbow)._mState Then
                    SetKeyColorRGB(colorsKeyboard, Keyboard.RZKEY.RZKEY_R, 0, 255, 0)
                End If

                REM // Highlight S if spiral Is active
                If _mScene._mEffects(_mIndexSpiral)._mState Then
                    SetKeyColorRGB(colorsKeyboard, Keyboard.RZKEY.RZKEY_S, 0, 255, 0)
                End If

                REM // Highlight L if landscape Is active
                If _mScene._mEffects(_mIndexLandscape)._mState Then
                    SetKeyColorRGB(colorsKeyboard, Keyboard.RZKEY.RZKEY_L, 0, 255, 0)
                End If

                REM // Highlight L if landscape Is active
                If _mScene._mEffects(_mIndexFire)._mState Then
                    SetKeyColorRGB(colorsKeyboard, Keyboard.RZKEY.RZKEY_F, 0, 255, 0)
                End If

                If (_mHotkeys) Then
                    SetKeyColorRGB(colorsKeyboard, Keyboard.RZKEY.RZKEY_H, 0, 255, 0)
                End If
            End If

#If USE_ARRAY_EFFECTS Then
            ChromaAnimationAPI.SetEffectCustom1D(Device1D.ChromaLink, colorsChromaLink)
            ChromaAnimationAPI.SetEffectCustom1D(Device1D.Headset, colorsHeadset)
            ChromaAnimationAPI.SetEffectCustom1D(Device1D.Mousepad, colorsMousepad)

            ChromaAnimationAPI.SetCustomColorFlag2D(Device2D.Keyboard, colorsKeyboard)
            ChromaAnimationAPI.SetEffectKeyboardCustom2D(Device2D.Keyboard, colorsKeyboard)

            ChromaAnimationAPI.SetEffectCustom2D(Device2D.Keypad, colorsKeypad)
            ChromaAnimationAPI.SetEffectCustom2D(Device2D.Mouse, colorsMouse)
#Else

            ChromaAnimationAPI.UpdateFrameName(ANIMATION_FINAL_CHROMA_LINK, 0, 0.1F, colorsChromaLink, sizeChromaLink)
            ChromaAnimationAPI.UpdateFrameName(ANIMATION_FINAL_HEADSET, 0, 0.1F, colorsHeadset, sizeHeadset)
            ChromaAnimationAPI.UpdateFrameName(ANIMATION_FINAL_KEYBOARD, 0, 0.1F, colorsKeyboard, sizeKeyboard)
            ChromaAnimationAPI.UpdateFrameName(ANIMATION_FINAL_KEYPAD, 0, 0.1F, colorsKeypad, sizeKeypad)
            ChromaAnimationAPI.UpdateFrameName(ANIMATION_FINAL_MOUSE, 0, 0.1F, colorsMouse, sizeMouse)
            ChromaAnimationAPI.UpdateFrameName(ANIMATION_FINAL_MOUSEPAD, 0, 0.1F, colorsMousepad, sizeMousepad)

            REM // display the change
            ChromaAnimationAPI.PreviewFrameName(ANIMATION_FINAL_CHROMA_LINK, 0)
            ChromaAnimationAPI.PreviewFrameName(ANIMATION_FINAL_HEADSET, 0)
            ChromaAnimationAPI.PreviewFrameName(ANIMATION_FINAL_KEYBOARD, 0)
            ChromaAnimationAPI.PreviewFrameName(ANIMATION_FINAL_KEYPAD, 0)
            ChromaAnimationAPI.PreviewFrameName(ANIMATION_FINAL_MOUSE, 0)
            ChromaAnimationAPI.PreviewFrameName(ANIMATION_FINAL_MOUSEPAD, 0)
#End If

            Thread.Sleep(33) REM //30 FPS
            timeMS += 33

        End While

        Return Nothing
    End Function

    Public Function HandleInput(keyInfo As ConsoleKeyInfo)
        Select Case (keyInfo.Key)
            Case ConsoleKey.Escape     REM //ESCAPE
                OnApplicationQuit()

            Case ConsoleKey.A
                _mAmmo = Not _mAmmo

            Case ConsoleKey.H
                _mHotkeys = Not _mHotkeys

            Case ConsoleKey.F
                _mScene._mEffects(_mIndexFire)._mState = Not _mScene._mEffects(_mIndexFire)._mState
            Case ConsoleKey.L
                _mScene._mEffects(_mIndexLandscape)._mState = Not _mScene._mEffects(_mIndexLandscape)._mState
            Case ConsoleKey.R
                _mScene._mEffects(_mIndexRainbow)._mState = Not _mScene._mEffects(_mIndexRainbow)._mState
            Case ConsoleKey.S
                _mScene._mEffects(_mIndexSpiral)._mState = Not _mScene._mEffects(_mIndexSpiral)._mState
        End Select
        Return Nothing
    End Function

    Public Function ExecuteItem(index As Integer, supportsStreaming As Boolean)
        Select Case (index)
            Case -9
                If supportsStreaming Then
                    _mShortCode = ChromaSDK.Stream._Default.Shortcode
                    _mLenShortCode = 0
                    ChromaAnimationAPI.CoreStreamGetAuthShortcode(_mShortCode, _mLenShortCode, "PC", "VB Game Loop Sample App 好")
                    If _mLenShortCode > 0 Then
                        _mShortCode = _mShortCode.Substring(0, _mLenShortCode)
                    End If
                End If
            Case -8
                If supportsStreaming Then
                    _mStreamId = ChromaSDK.Stream._Default.StreamId
                    _mLenStreamId = 0
                    ChromaAnimationAPI.CoreStreamGetId(_mShortCode, _mStreamId, _mLenStreamId)
                    If _mLenStreamId > 0 Then
                        _mStreamId = _mStreamId.Substring(0, _mLenStreamId)
                    End If
                End If
            Case -7
                If supportsStreaming Then
                    _mStreamKey = ChromaSDK.Stream._Default.StreamKey
                    _mLenStreamKey = 0
                    ChromaAnimationAPI.CoreStreamGetKey(_mShortCode, _mStreamKey, _mLenStreamKey)
                    If (_mLenStreamId > 0) Then
                        _mStreamKey = _mStreamKey.Substring(0, _mLenStreamKey)
                    End If
                End If
            Case -6
                If (supportsStreaming And ChromaAnimationAPI.CoreStreamReleaseShortcode(_mShortCode)) Then
                    _mShortCode = ChromaSDK.Stream._Default.Shortcode
                    _mLenShortCode = 0
                End If
            Case -5
                If (supportsStreaming And _mLenStreamId > 0 And _mLenStreamKey > 0) Then
                    ChromaAnimationAPI.CoreStreamBroadcast(_mStreamId, _mStreamKey)
                End If
            Case -4
                If supportsStreaming Then
                    ChromaAnimationAPI.CoreStreamBroadcastEnd()
                End If
            Case -3
                If (supportsStreaming And _mLenStreamId > 0) Then
                    ChromaAnimationAPI.CoreStreamWatch(_mStreamId, 0)
                End If
            Case -2
                If supportsStreaming Then
                    ChromaAnimationAPI.CoreStreamWatchEnd()
                End If
            Case -1
                If supportsStreaming Then
                    _mStreamFocus = ChromaSDK.Stream._Default.StreamFocus
                    _mLenStreamFocus = 0
                    ChromaAnimationAPI.CoreStreamGetFocus(_mStreamFocus, _mLenStreamFocus)
                End If
            Case 0
                If supportsStreaming Then
                    ChromaAnimationAPI.CoreStreamSetFocus(_mStreamFocusGuid)
                    _mStreamFocus = ChromaSDK.Stream._Default.StreamFocus
                    _mLenStreamFocus = 0
                    ChromaAnimationAPI.CoreStreamGetFocus(_mStreamFocus, _mLenStreamFocus)
                End If
        End Select
        Return Nothing
    End Function

End Class
