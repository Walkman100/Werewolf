Option Explicit Off
Imports System.Collections.Generic

Module werewolf
    Dim tmpString As String = ""
    Dim playerNames As New List(Of String)
    Dim cardList As New List(Of String)
    
    Sub Main(args As String())
        If args.Length = 0 Then
            Console.Write("Enter amount of players: ")
            tmpString = Console.Readline()
            If isnumeric(tmpString)
                For i = 1 to tmpString
                    playerNames.Add(i)
                Next
                j = 0
                For Each i In GenerateRandomList(tmpString)
                    Console.WriteLine(playerNames(j) & ": " & i)
                    j += 1
                Next
            Else
                Console.WriteLine("""" & tmpString & """ is not a number!")
            End If
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
