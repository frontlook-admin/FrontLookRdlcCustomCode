Imports System
Imports System.Text
Imports System.Diagnostics
Imports System.Collections.Generic
Imports Microsoft.VisualBasic

' Benchmark Test Application for RDLC Custom Code
' Run this to compare Original vs Optimized versions

Module BenchmarkTest
    
    ' Constants for testing
    Private Const TEST_ITERATIONS As Integer = 100
    Private Const TEST_STRING_COUNT As Integer = 50
    Private Const TEST_KEYVALUE_PAIRS As Integer = 50
    Private Const DELIMITER_CHAR As Char = Chr(177)
    
    ' Results storage
    Private results As New StringBuilder()
    
    Sub Main()
        Console.WriteLine("=" & New String("="c, 70))
        Console.WriteLine("RDLC Custom Code - Performance Benchmark Test")
        Console.WriteLine("=" & New String("="c, 70))
        Console.WriteLine()
        
        Console.WriteLine("Test Configuration:")
        Console.WriteLine("  - Test Iterations: " & TEST_ITERATIONS.ToString())
        Console.WriteLine("  - String Array Size: " & TEST_STRING_COUNT.ToString())
        Console.WriteLine("  - Key-Value Pairs: " & TEST_KEYVALUE_PAIRS.ToString())
        Console.WriteLine()
        Console.WriteLine("Starting benchmark tests...")
        Console.WriteLine()
        
        results.AppendLine("BENCHMARK TEST RESULTS")
        results.AppendLine("Date: " & DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
        results.AppendLine("Environment: " & Environment.OSVersion.ToString())
        results.AppendLine()
        results.AppendLine("Test Configuration:")
        results.AppendLine("  - Test Iterations: " & TEST_ITERATIONS.ToString())
        results.AppendLine("  - String Array Size: " & TEST_STRING_COUNT.ToString())
        results.AppendLine("  - Key-Value Pairs: " & TEST_KEYVALUE_PAIRS.ToString())
        results.AppendLine()
        
        ' Run all benchmark tests
        RunTest1_Logging()
        RunTest2_StringConcatenation()
        RunTest3_NumberToWords()
        RunTest4_KeyValueParsing()
        
        ' Display summary
        Console.WriteLine()
        Console.WriteLine("=" & New String("="c, 70))
        Console.WriteLine("BENCHMARK SUMMARY")
        Console.WriteLine("=" & New String("="c, 70))
        Console.WriteLine()
        Console.WriteLine(results.ToString())
        
        ' Save results to file
        Dim resultsPath As String = "G:\Repos\frontlook-admin\FrontLookRdlcCustomCode\BenchmarkResults_" & DateTime.Now.ToString("yyyyMMdd_HHmmss") & ".txt"
        Try
            System.IO.File.WriteAllText(resultsPath, results.ToString())
            Console.WriteLine()
            Console.WriteLine("Results saved to: " & resultsPath)
        Catch ex As Exception
            Console.WriteLine("Could not save results: " & ex.Message)
        End Try
        
        Console.WriteLine()
        Console.WriteLine("Press any key to exit...")
        Console.ReadKey()
    End Sub
    
    ' Test 1: Logging Performance
    Private Sub RunTest1_Logging()
        Console.WriteLine("Test 1: Logging Performance")
        Console.WriteLine("  Testing " & TEST_ITERATIONS.ToString() & " log writes...")
        
        Dim sw As New Stopwatch()
        Dim testPath As String = System.IO.Path.GetTempPath()
        Dim testFile As String = "BenchmarkLog_" & DateTime.Now.ToString("yyyyMMdd_HHmmss")
        
        sw.Start()
        For i As Integer = 1 To TEST_ITERATIONS
            WriteLogSimple("Test message number " & i.ToString(), testPath, testFile)
        Next
        sw.Stop()
        
        Dim elapsed As Long = sw.ElapsedMilliseconds
        Console.WriteLine("  ✓ Completed in: " & elapsed.ToString() & " ms")
        Console.WriteLine("  ✓ Average per log: " & (elapsed / TEST_ITERATIONS).ToString("F2") & " ms")
        Console.WriteLine()
        
        results.AppendLine("Test 1 - Logging Performance (" & TEST_ITERATIONS.ToString() & " calls):")
        results.AppendLine("  Total Time: " & elapsed.ToString() & " ms")
        results.AppendLine("  Average: " & (elapsed / TEST_ITERATIONS).ToString("F2") & " ms per call")
        results.AppendLine("  Expected (Optimized): ~100ms total")
        results.AppendLine("  Status: " & If(elapsed < 200, "✓ PASS", "⚠ SLOWER THAN EXPECTED"))
        results.AppendLine()
    End Sub
    
    ' Test 2: String Concatenation Performance
    Private Sub RunTest2_StringConcatenation()
        Console.WriteLine("Test 2: String Concatenation Performance")
        Console.WriteLine("  Testing concatenation of " & TEST_STRING_COUNT.ToString() & " strings x 10...")
        
        ' Prepare test data
        Dim testStrings(TEST_STRING_COUNT - 1) As String
        For i As Integer = 0 To TEST_STRING_COUNT - 1
            testStrings(i) = "Line number " & i.ToString() & " with some additional text for realism"
        Next
        
        ' Test with String concatenation (Original way)
        Dim sw1 As New Stopwatch()
        sw1.Start()
        For j As Integer = 1 To 10
            Dim result1 As String = ConcatenateWithString(testStrings)
        Next
        sw1.Stop()
        Dim time1 As Long = sw1.ElapsedMilliseconds
        
        ' Test with StringBuilder (Optimized way)
        Dim sw2 As New Stopwatch()
        sw2.Start()
        For j As Integer = 1 To 10
            Dim result2 As String = ConcatenateWithStringBuilder(testStrings)
        Next
        sw2.Stop()
        Dim time2 As Long = sw2.ElapsedMilliseconds
        
        Dim improvement As Double = If(time1 > 0, ((time1 - time2) / time1) * 100, 0)
        
        Console.WriteLine("  ✓ Original (String concat): " & time1.ToString() & " ms")
        Console.WriteLine("  ✓ Optimized (StringBuilder): " & time2.ToString() & " ms")
        Console.WriteLine("  ✓ Improvement: " & improvement.ToString("F1") & "%")
        Console.WriteLine()
        
        results.AppendLine("Test 2 - String Concatenation (" & TEST_STRING_COUNT.ToString() & " strings x 10):")
        results.AppendLine("  Original Method: " & time1.ToString() & " ms")
        results.AppendLine("  Optimized Method: " & time2.ToString() & " ms")
        results.AppendLine("  Improvement: " & improvement.ToString("F1") & "%")
        results.AppendLine("  Expected: ~75% improvement")
        results.AppendLine("  Status: " & If(improvement > 50, "✓ PASS", "⚠ LOWER THAN EXPECTED"))
        results.AppendLine()
    End Sub
    
    ' Test 3: Number to Words Performance
    Private Sub RunTest3_NumberToWords()
        Console.WriteLine("Test 3: Number to Words Conversion")
        Console.WriteLine("  Testing 100 conversions with cache hits...")
        
        Dim testNumbers() As Long = {1000, 5000, 10000, 25000, 50000}
        
        ' Test without caching (Original way)
        Dim sw1 As New Stopwatch()
        sw1.Start()
        For i As Integer = 1 To 20
            For Each num As Long In testNumbers
                Dim result As String = ToWordsInNoCaching(num)
            Next
        Next
        sw1.Stop()
        Dim time1 As Long = sw1.ElapsedMilliseconds
        
        ' Test with caching (Optimized way)
        Dim cache As New Dictionary(Of Long, String)
        Dim sw2 As New Stopwatch()
        sw2.Start()
        For i As Integer = 1 To 20
            For Each num As Long In testNumbers
                Dim result As String = ToWordsInWithCaching(num, cache)
            Next
        Next
        sw2.Stop()
        Dim time2 As Long = sw2.ElapsedMilliseconds
        
        Dim improvement As Double = If(time1 > 0, ((time1 - time2) / time1) * 100, 0)
        
        Console.WriteLine("  ✓ Without Cache: " & time1.ToString() & " ms")
        Console.WriteLine("  ✓ With Cache: " & time2.ToString() & " ms")
        Console.WriteLine("  ✓ Improvement: " & improvement.ToString("F1") & "%")
        Console.WriteLine()
        
        results.AppendLine("Test 3 - Number to Words (100 conversions with repeats):")
        results.AppendLine("  Without Caching: " & time1.ToString() & " ms")
        results.AppendLine("  With Caching: " & time2.ToString() & " ms")
        results.AppendLine("  Improvement: " & improvement.ToString("F1") & "%")
        results.AppendLine("  Expected: ~80% improvement")
        results.AppendLine("  Status: " & If(improvement > 60, "✓ PASS", "⚠ LOWER THAN EXPECTED"))
        results.AppendLine()
    End Sub
    
    ' Test 4: Key-Value Parsing Performance
    Private Sub RunTest4_KeyValueParsing()
        Console.WriteLine("Test 4: Key-Value List Parsing")
        Console.WriteLine("  Testing parsing of " & TEST_KEYVALUE_PAIRS.ToString() & " pairs x 20...")
        
        ' Create test data
        Dim testData As String = ""
        For i As Integer = 1 To TEST_KEYVALUE_PAIRS
            If testData <> "" Then testData &= DELIMITER_CHAR
            testData &= "Key" & i.ToString() & DELIMITER_CHAR & "Value" & i.ToString() & " with some data"
        Next
        
        ' Test original method (O(n²))
        Dim sw1 As New Stopwatch()
        sw1.Start()
        For i As Integer = 1 To 20
            Dim dict1 As New Collection()
            ParseKeyValueOriginal(dict1, testData)
        Next
        sw1.Stop()
        Dim time1 As Long = sw1.ElapsedMilliseconds
        
        ' Test optimized method (O(n))
        Dim sw2 As New Stopwatch()
        sw2.Start()
        For i As Integer = 1 To 20
            Dim dict2 As New Collection()
            ParseKeyValueOptimized(dict2, testData)
        Next
        sw2.Stop()
        Dim time2 As Long = sw2.ElapsedMilliseconds
        
        Dim improvement As Double = If(time1 > 0, ((time1 - time2) / time1) * 100, 0)
        
        Console.WriteLine("  ✓ Original Method: " & time1.ToString() & " ms")
        Console.WriteLine("  ✓ Optimized Method: " & time2.ToString() & " ms")
        Console.WriteLine("  ✓ Improvement: " & improvement.ToString("F1") & "%")
        Console.WriteLine()
        
        results.AppendLine("Test 4 - Key-Value Parsing (" & TEST_KEYVALUE_PAIRS.ToString() & " pairs x 20):")
        results.AppendLine("  Original Method (O(n²)): " & time1.ToString() & " ms")
        results.AppendLine("  Optimized Method (O(n)): " & time2.ToString() & " ms")
        results.AppendLine("  Improvement: " & improvement.ToString("F1") & "%")
        results.AppendLine("  Expected: ~50% improvement")
        results.AppendLine("  Status: " & If(improvement > 30, "✓ PASS", "⚠ LOWER THAN EXPECTED"))
        results.AppendLine()
    End Sub
    
    ' Helper: Simple logging (simulates optimized caching)
    Private Sub WriteLogSimple(message As String, path As String, fileName As String)
        Try
            Dim fullPath As String = System.IO.Path.Combine(path, fileName & ".log")
            System.IO.File.AppendAllText(fullPath, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") & " - " & message & vbCrLf)
        Catch
            ' Ignore
        End Try
    End Sub
    
    ' Helper: Original string concatenation
    Private Function ConcatenateWithString(strings As String()) As String
        Dim result As String = ""
        For Each str As String In strings
            If Not String.IsNullOrEmpty(str) Then
                If result <> "" Then result &= vbCrLf
                result &= str
            End If
        Next
        Return result
    End Function
    
    ' Helper: Optimized string concatenation
    Private Function ConcatenateWithStringBuilder(strings As String()) As String
        Dim sb As New StringBuilder()
        Dim first As Boolean = True
        For Each str As String In strings
            If Not String.IsNullOrEmpty(str) Then
                If Not first Then sb.Append(vbCrLf)
                sb.Append(str)
                first = False
            End If
        Next
        Return sb.ToString()
    End Function
    
    ' Helper: Number to words without caching
    Private Function ToWordsInNoCaching(number As Long) As String
        If number = 0 Then Return "zero"
        If number < 0 Then Return "minus " & ToWordsInNoCaching(Math.Abs(number))
        
        Dim words As String = ""
        If (number \ 10000000) > 0 Then
            words += ToWordsInNoCaching(number \ 10000000) & " Crore "
            number = number Mod 10000000
        End If
        If (number \ 100000) > 0 Then
            words += ToWordsInNoCaching(number \ 100000) & " Lakh "
            number = number Mod 100000
        End If
        If (number \ 1000) > 0 Then
            words += ToWordsInNoCaching(number \ 1000) & " Thousand "
            number = number Mod 1000
        End If
        If (number \ 100) > 0 Then
            words += ToWordsInNoCaching(number \ 100) & " Hundred "
            number = number Mod 100
        End If
        
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
    
    ' Helper: Number to words with caching
    Private Function ToWordsInWithCaching(number As Long, cache As Dictionary(Of Long, String)) As String
        If cache.ContainsKey(number) Then
            Return cache(number)
        End If
        
        Dim result As String = ToWordsInNoCaching(number)
        If number < 10000 Then
            cache(number) = result
        End If
        Return result
    End Function
    
    ' Helper: Original key-value parsing (O(n²))
    Private Sub ParseKeyValueOriginal(ByRef data As Collection, input As String)
        Dim words As String() = Split(input, DELIMITER_CHAR)
        Dim i As Integer
        For i = 1 To UBound(words)
            If ((i Mod 2) = 0) Then
                Dim key As String = CStr(Choose(i - 1, Split(input, DELIMITER_CHAR)))
                Dim value As String = CStr(Choose(i, Split(input, DELIMITER_CHAR)))
                If data.Contains(key.ToUpper()) Then data.Remove(key.ToUpper())
                data.Add(value, key.ToUpper())
            End If
        Next
    End Sub
    
    ' Helper: Optimized key-value parsing (O(n))
    Private Sub ParseKeyValueOptimized(ByRef data As Collection, input As String)
        Dim words As String() = Split(input, DELIMITER_CHAR)
        For i As Integer = 0 To words.Length - 2 Step 2
            Dim key As String = words(i).ToUpper()
            Dim value As String = words(i + 1)
            If data.Contains(key) Then data.Remove(key)
            data.Add(value, key)
        Next
    End Sub
    
End Module
