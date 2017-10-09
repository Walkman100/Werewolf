Option Explicit Off
Imports System.Collections.Generic
Imports System.IO.File

Module werewolf
    Dim tmpString As String = ""
    Dim cardOptions As String = ""
    Dim playerNames As New List(Of String)
    Dim cardList As New List(Of String)
    
    Sub Main(args As String())
        If args.Length = 0 Then
            Console.Write("Use Player nAmes or Numbers? (p/a/n/#): ")
            tmpString = Console.ReadKey().Key.ToString
            Console.Writeline()
            If tmpString = "N" Or tmpString = "0" ' # relates to 0 for some reason. So does \ and probably a few other characters but that's fine.
                Console.Write("Enter amount of players: ")
                Start(Console.ReadLine())
            ElseIf tmpString = "P" Or tmpString = "A"
                InputPlayerNames()
            Else
                Console.Writeline("""" & tmpString & """ isn't any of the valid inputs!")
            End If
            
        Else
            If args.length > 2 Then cardOptions = args(2)
            Select case args(0)
                Case "-h"
                    Console.Writeline("Werewolf - github.com/Walkman100/Werewolf")
                    WriteUsage()
                Case "-n"
                    If args.length > 1 Then: Start(args(1))
                    Else: Console.Write("Enter amount of players: ")
                           Start(Console.ReadLine())
                    End If
                Case "-p"
                    If args.length > 1 Then
                        For Each name In args(1).Split(",")
                            playerNames.Add(name)
                        Next
                        Start(playerNames.Count, False)
                    Else: InputPlayerNames()
                    End If
                Case "-l"
                    If args.length > 1 Then: LoadPlayerNames(args(1))
                        Start(playerNames.Count, False)
                    Else: Console.Write("Enter path to load names from: ")
                           LoadPlayerNames(Console.ReadLine())
                           Start(playerNames.Count, False)
                    End If
                Case Else
                    Console.Writeline("Unrecognised flag """ & args(0) & """!")
                    WriteUsage()
            End Select
        End If
    End Sub

    Sub WriteUsage()
        Dim flags As String = " [-h|-n [number of players]|-p [playernames seperated by ,]|-l [file to load playernames from]] [-c|-a|-n|-# (Use Card nAmes or Numbers)]"
        Dim programPath As String = System.Reflection.Assembly.GetExecutingAssembly().CodeBase
        If My.Computer.Info.OSPlatform = "Unix" Then
            Console.Writeline("Usage: mono " & programPath.Substring(programPath.LastIndexOf("/") +1) & flags)
        ElseIf My.Computer.Info.OSPlatform = "Win32NT" Then
            Console.Writeline("Usage: " & programPath.Substring(programPath.LastIndexOf("/") +1) & flags)
        Else
            Console.Writeline("Unrecognised platform """ & My.Computer.Info.OSPlatform & """! Please report at https://github.com/Walkman100/Werewolf/issues/new")
            Console.Writeline("Default usage info: " & System.Diagnostics.Process.GetCurrentProcess.ProcessName & ".exe" & flags)
        End If
    End Sub
    
    Sub InputPlayerNames()
        Console.WriteLine("Enter player names. Type 'done', 'exit' or 'd' when done." _
            & " Type 'save' or 'save <path>' to save the list of names to file." _
            & " Type 'load' or 'load <path' to load the list of names from file.")
        tmpString = Console.ReadLine()
        Do Until 0 <> 0
            If tmpString = "done" Or tmpString = "exit" Or tmpString = "d"
                Exit Do
            ElseIf tmpString = "save"
                Console.Write("Enter file name/location to save to: ")
                tmpString = Console.ReadLine()
                WriteAllLines(tmpString, playerNames)
            ElseIf tmpString.StartsWith("save ")
                WriteAllLines(tmpString.Substring(5), playerNames)
            ElseIf tmpString = "load"
                Console.Write("Enter file name/location to load from: ")
                LoadPlayerNames(Console.ReadLine())
            ElseIf tmpString.StartsWith("load ")
                LoadPlayerNames(tmpString.Substring(5))
            Else
                playerNames.Add(tmpString)
            End If
            tmpString = Console.ReadLine()
        Loop
        Start(playerNames.Count, False)
    End Sub
    
    Sub LoadPlayerNames(path As String)
        If Exists(path)
            playerNames.Clear()
            For Each line In ReadLines(path)
                playerNames.Add(line)
            Next
            Console.WriteLine("Loaded " & playerNames.Count() & " names")
        Else
            Console.WriteLine("""" & path & """ doesn't exist!")
        End If
    End Sub
    
    Sub Start(players As String, Optional generatePlayers As Boolean = True)
        If Not IsNumeric(players)
            Console.WriteLine("""" & players & """ is not a whole number!")
            Exit Sub
        End If
        
        If generatePlayers
            playerNames.Clear()
            For i = 1 to players
                playerNames.Add(i)
            Next
        End If
        
        If cardOptions = ""
            Console.Write("Use Card nAmes or Numbers? (c/a/n/#): ")
            tmpString = Console.ReadKey().Key.ToString
            Console.Writeline()
        Else
            If cardOptions.StartsWith("-") And cardOptions.Length > 1
                tmpString = cardOptions.Substring(1).ToUpper
                If tmpString = "#" Then tmpString = "0"
            Else
                Console.Write("Use Card nAmes or Numbers? (c/a/n/#): ")
                tmpString = Console.ReadKey().Key.ToString
                Console.Writeline()
            End If
        End If

        If tmpString = "N" Or tmpString = "0" ' # relates to 0 for some reason. So does \ and probably a few other characters but that's fine.
            j = 0
            For Each i In GenerateRandomList(players)
                Console.WriteLine(playerNames(j) & ": " & i)
                j += 1
            Next
        ElseIf tmpString = "C" Or tmpString = "A"
            SelectCards()
            RandomiseCards(players)
            j = 0
            For Each i In cardList
                Console.WriteLine(playerNames(j) & ": " & i)
                j += 1
            Next
        Else
            Console.Writeline("""" & tmpString & """ isn't any of the valid inputs!")
        End If

        If My.Computer.Info.OSPlatform = "Win32NT" Then
            Console.Write("Done! Press any key to continue . . . ")
            Console.ReadKey()
        End If
    End Sub
    
    Sub SelectCards()
        Dim selectedIndex As Int32 = 1
        Dim selectedIndexes As New List(Of Int32)
        selectedIndexes.Add(2)
        cardList.Clear()
        'cardList = ReadCardXML("cards.xml")
        cardList.Add("Werewolf")
        cardList.Add("Seer")
        cardList.Add("Hackmaster")
        cardList.Add("Witch")
        Do Until 0 <> 0
            j = 1
            For Each card in cardList
                If selectedIndexes.Contains(j) then
                    Console.ForegroundColor = ConsoleColor.Yellow
                End If
                If selectedIndex = j
                    Console.BackgroundColor = ConsoleColor.DarkBlue
                End If

                Console.WriteLine("######################")
                Console.WriteLine("# " & card.PadRight(19) & "#")
                Console.WriteLine("######################")

                Console.BackgroundColor = ConsoleColor.Black
                Console.ForegroundColor = ConsoleColor.Green
                j += 1
            Next
            Dim pressedKey As Int32 = Console.ReadKey(True).Key
            Select Case pressedKey
                Case ConsoleKey.UpArrow
                    If selectedIndex > 1 Then selectedIndex -= 1
                Case ConsoleKey.DownArrow
                    If selectedIndex < cardList.Count Then selectedIndex += 1
                Case ConsoleKey.Spacebar
                    If selectedIndexes.Contains(selectedIndex) Then: _
                        selectedIndexes.Remove(selectedIndex)
                    Else: selectedIndexes.Add(selectedIndex)
                    End If
                Case ConsoleKey.Enter
                    If selectedIndexes.Contains(selectedIndex) Then: _
                        selectedIndexes.Remove(selectedIndex)
                    Else: selectedIndexes.Add(selectedIndex)
                    End If
                Case ConsoleKey.Q
                    Exit Do
                Case ConsoleKey.D
                    Exit Do
                Case ConsoleKey.E
                    Exit Do
                Case ConsoleKey.Escape
                    Exit Do
            End Select
            Try
                Console.SetCursorPosition(0, Console.CursorTop - (cardList.Count *3) )
            Catch
                Try
                    Console.SetCursorPosition(0, 0)
                Catch
                    ' Just append as usual if we can't move the cursor
                End Try
            End Try
        Loop
        Console.ResetColor
        'set cardList to list of selected indexes variable with cards extracted from list of cards variable
    End Sub
    
    Sub RandomiseCards(players As Int32)
        
    End Sub
    
    ''' <summary>Generates a list of unique random numbers</summary>
    ''' <param name="size">the size of the list to generate</param>
    ''' <returns>a randomised list</returns>
    Function GenerateRandomList(size As Int32) As List(Of Int32)
        Dim initialList As New List(Of Int32)
        Dim randomList As New List(Of Int32)
        For i = 1 to size
            initialList.Add(i)
        Next
        Dim rng As New Random()
        Dim randomNumber As Int32
        For i = 1 to size
            randomNumber = rng.Next(initialList.Count())
            randomList.Add(initialList(randomNumber)) ' initialList.Item()
            initialList.RemoveAt(randomNumber)
        Next

        return randomList
    End Function
    
    ' Console commands:
    '  Console.Write("text")
    '  Console.WriteLine("text")
    '  text = Console.ReadLine("text")
    '  character = Console.ReadKey("text")
End Module
