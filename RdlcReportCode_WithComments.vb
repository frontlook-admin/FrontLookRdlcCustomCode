Imports System

''' <summary>
''' Provides utility functions for string manipulation, number-to-words conversion
''' with specific support for Indian currency format, and global data management for RDLC reports.
''' Source: Merged from https://github.com/frontlook-admin/RDLCReport_CustomCode
''' </summary>
Public Class StringAndNumberUtils

    ' =================
    ' Global variables
    ' =================
    
    ''' <summary>
    ''' Shared collection for storing global data accessible across report rendering.
    ''' Uses Microsoft.VisualBasic.Collection for named and indexed access.
    ''' </summary>
    Shared GlobalDict As Microsoft.VisualBasic.Collection

    ''' <summary>
    ''' NAV way - Legacy support for Data1, Data2, Data3 global variables.
    ''' Used with SetData/GetData functions for backward compatibility.
    ''' </summary>
    Shared Data1 As Object
    Shared Data2 As Object
    Shared Data3 As Object

    ' =================
    ' Logging variables
    ' =================
    
    ''' <summary>
    ''' Cached log file path for improved performance in cached logging.
    ''' </summary>
    Private cachedLogPath As String = Nothing
    
    ''' <summary>
    ''' Cached file path for comparison in cached logging.
    ''' </summary>
    Private cachedFilePath As String = Nothing
    
    ''' <summary>
    ''' Cached file name for comparison in cached logging.
    ''' </summary>
    Private cachedFileName As String = Nothing
    
    ''' <summary>
    ''' Cached date string for detecting date changes in cached logging.
    ''' </summary>
    Private cachedDate As String = Nothing

    ' ==========================
    ' Logging Methods
    ' ==========================

    ''' <summary>
    ''' Writes a log message to a file with timestamp (simple version without caching).
    ''' </summary>
    ''' <param name="message">The message to log</param>
    ''' <param name="filePath">Optional file path for the log file. Defaults to "C:\Temp"</param>
    ''' <param name="fileName">Optional file name for the log file. If not provided, defaults to "CliReportDebug_yyyyMMdd". If provided, date will be appended as "fileName_yyyyMMdd"</param>
    ''' <remarks>
    ''' The method automatically creates the directory if it doesn't exist.
    ''' Each log entry is timestamped with format "yyyy-MM-dd HH:mm:ss.fff".
    ''' Logging errors are silently ignored to prevent application crashes.
    ''' This version always generates a new path - use WriteLogCached for better performance in same session.
    ''' </remarks>
    ''' <example>
    ''' ' Default usage - logs to C:\Temp\CliReportDebug_20251011.log
    ''' WriteLog("This is a test message")
    ''' 
    ''' ' Custom filepath - logs to D:\Logs\CliReportDebug_20251011.log
    ''' WriteLog("Custom path", "D:\Logs")
    ''' 
    ''' ' Custom filepath and filename - logs to D:\Logs\MyReport_20251011.log
    ''' WriteLog("Custom file", "D:\Logs", "MyReport")
    ''' </example>
    Private Sub WriteLog(ByVal message As String, Optional ByVal filePath As String = "C:\Temp", Optional ByVal fileName As String = "")
        Try
            ' Set default filename if not provided
            If String.IsNullOrEmpty(fileName) Then
                fileName = "CliReportDebug_" & DateTime.Now.ToString("yyyyMMdd")
            Else
                ' Append date to the provided filename
                fileName = fileName & "_" & DateTime.Now.ToString("yyyyMMdd")
            End If
            
            ' Create directory if it doesn't exist
            If Not System.IO.Directory.Exists(filePath) Then
                System.IO.Directory.CreateDirectory(filePath)
            End If
            
            ' Construct full log file path using Path.Combine
            Dim fullPath As String = System.IO.Path.Combine(filePath, fileName & ".log")
            
            ' Write log entry
            System.IO.File.AppendAllText(fullPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & " - " & message & vbCrLf)
        Catch ex As Exception
            ' Ignore logging errors
        End Try
    End Sub

    ''' <summary>
    ''' Writes a log message to a file with timestamp (cached version for better performance).
    ''' </summary>
    ''' <param name="message">The message to log</param>
    ''' <param name="filePath">Optional file path for the log file. Defaults to "C:\Temp"</param>
    ''' <param name="fileName">Optional file name for the log file. If not provided, defaults to "CliReportDebug_yyyyMMdd"</param>
    ''' <remarks>
    ''' This version caches the log file path for better performance when writing multiple log entries.
    ''' The cache is automatically invalidated when:
    ''' - File path changes
    ''' - File name changes
    ''' - Date changes (ensures logs go to correct date file)
    ''' Use this for multiple log writes in the same report execution.
    ''' </remarks>
    ''' <example>
    ''' ' Multiple logs in same session - more efficient than WriteLog
    ''' WriteLogCached("Starting processing")
    ''' WriteLogCached("Processing item 1")
    ''' WriteLogCached("Completed")
    ''' </example>
    Private Sub WriteLogCached(ByVal message As String, Optional ByVal filePath As String = "C:\Temp", Optional ByVal fileName As String = "")
        Try
            Dim currentDate As String = DateTime.Now.ToString("yyyyMMdd")
            
            ' Check if we need to regenerate the cached path
            If String.IsNullOrEmpty(cachedLogPath) OrElse _
               cachedFilePath <> filePath OrElse _
               cachedFileName <> fileName OrElse _
               cachedDate <> currentDate Then
                
                ' Set default filename if not provided
                If String.IsNullOrEmpty(fileName) Then
                    fileName = "CliReportDebug_" & currentDate
                Else
                    fileName = fileName & "_" & currentDate
                End If
                
                ' Create directory if it doesn't exist
                If Not System.IO.Directory.Exists(filePath) Then
                    System.IO.Directory.CreateDirectory(filePath)
                End If
                
                ' Cache the values
                cachedFilePath = filePath
                cachedFileName = fileName
                cachedDate = currentDate
                cachedLogPath = System.IO.Path.Combine(filePath, fileName & ".log")
            End If
            
            ' Write log entry using cached path
            System.IO.File.AppendAllText(cachedLogPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & " - " & message & vbCrLf)
        Catch ex As Exception
            ' Ignore logging errors
        End Try
    End Sub

    ' ==========================
    ' Global Data Management (from RDLCReport_CustomCode)
    ' ==========================

    ''' <summary>
    ''' Gets a value from the global dictionary by name or index number.
    ''' </summary>
    ''' <param name="Key">The key (name or number) to retrieve</param>
    ''' <returns>The value associated with the key, or an error message if not found</returns>
    ''' <remarks>
    ''' Keys are case-insensitive. Numeric keys start at index 1.
    ''' Returns descriptive error messages like "?KeyName?" if key not found.
    ''' </remarks>
    ''' <example>
    ''' =Code.GetVal("CompanyName")'
    ''' =Code.GetVal(1)'
    ''' </example>
    Public Function GetVal(Key as Object)
      Return GetVal2(GlobalDict,Key)
    End Function

    ''' <summary>
    ''' Gets a value from a specified data collection by name or index number.
    ''' </summary>
    ''' <param name="Data">The collection to retrieve from</param>
    ''' <param name="Key">The key (name or number) to retrieve</param>
    ''' <returns>The value associated with the key, or an error message if not found</returns>
    Public Function GetVal2(ByRef Data as Object,Key as Object)
      'if Key As Number
      If IsNumeric(Key) then
        Dim i as Long
        Integer.TryParse(Key,i)
        if (i=0) then
        return "Index starts at 1"
        end if
        if (Data.Count = 0) OR (i = 0) OR (i > Data.Count) then
          Return "Invalid Index: '"+CStr(i)+"'! Collection Count = "+ CStr(Data.Count)
        end if  
        Return Data.Item(i)
      end if
     
      'if Key As String
      Key = CStr(Key).ToUpper() ' Key is Case Insensitive
      Select Case True
        Case IsNothing(Data)
          Return "CollectionEmpty"
        Case IsNothing(Key)
          Return "KeyEmpty"
        Case (not Data.Contains(Key))
          Return "?"+CStr(Key)+"?"  ' Not found
        Case Data.Contains(Key)
          Return Data.Item(Key)
        Case else
          Return "Something else failed"
      End Select 
     
    End Function

    ''' <summary>
    ''' Sets global data from a key-value list for use in report headers/footers.
    ''' </summary>
    ''' <param name="KeyValueList">String with Chr(177) as separator: Key1§Value1§Key2§Value2</param>
    ''' <returns>True to set control Hidden property</returns>
    ''' <remarks>
    ''' Call this in a hidden tablix cell's Hidden property: =Code.SetGlobalData(Fields!GlobalData.Value)
    ''' The body renders before header/footer, allowing data transfer across report sections.
    ''' </remarks>
    Public Function SetGlobalData(KeyValueList as Object)
      SetDataAsKeyValueList(GlobalDict,KeyValueList)
      Return True 'Set Control to Hidden=true
    End Function
     
    ''' <summary>
    ''' Parses a key-value list string and adds items to a collection.
    ''' </summary>
    ''' <param name="SharedData">The collection to populate</param>
    ''' <param name="NewData">String with Chr(177) as separator</param>
    ''' <remarks>
    ''' Format: Key1§Value1§Key2§Value2§... where § is Chr(177)
    ''' Handles odd-length lists by treating last item as key with empty value.
    ''' </remarks>
    Public Function SetDataAsKeyValueList(ByRef SharedData as Object,NewData as Object)
      Dim i as integer
      Dim words As String() = Split(CStr(NewData),Chr(177))
      Dim Key As String
      Dim Value As String
      For i = 1 To UBound(words)   
        if ((i mod 2) = 0) then
          Key   = Cstr(Choose(i-1, Split(Cstr(NewData),Chr(177))))     
          Value = Cstr(Choose(i, Split(Cstr(NewData),Chr(177))))
          AddKeyValue(SharedData,Key,Value)
        end if
        ' If last item in list only has a key
        if (i = UBound(words)) and ((i mod 2) = 1) then
          Key   = Cstr(Choose(i, Split(Cstr(NewData),Chr(177))))     
          Value = ""
          AddKeyValue(SharedData,Key,Value)
        end if
      Next 
    End Function
     
    ''' <summary>
    ''' Adds or updates a key-value pair in a collection.
    ''' </summary>
    ''' <param name="Data">The collection to modify (created if Nothing)</param>
    ''' <param name="Key">The key (converted to uppercase for case-insensitive access)</param>
    ''' <param name="Value">The value to store</param>
    ''' <returns>The count of items in the collection</returns>
    ''' <remarks>
    ''' If key is empty, uses next numeric index (Count+1).
    ''' Replaces existing values with same key.
    ''' </remarks>
    Public Function AddKeyValue(ByRef Data as Object, Key as Object,Value as Object)
      if IsNothing(Data) then
         Data = New Microsoft.VisualBasic.Collection
      End if
     
      Dim RealKey as String
      if (CStr(Key) <> "") Then
        RealKey = CStr(Key).ToUpper()
      else
        RealKey = CStr(Data.Count +1)
      End if
      ' Replace value if it already exists
      if Data.Contains(RealKey) then
         Data.Remove(RealKey)
      End if
     
      Data.Add(Value,RealKey)   
     
      Return Data.Count
    End Function

    ' ==========================
    ' NAV Way - Legacy SetData & GetData
    ' ==========================

    ''' <summary>
    ''' Sets data in one of three global variables (NAV way for backward compatibility).
    ''' </summary>
    ''' <param name="NewData">String with Chr(177) as separator</param>
    ''' <param name="Group">Which global variable to use (1, 2, or 3)</param>
    ''' <returns>True to hide the control</returns>
    ''' <remarks>
    ''' Legacy method for NAV compatibility. Consider using SetGlobalData with named keys instead.
    ''' Used in hidden tablix cell: =Code.SetData(Fields!DataField.Value, 1)
    ''' </remarks>
    ''' <example>
    ''' =Code.SetData(Fields!GlobalData.Value, 1)
    ''' </example>
    Public Function SetData(NewData as Object, Group as integer)
      ' NewData - string with char177 as separator char 
      ' Group   - select which of the 3 globals (1, 2, or 3)
      ' Return True - Required to hide the blind table
      If Group = 1 and NewData <> "" Then
          Data1 = NewData
      End If

      If Group = 2 and NewData <> "" Then
          Data2 = NewData
      End If

      If Group = 3 and NewData <> "" Then
          Data3 = NewData
      End If
      Return True
    End Function

    ''' <summary>
    ''' Gets data by position number from one of three global variables (NAV way).
    ''' </summary>
    ''' <param name="Num">Position number of the value (1-based)</param>
    ''' <param name="Group">Which global variable to use (1, 2, or 3)</param>
    ''' <returns>The value at the specified position</returns>
    ''' <remarks>
    ''' Legacy method for NAV compatibility. Consider using GetVal with named keys instead.
    ''' Requires counting positions in the data list.
    ''' </remarks>
    ''' <example>
    ''' =Code.GetData(1, 1)  ' Gets first value from Data1
    ''' =Code.GetData(3, 2)  ' Gets third value from Data2
    ''' </example>
    Public Function GetData(Num as Integer, Group as integer) as Object
      ' Num    - position of the string you want to have 
      ' Group  - select which of the 3 globals (1, 2, or 3)
      ' Object - return value  

      if Group = 1 then
        Return Cstr(Choose(Num, Split(Cstr(Data1),Chr(177))))
      End If

      if Group = 2 then
        Return Cstr(Choose(Num, Split(Cstr(Data2),Chr(177))))
      End If

      if Group = 3 then
        Return Cstr(Choose(Num, Split(Cstr(Data3),Chr(177))))
      End If
    End Function

    ' ==========================
    ' String Concatenation Methods
    ' ==========================

    ''' <summary>
    ''' Concatenates non-empty strings from an array with line breaks (CRLF).
    ''' </summary>
    ''' <param name="strings">Array of strings to concatenate</param>
    ''' <returns>Concatenated non-empty strings separated by line breaks</returns>
    ''' <remarks>Empty strings are excluded from the result</remarks>
    Public Function ConcatenateNonEmptyWithCrLf(ByVal strings As String()) As String
        Dim result As String = ""
        For Each str As String In strings
            If Not String.IsNullOrEmpty(str) Then
                If result <> "" Then
                    result &= vbCrLf
                End If
                result &= str
            End If
        Next
        Return result
    End Function

    ''' <summary>
    ''' Concatenates non-empty strings with the specified delimiter.
    ''' </summary>
    ''' <param name="strings">Array of strings to concatenate</param>
    ''' <param name="delimiter">Delimiter to insert between non-empty strings</param>
    ''' <returns>Concatenated non-empty strings with the specified delimiter</returns>
    ''' <remarks>Empty strings are excluded from the result</remarks>
    ''' <example>
    ''' ConcatenateNonEmptyWithCrLfAndDelimiter({"Hello", "", "World"}, ",") returns "Hello,World"
    ''' </example>
    Public Function ConcatenateNonEmptyWithCrLfAndDelimiter(ByVal strings As String(), ByVal delimiter As String) As String
        Dim result As String = ""
        For Each str As String In strings
            If Not String.IsNullOrEmpty(str) Then
                If result <> "" Then
                    result &= delimiter
                End If
                result &= str
            End If
        Next
        Return result
    End Function

    ''' <summary>
    ''' Concatenates non-empty strings with the specified delimiter.
    ''' </summary>
    ''' <param name="strings">Array of strings to concatenate</param>
    ''' <param name="delimiter">Delimiter to insert between non-empty strings</param>
    ''' <returns>Concatenated non-empty strings with the specified delimiter</returns>
    ''' <remarks>Empty strings are excluded from the result</remarks>
    Public Function ConcatenateNonEmptyWithDelimiter(ByVal strings As String(), ByVal delimiter As String) As String
        Dim result As String = ""
        For Each str As String In strings
            If Not String.IsNullOrEmpty(str) Then
                If result <> "" Then
                    result &= delimiter
                End If
                result &= str
            End If
        Next
        Return result
    End Function
   
    ''' <summary>
    ''' Concatenates all strings with line breaks (CRLF), including empty strings.
    ''' </summary>
    ''' <param name="strings">Array of strings to concatenate</param>
    ''' <returns>All strings concatenated with line breaks</returns>
    ''' <remarks>Unlike ConcatenateNonEmptyWithCrLf, this includes empty strings</remarks>
    Public Function ConcatenateWithCrLf(ByVal strings As String()) As String
        Return String.Join(vbCrLf, strings)
    End Function

    ''' <summary>
    ''' Array containing the denominations for Indian currency (Rupees and Paise)
    ''' </summary>
    Public currencyDenotionIndian As String() = New String() {"Rupees", "Paise"}

    ''' <summary>
    ''' Converts a numeric value to its word representation in Indian format.
    ''' </summary>
    ''' <param name="number">The number to convert to words</param>
    ''' <param name="IfCurrency">Whether to format as currency</param>
    ''' <param name="ShowCurrency">Whether to include the currency name (Rupees/Paise)</param>
    ''' <param name="CurrencyDenotion">Custom currency denomination (not used in current implementation)</param>
    ''' <returns>Word representation of the number</returns>
    ''' <example>
    ''' ToWordsIn(1234.56, True, True) returns "Rupees One Thousand Two Hundred and Thirty-Four And Fifty-Six Paise Only"
    ''' </example>
    Public Function ToWordsIn(number As Double, Optional IfCurrency As Boolean = True, Optional ShowCurrency As Boolean = True, Optional CurrencyDenotion As String = "") As String
        Dim num = number.ToString().Split("."c)
        Dim words1 = ToWordsIn(Long.Parse(num(0)))

        Dim word2 As String = String.Empty
        If num.Length > 1 AndAlso Long.Parse(num(1)) > 0 Then
            word2 = ToWordsInAfterPoint(num(1), IfCurrency)
        End If
        If IfCurrency Then
            If ShowCurrency Then
                
                Return If(num.Length > 1 AndAlso Long.Parse(num(1)) > 0, currencyDenotionIndian(0) & " " & words1.Replace(" and ", " ") & " And " & word2 & " " & currencyDenotionIndian(1) & " Only", currencyDenotionIndian(0) & " " & words1 & " Only")
            End If
            Return If(num.Length > 1 AndAlso Long.Parse(num(1)) > 0, words1.Replace(" and ", " ") & " And " & word2 & " " & currencyDenotionIndian(1) & " Only", words1 & " Only")
        End If
        Return If(num.Length > 1 AndAlso Long.Parse(num(1)) > 0, words1 & " Point " & word2, words1)
    End Function

    ''' <summary>
    ''' Converts a Long integer to its word representation in Indian numbering system.
    ''' </summary>
    ''' <param name="number">The number to convert</param>
    ''' <returns>Word representation using Indian numbering system (crores, lakhs, etc.)</returns>
    ''' <remarks>
    ''' Handles numbers in the Indian numbering system with units:
    ''' - Crore (10,000,000)
    ''' - Lakh (100,000)
    ''' - Thousand (1,000)
    ''' - Hundred (100)
    ''' </remarks>
    Public Function ToWordsIn(number As Long) As String
        If number = 0 Then Return "zero"
        If number < 0 Then Return "minus " & ToWordsIn(Math.Abs(number))
        Dim words As String = ""
        If (number \ 10000000) > 0 Then words += ToWordsIn(number \ 10000000) & " Crore " : number = number Mod 10000000
        If (number \ 100000) > 0 Then words += ToWordsIn(number \ 100000) & " Lakh " : number = number Mod 100000
        If (number \ 1000) > 0 Then words += ToWordsIn(number \ 1000) & " Thousand " : number = number Mod 1000
        If (number \ 100) > 0 Then words += ToWordsIn(number \ 100) & " Hundred " : number = number Mod 100

        If number <= 0 Then Return words
        If words <> "" Then words += "and "
        Dim unitsMap = New String() {"Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"}
        Dim tensMap = New String() {"Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "seventy", "Eighty", "Ninety"}
        If number < 20 Then
            words += unitsMap(number)
        Else
            words += tensMap(number \ 10)
            If (number Mod 10) > 0 Then words += "-" & unitsMap(number Mod 10)
        End If
        Return words
    End Function

    ''' <summary>
    ''' Converts the decimal part of a number to words.
    ''' </summary>
    ''' <param name="number">The decimal part as a string</param>
    ''' <param name="IfCurrency">Whether to format as currency</param>
    ''' <returns>Word representation of the decimal part</returns>
    ''' <remarks>
    ''' For currency, treats the decimal as a two-digit number (e.g., "50" paise).
    ''' For non-currency, reads each digit separately.
    ''' </remarks>
    Public Function ToWordsInAfterPoint(number As String, IfCurrency As Boolean) As String
        Dim word As String = String.Empty
        If Long.Parse(number) <= 0 Then
            Return word
        End If
        If IfCurrency Then
            If number.Length = 1 Then
                number = number & "0"
            End If
            word = ToWordsIn(Long.Parse(number))
        Else
            Dim pt = number.ToCharArray()
            Dim i as Integer = 0
            For i = 0 To pt.Length - 1
                word += ToWordsIn(Long.Parse(pt(i).ToString()))
            Next
        End If

        Return word
    End Function

    ''' <summary>
    ''' Converts a number to a simplified word format with appropriate Indian units.
    ''' </summary>
    ''' <param name="number">The number to convert</param>
    ''' <returns>Simplified representation like "1.5 Lakh", "2 Crore", etc.</returns>
    ''' <remarks>
    ''' Provides a more compact representation compared to the full word form.
    ''' Uses decimal format for non-whole divisions (e.g., "1.5 Lakh" instead of "One Lakh Fifty Thousand").
    ''' </remarks>
    ''' <example>
    ''' FL_NumberToWordsMinimised(150000) returns "1.5 Lakh"
    ''' </example>
    Public Function FL_NumberToWordsMinimised(number As Long) As String
        Dim words As String = ""
        Dim unit As String = ""
        Dim divider = 100
        If number > 99 AndAlso number < 999 Then
            divider = 100
            unit = "Hundred"
        ElseIf number >= 999 AndAlso number < 99999 Then
            divider = 1000
            unit = "Thousand"
        ElseIf number >= 99999 AndAlso number < 9999999 Then
            divider = 100000
            unit = "Lakh"
        ElseIf number >= 9999999 Then
            divider = 10000000
            unit = "Crore"
        End If
        Dim no = Convert.ToDecimal(Convert.ToDecimal(number) / divider)
        If Math.Floor(no) <> no Then
            words = no.ToString("F1") & " " & unit
        Else
            words = no & " " & unit
        End If
        Return words
    End Function
End Class
