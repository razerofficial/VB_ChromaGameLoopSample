Imports VB_ChromaGameLoopSample.ChromaSDK
Imports System.Threading

Module Program

    Private Function PrintLegend(app As SampleApp, startIndex As Integer, selectedIndex As Integer, maxItems As Integer, supportsStreaming As Boolean, platform As Byte)

        Console.WriteLine("VB GAME LOOP CHROMA SAMPLE APP")
        Console.WriteLine()
        Console.WriteLine("Use `ESC` to QUIT.")
        If (supportsStreaming) Then
            Console.WriteLine("Press `P` to switch streaming platforms.")
        End If
        Console.WriteLine("Press `A` for ammo/health.")
        Console.WriteLine("Press `F` for fire.")
        Console.WriteLine("Press `H` to toggle hotkeys.")
        Console.WriteLine("Press `L` for landscape.")
        Console.WriteLine("Press `R` for rainbow.")
        Console.WriteLine("Press `S` for spiral.")
        Console.WriteLine()

        If supportsStreaming Then

            Console.WriteLine()

            Console.WriteLine("Streaming Info (SUPPORTED):")
            Dim status As ChromaSDK.Stream.StreamStatusType = ChromaAnimationAPI.CoreStreamGetStatus()
            Console.WriteLine(String.Format("Status: {0}", ChromaAnimationAPI.CoreStreamGetStatusString(status)))
            Console.WriteLine(String.Format("Shortcode: {0}", app.GetShortcode()))
            Console.WriteLine(String.Format("Stream Id: {0}", app.GetStreamId()))
            Console.WriteLine(String.Format("Stream Key: {0}", app.GetStreamKey()))
            Console.WriteLine(String.Format("Stream Focus: {0}", app.GetStreamFocus()))
            Console.WriteLine()

            For index As Integer = startIndex To 0 Step 1
                If index.Equals(selectedIndex) Then
                    Console.Write("[*] ")
                Else
                    Console.Write("[ ] ")
                End If
                Console.Write("{0, 8}", app.GetEffectName(index, platform))
            Next

            Console.WriteLine()
            Console.WriteLine()
            Console.WriteLine("Press ENTER to execute selection.")
        End If
        Return Nothing
    End Function

    Sub Main()

        Dim sampleApp As SampleApp = New SampleApp()
        sampleApp.Start()

        If sampleApp.GetInitResult().Equals(RazerErrors.RZRESULT_SUCCESS) Then
            Dim supportsStreaming As Boolean = ChromaAnimationAPI.CoreStreamSupportsStreaming()

            Const START_INDEX As Integer = -9
            Const MAX_ITEMS As Integer = 0

            Dim selectedIndex As Integer = START_INDEX

            Dim platform As Byte = 0

            Dim inputTimer As DateTime = DateTime.MinValue

            Dim ts As ThreadStart = New ThreadStart(AddressOf sampleApp.GameLoop)
            Dim thread As Thread = New Thread(ts)
            thread.Start()
            While (True)
                If inputTimer < DateTime.Now Then
                    Console.Clear()
                    PrintLegend(sampleApp, START_INDEX, selectedIndex, MAX_ITEMS, supportsStreaming, platform)
                    inputTimer = DateTime.Now + TimeSpan.FromMilliseconds(100)
                End If
                Dim keyInfo As ConsoleKeyInfo = Console.ReadKey()

                sampleApp.HandleInput(keyInfo)

                If keyInfo.Key.Equals(ConsoleKey.UpArrow) Then
                    If (selectedIndex > START_INDEX) Then
                        selectedIndex = selectedIndex - 1
                    End If
                ElseIf keyInfo.Key.Equals(ConsoleKey.DownArrow) Then
                    If (selectedIndex < MAX_ITEMS) Then
                        selectedIndex = selectedIndex + 1
                    End If
                ElseIf keyInfo.Key.Equals(ConsoleKey.Escape) Then
                    Exit While
                ElseIf keyInfo.Key.Equals(ConsoleKey.P) Then
                    platform = (platform + 1) Mod 4 REM PC, AMAZON LUNA, MS GAME PASS, NVIDIA GFN
                ElseIf keyInfo.Key.Equals(ConsoleKey.Enter) Then
                    sampleApp.ExecuteItem(selectedIndex, supportsStreaming, platform)
                End If
                Thread.Sleep(1)

            End While

            thread.Join()

        End If



        Console.WriteLine("{0}", "[EXIT]")

    End Sub

End Module
