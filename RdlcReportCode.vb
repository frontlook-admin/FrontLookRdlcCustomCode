' ==========================================
' Source: https://github.com/frontlook-admin/RDLCReport_CustomCode
' Merged with logging and utility functions
' ==========================================

' =================
' Global variables
' =================
Shared GlobalDict As Microsoft.VisualBasic.Collection

' NAV way - Legacy support for Data1, Data2, Data3
Shared Data1 As Object
Shared Data2 As Object
Shared Data3 As Object

' =================
' Logging variables
' =================
Private cachedLogPath As String = Nothing
Private cachedFilePath As String = Nothing
Private cachedFileName As String = Nothing
Private cachedDate As String = Nothing

' ==========================
' Logging Methods
' ==========================

' Simple logging without caching (always generates new path)
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

' Advanced logging with caching for better performance in same session
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
' Get value by name or number
' ==========================

Public Function GetVal(Key as Object)
  Return GetVal2(GlobalDict,Key)
End Function

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

' ===========================================
' Set global values from the body 
' ===========================================

Public Function SetGlobalData(KeyValueList as Object)
  SetDataAsKeyValueList(GlobalDict,KeyValueList)
  Return True 'Set Control to Hidden=true
End Function
 
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

' SetData - saves a list of values in Data1, Data2, or Data3
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

' GetData - returns a value from one of the 3 lists at position number
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
   
 Public Function ConcatenateWithCrLf(ByVal strings As String()) As String
        Return String.Join(vbCrLf, strings)
    End Function

Public currencyDenotionIndian As String() = New String() {"Rupees", "Paise"}

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
