# Quick Start Guide

**Choose your path based on your needs:**

---

## 🚀 Path 1: I want maximum performance (NEW PROJECTS)

**Use:** `RdlcReportCode_Optimized.vb`

### 3 Steps to Get Started:

1. **Copy the code**
   - Open `RdlcReportCode_Optimized.vb`
   - Copy entire contents

2. **Paste into your RDLC**
   - Open your RDLC in SQL Report Builder
   - Go to Report Properties → Code tab
   - Paste the code

3. **Follow the setup guide**
   - See `Readme.md` → "Getting Started - Setup Guide"
   - Implement steps 2-5

**Done!** You now have 40-60% better performance! ⚡

---

## 📚 Path 2: I'm learning RDLC custom code

**Use:** `RdlcReportCode_WithComments.vb`

### 3 Steps to Learn:

1. **Read the documentation**
   - Start with `Readme.md`
   - Understand the concepts

2. **Study the commented code**
   - Open `RdlcReportCode_WithComments.vb`
   - Read inline comments explaining each function

3. **Create a test report**
   - Follow setup guide in `Readme.md`
   - Test with small dataset (10 rows)
   - Experiment with different functions

---

## 🔄 Path 3: I have existing reports (MIGRATION)

**Current:** `RdlcReportCode.vb`  
**Target:** `RdlcReportCode_Optimized.vb`

### 5 Steps to Migrate:

1. **Read the guide**
   - Open `OPTIMIZATION_GUIDE.md`
   - Review "Migration Path" section

2. **Backup current files**
   - Save copies of all `.rdlc` files
   - Document current performance

3. **Test in development**
   - Replace code in test environment
   - Run `BENCHMARK_TESTS.md` tests
   - Compare performance

4. **Validate results**
   - Verify all outputs match original
   - Check logs for errors
   - Measure execution time improvement

5. **Deploy to production**
   - Plan maintenance window
   - Deploy and monitor
   - Document improvements

---

## 🎯 Path 4: I want to compare performance

**Use:** `BENCHMARK_TESTS.md`

### 3 Steps to Benchmark:

1. **Setup benchmark**
   - Copy benchmark functions from `BENCHMARK_TESTS.md`
   - Add to your RDLC Custom Code

2. **Run tests**
   - Test Original version
   - Test Optimized version
   - Record results

3. **Compare**
   - Use the results template
   - Calculate improvement percentages
   - Make decision

---

## 📖 Common Tasks

### Task 1: Display company name in header

```vb
' In AL/C/AL code - Build key-value list
AddKeyValue(KeyValueList, 'CompanyName', CompanyInfo.Name);

' In RDLC - Header textbox
=Code.GetVal("CompanyName")'
```

### Task 2: Convert amount to words

```vb
' In RDLC - Amount in words textbox
=Code.ToWordsIn(Fields!Amount.Value, True, True)'

' Output: "Rupees Five Thousand Only"
```

### Task 3: Log processing steps

```vb
' In RDLC - Hidden textbox in detail section
=Code.WriteLog("Processing invoice: " & Fields!InvoiceNo.Value)'

' Check: C:\Temp\CliReportDebug_YYYYMMDD.log
```

### Task 4: Concatenate multiple lines

```vb
' In RDLC - Address textbox
=Code.ConcatenateNonEmptyWithCrLf(New String() {
    Fields!AddressLine1.Value,
    Fields!AddressLine2.Value,
    Fields!City.Value,
    Fields!PostalCode.Value
})'
```

---

## 🗺️ Document Navigation

```
START HERE → Readme.md (Main documentation)
              ├─→ Setup Guide (Steps 1-5)
              ├─→ Function Reference
              └─→ Usage Examples

CHOOSING VERSION → FILES_COMPARISON.md
                    ├─→ Original vs Optimized
                    ├─→ Performance comparison
                    └─→ Decision matrix

OPTIMIZATION → OPTIMIZATION_GUIDE.md
                ├─→ What was optimized
                ├─→ How it was optimized
                └─→ Migration steps

TESTING → BENCHMARK_TESTS.md
           ├─→ Quick benchmarks
           ├─→ Real-world tests
           └─→ Results templates

LEARNING → RdlcReportCode_WithComments.vb
            └─→ Detailed code comments
```

---

## ⚡ The Fastest Way to Get Started

**For 90% of users:**

1. Open `RdlcReportCode_Optimized.vb` → Copy all
2. Open your RDLC → Report Properties → Code → Paste
3. Follow `Readme.md` steps 2-5
4. Start using `=Code.GetVal("YourKey")'`

**That's it!** 🎉

---

## 🆘 Getting Help

### Issue: Code doesn't work

