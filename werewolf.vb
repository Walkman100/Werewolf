Option Explicit Off
Imports System.Collections.Generic
Imports System.IO.File
Imports System.Xml

Module werewolf
    Dim tmpString As String = ""
    Dim cardOptions As String = ""
    Dim lstPlayerNames As New List(Of String)
    Dim lstCards As New List(Of Dictionary(Of String, String))
    Dim lstSelectedCards As New List(Of Dictionary(Of String, String))
    
    Sub Main(args As String())
        If args.Length = 0 Then
            Console.Write("Use Player nAmes or Numbers? (p/a/n/#): ")
            tmpString = Console.ReadKey().Key.ToString
            Console.WriteLine()
            If tmpString = "N" Or tmpString = "0" ' # relates to 0 for some reason. So does \ and probably a few other characters but that's fine.
                Console.Write("Enter amount of players: ")
                Start(Console.ReadLine())
            ElseIf tmpString = "P" Or tmpString = "A"
                InputPlayerNames()
            Else
                Console.WriteLine("""" & tmpString & """ isn't any of the valid inputs!")
            End If
            
        Else
            If args.length > 2 Then cardOptions = args(2)
            Select case args(0)
                Case "-h", "--help", "-?", "/?"
                    WriteUsage()
                Case "-n", "--numbers", "-#", "--amount"
                    If args.length > 1 Then: Start(args(1))
                    Else: Console.Write("Enter amount of players: ")
                           Start(Console.ReadLine())
                    End If
                Case "-p", "--players", "-a", "--names"
                    If args.length > 1 Then
                        For Each name In args(1).Split(",")
                            lstPlayerNames.Add(name)
                        Next
                        Start(lstPlayerNames.Count, False)
                    Else: InputPlayerNames()
                    End If
                Case "-l", "--load", "-f", "--file"
                    If args.length > 1 Then: LoadPlayerNames(args(1))
                        Start(lstPlayerNames.Count, False)
                    Else: Console.Write("Enter path to load names from: ")
                           LoadPlayerNames(Console.ReadLine())
                           Start(lstPlayerNames.Count, False)
                    End If
                Case Else
                    Console.WriteLine("Unrecognised flag """ & args(0) & """!")
                    WriteUsage()
            End Select
        End If
    End Sub
    
    Sub WriteUsage()
        Dim flags As String = " [OPTION [OPTIONARGUMENT] [CARDOPTION]]" & vbNewLine & "Tools to help with Werewolf Narrating - https://github.com/Walkman100/Werewolf" & vbNewLine
        Dim programPath As String = System.Reflection.Assembly.GetExecutingAssembly().CodeBase
        If My.Computer.Info.OSPlatform = "Unix" Then
            Console.WriteLine("Usage: mono " & programPath.Substring(programPath.LastIndexOf("/") +1) & flags)
        ElseIf My.Computer.Info.OSPlatform = "Win32NT" Then
            Console.WriteLine("Usage: " & programPath.Substring(programPath.LastIndexOf("/") +1) & flags)
        Else
            Console.WriteLine("Unrecognised platform """ & My.Computer.Info.OSPlatform & """! Please report at https://github.com/Walkman100/Werewolf/issues/new")
            Console.WriteLine("Default usage info: " & System.Diagnostics.Process.GetCurrentProcess.ProcessName & ".exe" & flags)
        End If
        
        flags  = " -h, --help".PadRight(35)                        & " Show this help" & vbNewLine
        flags &= " -n, --numbers [number of players]".PadRight(35) & " Use player numbers" & vbNewLine
        flags &= " -#, --amount [number of players]".PadRight(35) & "  Same as -n" & vbNewLine
        flags &= " -p, --players [playernames]".PadRight(35) & " Use player names. `playernames` must be comma-separated" & vbNewLine
        flags &= " -a, --names [playernames]".PadRight(35) & "  Same as -p" & vbNewLine
        flags &= " -l, --load [file]".PadRight(35) & " Use player names and load from `file`" & vbNewLine
        flags &= " -f, --file [file]".PadRight(35) & "  Same as -l" & vbNewLine & vbNewLine
        flags &= "CARDOPTIONS: (option argument above has to be supplied to use these)" & vbNewLine
        flags &= " -c, -a".PadRight(35) & " Use card names" & vbNewLine
        flags &= " -n, -#".PadRight(35) & " Use card numbers" & vbNewLine
        flags &= vbNewLine & "Examples:" & vbNewLine
        If My.Computer.Info.OSPlatform = "Unix" Then
            flags &= "  mono " & programPath.Substring(programPath.LastIndexOf("/") +1) & " -n 10 -n".PadRight(25) & "Use card numbers for 10 players identified by numbers" & vbNewLine
            flags &= "  mono " & programPath.Substring(programPath.LastIndexOf("/") +1) & " -p Larry,Jane,Robert -c".PadRight(25) & "Use card names for 3 players identified by names"
        ElseIf My.Computer.Info.OSPlatform = "Win32NT" Then
            flags &= "  " & programPath.Substring(programPath.LastIndexOf("/") +1) & " -n 10 -n".PadRight(25) & "Use card numbers for 10 players identified by numbers" & vbNewLine
            flags &= "  " & programPath.Substring(programPath.LastIndexOf("/") +1) & " -p Larry,Jane,Robert -c".PadRight(25) & "Use card names for 3 players identified by names"
        End If
        
        Console.WriteLine(flags)
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
                WriteAllLines(tmpString, lstPlayerNames)
            ElseIf tmpString.StartsWith("save ")
                WriteAllLines(tmpString.Substring(5), lstPlayerNames)
            ElseIf tmpString = "load"
                Console.Write("Enter file name/location to load from: ")
                LoadPlayerNames(Console.ReadLine())
            ElseIf tmpString.StartsWith("load ")
                LoadPlayerNames(tmpString.Substring(5))
            Else
                lstPlayerNames.Add(tmpString)
            End If
            tmpString = Console.ReadLine()
        Loop
        Start(lstPlayerNames.Count, False)
    End Sub
    
    Sub LoadPlayerNames(path As String)
        If Exists(path)
            lstPlayerNames.Clear()
            For Each line In ReadLines(path)
                lstPlayerNames.Add(line)
            Next
            Console.WriteLine("Loaded " & lstPlayerNames.Count() & " names")
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
            lstPlayerNames.Clear()
            For i = 1 to players
                lstPlayerNames.Add(i)
            Next
        End If
        
        If cardOptions = ""
            Console.Write("Use Card nAmes or Numbers? (c/a/n/#): ")
            tmpString = Console.ReadKey().Key.ToString
            Console.WriteLine()
        Else
            If cardOptions.StartsWith("-") And cardOptions.Length > 1
                tmpString = cardOptions.Substring(1).ToUpper
                If tmpString = "#" Then tmpString = "0"
            Else
                Console.Write("Use Card nAmes or Numbers? (c/a/n/#): ")
                tmpString = Console.ReadKey().Key.ToString
                Console.WriteLine()
            End If
        End If
        
        If tmpString = "N" Or tmpString = "0" ' # relates to 0 for some reason. So does \ and probably a few other characters but that's fine.
            j = 0
            For Each i In GenerateRandomList(players)
                Console.WriteLine(lstPlayerNames(j) & ": " & i)
                j += 1
            Next
        ElseIf tmpString = "C" Or tmpString = "A"
            If lstCards.Count = 0 Then lstCards = ReadCardXML("cards.xml")
            SelectCards()
            RandomiseCards(players)
            j = 0
            For Each cardDict In lstSelectedCards
                If cardDict.ContainsKey("name")
                    Console.WriteLine(lstPlayerNames(j) & ": " & cardDict.Item("name"))
                    j += 1
                End If
            Next
        Else
            Console.WriteLine("""" & tmpString & """ isn't any of the valid inputs!")
        End If
        
        If My.Computer.Info.OSPlatform = "Win32NT" Then
            Console.Write("Done! Press any key to continue . . . ")
            Console.ReadKey()
        End If
    End Sub
    
    Sub SelectCards()
        Dim intSelectedIndex As Int32 = 1
        Dim lstSelectedIndexes As New List(Of Int32)
        
        Do Until 0 <> 0
            j = 1
            For Each cardDict in lstCards
                If lstSelectedIndexes.Contains(j) then
                    Console.ForegroundColor = ConsoleColor.Yellow
                End If
                If intSelectedIndex = j
                    Console.BackgroundColor = ConsoleColor.DarkBlue
                End If
                
                Console.WriteLine("##########################")
                Console.WriteLine("# " & cardDict.Item("name").PadRight(23) & "#")
                Console.WriteLine("##########################")
                
                Console.BackgroundColor = ConsoleColor.Black
                Console.ForegroundColor = ConsoleColor.Green
                j += 1
            Next
            
            Dim pressedKey As Int32 = Console.ReadKey(True).Key
            Select Case pressedKey
                Case ConsoleKey.UpArrow, ConsoleKey.LeftArrow
                    If intSelectedIndex > 1 Then intSelectedIndex -= 1
                Case ConsoleKey.DownArrow, ConsoleKey.RightArrow
                    If intSelectedIndex < lstCards.Count Then intSelectedIndex += 1
                Case ConsoleKey.Spacebar, ConsoleKey.Enter
                    If lstSelectedIndexes.Contains(intSelectedIndex) Then: _
                        lstSelectedIndexes.Remove(intSelectedIndex)
                    Else: lstSelectedIndexes.Add(intSelectedIndex)
                    End If
                Case ConsoleKey.Q, ConsoleKey.D, ConsoleKey.E, ConsoleKey.Escape
                    Exit Do
            End Select
            
            Try
                Console.SetCursorPosition(0, Console.CursorTop - (lstCards.Count *3) )
            Catch
                Try
                    Console.SetCursorPosition(0, 0)
                Catch
                    ' Just append as usual if we can't move the cursor
                End Try
            End Try
        Loop
        Console.ResetColor
        
        lstSelectedCards.Clear()
        j = 1
        For Each cardDict in lstCards
            If lstSelectedIndexes.Contains(j) then
                lstSelectedCards.Add(cardDict)
            End If
            j += 1
        Next
    End Sub
    
    Sub RandomiseCards(players As Int32)
        Dim lstReturnCards As New List(Of Dictionary(Of String, String))
        
        Dim rng As New Random()
        Dim randomNumber As Int32
        
        If players > lstSelectedCards.Count()
            Do Until lstSelectedCards.Count() = players
                randomNumber = rng.Next(lstCards.Count())
                lstSelectedCards.Add(lstCards(randomNumber))
            Loop
            ' add more random cards from lstCards
        End If
        
        For i = 1 to players
            randomNumber = rng.Next(lstSelectedCards.Count())
            lstReturnCards.Add(lstSelectedCards(randomNumber))
            lstSelectedCards.RemoveAt(randomNumber)
        Next

        lstSelectedCards = lstReturnCards
    End Sub
    
    ''' <summary>Generates a list of unique random numbers</summary>
    ''' <param name="size">the size of the list to generate</param>
    ''' <returns>a randomised list</returns>
    Function GenerateRandomList(size As Int32) As List(Of Int32)
        Dim lstInitial As New List(Of Int32)
        Dim lstRandom As New List(Of Int32)
        For i = 1 to size
            lstInitial.Add(i)
        Next
        Dim rng As New Random()
        Dim randomNumber As Int32
        For i = 1 to size
            randomNumber = rng.Next(lstInitial.Count())
            lstRandom.Add(lstInitial(randomNumber)) ' lstInitial.Item()
            lstInitial.RemoveAt(randomNumber)
        Next
        
        return lstRandom
    End Function
    
    Function ReadCardXML(path As String) As List(Of Dictionary(Of String, String))
        Dim reader As XmlReader = XmlReader.Create(path)
        Try
            reader.Read()
        Catch ex As XmlException
            reader.Close
            Exit Function
        End Try
        
        Dim elementAttribute As String
        Dim lstReturnCards As New List(Of Dictionary(Of String, String))
        If reader.IsStartElement() AndAlso reader.Name = "werewolf" Then
            If reader.Read AndAlso reader.IsStartElement() AndAlso reader.Name = "cards" Then
                While reader.IsStartElement
                    If reader.Read AndAlso reader.IsStartElement() AndAlso reader.Name = "card" Then
                        Dim tmpDict As New Dictionary(Of String, String)
                        
                        elementAttribute = reader("name")
                        If elementAttribute IsNot Nothing Then
                            tmpDict.Add("name", elementAttribute)
                            
                            elementAttribute = reader("description")
                            If elementAttribute IsNot Nothing Then
                                tmpDict.Add("description", elementAttribute)
                            End If
                            
                            elementAttribute = reader("team")
                            If elementAttribute IsNot Nothing Then
                                tmpDict.Add("team", elementAttribute)
                            End If
                        End If
                        
                        lstReturnCards.Add(tmpDict)
                    End If
                End While
            End If
        End If
        reader.Close
        return lstReturnCards
    End Function
End Module
