Option Explicit Off
Imports System.Collections.Generic

Module werewolf
    Dim tmpString As String = ""
    Dim playerNames As New List(Of String)
    Dim cardList As New List(Of String)
    
    Sub Main(args As String())
        If args.Length = 0 Then
            Console.Write("Enter amount of players: ")
            Start(Console.Readline())
        Else
            Select case args(0)
                Case "-h"
                    Console.Writeline("Werewolf - github.com/Walkman100/Werewolf")
                    WriteUsage()
                Case "-p"
                    If args.length > 1 Then : Start(args(1))
                    Else : Console.Write("Enter amount of players: ")
                           Start(Console.Readline())
                    End If
                Case Else
                    Console.Writeline("Unrecognised flag """ & args(0) & """!")
                    WriteUsage()
            End Select
        End If
    End Sub

    Sub WriteUsage()
        Dim flags As String = " [-h|-p [number of players]]"
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
    
    Sub Start(players As String)
        If isnumeric(players)
            playerNames.Clear()
            For i = 1 to players
                playerNames.Add(i)
            Next
            
            j = 0
            For Each i In GenerateRandomList(players)
                Console.WriteLine(playerNames(j) & ": " & i)
                j += 1
            Next
        Else
            Console.WriteLine("""" & players & """ is not a number!")
        End If
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