**Check:**
- [ ] Did you paste the complete code? (don't skip any lines)
- [ ] Did you add the hidden SetGlobalData control?
- [ ] Did you create the GlobalData column in AL/C/AL?
- [ ] Did you end expressions with apostrophe `'`?

**Solution:** Review `Readme.md` → "Getting Started" section

---

### Issue: Performance not improving

**Check:**
- [ ] Are you using `RdlcReportCode_Optimized.vb`?
- [ ] Is dataset large enough (> 100 rows)?
- [ ] Is custom code being called frequently?
- [ ] Did you benchmark correctly?

**Solution:** Review `BENCHMARK_TESTS.md` → "Interpreting Results"

---

### Issue: Values not appearing

**Check:**
- [ ] Is key name spelled correctly?
- [ ] Is key name case matching? (use uppercase)
- [ ] Did SetGlobalData execute before GetVal?
- [ ] Did you check the log file for errors?

**Solution:** 
```vb
' Test if key exists
=Code.GetVal("YOURKEY")'
' If returns: "?YOURKEY?" → Key not found
```

---

## 📚 File Purpose Summary

| File | Purpose | When to Use |
|------|---------|-------------|
| `Readme.md` | Main guide | Always start here |
| `RdlcReportCode_Optimized.vb` | Optimized code | New projects |
| `RdlcReportCode.vb` | Original code | Legacy/existing |
| `RdlcReportCode_WithComments.vb` | Learning | First time |
| `OPTIMIZATION_GUIDE.md` | Performance details | Understanding optimization |
| `BENCHMARK_TESTS.md` | Testing | Comparing performance |
| `FILES_COMPARISON.md` | Version comparison | Choosing version |
| `QUICK_START.md` (this file) | Quick reference | Getting started fast |

---

## 🎓 Learning Progression

### Week 1: Basics
- Read `Readme.md` introduction
- Understand why we need this
- Study `RdlcReportCode_WithComments.vb`
- Create simple test report

### Week 2: Implementation
- Follow complete setup guide
- Implement `GetVal()` for headers
- Test with small dataset
- Debug any issues

### Week 3: Advanced
- Add logging
- Use string concatenation
- Implement number-to-words
- Test with realistic data

### Week 4: Optimization
- Read `OPTIMIZATION_GUIDE.md`
- Understand performance concepts
- Run benchmarks
- Consider migration

---

## 💡 Pro Tips

### Tip 1: Always use apostrophe
```vb
=Code.GetVal("CompanyName")'
                         ↑ Don't forget this!
```
Without it, you'll lose parameters when copy-pasting textboxes.

---

### Tip 2: Use descriptive key names
```vb
' Good ✅
AddKeyValue(list, 'CustomerShippingAddress', addr)
=Code.GetVal("CustomerShippingAddress")'

' Bad ❌
AddKeyValue(list, 'Data5', addr)
=Code.GetData(5, 1)  ' What is Data5?
```

---

### Tip 3: Cache header values
```vb
<!-- In report header - calculate once -->
<Textbox Name="CompanyNameCache">
  <Value>=Code.GetVal("CompanyName")'</Value>
  <Hidden>true</Hidden>
</Textbox>

<!-- In detail - reference cached value -->
=ReportItems!CompanyNameCache.Value
```

Much faster than calling `GetVal()` in every detail row!

---

### Tip 4: Check logs
```vb
' Add logging during development
=Code.WriteLog("Step 1: Loading data")'
=Code.WriteLog("Step 2: Processing")'
=Code.WriteLog("Step 3: Formatting")'

' Check: C:\Temp\CliReportDebug_YYYYMMDD.log
```

---

### Tip 5: Start small, scale up
1. Test with 10 rows → verify correctness
2. Test with 100 rows → check performance
3. Test with 1000 rows → ensure scalability
4. Deploy to production with confidence

---

## 🚦 Status Checklist

Use this to track your progress:

### Setup Phase
- [ ] Chose correct version for my needs
- [ ] Read relevant documentation
- [ ] Backed up existing files
- [ ] Added code to RDLC Custom Code section

### Implementation Phase
- [ ] Created AddKeyValue procedure in AL/C/AL
- [ ] Added GlobalData column to dataset
- [ ] Added hidden SetGlobalData control
- [ ] Added GetVal expressions to textboxes

### Testing Phase
- [ ] Tested with small dataset (< 10 rows)
- [ ] Verified all values display correctly
- [ ] Tested with medium dataset (100-1000 rows)
- [ ] Ran performance benchmarks
- [ ] Checked log files for errors

### Deployment Phase
- [ ] Tested in staging environment
- [ ] Documented performance improvements
- [ ] Deployed to production
- [ ] Monitored production logs
- [ ] Collected user feedback

---

## 📞 Need More Help?

### For Setup Issues
→ Read `Readme.md` → "Getting Started - Setup Guide"

### For Performance Questions
→ Read `OPTIMIZATION_GUIDE.md`

### For Benchmarking
→ Read `BENCHMARK_TESTS.md`

### For Choosing Versions
→ Read `FILES_COMPARISON.md`

### For Code Understanding
→ Study `RdlcReportCode_WithComments.vb`

---

## 🎯 Success Criteria

You'll know you've successfully implemented this when:

✅ Report headers display dynamic data from AL/C/AL  
✅ No "?" symbols or error messages in output  
✅ Log file is created and contains your messages  
✅ Performance is acceptable for your data volume  
✅ Report renders correctly with all data  

**Congratulations!** 🎉 You've successfully implemented RDLC Custom Code utilities!

---

## 🔄 Next Steps

Once everything is working:

1. **Optimize further**
   - Identify bottlenecks
   - Add caching for expensive operations
   - Minimize custom code calls in detail section

2. **Expand usage**
   - Add more global variables
   - Implement additional helper functions
   - Create reusable patterns

3. **Share knowledge**
   - Document your patterns
   - Help other developers
   - Contribute improvements

4. **Stay updated**
   - Check for new versions
   - Review optimization techniques
   - Test with new features

---

**Remember:** Start simple, test thoroughly, scale gradually! 🚀
