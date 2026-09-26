using Azure;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
namespace CleanserBlazorUI.Helpers;
public class StringHelper 
{
    string FILENAME = string.Empty;
    public bool AccountCustomerInValidCharacter(string input)
    {
        // Return false if input is null
        if (input == null)
            return false;

        // Check each character in the string
        foreach (char c in input)
        {
            if (!Char.IsLetterOrDigit(c) && c != '.' && c != '-' && c != '/')
            {
                return false;
            }
        }

        // All characters are valid
        return true;
    }
    public bool StartsWithFourXOrZero(string input)
    {
        // Check for null or insufficient length first
        if (input == null || input.Length < 4)
            return false;

        // Check the first four characters
        string firstFour = input.Substring(0, 4);
        return firstFour.Equals("xxxx", StringComparison.OrdinalIgnoreCase)
            || firstFour == "0000";
    }
    public bool ContainsInvalidCharactersBusiness(string input)
    {
        foreach (char c in input)
        {
            if (!IsAllowedCharacter(c) && c != '‘' && c != '\'')
            {
                return true; // Invalid character found
            }
        }
        return false; // All characters are valid
    }
    private readonly HashSet<char> AllowedSymbols = new HashSet<char>
{
    '(', ')', '&', '@', ':', '/', '_', '-', ' ', '\'', '‘', '’'
};
    private bool IsAllowedCharacter(char c)
    {
        return (c >= 'a' && c <= 'z') ||
               (c >= 'A' && c <= 'Z') ||
               (c >= '0' && c <= '9') ||
               AllowedSymbols.Contains(c);
    }
    //private static bool IsAllowedCharacter(char c)
    //{
    //    // Check if character is a letter, digit, allowed symbol, or space
    //    return char.IsLetterOrDigit(c)
    //        || "()&@:/_- ".Contains(c);
    //}
    public string CleanString(string input)
    {
        // Regex pattern to detect a domain (e.g., "example.com" or "sub.example.co.uk")
        string domainPattern = @"\b([a-zA-Z0-9-]+\.)+[a-zA-Z]{2,}\b";
        bool containsDomain = Regex.IsMatch(input, domainPattern);

        string cleaned;
        if (containsDomain)
        {
            // Replace commas and underscores, leave dots
            cleaned = Regex.Replace(input, @"[, _]", " ");
        }
        else
        {
            // Replace dots, commas, and underscores
            cleaned = Regex.Replace(input, @"[., _]", " ");
        }

        // Collapse multiple spaces and trim
        cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();
        return cleaned;
    }

    //public string ConvertBusinessShortFormToLongForm(string input)
    //{
    //    input = input.Trim().Replace(".", " ").Replace(",", " ").ToUpper();

    //    var shortForms = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    //    {
    //        { "A/C", "ACCOUNT" },
    //        { "ACCT", "ACCOUNT" },
    //        { "ASSOC", "ASSOCIATION" },
    //        { "ASSO", "ASSOCIATION" },
    //        { "CO", "COMPANY" },
    //        { "COM", "COMPANY" },
    //        { "CONST", "CONSTRUCTION" },
    //        { "CON", "CONSULT" },
    //        { "CRDT", "CREDIT" },
    //        { "DEVT", "DEVELOPMENT" },
    //        { "DEV’T", "DEVELOPMENT" },
    //        { "ENG", "ENGINEERING" },
    //        { "GH", "GHANA" },
    //        { "(GH)", "GHANA" },
    //        { "GOV", "GOVERNMENT" },
    //        { "GOV’T", "GOVERNMENT" },
    //        { "GRP", "GROUP" },
    //        { "INT", "INTERNATIONAL" },
    //        { "INV", "INVESTMENT" },
    //        { "LT", "LIMITED" },
    //        { "LTD", "LIMITED" },
    //        { "MKTG", "MARKETING" },
    //        { "ND", "AND" },
    //        { "SCH", "SCHOOL" },
    //        { "SER", "SERVICE" },
    //        { "SERV", "SERVICE" },
    //        { "SYS", "SYSTEM" },
    //        { "TRAD", "TRADING" },
    //        { "WKS", "WORKS" },
    //        { "WK", "WORKS" },
    //        { "ENT", "ENTERPRISE" },
    //        { "HOSP", "HOSPITAL" }
    //    };

    //    var tokens = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
    //    for (int i = 0; i < tokens.Length; i++)
    //    {
    //        if (shortForms.TryGetValue(tokens[i], out var expanded))
    //        {
    //            tokens[i] = expanded;
    //        }
    //    }
    //    return string.Join(" ", tokens);
    //}
    // Filenames encode type as underscore-delimited tokens, and DUD/JUD variants
    // combine with IND/BUS in either order (e.g. ABSA0124_DUD_IND, ABSA0124_JUD_BUS).
    // Taking a fixed token index (the old approach) misclassifies every DUD/JUD
    // file as "DUD"/"JUD" instead of "ind"/"bus", which breaks subscriber-shortcode
    // lookup for those files regardless of whether the shortcode exists. Scanning
    // all tokens for an exact IND/BUS match handles every combination uniformly.
    public string GetBusinessTypeFromFile(string _file)
    {
        var nameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(_file);
        var tokens = nameWithoutExtension.Split('_', StringSplitOptions.RemoveEmptyEntries);
        if (tokens.Any(t => string.Equals(t, "IND", StringComparison.OrdinalIgnoreCase)))
        {
            return "ind";
        }
        if (tokens.Any(t => string.Equals(t, "BUS", StringComparison.OrdinalIgnoreCase)))
        {
            return "bus";
        }
        return string.Empty;
    }


    /// <summary>
    /// Checks if the input string contains any non-numeric characters.
    /// Returns true if the string contains at least one non-numeric character, otherwise false.
    /// </summary>
    /// <param name="input">The string to be checked</param>
    /// <returns>True if the string contains non-numeric characters, otherwise false</returns>
    public bool IsContainingNonNumericCharacters(string input)
    {
        // If the string is null or empty, we consider it as containing non-numeric characters
        if (string.IsNullOrEmpty(input))
        {
            return true;
        }

        // Check each character in the string
        foreach (char c in input)
        {
            // If any character is not a digit, return true
            if (!char.IsDigit(c) && c != '.')
            {
                return true;
            }
        }

        // If all characters are digits, return false
        return false;
    }

    public string JoinErrors(IEnumerable<string> errors)
    {
        return string.Join(" | ", errors.Where(e => !string.IsNullOrWhiteSpace(e)));
    }

    public string GetSamePersonalIDPartialMismatchError(string idField)
    {
        return $"CROSS-RECORD CONFLICT: {idField} IS SHARED ACROSS MULTIPLE CUSTOMER IDS WITH SAME DOB BUT SIMILAR NAME(S) — POSSIBLE MISSPELLING";
    }
    //public bool ContainsAnyLetter(string input)
    //{
    //    foreach (char c in input)
    //    {
    //        if (char.IsLetter(c))
    //        {
    //            return true; // Return true if at least one letter is found
    //        }
    //    }
    //    return false; // Return false if no letters are found
    //}

    public string GetFirstPartExcludeLastPart(string input, int n)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= n)
            return string.Empty;

        return input[..^n]; // Using range operator to remove last n characters
    }

    public string GetLast_N_CharsOfString(string data, int howManyLastWords)
    {
        if (string.IsNullOrEmpty(data)) return data;

        ReadOnlySpan<char> span = data.AsSpan();
        return span.Length > howManyLastWords ? new string(span[^howManyLastWords..]) : data;
    }

    bool IsOnlyDigits(string data)
    {
        if (string.IsNullOrEmpty(data)) return false;

        var span = data.AsSpan();
        for (int i = 0; i < span.Length; i++)
        {
            if (!char.IsDigit(span[i])) return false;
        }
        return true;
    }
    public string GetLoanStatusCode(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        input = input.Trim().ToUpper();

        return input switch
        {
            "A" => "A",
            "B" => "B",
            "C" => "C",
            "D" => "D",
            "E" => "E",
            "CURRENT" => "A",
            "OLEM" => "B",
            "SUBSTANDARD" => "C",
            "DOUBTFUL" => "D",
            "LOSS" => "E",
        };
    }
    public bool HasDigitRepeatingMoreThanFiveTimes(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length < 6) return false;

        Dictionary<char, int> frequency = new Dictionary<char, int>();

        foreach (char ch in input)
        {
            if (char.IsDigit(ch)) // Check only numbers
            {
                if (frequency.ContainsKey(ch))
                {
                    frequency[ch]++;
                    if (frequency[ch] > 5) return true; // Early exit
                }
                else
                {
                    frequency[ch] = 1;
                }
            }
        }

        return false;
    }
    public bool HasLetterRepeatingMoreThanFiveTimes(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length < 6) return false;

        Dictionary<char, int> frequency = new Dictionary<char, int>();

        foreach (char ch in input)
        {
            if (char.IsLetter(ch)) // Ignore non-letter characters if needed
            {
                if (frequency.ContainsKey(ch))
                {
                    frequency[ch]++;
                    if (frequency[ch] > 5) return true; // Early exit
                }
                else
                {
                    frequency[ch] = 1;
                }
            }
        }

        return false;
    }
    //public bool IsEmptyData_CPT(object input)
    //{
    //    if (input == null)
    //        return true;

    //    if (input is string str)
    //    {
    //        // Trim to remove whitespaces before checking
    //        str = str.Trim();
    //        return string.IsNullOrEmpty(str) || str == "-" || str == "0";
    //    }

    //    if (input is int intValue)
    //        return intValue == 0;

    //    if (input is double doubleValue)
    //        return doubleValue == 0.0;

    //    // Add more numeric types if needed
    //    return false;
    //}
    public bool IsEmptyDataChargeOff(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return true;
        }
        return false;
    }
    public bool IsEmptyData(string input)
    {
        if (input == "-" || input == "0" || string.IsNullOrWhiteSpace(input))
        {
            return true;
        }
        return false;
    }
    public bool IsEmptyDataNDIA(string input)
    {
        if (input == "-" || input == "0" || string.IsNullOrWhiteSpace(input))
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Normalizes a numeric string by determining whether the comma is used as a decimal separator or a thousand separator.
    /// If the comma is used as a thousand separator, it is removed.
    /// If it is used as a decimal separator, it is replaced with a period.
    /// </summary>
    /// <param name="input">The input numeric string.</param>
    /// <returns>A normalized numeric string with appropriate formatting.</returns>
    public string NormalizeDecimalOrComma(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return input;
        }

        // Trim any leading or trailing whitespace.
        input = input.Trim();

        // Count how many commas are in the input.
        int commaCount = input.Count(c => c == ',');

        // If there are no commas, return the input as is.
        if (commaCount == 0)
        {
            return input;
        }

        // If there are multiple commas, assume they are thousand separators.
        if (commaCount > 1)
        {
            return input.Replace(",", "");
        }

        // For exactly one comma, determine its role by checking the number of digits after it.
        int commaIndex = input.IndexOf(',');
        int digitsAfterComma = input.Length - commaIndex - 1;

        // If fewer than 3 digits follow the comma, treat it as a decimal separator.
        if (digitsAfterComma < 3)
        {
            // Replace the comma with a period.
            return input.Replace(',', '.');
        }
        else
        {
            // Otherwise, assume it is a thousand separator and remove it.
            return input.Replace(",", "");
        }
    }
    public (decimal Value, bool IsValid) ConvertStringToDecimal(string input)
    {
        if (decimal.TryParse(input, out decimal result))
        {
            // Check if the number has exactly two decimal places
            int decimalPlaces = BitConverter.GetBytes(decimal.GetBits(result)[3])[2];
            if (decimalPlaces <= 2)
            {
                return (result, true);
            }
        }

        return (0m, false);
    }
    public (decimal Value, bool IsValid) ValidateDecimalInput(string input)
    {
        var (value, isValid) = ConvertStringToDecimal(input);
        if (isValid)
        {
            // Conversion successful and decimal places are not more than 2
            return (value, true);
        }

        // Conversion failed or more than two decimal places
        return (0m, false);
    }
        public CellDataAndStatus[] FacilityAmount_DisbursementAmt(string facilityAmount, string disbursementAmt)
    {
        CellDataAndStatus[] cds = [new CellDataAndStatus(facilityAmount), new CellDataAndStatus(disbursementAmt)];
        facilityAmount = facilityAmount.Replace("’", "").Replace("-", "").Replace(",", "");
        disbursementAmt = disbursementAmt.Replace("’", "").Replace("-", "").Replace(",", "");
        //determin comma
        facilityAmount = NormalizeDecimalOrComma(facilityAmount);
        disbursementAmt = NormalizeDecimalOrComma(disbursementAmt);


        var _f_Data = CleanToTwoDecimalPlaces(facilityAmount);
        var _d_Data = CleanToTwoDecimalPlaces(disbursementAmt);


        var F_Amt = ValidateDecimalInput(_f_Data.Data).Value;
        var D_Amt = ValidateDecimalInput(_d_Data.Data).Value;


        // Condition 1: None of the data can be empty or zero
        if (D_Amt > 0 && F_Amt > 0)
        {
            cds[0].Passed = true;
            cds[1].Passed = true;
            cds[0].Data = F_Amt.ToString();
            cds[1].Data = D_Amt.ToString();
            return cds;
        }

        // Condition 2: Facility amount is empty
        if (IsEmptyData(facilityAmount) && D_Amt > 0)
        {
            cds[0].Passed = true;
            cds[1].Passed = true;
            cds[0].Data = D_Amt.ToString();
            cds[1].Data = D_Amt.ToString();
            var yyy = cds;
            return cds;

        }

        // Condition 3: Disbursement amount is empty
        if (IsEmptyData(disbursementAmt) && F_Amt > 0)
        {
            cds[0].Passed = true;
            cds[1].Passed = true;
            cds[0].Data = F_Amt.ToString();
            cds[1].Data = F_Amt.ToString();
            return cds;
        }

        // Condition 4: None of the data can be empty or zero
        if ((F_Amt == 0 && D_Amt == 0) || (IsEmptyData(facilityAmount) && IsEmptyData(disbursementAmt))) 
        {
            //throw new InvalidOperationException("Both f_amt and d_amt cannot be empty or zero.");
            cds[0].Passed = false;
            cds[1].Passed = false;
            cds[0].Errors = new List<string>() { " FACILITYAMOUNT, DISBURSEMENTAMT CANNOT BE EMPTY OR ZERO " };
            cds[1].Errors = new List<string>() { " FACILITYAMOUNT, DISBURSEMENTAMT CANNOT BE EMPTY OR ZERO " };
            var vvv = cds;
            return cds;
        }



        cds[0].Passed = false;
        cds[1].Passed = false;
        cds[0].Errors = new List<string>() { "FACILITYAMOUNT, DISBURSEMENTAMT CANNOT BE EMPTY OR ZERO" };
        cds[1].Errors = new List<string>() { "FACILITYAMOUNT, DISBURSEMENTAMT CANNOT BE EMPTY OR ZERO" };
        return cds;
    }
    public string TransformAddData(string data)
    {
        //postaddress//Preadd//

        if (string.IsNullOrWhiteSpace(data))
            return string.Empty;
        //data = data.Replace("@", "AT").Replace("#", "NUMBER")
        //           .Replace(";", " ").Replace("?", " ")
        //           .Replace("*", " ").Replace(":", " ").Replace("+", " ").Replace("-", " ").ToUpper();


        data = data.ToUpper().Trim().Replace("  ", " ").Replace(".", " ");
        // Replace specific terms
        if (data.Contains("HNO"))
            data = data.Replace("HNO", "HOUSE NUMBER");

        if (data.Contains("H/NO"))
            data = data.Replace("H/NO", "HOUSE NUMBER");

        if (data.Contains("H/N"))
            data = data.Replace("H/N", "HOUSE NUMBER");
        return data;
    }
    public string HandleExponential(string input)
    {
        if (input != null && input.Contains("E+"))
        {
            return string.Empty;
        }
        return input;
    }
    public string NewRemoveMobile(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return string.Empty;

        // List of titles to remove (in uppercase for case-insensitive comparison)
        string[] titles = { "(", ")", "'" };

        // Trim leading/trailing spaces and split the input into words
        var words = data.Trim().Split(' ');

        // Check if the first word is a title
        if (words.Length > 0 && Array.Exists(titles, title => title.Equals(words[0].ToUpper())))
        {
            // Remove the title and join the remaining words
            return string.Join(" ", words.Skip(1)).Trim();
        }

        // Return the original string if no title is found
        return data;
    }
    //public string InValidBusinessName(string data)
    //{
    //    if (string.IsNullOrWhiteSpace(data))
    //        return string.Empty;

    //}
    public string NewRemoveMiddles(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return string.Empty;

        // List of titles to remove (in uppercase for case-insensitive comparison)
        string[] titles = { "MRS", "MS", "MISS", "REV","MR","REV" };

        // Trim leading/trailing spaces and split the input into words
        var words = data.Trim().Split(' ');

        // Check if the first word is a title
        if (words.Length > 0 && Array.Exists(titles, title => title.Equals(words[0].ToUpper())))
        {
            // Remove the title and join the remaining words
            return string.Join(" ", words.Skip(1)).Trim();
        }

        // Return the original string if no title is found
        return data;
    }
    public string NewRemoveTitles(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return string.Empty;

        // List of titles to remove (in uppercase for case-insensitive comparison)
        string[] titles = { "MRS", "MS", "MISS", "REV", "MR", "REV" };

        // Trim leading/trailing spaces and split the input into words
        var words = data.Trim().Split(' ');

        // Check if the first word is a title
        if (words.Length > 0 && Array.Exists(titles, title => title.Equals(words[0].ToUpper())))
        {
            // Remove the title and join the remaining words
            return string.Join(" ", words.Skip(1)).Trim();
        }

        // Return the original string if no title is found
        return data;
    }
    public bool IsZeroOrEmpty(CurrencyValidationResult data)
    {
        return data.IsZero && string.IsNullOrWhiteSpace(data.ValidData.ToString());
    }
    bool IsNullOrWhiteSpace(string data)
    {
        return true;
    }
    public bool IsNumber(string input)
    {
        // Check for null or empty input
        if (string.IsNullOrWhiteSpace(input))
            return false;

        // Ensure there are no leading or trailing whitespace characters
        if (input.Trim() != input)
            return false;

        // Attempt to parse the string as a double
        return double.TryParse(input, out _);
    }
    public CellDataAndStatus[] CurBal0_AmtOverdue1_WOffAmount2_NDIA3_Fsc4_d_Date5(string currentBalance, string amtInArrears, string writtenOffAmount, string nDIA, string fcode, string f_isDate, string filename)
    {
        if(IsNumber(fcode))
        {
            fcode = string.Empty;
        }

        string _currentBalance = currentBalance.Replace(",","");
        string _amtInArrears = amtInArrears.Replace(",", ""); ;
        string _writtenOffAmount = writtenOffAmount.Replace(",", ""); ;
        CellDataAndStatus[] curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5 = [new CellDataAndStatus(currentBalance), new CellDataAndStatus(amtInArrears), new CellDataAndStatus(writtenOffAmount), new CellDataAndStatus(nDIA), new CellDataAndStatus(fcode), new CellDataAndStatus(f_isDate)];







        if (string.IsNullOrWhiteSpace(currentBalance) && string.IsNullOrWhiteSpace(fcode))
        {
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { "CURRENT BALANCE SHOULD NOT BE EMPTY" };
            return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
        }


        if (currentBalance.Length == 1 && (currentBalance == "-" || currentBalance == "'-"))
        {
            currentBalance = "0";
        }
        if (amtInArrears.Length == 1 && (amtInArrears == "-" || amtInArrears == "'-"))
        {
            amtInArrears = "0";
        }
        if (writtenOffAmount.Length == 1 && (writtenOffAmount == "-" || writtenOffAmount == "'-"))
        {
            writtenOffAmount = "0";
        }
        if (nDIA.Length == 1 && (nDIA == "-" || nDIA == "'-"))
        {
            nDIA = "0";
        }
        if (currentBalance.Length > 1)
        {
            currentBalance = currentBalance.Replace("’", "").Replace("-", "");
        }
        if (amtInArrears.Length > 1)
        {
            amtInArrears = amtInArrears.Replace("’", "").Replace("-", "");
        }
        if (writtenOffAmount.Length > 1)
        {
            writtenOffAmount = writtenOffAmount.Replace("’", "").Replace("-", "");
        }
        if (nDIA.Length > 1)
        {
            nDIA = nDIA.Replace("’", "").Replace("-", "");

        }
        fcode = fcode.Trim().Replace(" ", "");


        currentBalance = NormalizeDecimalOrComma(currentBalance);
        amtInArrears = NormalizeDecimalOrComma(amtInArrears);
        writtenOffAmount = NormalizeDecimalOrComma(writtenOffAmount);


        var _data_currentBalance = CleanToTwoDecimalPlaces(currentBalance);
        var _data_amtInArrears = CleanToTwoDecimalPlaces(amtInArrears);
        var _data_writtenOffAmount = CleanToTwoDecimalPlaces(writtenOffAmount);


        var _data_currentBalance_Amt = ValidateDecimalInput(_data_currentBalance.Data);
        var _data_amtInArrears_Amt = ValidateDecimalInput(_data_amtInArrears.Data);
        var _data_writtenOffAmount_Amt = ValidateDecimalInput(_data_writtenOffAmount.Data);

        if (IsEmptyData(currentBalance)) { currentBalance = string.Empty; }
        else if (_data_currentBalance_Amt.IsValid) { currentBalance = _data_currentBalance_Amt.Value.ToString(); }
        else
        {
            // Previously fell through to _data_currentBalance_Amt.Value.ToString()
            // unconditionally -- ValidateDecimalInput returns (0m, false) on any
            // parse failure, so a genuinely non-empty, non-zero balance that
            // failed to parse for any reason was silently turned into "0" here.
            // That corrupted financial data outright, and specifically could
            // make a closed/paid-up/settled facility's real non-zero balance
            // look like a valid zero balance downstream (C_Finantial), passing
            // a record to Clean that should have failed. Preserve the cleaned-
            // but-unparsed value instead of silently discarding it -- financial
            // values are never altered by this cleaning step, only validated.
            currentBalance = _data_currentBalance.Data;
        }
        if (IsEmptyData(amtInArrears)) { amtInArrears = string.Empty; }
        else if (_data_amtInArrears_Amt.IsValid) { amtInArrears = _data_amtInArrears_Amt.Value.ToString(); }
        else { amtInArrears = _data_amtInArrears.Data; }
        if (IsEmptyData(writtenOffAmount)) { writtenOffAmount = string.Empty; }
        else if (_data_writtenOffAmount_Amt.IsValid) { writtenOffAmount = _data_writtenOffAmount_Amt.Value.ToString(); }
        else { writtenOffAmount = _data_writtenOffAmount.Data; }

        int _numbe_dia = 0;
        int.TryParse(nDIA, out _numbe_dia);


        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = currentBalance;
        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[1].Data = amtInArrears;
        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[2].Data = writtenOffAmount;
        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[3].Data = nDIA;
        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = fcode;
        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;
        if (string.IsNullOrWhiteSpace(f_isDate))
        {
            f_isDate = GetFacilityDateFromFileName(filename, false).LastDate;
        }
        fcode = fcode.ToUpper();

        //active with balances -- blank fcode is treated the same as
        //explicit "A" (Active), per spec.
        if (fcode == "A" || fcode == "ACTIVE" || string.IsNullOrWhiteSpace(fcode))
        {
            // The only time Active (or blank, treated as Active) legitimately
            // becomes Closed is when every one of the four balance-related
            // fields is genuinely zero/blank -- a facility with zero balance,
            // zero arrears, zero written-off amount, and zero NDIA is
            // effectively closed regardless of what status was submitted (or
            // left blank).
            if (C_Finantial(currentBalance, amtInArrears, writtenOffAmount, nDIA))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[1].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[2].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[3].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "C";
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }

            // Otherwise, an Active (or blank) facility must have a genuine,
            // non-zero balance -- a zero/blank current balance on an active
            // loan is itself invalid, not something to silently pass through.
            if (!IsEmptyDataChargeOff(currentBalance) && _data_currentBalance_Amt.Value > 0)
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = string.Empty;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _data_currentBalance_Amt.Value.ToString();
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[1].Data = _data_amtInArrears_Amt.Value.ToString();
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = string.Empty;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }

            // Neither condition met: the four fields aren't all zero (so this
            // isn't a legitimate Closed conversion), yet the current balance
            // is zero/blank despite the facility being Active -- fail
            // explicitly instead of falling through to a looser check.
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " CURRENT BALANCE SHOULD NOT BE 0 OR EMPTY FOR ACTIVE LOANS " };
            return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
        }


        //Restructured
        if (fcode == "R" || fcode == "E" || fcode == "RESTRUCTURED" || fcode == "EXTENDED")
        {
            if (IsEmptyData(amtInArrears) && _data_currentBalance_Amt.Value > 0 && IsEmptyData(writtenOffAmount) && IsEmptyData(nDIA) )
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "E";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " CURRENT BALANCE SHOULD NOT BE ZERO; AMOUNT IN ARREARS,  WRITTEN OF AMOUNT AND NDIA'S SHOULD BE ZERO " };
            return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
        }

        //END Restructured


        //LEGAl
        if (fcode == "L" || fcode == "LEGAL")
        {
            if (_data_amtInArrears_Amt.Value > 0 && _data_currentBalance_Amt.Value > 0 && _numbe_dia > 0)
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "L";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
            else
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " AMOUNT IN ARREARS AND CURRENT BALANCE SHOULD NOT BE ZERO " };
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
        }

        //END Legal


            /// Active with zero balances
            if ( fcode == "A" || fcode == "ACTIVE")
        {
            if (_data_currentBalance_Amt.Value == 0 && _data_amtInArrears_Amt.Value == 0 && _data_writtenOffAmount_Amt.Value == 0 && _numbe_dia == 0)
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "C";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
            else
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " CURRENT BALANCE SHOULD NOT BE EMPTY " };
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }

        }


        ////Charged off or Active or Loan against policy or approved but not disbursed
        if (fcode == "G" || fcode == "CHARGEDOFF" || fcode == "CHARGEOFF" || fcode == "N" || fcode == "B" || fcode == "LOANAGAINSTPOLICY" || fcode == "APPROVEDBUTNOTDISBURSED")
        {
            if (!IsEmptyDataChargeOff(currentBalance) && _data_currentBalance_Amt.Value >= 0)
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = string.Empty;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = string.Empty;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
            else
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " CURRENT BALANCE SHOULD NOT BE EMPTY " };
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
          
        }

        ////END Charge of





        ////Active or Loan againts policy or approve but not disburse
        //if (fcode == "N" || fcode == "B" || fcode == "A" || fcode == "LOANAGAINSTPOLICY" || fcode == "APPROVEDBUTNOTDISBURSED" || fcode == "ACTIVE")
        //{
            
        //    if (_data_currentBalance_Amt.Value >= 0 && !IsEmptyDataChargeOff(currentBalance))
        //    {
        //        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
        //        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = currentBalance;
        //        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = string.Empty;
        //        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = string.Empty;
        //        return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
        //    }
        //    else
        //    {
        //        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
        //        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
        //        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " CURRENT BALANCE AND AMOUNT IN ARREARS SHOULD NOT BE EMPTY " };
        //        return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
        //    }
        //}

        ////END Loan againts policy or approve but not disburse



        //Dispute
        if (fcode == "D" || fcode == "DISPUTED" || fcode == "DISPUTE")
        {
            if (_data_amtInArrears_Amt.Value > 0 && _data_currentBalance_Amt.Value > 0 && (_data_currentBalance_Amt.Value > _data_amtInArrears_Amt.Value))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "D";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
            else
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " AMOUNT IN ARREARS SHOULD NOT BE ZERO OR LESS THAN CURRENT BALANCE; CURRENT BALANCE SHOULD NOT BE ZERO  " };
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
        }

        //END Dispute



        //Covid or Cagd
        if (fcode == "RV" || fcode == "V" || fcode == "X")
        {
            if (!IsEmptyDataChargeOff(currentBalance))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = fcode;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
            else
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " CURRENT BALANCE SHOULD NOT BE EMPTY  " };
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
        }

        // Dead
        if (fcode == "Z" || fcode == "DEAD" || fcode == "DECEASED")
        {
            if (!IsEmptyDataChargeOff(currentBalance))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "Z";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " CURRENTBALANCE SHOULD NOT BE EMPTY " };
            return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
        }

        //END Covid




     

        ////C P T with codes
        if (fcode == "C" || fcode == "CLOSED" || fcode == "COMPLETED" || fcode == "COMPLETE")
        {
            if (C_Finantial(currentBalance, amtInArrears, writtenOffAmount, nDIA))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "C";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[1].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[2].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[3].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;//f_isDate;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
            else
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " CURRENTBALANCE,AMTINARREARS,WRITTENOFFAMOUNT AND NDIA SHOULD BE 0 FOR CLOSED LOANS " };
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }

        }
        if (fcode == "P" || fcode == "PAIDUP" || fcode == "PAIDUPDEFAULT" || fcode == "PAID")
        {
            if (C_Finantial(currentBalance, amtInArrears, writtenOffAmount, nDIA))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "P";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[1].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[2].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[3].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;//f_isDate;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
            else
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " CURRENTBALANCE,AMTINARREARS,WRITTENOFFAMOUNT AND NDIA SHOULD BE 0 FOR PAID UP LOANS " };
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }

        }
        if (fcode == "T" || fcode == "EARLYSETTLEMENT" || fcode == "EARLYSETTLED" || fcode == "SETTLEDEARLY")
        {
            if (C_Finantial(currentBalance, amtInArrears, writtenOffAmount, nDIA))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "T";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[1].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[2].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[3].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;//f_isDate;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }
            else
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " CURRENTBALANCE,AMTINARREARS,WRITTENOFFAMOUNT AND NDIA SHOULD BE 0 FOR EARLY SETTLED LOANS " };
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }

        }
        //END OF CPT




        //Writtenoff
        // Without code
        //Cond 1: Amount in arrears = Written off amount,current balance = 0 without code
        if ((_data_currentBalance_Amt.Value == 0) && (_data_amtInArrears_Amt.IsValid && _data_amtInArrears_Amt.Value > 0) && (_data_amtInArrears_Amt.Value == _data_writtenOffAmount_Amt.Value) && string.IsNullOrWhiteSpace(fcode))
        {
            if (string.IsNullOrWhiteSpace(fcode))
            {
                fcode = "W";
            }
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = fcode;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = "0";
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[1].Data = _data_amtInArrears_Amt.Value.ToString();
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[2].Data = _data_writtenOffAmount_Amt.Value.ToString();
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;//f_isDate;
            return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
        }


        //Cond 2: Amount in arrears = Written off amount = current balance, without code
        if ((_data_amtInArrears_Amt.IsValid && _data_amtInArrears_Amt.Value > 0) && (_data_amtInArrears_Amt.Value == _data_writtenOffAmount_Amt.Value && _data_amtInArrears_Amt.Value == _data_currentBalance_Amt.Value))
        {
            if (string.IsNullOrWhiteSpace(fcode))
            {
                fcode = "W";
            }
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = "0";
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[1].Data = _data_amtInArrears_Amt.Value.ToString();
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[2].Data = _data_writtenOffAmount_Amt.Value.ToString();
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "W";//fcode;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;
            return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
        }

        // Cond 3: Written off amount = current balance, without fcode: 
        if (
            (_data_writtenOffAmount_Amt.IsValid && _data_writtenOffAmount_Amt.Value > 0) &&
         (_data_writtenOffAmount_Amt.Value == _data_currentBalance_Amt.Value) && IsEmptyData(amtInArrears) && IsEmptyData(fcode))
        {
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = "0";
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "W";
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[1].Data = _data_writtenOffAmount_Amt.Value.ToString();
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[2].Data = _data_writtenOffAmount_Amt.Value.ToString();
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;
            return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
        }

        //WritenOffAmount with fcode
        if (fcode == "W")
        {
            //Cond 1: Amount in arrears = Written off amount,current balance = 0
            if (
                 (_data_currentBalance_Amt.Value == 0) &&
                 (_data_amtInArrears_Amt.IsValid && _data_amtInArrears_Amt.Value > 0) &&
                 (_data_amtInArrears_Amt.Value == _data_writtenOffAmount_Amt.Value))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = fcode;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = "0";
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }

            //Cond 2: Amount in arrears = Written off amount = current balance = 0
            if (C_Finantial_Written(currentBalance, amtInArrears, writtenOffAmount) && fcode == "W")
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " WRITTENOFFAMOUNT AND AMTINARREARS SHOULD BE THE SAME " };
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }

            // Cond 3: Amount in arrears = current balance, Written off amount = 0

            if ((_data_amtInArrears_Amt.IsValid && _data_amtInArrears_Amt.Value > 0) && ((_data_amtInArrears_Amt.Value == _data_currentBalance_Amt.Value) && IsEmptyData(writtenOffAmount)))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "W";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[2].Data = _data_amtInArrears_Amt.Value.ToString();
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }

            // Cond 4: Written off amount = current balance, Amount in arrears = 0
            if ((_data_writtenOffAmount_Amt.IsValid && _data_writtenOffAmount_Amt.Value > 0) && ((_data_writtenOffAmount_Amt.Value == _data_currentBalance_Amt.Value) && IsEmptyData(amtInArrears)))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "W";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[1].Data = _data_writtenOffAmount_Amt.Value.ToString();
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }

            //Cond 5: Amount in arrears = Written off amount = current balance
            if ((_data_amtInArrears_Amt.IsValid && _data_amtInArrears_Amt.Value > 0) && (_data_amtInArrears_Amt.Value == _data_writtenOffAmount_Amt.Value && _data_amtInArrears_Amt.Value == _data_currentBalance_Amt.Value))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = true;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = "0";
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[4].Data = "W";//fcode;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[5].Data = f_isDate;
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }

            //Cond 6: Amount in arrears != Written off amount
            if ((_data_currentBalance_Amt.Value == 0) && (_data_amtInArrears_Amt.IsValid && _data_amtInArrears_Amt.Value > 0) && (_data_amtInArrears_Amt.Value != _data_writtenOffAmount_Amt.Value))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " WRITTENOFFAMOUNT AND AMTINARREARS SHOULD BE THE SAME FOR WRITTENOFFAMOUNT " };
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }

            //Cond 7: Amount in arrears != Current balance
            if ((_data_currentBalance_Amt.Value == 0) && (_data_amtInArrears_Amt.IsValid && _data_amtInArrears_Amt.Value > 0) && (_data_amtInArrears_Amt.Value != _data_writtenOffAmount_Amt.Value))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " WRITTENOFFAMOUNT AND AMTINARREARS SHOULD BE THE SAME FOR WRITTENOFFAMOUNT " };
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }


            //Cond 8: Written off amount != Current balance
            if ((_data_currentBalance_Amt.Value == 0) && (_data_amtInArrears_Amt.IsValid && _data_amtInArrears_Amt.Value > 0) && (_data_amtInArrears_Amt.Value != _data_writtenOffAmount_Amt.Value))
            {
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
                curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " WRITTENOFFAMOUNT AND AMTINARREARS SHOULD BE THE SAME FOR WRITTENOFFAMOUNT " };
                return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
            }




            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
            curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Errors = new List<string>() { " WRITTENOFFAMOUNT AND AMTINARREARS SHOULD BE THE SAME FOR WRITTENOFFAMOUNT " };
            return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
        }
        //END OF WTRITTENOFF
        //WritenOffAmount with fcode available


        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Data = _currentBalance;
        curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5[0].Passed = false;
        return curBal0_amtInArrears1_wOffAmount2_nDIA3_fsc4_Date5;
    }

    bool C_Finantial_Written(string currentBalance, string amtInArrears, string writtenOffAmount)
    {
        bool zeroFinantial = IsEmptyData(currentBalance) && IsEmptyData(amtInArrears) && IsEmptyData(writtenOffAmount);
        return zeroFinantial;
    }
    bool C_Finantial(string currentBalance, string amtInArrears, string writtenOffAmount, string ndia)
    {
        bool zeroFinantial = IsEmptyData(currentBalance) && IsEmptyData(amtInArrears) && IsEmptyData(writtenOffAmount) && IsEmptyDataNDIA(ndia);
        return zeroFinantial;
    }
    bool Finantial_IsValidDecimal(string currentBalance, string amtInArrears, string writtenOffAmount, string ndia)
    {
        bool zeroFinantial = IsEmptyData(currentBalance) && IsEmptyData(amtInArrears) && IsEmptyData(writtenOffAmount) && IsEmptyDataNDIA(ndia);
        return zeroFinantial;
    }
    private readonly HashSet<string> Titles = new HashSet<string>
    {
        "MR", "MRS", "MS", "MISS", "DR", "PROF", "SIR", "REV","CAPT", "WO", "SSGT", "SQL", "SGT", "MGEN", "MAJ", "LCOL", "LT", "LCPL", "FLT", "CPL", "CDR", "COL"
    };
    private readonly Dictionary<string, string> TitleMap = new(StringComparer.OrdinalIgnoreCase)
    {
        { "CAPTAIN", "CAPT" },
        { "WARRANT OFFICER CLASS I", "WO" },
        { "WARRANT OFFICER CLASS II", "WO" },
        { "STAFF SERGEANT", "SSGT" },
        { "SQUADRON LEADER", "SQL" },
        { "SERGEANT", "SGT" },
        { "SENIOR WARRANT OFFICER CLASS 2", "WO" },
        { "MAJOR GENERAL", "MGEN" },
        { "MAJOR", "MAJ" },
        { "LIEUTENANT COLONEL", "LCOL" },
        { "LIEUTENANT", "LT" },
        { "LANCE-CORPORAL", "LCPL" },
        { "FLIGHT LIEUTENANT", "FLT" },
        { "CORPORAL", "CPL" },
        { "COMMANDER", "CDR" },
        { "COLONEL", "COL" }
    };
    public string ConvertToShortTitle(string titleFullName)
    {
        string normalizedFullName = titleFullName.Trim().ToLowerInvariant();
        foreach (var key in TitleMap.Keys)
        {
            if (string.Equals(key.Trim(), normalizedFullName, StringComparison.OrdinalIgnoreCase))
            {
                return TitleMap[key];
            }
        }
        return titleFullName;
    }
    public string NewFilterNameByTitle(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;
        name = ConvertToShortTitle(name);

        foreach (var title in Titles)
        {
            if (title.ToUpper() == name.ToUpper())
                return name.ToUpper();
        }

        return string.Empty;
    }
    public CellDataAndStatus[] ValidateIDsMain1_G(string nationalID, string voterID, string driver, string dassport, string sSNIT, string otherIDs, string d_date)
    {
        var _date = CheckDate(d_date);
        CellDataAndStatus[] cds =
          [
            new CellDataAndStatus(nationalID),
            new CellDataAndStatus(voterID),
            new CellDataAndStatus(driver),
            new CellDataAndStatus(dassport),
            new CellDataAndStatus(sSNIT),
            new CellDataAndStatus(otherIDs)
        ];
        nationalID = RemoveHypens(nationalID);
        voterID = RemoveHypens(voterID);
        driver = RemoveHypens(driver);
        dassport = RemoveHypens(dassport);
        sSNIT = RemoveHypens(sSNIT);
        otherIDs = RemoveHypens(otherIDs);

        var response = ProcessArguments(nationalID, voterID, driver, dassport, sSNIT);
        if (
            string.IsNullOrWhiteSpace(response.Item1) &&
            string.IsNullOrWhiteSpace(response.Item2) &&
            string.IsNullOrWhiteSpace(response.Item3) &&
            string.IsNullOrWhiteSpace(response.Item4) &&
            string.IsNullOrWhiteSpace(response.Item5))
        {
            cds[0].Passed = true;
        }
        else
        {
            cds[0].Passed = true;
        }
        cds[0].Data = response.Item1;
        cds[1].Data = response.Item2;
        cds[2].Data = response.Item3;
        cds[3].Data = response.Item4;
        cds[4].Data = response.Item5 + " " + response.Item5;
        //cds[5].Data = response.Item5;


        return cds;
    }
    public CellDataAndStatus[] ValidateIDsMain1(string nationalID, string voterID, string driver, string dassport, string sSNIT, string otherIDs, string disbur_inputDate)
    {
        var checkDate = CheckDate(disbur_inputDate);
        CellDataAndStatus[] cds = [ new CellDataAndStatus(nationalID),new CellDataAndStatus(voterID), new CellDataAndStatus(driver), new CellDataAndStatus(dassport),new CellDataAndStatus(sSNIT), new CellDataAndStatus(otherIDs)];

        nationalID = RemoveHypens(nationalID);
        voterID = RemoveHypens(voterID);
        driver = RemoveHypens(driver);
        dassport = RemoveHypens(dassport);
        sSNIT = RemoveHypens(sSNIT);
        otherIDs = RemoveHypens(otherIDs);

        var response = ProcessArguments(nationalID, voterID, driver, dassport, sSNIT);

        if ((response.Item1.Contains("GHA") || (response.Item1.Contains("GH")) || response.Item1.Contains("GHS")) && (response.Item1.Length < 13 || response.Item1.Length > 13))
        {
            if (response.Item1.Length > 3 && response.Item1.Substring(0, 2) == "GH")
            {
                response.Item1 = string.Empty;
            }
        }
        if ((response.Item2.Contains("GHA") || (response.Item2.Contains("GH")) || response.Item2.Contains("GHS")) && (response.Item2.Length == 10))
        {
            if (response.Item2.Length > 3 && response.Item2.Substring(0, 2) == "GH")
            {
                response.Item2 = string.Empty;
            }
        }
        if ((response.Item3.Contains("GHA") || (response.Item3.Contains("GH")) || response.Item3.Contains("GHS")) && (response.Item3.Length < 13 || response.Item3.Length > 13))
        {
            if (response.Item3.Length > 3 && response.Item3.Substring(0, 2) == "GH")
            {
                response.Item3 = string.Empty;
            }
        }
        if ((response.Item4.Contains("GHA") || (response.Item4.Contains("GH")) || response.Item4.Contains("GHS")) && (response.Item4.Length < 13 || response.Item4.Length > 13))
        {
            if (response.Item4.Length > 3 && response.Item4.Substring(0, 2) == "GH")
            {
                response.Item4 = string.Empty;
            }
        }
        if ((response.Item5.Contains("GHA") || (response.Item5.Contains("GH")) || response.Item5.Contains("GHS")) && (response.Item5.Length < 13 || response.Item5.Length > 13))
        {
            if (response.Item5.Length > 3 && response.Item5.Substring(0, 2) == "GH")
            {
                response.Item5 = string.Empty;
            }
        }



        if (StartsWithFourXOrZero(response.Item1))
        {
            response.Item1 = string.Empty;
        }
        if (StartsWithFourXOrZero(response.Item2))
        {
            response.Item2 = string.Empty;
        }
        if (StartsWithFourXOrZero(response.Item3))
        {
            response.Item3 = string.Empty;
        }
        if (StartsWithFourXOrZero(response.Item4))
        {
            response.Item4 = string.Empty;
        }
        if (StartsWithFourXOrZero(response.Item5))
        {
            response.Item5 = string.Empty;
        }


        if (response.Item1.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            response.Item1 = string.Empty;
        }
        if (response.Item2.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            response.Item2 = string.Empty;
        }
        if (response.Item3.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            response.Item3 = string.Empty;
        }
        if (response.Item4.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            response.Item4 = string.Empty;
        }
        if (response.Item5.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            response.Item5 = string.Empty;
        }






        if (
            string.IsNullOrWhiteSpace(response.Item1) &&
            string.IsNullOrWhiteSpace(response.Item2) &&
            string.IsNullOrWhiteSpace(response.Item3) &&
            string.IsNullOrWhiteSpace(response.Item4) &&
            string.IsNullOrWhiteSpace(response.Item5) && checkDate.IsValidFormat && checkDate.TheDate.Year  > 2019)
        {
            cds[0].Passed = false;
            cds[0].Errors = new List<string>() { "WRONG OR EMPTY IDS. PROVIDE VALID IDS" };
        }
        else
        {
            cds[0].Passed = true;
        }
        cds[0].Data = response.Item1;
        cds[1].Data = response.Item2;
        cds[2].Data = response.Item3;
        cds[3].Data = response.Item4;
        cds[4].Data = response.Item5;
        cds[5].Data = response.Item5;


        return cds;
    }

    public CellDataAndStatus[] ValidateIDsMain1_DUD(string nationalID, string voterID, string driver, string dassport, string sSNIT, string otherIDs)
    {
        CellDataAndStatus[] cds =
          [
            new CellDataAndStatus(nationalID),
            new CellDataAndStatus(voterID),
            new CellDataAndStatus(driver),
            new CellDataAndStatus(dassport),
            new CellDataAndStatus(sSNIT),
            new CellDataAndStatus(otherIDs)
        ];

        nationalID = RemoveHypens(nationalID);
        voterID = RemoveHypens(voterID);
        driver = RemoveHypens(driver);
        dassport = RemoveHypens(dassport);
        sSNIT = RemoveHypens(sSNIT);
        otherIDs = RemoveHypens(otherIDs);

        var response = ProcessArguments(nationalID, voterID, driver, dassport, sSNIT);

        if ((response.Item1.Contains("GHA") || (response.Item1.Contains("GH")) || response.Item1.Contains("GHS")) && (response.Item1.Length < 13 || response.Item1.Length > 13))
        {
            if (response.Item1.Length > 3 && response.Item1.Substring(0, 2) == "GH")
            {
                response.Item1 = string.Empty;
            }
        }
        if ((response.Item2.Contains("GHA") || (response.Item2.Contains("GH")) || response.Item2.Contains("GHS")) && (response.Item2.Length == 10))
        {
            if (response.Item2.Length > 3 && response.Item2.Substring(0, 2) == "GH")
            {
                response.Item2 = string.Empty;
            }
        }
        if ((response.Item3.Contains("GHA") || (response.Item3.Contains("GH")) || response.Item3.Contains("GHS")) && (response.Item3.Length < 13 || response.Item3.Length > 13))
        {
            if (response.Item3.Length > 3 && response.Item3.Substring(0, 2) == "GH")
            {
                response.Item3 = string.Empty;
            }
        }
        if ((response.Item4.Contains("GHA") || (response.Item4.Contains("GH")) || response.Item4.Contains("GHS")) && (response.Item4.Length < 13 || response.Item4.Length > 13))
        {
            if (response.Item4.Length > 3 && response.Item4.Substring(0, 2) == "GH")
            {
                response.Item4 = string.Empty;
            }
        }
        if ((response.Item5.Contains("GHA") || (response.Item5.Contains("GH")) || response.Item5.Contains("GHS")) && (response.Item5.Length < 13 || response.Item5.Length > 13))
        {
            if (response.Item5.Length > 3 && response.Item5.Substring(0, 2) == "GH")
            {
                response.Item5 = string.Empty;
            }
        }

        if (response.Item1.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            response.Item1 = string.Empty;
        }
        if (response.Item2.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            response.Item2 = string.Empty;
        }
        if (response.Item3.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            response.Item3 = string.Empty;
        }
        if (response.Item4.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            response.Item4 = string.Empty;
        }
        if (response.Item5.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        {
            response.Item5 = string.Empty;
        }


        if (StartsWithFourXOrZero(response.Item1))
        {
            response.Item1 = string.Empty;
        }
        if (StartsWithFourXOrZero(response.Item2))
        {
            response.Item2 = string.Empty;
        }
        if (StartsWithFourXOrZero(response.Item3))
        {
            response.Item3 = string.Empty;
        }
        if (StartsWithFourXOrZero(response.Item4))
        {
            response.Item4 = string.Empty;
        }
        if (StartsWithFourXOrZero(response.Item5))
        {
            response.Item5 = string.Empty;
        }



        if (
            string.IsNullOrWhiteSpace(response.Item1) &&
            string.IsNullOrWhiteSpace(response.Item2) &&
            string.IsNullOrWhiteSpace(response.Item3) &&
            string.IsNullOrWhiteSpace(response.Item4) &&
            string.IsNullOrWhiteSpace(response.Item5))
        {
            cds[0].Passed = false;
            cds[0].Errors = new List<string>() { "WRONG OR EMPTY IDS. PROVIDE VALID IDS" };
        }
        else
        {
            cds[0].Passed = true;
        }
        cds[0].Data = response.Item1;
        cds[1].Data = response.Item2;
        cds[2].Data = response.Item3;
        cds[3].Data = response.Item4;
        cds[4].Data = response.Item5;
        cds[5].Data = response.Item5;


        return cds;
    }

    public string RemoveHypens(string data)
    {

        data = HandleExponential(data);
        if (HasSixConsecutiveZerosAtEnd(data))
        {
            data = string.Empty;
        }
        if (HasLetterRepeatingMoreThanFiveTimes(data))
        {
            data = string.Empty;
        }
       
        data = data.Trim().Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "").Replace("[", "").Replace("]", "").ToUpper().Replace(".", "");
        if (data.Contains("_") || data.Contains("&"))
        {
            data = string.Empty;
        }
        if (data.Length > 4)
        {
            if ((NewIsOnlyLetters(data.Substring(0, 5)) && data.Length < 13) || (NewIsOnlyLetters(data.Substring(0, 5)) && data.Length > 13))
            {
                data = string.Empty;
            }
        }
        if (NewIsOnlyLetters(data))
        {
            data = string.Empty;
        }
        return data;
    }
    public bool NewIsOnlyLetters(string data)
    {
        if (string.IsNullOrEmpty(data))
            return false;

        foreach (char c in data)
        {
            if (!char.IsLetter(c))
                return false;
        }
        return true;
    }
    public (string, string, string, string, string, string) ProcessArguments(string arg1, string arg2, string arg3, string arg4, string arg5)
    {
        // Define allowed lengths for each argument position.
        List<int[]> allowedLengths = new List<int[]>
        {
            new int[] { 13 },
            new int[] { 10 },
            new int[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 },
            new int[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 },
            new int[] { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 }
        };

        // Place the five arguments into an array for easy processing.
        string[] args = [arg1, arg2, arg3, arg4, arg5];

        // Array to store the final values for each argument slot.
        string[] results = new string[5];
        // List to keep any input that does not match its allowed length.
        List<string> unassigned = new List<string>();

        // First pass: validate each argument against its allowed lengths.
        for (int i = 0; i < 5; i++)
        {
            // Use null-coalescing operator in case the input is null.
            int len = args[i]?.Length ?? 0;
            if (allowedLengths[i].Contains(len))
            {//args[i] //NewIsOnlyLetters
                if (NewIsOnlyLetters(args[i].Substring(0, 3)) && args[i].Length == 13)
                {
                    // The argument has a valid length so we keep it.
                    results[i] = args[i];
                }
                else
                {
                    unassigned.Add(args[i]);
                }

            }
            else
            {
                // Otherwise, add it to the unassigned pool.
                unassigned.Add(args[i]);
            }
        }

        // Second pass: for any slot that is still empty (invalid/missing originally),
        // try to find an unassigned argument that matches the allowed lengths for that slot.
        for (int i = 0; i < 5; i++)
        {
            if (string.IsNullOrEmpty(results[i]))
            {
                bool foundCandidate = false;
                // Search the unassigned pool.
                for (int j = 0; j < unassigned.Count; j++)
                {
                    string candidate = unassigned[j];
                    // Check candidate against allowed lengths for the current slot.
                    if (candidate != null && allowedLengths[i].Contains(candidate.Length))
                    {
                        results[i] = candidate;
                        // Remove this candidate from the unassigned list.
                        unassigned.RemoveAt(j);
                        foundCandidate = true;
                        break; // Move on to the next slot.
                    }
                }
                // If no candidate was found, assign an empty string.
                if (!foundCandidate)
                {
                    results[i] = "";
                }
            }
        }

        // Finally, join any remaining unassigned arguments into one string.
        string joinedUnassigned = string.Join("", unassigned);

        // Return all six data pieces:
        // results[0]..results[4] are the five arguments (original or replaced)
        // joinedUnassigned is the concatenation of any leftovers.
        return (results[0], results[1], results[2], results[3], results[4], joinedUnassigned);
    }
    private string NormalizeNationalId(string id)
    {
        if (id.Length != 13)
            return null;

        string lettersPart = id.Substring(0, 3);
        string digitsPart = id.Substring(3);

        // Check that the first three characters are letters.
        if (!lettersPart.All(char.IsLetter))
            return null;

        // If the national ID starts with "GHA", allow replacing 'O'/'o' with '0'
        if (lettersPart == "GHA")
        {
            var normalizedDigits = new char[10];
            for (int i = 0; i < 10; i++)
            {
                char c = digitsPart[i];
                if (c == 'O' || c == 'o')
                {
                    normalizedDigits[i] = '0';
                }
                else if (char.IsDigit(c))
                {
                    normalizedDigits[i] = c;
                }
                else
                {
                    return null;
                }
            }
            return lettersPart + new string(normalizedDigits);
        }
        else
        {
            // For IDs not starting with "GHA", the digits part must contain only digits.
            if (!digitsPart.All(char.IsDigit))
                return null;
            return id;
        }
    }
    public string RemoveAllSpaces(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return string.Concat(input.Where(c => c != ' '));
    }
    public CurrencyValidationResult ValidateToDecimalCurrency(string currencyData)
    {
        var result = new CurrencyValidationResult();

        // Check if the data is empty
        if (string.IsNullOrWhiteSpace(currencyData))
        {
            result.IsEmpty = true;
            result.IsValid = true; // Empty data is valid
            result.ValidData = null; // Keep ValidData empty (null)
            return result;
        }

        // Try to parse the data as a decimal
        if (decimal.TryParse(currencyData, out decimal parsedValue))
        {
            // Check if the value is zero or positive
            result.IsZero = parsedValue == 0;
            result.IsGreaterThanZero = parsedValue > 0;

            // Check if the value is an integer OR has up to two decimal places
            bool isInteger = parsedValue == Math.Floor(parsedValue);
            bool hasValidDecimalFormat = currencyData.Contains('.')
                ? currencyData.Split('.').Length == 2 && currencyData.Split('.')[1].Length <= 2
                : true; // No decimal part (whole number)

            if (isInteger || hasValidDecimalFormat)
            {
                result.IsValid = true;
                result.ValidData = parsedValue; // Store the valid data
            }
            else
            {
                result.IsValid = false;
            }
        }
        else
        {
            // Parsing failed; invalid data
            result.IsValid = false;
        }

        return result;
    }

    public string AddressReplaceSpecialCharacters(string address)
    {
        if (string.IsNullOrEmpty(address))
            return address;

        var specialCharacters = new HashSet<char>
    {
        '!', '$', '%', '^', '*', '=', '+', '[', ']', '{', '}', ';', ':', '<', '>', ',', '?', '`', '~'
    };

        char[] result = address.ToCharArray();

        for (int i = 0; i < result.Length; i++)
        {
            if (specialCharacters.Contains(result[i]))
            {
                result[i] = ' ';
            }
        }

        return new string(result);
    }

    public string ReplaceMultipleSpaces(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        StringBuilder result = new StringBuilder();
        bool previousWasSpace = false;

        foreach (char c in input)
        {
            if (c == ' ')
            {
                if (!previousWasSpace)
                {
                    result.Append(c);
                    previousWasSpace = true;
                }
            }
            else
            {
                result.Append(c);
                previousWasSpace = false;
            }
        }

        return result.ToString();
    }
    public string RemoveSystemErroNames(string data)
    {

       
        if (string.IsNullOrWhiteSpace(data))
            return string.Empty;
        data = data.Replace("–", "-").TrimStart().TrimEnd();;
        string[] invalidWords = { "N/A", "NA", "NULL", "NILL", "NONE", "NOTAPPLICABLE", "NOT APPLICABLE","NOT","APPLICABLE", "#NAME?", "#NAME", "UNKNOWN", "NOT AVAILABLE", "NOTAVAILABLE" };

        // Normalize spaces and split words
        var words = data.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var filteredWords = new List<string>();

        foreach (var word in words)
        {
            bool isInvalid = false;
            foreach (var invalid in invalidWords)
            {
                if (word.Equals(invalid, StringComparison.OrdinalIgnoreCase))
                {
                    isInvalid = true;
                    break;
                }
            }
            if (!isInvalid)
            {
                filteredWords.Add(word);
            }
        }
        string _filteredWords = string.Join(" ", filteredWords);
        _filteredWords = ReplaceMultipleSpaces(_filteredWords).Trim().Replace('\'', '’');
        return _filteredWords;
    }
    /// <summary>
    /// Cleans up four address fields based on the following criteria:
    /// 1. If an address is longer than 100 characters, the extra characters are moved to the next address.
    /// 2. Empty addresses are removed and shifted to the left.
    /// </summary>
    public CellDataAndStatus[] CleanAddresses(string addr1, string addr2, string addr3, string addr4)
    {
        CellDataAndStatus[] cds =
       [
           new CellDataAndStatus(addr1),
            new CellDataAndStatus(addr2),
            new CellDataAndStatus(addr3),
            new CellDataAndStatus(addr4),
        ];


        addr1 = HandleExponential(addr1);
        addr2 = HandleExponential(addr2);
        addr3 = HandleExponential(addr3);
        addr4 = HandleExponential(addr4);


        addr1 = RemoveSystemErroNames(addr1);
        addr2 = RemoveSystemErroNames(addr2);
        addr3 = RemoveSystemErroNames(addr3);
        addr4 = RemoveSystemErroNames(addr4);

        addr1 = AddressReplaceSpecialCharacters(addr1);
        addr2 = AddressReplaceSpecialCharacters(addr2);
        addr3 = AddressReplaceSpecialCharacters(addr3);
        addr4 = AddressReplaceSpecialCharacters(addr4);

        addr1 = ReplaceEmail(addr1);
        addr2 = ReplaceEmail(addr2);
        addr3 = ReplaceEmail(addr3);
        addr4 = ReplaceEmail(addr4);


        addr1 = TransformAddData(addr1);
        addr2 = TransformAddData(addr2);
        addr3 = TransformAddData(addr3);
        addr4 = TransformAddData(addr4);

        addr1 = addr1.Replace("#", "NO");
        addr2 = addr2.Replace("#", "NO");
        addr3 = addr3.Replace("#", "NO");
        addr4 = addr4.Replace("#", "NO");


        // Initialize the addresses array (treat null as an empty string)
        string[] addresses = [addr1, addr2, addr3, addr4];

        // Process the first three addresses for overflow.
        for (int i = 0; i < addresses.Length - 1; i++)
        {
            if (addresses[i].Length > 99)
            {
                string overflow = addresses[i].Substring(99);
                addresses[i] = addresses[i].Substring(0, 99);
                addresses[i + 1] = overflow + addresses[i + 1];
            }
        }

        // Remove empty addresses and shift the remaining ones to the left.
        List<string> cleanedAddresses = addresses.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

        // Ensure we have exactly 4 elements by adding empty strings if necessary.
        while (cleanedAddresses.Count < 4)
        {
            cleanedAddresses.Add("");
        }

        // Assign the cleaned addresses to individual output parameters
        cds[0].Data = cleanedAddresses[0];
        cds[1].Data = cleanedAddresses[1];
        cds[2].Data = cleanedAddresses[2];
        cds[3].Data = cleanedAddresses[3];
        return cds;
    }
    public (bool IsValidFormat, bool IsEighteenOrAbove, bool IsFutureDate, DateTime TheDate) CheckDate(string dateString)
    {
        if (dateString == null)
        {
            return (false, false, false, DateTime.MinValue);
        }

        dateString = dateString.Replace(" ", "").Trim();
        // Try to parse the date string in yyyyMMdd format
        if (DateTime.TryParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
        {
            DateTime today = DateTime.Today;
            DateTime eighteenYearsAgo = today.AddYears(-18);

            bool isEighteenOrAbove = parsedDate <= eighteenYearsAgo;
            bool isFutureDate = parsedDate > today;

            return (true, isEighteenOrAbove, isFutureDate, parsedDate);
        }

        // Return false if the format is invalid
        return (false, false, false, parsedDate);
    }

    //public string FilterBusinessKeywords(string input)
    //{
    //    var f1 = _businessname.Count;
    //    var f2 = BusinessKeyNames.Count;
    //    if (string.IsNullOrWhiteSpace(input))
    //        return string.Empty;

    //    // Convert input to uppercase for case-insensitive comparison
    //    string inputUpper = input.ToUpper();

    //    // Split input into words
    //    string[] words = Regex.Split(inputUpper, @"\s+")
    //                          .Where(w => !string.IsNullOrWhiteSpace(w))
    //                          .ToArray();

    //    // Keep only words that exist in businessKeywords
    //    var filteredWords = words.Where(word => BusinessKeyNames.Contains(word)).ToList();

    //    // Special handling for multi-word entries like "& CO" and "& SONS"
    //    if (inputUpper.Contains("& CO")) filteredWords.Add("& CO");
    //    if (inputUpper.Contains("& SONS")) filteredWords.Add("& SONS");

    //    // If no valid keywords found, return an empty string
    //    if (filteredWords.Any())
    //    {
    //        return input.ToUpper();
    //    }
    //    else
    //    {
    //        return string.Empty;
    //    }
    //}

    //public bool ContainsRestrictedWord(string input)
    //{
    //    if (string.IsNullOrWhiteSpace(input))
    //        return false;

    //    // Split by common word separators (space, comma, period, hyphen, etc.)
    //    var words = input.ToUpper().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

    //    return words.Any(word => BusinessKeyNames.Contains(word));
    //}






    //STEPS
    //1. ContainsInvalidCharacters
    //2. RemoveDuplicateNamesRemoveDuplicate
    //3. RemoveSpacesAroundHyphen
    //4. RemoveInvalidName
    //5. ProcessFullNames
    public (string CleanedSurname, string CleanedFirstname, string CleanedMiddlename) RemoveDuplicateNamesRemoveDuplicate(string surname, string firstname, string middlename)
    {


        //surname = RemoveTilesFromName(surname);
        //firstname = RemoveTilesFromName(firstname);
        //middlename = RemoveTilesFromName(middlename);
        // List to keep track of words we have already seen.
        // Using OrdinalIgnoreCase so that "John" and "john" are treated as the same.
        var seenWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase);



        // Process the surname:
        var surnameWords = surname.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                  .Select(word => word.Trim())
                                  .Distinct(StringComparer.OrdinalIgnoreCase)
                                  .ToList();
        // Add surname words to the global set.
        foreach (var word in surnameWords)
        {
            seenWords.Add(word);
        }

        // Process the firstname:
        var firstnameWords = firstname.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                      .Select(word => word.Trim())
                                      .Distinct(StringComparer.OrdinalIgnoreCase)
                                      // Remove words already used in surname.
                                      .Where(word => !seenWords.Contains(word))
                                      .ToList();
        // Add the remaining firstname words to the set.
        foreach (var word in firstnameWords)
        {
            seenWords.Add(word);
        }

        // Process the middlename:
        var middlenameWords = middlename.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(word => word.Trim())
                                        .Distinct(StringComparer.OrdinalIgnoreCase)
                                        // Remove words already used in surname or firstname.
                                        .Where(word => !seenWords.Contains(word))
                                        .ToList();

        // Combine the words back into strings.
        string cleanedSurname = string.Join(" ", surnameWords);
        string cleanedFirstname = string.Join(" ", firstnameWords);
        string cleanedMiddlename = string.Join(" ", middlenameWords);

        return (cleanedSurname, cleanedFirstname, cleanedMiddlename);
    }
    public bool ContainsInvalidCharacters(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return false; // Consider empty/null values as valid
        name = name.Replace("_", " ").Replace("_", " ").Replace(",", "").Replace("`", "");
        foreach (char c in name)
        {
            if (!(char.IsLetter(c) || c == '.' || c == ' ' || c == '-'))
            {
                return true; // Found an invalid character
            }
        }

        return false;
    }//.RemoveBusinessKeywords
    public bool HasInvalidCharacters(string surname, string firstName, string middleName)
    {
        return ContainsInvalidCharacters(surname) || ContainsInvalidCharacters(firstName) || ContainsInvalidCharacters(middleName);
    }
    public string RemoveSpacesAroundHyphen(string input)
    {
        input = input.Trim().ToUpper();
        input = NewRemoveTitles(input);
        return input.Replace(" - ", "-").Replace(" -", "-").Replace("- ", "-").Replace("."," ");
    }

    public (string Surname, string FirstName, string MiddleName, string InvalidNames) RemoveInvalidName(string surname, string firstName, string middleName)
    {
        List<string> invalidNames = new List<string>();

        string CleanName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return string.Empty;
            var words = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var validWords = new List<string>();

            foreach (var word in words)
            {
                if (word.Length >= 3)
                    validWords.Add(word);
                else
                    invalidNames.Add(word);
            }

            return string.Join(" ", validWords);
        }

        string cleanSurname = CleanName(surname);
        string cleanFirstName = CleanName(firstName);
        string cleanMiddleName = CleanName(middleName);
        string invalidNamesStr = string.Join(" ", invalidNames);

        return (cleanSurname, cleanFirstName, cleanMiddleName, invalidNamesStr);
    }
    public (string Surname, string FirstName, string MiddleName) ProcessFullNames_G(string surname, string firstName, string middleName)
    {
        surname = surname + "APPPPPPPPPPA";
        firstName = firstName + "APPPPPPPPPPA";
        middleName = middleName + "APPPPPPPPPPA";
        // Split each input into words, removing any empty entries.
        List<string> surnameWords = string.IsNullOrWhiteSpace(surname)
            ? new List<string>()
            : surname.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        List<string> firstNameWords = string.IsNullOrWhiteSpace(firstName)
            ? new List<string>()
            : firstName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        List<string> middleNameWords = string.IsNullOrWhiteSpace(middleName)
            ? new List<string>()
            : middleName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();


        // These pointers track which words have been used from each list.
        int sIndex = 0, fIndex = 0, mIndex = 0, iIndex = 0;
        string finalSurname = string.Empty;
        string finalFirstName = string.Empty;
        List<string> finalMiddleNames = new List<string>();

        // --------------------------
        // Step 1: Determine Surname
        // --------------------------
        if (surnameWords.Count > sIndex)
        {
            finalSurname = surnameWords[sIndex];
            sIndex++;
        }
        else if (firstNameWords.Count > fIndex)
        {
            finalSurname = firstNameWords[fIndex];
            fIndex++;
        }
        else if (middleNameWords.Count > mIndex)
        {
            finalSurname = middleNameWords[mIndex];
            mIndex++;
        }


        // --------------------------
        // Step 2: Determine First Name
        // --------------------------
        // First, look exclusively in the firstName field.
        if (firstNameWords.Count > fIndex)
        {
            finalFirstName = firstNameWords[fIndex];
            fIndex++;
        }
        // If the firstName field is empty, then (only) if the surname field was provided,
        // use the next word from the surname.
        else if (surnameWords.Count > sIndex && !string.IsNullOrWhiteSpace(surname))
        {
            finalFirstName = surnameWords[sIndex];
            sIndex++;
        }
        // Otherwise, look for a word in the middleName field.
        else if (middleNameWords.Count > mIndex)
        {
            finalFirstName = middleNameWords[mIndex];
            mIndex++;
        }


        // --------------------------
        // Step 3: Compile Middle Name
        // --------------------------
        // Append all remaining words from each field (in order) to form the middle name.
        if (sIndex < surnameWords.Count)
            finalMiddleNames.AddRange(surnameWords.Skip(sIndex));
        if (fIndex < firstNameWords.Count)
            finalMiddleNames.AddRange(firstNameWords.Skip(fIndex));
        if (mIndex < middleNameWords.Count)
            finalMiddleNames.AddRange(middleNameWords.Skip(mIndex));


        string finalMiddleName = string.Join(" ", finalMiddleNames);

        return (finalSurname, finalFirstName, finalMiddleName);
    }
    public (string Surname, string FirstName, string MiddleName) ProcessFullNames(string surname, string firstName, string middleName)
    {
        // Split each input into words, removing any empty entries.
        List<string> surnameWords = string.IsNullOrWhiteSpace(surname)
            ? new List<string>()
            : surname.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        List<string> firstNameWords = string.IsNullOrWhiteSpace(firstName)
            ? new List<string>()
            : firstName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        List<string> middleNameWords = string.IsNullOrWhiteSpace(middleName)
            ? new List<string>()
            : middleName.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();


        // These pointers track which words have been used from each list.
        int sIndex = 0, fIndex = 0, mIndex = 0, iIndex = 0;
        string finalSurname = string.Empty;
        string finalFirstName = string.Empty;
        List<string> finalMiddleNames = new List<string>();

        // --------------------------
        // Step 1: Determine Surname
        // --------------------------
        if (surnameWords.Count > sIndex)
        {
            finalSurname = surnameWords[sIndex];
            sIndex++;
        }
        else if (firstNameWords.Count > fIndex)
        {
            finalSurname = firstNameWords[fIndex];
            fIndex++;
        }
        else if (middleNameWords.Count > mIndex)
        {
            finalSurname = middleNameWords[mIndex];
            mIndex++;
        }


        // --------------------------
        // Step 2: Determine First Name
        // --------------------------
        // First, look exclusively in the firstName field.
        if (firstNameWords.Count > fIndex)
        {
            finalFirstName = firstNameWords[fIndex];
            fIndex++;
        }
        // If the firstName field is empty, then (only) if the surname field was provided,
        // use the next word from the surname.
        else if (surnameWords.Count > sIndex && !string.IsNullOrWhiteSpace(surname))
        {
            finalFirstName = surnameWords[sIndex];
            sIndex++;
        }
        // Otherwise, look for a word in the middleName field.
        else if (middleNameWords.Count > mIndex)
        {
            finalFirstName = middleNameWords[mIndex];
            mIndex++;
        }


        // --------------------------
        // Step 3: Compile Middle Name
        // --------------------------
        // Append all remaining words from each field (in order) to form the middle name.
        if (sIndex < surnameWords.Count)
            finalMiddleNames.AddRange(surnameWords.Skip(sIndex));
        if (fIndex < firstNameWords.Count)
            finalMiddleNames.AddRange(firstNameWords.Skip(fIndex));
        if (mIndex < middleNameWords.Count)
            finalMiddleNames.AddRange(middleNameWords.Skip(mIndex));


        string finalMiddleName = string.Join(" ", finalMiddleNames);

        return (finalSurname, finalFirstName, finalMiddleName);
    }
    public CellDataAndStatus[] Surname_FirtName_MiddleName_Garantor(string surname, string firsname, string middlename)
    {
        CellDataAndStatus[] cds = [new CellDataAndStatus(surname), new CellDataAndStatus(firsname), new CellDataAndStatus(middlename)];

        //if (ContainsRestrictedWord(surname) || ContainsRestrictedWord(firsname) || ContainsRestrictedWord(middlename))
        //{
        //    cds[0].Passed = true; cds[1].Passed = true;
        //    return cds;
        //}
        //else
        //{

            if (ContainsInvalidCharacters(surname) || ContainsInvalidCharacters(firsname) || ContainsInvalidCharacters(middlename))
            {
                cds[0].Passed = true; cds[1].Passed = true; return cds;
            }
            surname = RemoveSpacesAroundHyphen(surname);
            firsname = RemoveSpacesAroundHyphen(firsname);
            middlename = RemoveSpacesAroundHyphen(middlename);
            var dup_names = RemoveDuplicateNamesRemoveDuplicate(surname, firsname, middlename);
            var valid_and_inv_names = RemoveInvalidName(dup_names.CleanedSurname, dup_names.CleanedFirstname, dup_names.CleanedMiddlename);
            var completeName = ProcessFullNames(valid_and_inv_names.Surname, valid_and_inv_names.FirstName, valid_and_inv_names.MiddleName + " " + valid_and_inv_names.InvalidNames);
            cds[0].Passed = true; cds[1].Passed = true;
            cds[0].Data = completeName.Surname.Replace("APPPPPPPPPPA", "").ToUpper();
            cds[1].Data = completeName.FirstName.Replace("APPPPPPPPPPA", "").ToUpper();
            cds[2].Data = completeName.MiddleName.Replace("APPPPPPPPPPA", "").ToUpper();
            return cds;
        //}
    }


    //STEPS
    //1. ContainsInvalidCharacters
    //2. RemoveDuplicateNamesRemoveDuplicate
    //3. RemoveSpacesAroundHyphen
    //4. RemoveInvalidName
    //5. ProcessFullNames
    public CellDataAndStatus[] Surname_FirtName_MiddleName(string surname, string firsname, string middlename)
    {
        CellDataAndStatus[] cds = [new CellDataAndStatus(surname), new CellDataAndStatus(firsname), new CellDataAndStatus(middlename)];

        //if (ContainsRestrictedWord(surname) || ContainsRestrictedWord(firsname) || ContainsRestrictedWord(middlename))
        //{
        //    cds[0].Passed = false; cds[1].Passed = false;
        //    cds[0].Errors = new List<string>() { " SURNAME AND FIRSTNAME ARE MANDATORY AND SHOULD CONTAIN VALID NAMES " }; cds[1].Errors = new List<string>() { " SURNAME AND FIRSTNAME ARE MANDATORY AND SHOULD CONTAIN VALID NAMES " };
        //    return cds;
        //}
        //else
        //{

            if (ContainsInvalidCharacters(surname) || ContainsInvalidCharacters(firsname) || ContainsInvalidCharacters(middlename))
            {
                cds[0].Passed = false; cds[1].Passed = false;
                cds[0].Errors = new List<string>() { " NAMES CANNOT CONTAIN SEPECIAL CHARACTER(S) " }; cds[1].Errors = new List<string>() { " NAMES CANNOT CONTAIN SEPECIAL CHARACTER(S) " };
                return cds;
            }
           


            surname = RemoveSpacesAroundHyphen(surname);
            firsname = RemoveSpacesAroundHyphen(firsname);
            middlename = RemoveSpacesAroundHyphen(middlename);
            var dup_names = RemoveDuplicateNamesRemoveDuplicate(surname, firsname, middlename);
            var valid_and_inv_names = RemoveInvalidName(dup_names.CleanedSurname, dup_names.CleanedFirstname, dup_names.CleanedMiddlename);
            var completeName = ProcessFullNames(valid_and_inv_names.Surname, valid_and_inv_names.FirstName, valid_and_inv_names.MiddleName + " " + valid_and_inv_names.InvalidNames);
            if (!string.IsNullOrWhiteSpace(completeName.Surname) && !string.IsNullOrWhiteSpace(completeName.FirstName)
                && completeName.Surname.Length >= 3 && completeName.FirstName.Length >= 3)
            {
                cds[0].Passed = true; cds[1].Passed = true;
                cds[0].Data = completeName.Surname.ToUpper();
                cds[1].Data = completeName.FirstName.ToUpper();
                //cds[2].Data = completeName.MiddleName.ToUpper();
                cds[2].Data = NewRemoveMiddles(completeName.MiddleName.ToUpper());
                return cds;
            }
            cds[0].Passed = false; cds[1].Passed = false;
            cds[0].Errors = new List<string>() { " SURNAME AND FIRSTNAME ARE MANDATORY AND SHOULD CONTAIN VALID NAMES " }; cds[1].Errors = new List<string>() { " SURNAME AND FIRSTNAME ARE MANDATORY AND SHOULD CONTAIN VALID NAMES " };
            return cds;
        //}
    }
    private readonly HashSet<string> WordsToRemove = new HashSet<string>
    {
        "N/A","NA", "NULL","NILL","NONE", "NOTAPPLICABLE", "NOT APPLICABLE","#NAME?", "#NAME" // Add your words here
    };
    private readonly HashSet<char> SpecialChars = new HashSet<char>
    {
        ',', '?', '=', '*', '[', ']', '(', ')','.'
    };
    public string NewCleanText(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        StringBuilder result = new StringBuilder();
        StringBuilder wordBuffer = new StringBuilder();
        bool lastWasSpace = false;

        foreach (char c in input)
        {
            if (char.IsWhiteSpace(c))
            {
                if (wordBuffer.Length > 0)
                {
                    string word = wordBuffer.ToString();
                    if (!WordsToRemove.Contains(word))
                    {
                        if (result.Length > 0) result.Append(' ');
                        result.Append(word);
                    }
                    wordBuffer.Clear();
                }

                if (!lastWasSpace)
                {
                    result.Append(' ');
                    lastWasSpace = true;
                }
            }
            else if (SpecialChars.Contains(c))
            {
                continue;
            }
            else
            {
                wordBuffer.Append(c);
                lastWasSpace = false;
            }
        }

        // Add the last word if needed
        if (wordBuffer.Length > 0)
        {
            string word = wordBuffer.ToString();
            if (!WordsToRemove.Contains(word))
            {
                if (result.Length > 0) result.Append(' ');
                result.Append(word);
            }
        }

        return result.ToString().ToUpper().Trim();
    }
    public bool IsSpecialCharacter(char character)
    {
        return character switch
        {
            ',' or '?' or '=' or '*' or '[' or ']' or '(' or ')' or '’' => true,
            _ => false
        };
    }
    private void HandleSpecialCharacter(char character, StringBuilder cleanedString, ref bool previousWasSpace, int handlingMode)
    {
        switch (handlingMode)
        {
            case 1: // Remove
                break;
            case 2 when !previousWasSpace: // Replace with space
                cleanedString.Append(' ');
                previousWasSpace = true;
                break;
            case 3: // Wrap with spaces
                if (!previousWasSpace)
                    cleanedString.Append(' ');
                cleanedString.Append(character);
                cleanedString.Append(' ');
                previousWasSpace = true;
                break;
        }
    }
    private void HandleNonAlphanumericCharacter(StringBuilder cleanedString, ref bool previousWasSpace)
    {
        if (!previousWasSpace)
        {
            cleanedString.Append(' ');
            previousWasSpace = true;
        }
    }
    private string ReplaceWordApostrophes(StringBuilder input)
    {
        return input.Replace('\'', '’').ToString();
    }
    private string CollapseSpaces(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        StringBuilder result = new StringBuilder(input.Length);
        bool previousWasSpace = false;

        foreach (char c in input)
        {
            if (c == ' ')
            {
                if (!previousWasSpace)
                {
                    result.Append(' ');
                    previousWasSpace = true;
                }
            }
            else
            {
                result.Append(c);
                previousWasSpace = false;
            }
        }

        return result.ToString().Trim();
    }
    public string GhanacardIdValidation(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            return input;
        }

        var result = new StringBuilder(input.Length);

        foreach (char c in input)
        {
            if ((c >= 'A' && c <= 'Z') || // Uppercase letters
                (c >= 'a' && c <= 'z') || // Lowercase letters
                (c >= '0' && c <= '9'))   // Digits
            {
                result.Append(c);
            }
        }

        return result.ToString().ToUpper();
    }
    public string ReplaceSomeSpecialCharWithSpace(string data)
    {
        return ReplaceCharacters(data, true, true).ToUpper();
    }
    public string ReplaceSomeSpecialCharNoSpace(string data)
    {
        return ReplaceCharacters(data, false, true).ToUpper();
    }
    public string ReplaceAlphanumericWithSpace(string data)
    {
        return ReplaceCharacters(data, true, false).ToUpper();
    }
    public string ReplaceAlphanumericNoSpace(string data)
    {
        return ReplaceCharacters(data, false, false).ToUpper();
    }
    public string ReplaceAlphabetWithSpace(string data)
    {
        return ReplaceCharacters(data, true, false, true).ToUpper();
    }
    public string ReplaceAlphabetNoSpace(string data)
    {
        return ReplaceCharacters(data, false, false, true).ToUpper();
    }
    public string BasicClean(string data, bool hasSpace)
    {
        string[] patterns = { "N/A", "NULL", "NOT APPLICABLE" };
        foreach (string pattern in patterns)
        {
            data = ReplaceWholeWord(data, pattern, hasSpace ? " " : string.Empty);
        }
        data = RemoveMultipleWhitespace(data);
        data = RemoveSpecialCharactersNew(data);
        return data.ToUpper();
    }
    public string RemoveTilesFromName(string data, bool hasSpace)
    {
        string[] patterns = { "N/A", "NULL", "NOT APPLICABLE" };
        foreach (string pattern in patterns)
        {
            data = ReplaceWholeWord(data, pattern, hasSpace ? " " : string.Empty);
        }
        data = RemoveMultipleWhitespace(data);
        data = RemoveSpecialCharactersNew(data);
        return data.ToUpper();
    }
    private string ReplaceWholeWord(string input, string pattern, string replacement)
    {
        if (string.IsNullOrEmpty(pattern))
            return input;

        List<int> indices = new List<int>();
        int patternLength = pattern.Length;
        int inputLength = input.Length;

        for (int i = 0; i <= inputLength - patternLength; i++)
        {
            bool isMatch = true;
            for (int j = 0; j < patternLength; j++)
            {
                if (char.ToLowerInvariant(input[i + j]) != char.ToLowerInvariant(pattern[j]))
                {
                    isMatch = false;
                    break;
                }
            }

            if (isMatch)
            {
                bool startBoundary = (i == 0) || !IsWordChar(input[i - 1]);
                bool endBoundary = (i + patternLength >= inputLength) || !IsWordChar(input[i + patternLength]);

                if (startBoundary && endBoundary)
                {
                    indices.Add(i);
                    i += patternLength - 1; // Skip over the matched pattern
                }
            }
        }

        // Replace matches starting from the end to maintain correct indices
        StringBuilder sb = new StringBuilder(input);
        foreach (int index in indices.OrderByDescending(x => x))
        {
            sb.Remove(index, patternLength);
            sb.Insert(index, replacement);
        }

        return sb.ToString();
    }
    private bool IsWordChar(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }
    private string RemoveMultipleWhitespace(string input)
    {
        List<char> result = new List<char>();
        int i = 0;
        while (i < input.Length)
        {
            if (char.IsWhiteSpace(input[i]))
            {
                int start = i;
                while (i < input.Length && char.IsWhiteSpace(input[i]))
                    i++;
                int length = i - start;
                if (length < 2)
                    result.Add(input[start]); // Keep single whitespace
            }
            else
            {
                result.Add(input[i]);
                i++;
            }
        }
        return new string(result.ToArray()).Trim();
    }
    private string RemoveSpecialCharactersNew(string input)
    {
        char[] charsToRemove = { ',', '?', '=', '*', '[', ']', '(', ')' };
        char apostropheToReplace = '’';
        char apostropheReplacement = '\'';
        StringBuilder sb = new StringBuilder();
        foreach (char c in input)
        {
            if (Array.IndexOf(charsToRemove, c) >= 0)
                continue;
            if (c == apostropheToReplace)
                sb.Append(apostropheReplacement);
            else
                sb.Append(c);
        }
        return sb.ToString().ToUpperInvariant();
    }
    private string ReplaceCharacters(string data, bool replaceWithSpace, bool includeSpecialChars, bool lettersOnly = false)
    {
        if (string.IsNullOrEmpty(data))
            return data;

        var result = new StringBuilder(data.Length);
        bool previousWasSpace = false;

        foreach (char c in data)
        {
            if (lettersOnly ? char.IsLetter(c) : char.IsLetterOrDigit(c))
            {
                result.Append(c);
                previousWasSpace = false;
            }
            else if (includeSpecialChars && IsSpecialChar(c))
            {
                if (replaceWithSpace && !previousWasSpace)
                {
                    result.Append(' ');
                    previousWasSpace = true;
                }
            }
            else if (replaceWithSpace && !previousWasSpace)
            {
                result.Append(' ');
                previousWasSpace = true;
            }
        }

        return result.ToString().Trim();
    }
    private bool IsSpecialChar(char c)
    {
        return c == ',' || c == '?' || c == '=' || c == '*' || c == '[' || c == ']' || c == '(' || c == ')' || c == '’';
    }
    string RemoveSpecialCharactera(string str)
    {
        StringBuilder sb = new StringBuilder();

        foreach (char c in str)
        {
            // Check if the character is a letter or a digit
            if (char.IsLetterOrDigit(c))
            {
                sb.Append(c); // Append only alphanumeric characters
            }
        }

        return sb.ToString();
    }
    private string Sanitize(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        Span<char> buffer = stackalloc char[input.Length];
        int cleanIndex = 0;

        foreach (char c in input.AsSpan())
        {
            if (char.IsLetterOrDigit(c))
            {
                buffer[cleanIndex++] = c;
            }
        }

        return cleanIndex == 0 ?
            string.Empty :
            new string(buffer.Slice(0, cleanIndex));
    }
    public string RemoveSpecialCharacters_ID(string input)
    {
        StringBuilder sb = new StringBuilder();

        foreach (char c in input)
        {
            // Keep letters, digits, and hyphens
            if (char.IsLetterOrDigit(c) || c == '-')
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
    private string RemovePattern(string input, string pattern)
    {
        int index;
        while ((index = input.IndexOf(pattern, StringComparison.OrdinalIgnoreCase)) != -1)
        {
            input = input.Remove(index, pattern.Length);
        }
        return input;
    }
    private string RemoveExtraSpaces(string input)
    {
        var result = new StringBuilder();
        bool spaceAdded = false;

        foreach (char c in input)
        {
            if (char.IsWhiteSpace(c))
            {
                if (!spaceAdded)
                {
                    result.Append(' ');
                    spaceAdded = true;
                }
            }
            else
            {
                result.Append(c);
                spaceAdded = false;
            }
        }

        return result.ToString();
    }
    private string RemoveNonAlphanumeric(string input)
    {
        var result = new StringBuilder();
        foreach (char c in input)
        {
            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
            {
                result.Append(c);
            }
        }
        return result.ToString();
    }
    public CellDataAndStatus TextFieldNoSpace(string data, string cellHeader, int dataLength)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = RemoveSpecialCharacters_ID(data);
        return cellData;
    }
    public string NumericField(string data)
    {
        data = new string(data.Where(char.IsDigit).ToArray());
        return data;
    }
    public CellDataAndStatus NormalizeGender(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Passed = true;
        data = Regex.Replace(data, "[^a-zA-Z]", "").ToUpper();
        if (string.IsNullOrWhiteSpace(data))
        {
            cellData.Data = string.Empty;
            return cellData;
        }

        // Define accepted male terms
        var maleTerms = new HashSet<string>
        {
           "MALE", "M", "MAN", "BOY", "GUY", "GENTLEMAN"
        };

        // Define accepted female terms
        var femaleTerms = new HashSet<string>
        {
            "FEMALE", "F", "WOMAN", "GIRL", "LADY"
        };

        // Check if input matches male terms
        if (maleTerms.Contains(data))
        {
            cellData.Data = "M";
            return cellData;
        }

        // Check if input matches female terms
        if (femaleTerms.Contains(data))
        {
            cellData.Data = "F";
            return cellData;
        }

        // Return null for invalid inputs
        cellData.Data = string.Empty;
        return cellData;
    }
    public string CleanIDs(string data)
    {
        data = ReplaceDoubleSpacingAndApostrophe(data);
        data = RemoveAllSpacing(data);
        return data;
    }
    public string ReplaceDoubleSpacingAndApostrophe(string data)
    {
        data = Regex.Replace(data, @"\s+", " ");
        return data;
    }
    public string RemoveAllSpacing(string data)
    {
        data = ReplaceDoubleSpacingAndApostrophe(data);
        data = Regex.Replace(data, @"\s+", " ");
        return data;
    }
    public string NormalizeCurrency(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return "GHS";
        }

        // Patterns for Ghana Cedis (GHS)
        string cediPattern = @"(?i)(ghana\s*cedis?|gh[cs]|gh¢|cedis?|cede(?:es|is)?)";
        // Patterns for US Dollars (USD)
        string dollarPattern = @"(?i)(us\s*dollars?|usd|bucks|dollars?)";

        // Normalize Ghana Cedis to GHS
        string normalized = Regex.Replace(input, cediPattern, "GHS");

        // Normalize US Dollars to USD
        normalized = Regex.Replace(normalized, dollarPattern, "USD");

        // Remove extra spaces and trim
        normalized = Regex.Replace(normalized, @"\s+", " ").Trim();

        // Capitalize correctly
        normalized = Regex.Replace(normalized, @"\bghs\b", "GHS", RegexOptions.IgnoreCase);
        normalized = Regex.Replace(normalized, @"\busd\b", "USD", RegexOptions.IgnoreCase);

        return normalized;
    }
    public string[] ArangedData4(string data0, string data1, string data2, string data3)
    {
        //Collect the addresses into a list
        List<string> main_data = new List<string> { data0, data1, data2, data3 };

        // Remove any null or empty strings
        main_data = main_data.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

        // Add placeholders to ensure the list has four elements
        while (main_data.Count < 4)
        {
            main_data.Add(string.Empty);
        }

        // Return the arranged addresses
        return main_data.ToArray();
    }
    private readonly string[] HouseNumberPatterns = new[]
    {
        "H No.", "House No.", "H No", "Hse No.", "Hse No", "Hse", "House #", "H#", "H/N", "H/No", "HNO", "HN", "No. ", "House Number",
        "BLK", "Block", "Blk", // Added BLK as BLOCK
        "APT", "Apartment", "Apt", // Added APT as APARTMENT
        "UNIT", "Unit", // Added UNIT
        "FLR", "Floor", "Flr", // Added FLR as FLOOR
        "ST", "Street", "St", // Added ST as STREET
        "RD", "Road", "Rd", // Added RD as ROAD
        "AVE", "Avenue", "Ave", // Added AVE as AVENUE
        "BLVD", "Boulevard", "Blvd", // Added BLVD as BOULEVARD
        "LN", "Lane", "Ln", // Added LN as LANE
        "DR", "Drive", "Dr", // Added DR as DRIVE
        "CT", "Court", "Ct", // Added CT as COURT
        "PL", "Place", "Pl", // Added PL as PLACE
        "STE", "Suite", "Ste" // Added STE as SUITE
    };
    private readonly string[] ExclusionPatterns = new[]
    {
        "P.O. Box", "PO Box", "Post Office Box" // Added more variations for exclusion
    };
    private string RemovePhoneNumbers(string input)
    {
        var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>();

        foreach (var word in words)
        {
            if (!IsPhoneNumber(word))
            {
                result.Add(word);
            }
        }

        return string.Join(" ", result);
    }
    private bool IsPhoneNumber(string word)
    {
        int digitCount = 0;
        foreach (char c in word)
        {
            if (char.IsDigit(c))
            {
                digitCount++;
            }
            else if (!char.IsWhiteSpace(c) && c != '-' && c != '.' && c != '(' && c != ')')
            {
                return false;
            }
        }
        return digitCount >= 10; // Minimum 10 digits for a phone number
    }
    private string RemoveEmailAddresses(string input)
    {
        var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var result = new List<string>();

        foreach (var word in words)
        {
            if (!IsEmailAddress(word))
            {
                result.Add(word);
            }
        }

        return string.Join(" ", result);
    }
    private bool IsEmailAddress(string word)
    {
        return word.Contains("@") && word.Contains(".");
    }
    //private string RemoveBusinessKeywords111(string input)
    //{
    //    foreach (string keyword in BusinessKeyNames)
    //    {
    //        input = input.Replace(keyword, "", StringComparison.OrdinalIgnoreCase);
    //    }

    //    return input;
    //}
    private string NormalizeSpaces(string input)
    {
        var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return string.Join(" ", words);
    }
    private string CapitalizeWords(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return string.Empty;
        }

        var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < words.Length; i++)
        {
            words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
        }

        return string.Join(" ", words);
    }
    public bool IsDataLengthExceed(string data, int isGreaterThan = 100)
    {
        if (string.IsNullOrWhiteSpace(data)) return false;
        if (data.Length > isGreaterThan) return true;
        return false;
    }
    public string ReplaceEmail(string data)
    {
        string emailPattern = @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}";

        // Replace email addresses with an empty string
        data = Regex.Replace(data, emailPattern, "");
        return data;
    }
    public string ErrorWrapper(string cellHeader, string message)
    {
        return $"  {cellHeader}: {message}";
    }
    private readonly HashSet<char> SpecialCharacters = new HashSet<char>
    {
        '!', '$', '%', '^', '*', '=', '+', '[', ']', '{', '}', ';', ':', '\"', '<', '>', ',', '?', '`', '~'
    };
    /// <summary>
    /// Arranges, normalizes, and truncates (if needed) four address strings.
    /// </summary>
    /// <param name="data0">Address 1</param>
    /// <param name="data1">Address 2</param>
    /// <param name="data2">Address 3</param>
    /// <param name="data3">Address 4</param>
    /// <returns>An array of four addresses with the required processing applied.</returns>
    public CellDataAndStatus[] ArrangeAndNormalizeAddresses(string data0, string data1, string data2, string data3)
    {
        CellDataAndStatus[] address = [new CellDataAndStatus(data0), new CellDataAndStatus(data1), new CellDataAndStatus(data2), new CellDataAndStatus(data3)];
        // Collect the addresses into a list.
        List<string> addresses = new List<string> { data0, data1, data2, data3 };

        // Process each address.
        for (int i = 0; i < addresses.Count; i++)
        {
            if (!string.IsNullOrWhiteSpace(addresses[i]))
            {
                // 1. Remove any special characters (only those in the hashset are removed).
                addresses[i] = RemoveSpecialCharacters(addresses[i]);

                // 2. Normalize house number patterns.
                addresses[i] = NormalizeHouseNumber(addresses[i]);

                // 3. Ensure that the address length does not exceed 100 characters.
                if (addresses[i].Length > 100)
                {
                    addresses[i] = addresses[i].Substring(0, 100);
                }
            }
        }

        // Remove any null/empty strings and then shift addresses so that the first ones are filled.
        addresses = addresses.Where(a => !string.IsNullOrWhiteSpace(a)).ToList();

        // Add empty string placeholders if there are less than four addresses.
        while (addresses.Count < 4)
        {
            addresses.Add(string.Empty);
        }

        var _addresses = addresses.ToArray();
        address[0].Data = _addresses[0];
        address[1].Data = _addresses[1];
        address[2].Data = _addresses[2];
        address[3].Data = _addresses[3];
        return address;
    }

    /// <summary>
    /// Removes all special characters from the input that are contained in the SpecialCharacters hash set.
    /// Alphanumeric characters and any characters not in the hash set remain.
    /// </summary>
    private string RemoveSpecialCharacters(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        StringBuilder sb = new StringBuilder();
        foreach (char c in input)
        {
            if (!SpecialCharacters.Contains(c))
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    /// <summary>
    /// Normalizes house number patterns in the address by replacing them with standardized tokens.
    /// </summary>
    public string NormalizeHouseNumber(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return string.Empty;

        StringBuilder result = new StringBuilder(data.Length);
        result.Append(data);

        // Replace specific characters.
        result.Replace("#", "NUMBER");
        result.Replace("@", "AT");
        result.Replace("&", "AND");

        // Replace house number patterns.
        foreach (var pattern in HouseNumberPatterns)
        {
            int index = 0;
            while ((index = result.ToString().IndexOf(pattern, index, StringComparison.OrdinalIgnoreCase)) != -1)
            {
                // Check if the pattern is part of an exclusion context.
                bool isExcluded = false;
                foreach (var exclusion in ExclusionPatterns)
                {
                    if (index >= exclusion.Length &&
                        result.ToString().Substring(index - exclusion.Length, exclusion.Length)
                              .Equals(exclusion, StringComparison.OrdinalIgnoreCase))
                    {
                        isExcluded = true;
                        break;
                    }
                }

                if (!isExcluded)
                {
                    string replacement = GetReplacementForPattern(pattern);
                    result.Remove(index, pattern.Length);
                    result.Insert(index, replacement);
                    index += replacement.Length;
                }
                else
                {
                    index += pattern.Length;
                }
            }
        }

        return result.ToString();
    }
    /// <summary>
    /// Returns the replacement string for a given house number pattern.
    /// </summary>
    private string GetReplacementForPattern(string pattern)
    {
        switch (pattern.ToUpper())
        {
            case "BLK":
            case "BLOCK":
                return "BLOCK";
            case "APT":
            case "APARTMENT":
                return "APARTMENT";
            case "UNIT":
                return "UNIT";
            case "FLR":
            case "FLOOR":
                return "FLOOR";
            case "ST":
            case "STREET":
                return "STREET";
            case "RD":
            case "ROAD":
                return "ROAD";
            case "AVE":
            case "AVENUE":
                return "AVENUE";
            case "BLVD":
            case "BOULEVARD":
                return "BOULEVARD";
            case "LN":
            case "LANE":
                return "LANE";
            case "DR":
            case "DRIVE":
                return "DRIVE";
            case "CT":
            case "COURT":
                return "COURT";
            case "PL":
            case "PLACE":
                return "PLACE";
            case "STE":
            case "SUITE":
                return "SUITE";
            default:
                return "HOUSE NUMBER"; // Default replacement for unknown patterns.
        }
    }
    public string ValidateCurrency(string data)
    {
        if (string.IsNullOrWhiteSpace(data) || data == "GH" || data == "GHC" || data == "cedis")
        {
            data = "GHS";
        }
        else if (data.Length == 3)
        {
            data = data.ToUpper();
        }
        else
        {
            data = string.Empty;
        }
        return data;
    }
    public CellDataAndStatus NormarlizeMaritalStatus(string data)
    {
        var celldata = new CellDataAndStatus(data);
        data = RemoveSystemErroNames(data); ////DSMW

        if (data == "S" || data == "W" || data == "D" || data == "M" || data == "SINGLE" || data == "WIDOWED" || data == "DIVORCED" || data == "MARRIED")
        {
            celldata.Passed = true;
            celldata.Data = data.Substring(0, 1).ToUpper();
            return celldata;
        }
        else
        {
            celldata.Data = string.Empty;
            celldata.Passed = true;
            return celldata;
        }
    }
    public CellDataAndStatus[] ProcessPhoneNumbers(string phone1, string phone2)
    {
        CellDataAndStatus[] cds = { new CellDataAndStatus(phone1), new CellDataAndStatus(phone2) };
        cds[0].Data = phone1;
        cds[1].Data = phone2;
        // Validate phone numbers
        bool isPhone1Valid = IsValidPhoneNumber(phone1);
        bool isPhone2Valid = IsValidPhoneNumber(phone2);

        // Set invalid phone numbers to empty string
        string processedPhone1 = isPhone1Valid ? phone1 : "";
        string processedPhone2 = isPhone2Valid ? phone2 : "";

        // Shift phone2 to phone1 if phone2 is valid and phone1 is invalid
        if (!isPhone1Valid && isPhone2Valid)
        {
            processedPhone1 = processedPhone2;
            processedPhone2 = "";
        }

        if (string.IsNullOrWhiteSpace(processedPhone1))
        {
            cds[0].Passed = false;
        }
        else
        {
            cds[0].Passed = true;
        }
        cds[0].Data = processedPhone1;
        cds[1].Data = processedPhone2;
        return cds;
    }
    public bool IsValidPhoneNumber(string phoneNumber)
    {
        // Check if the phone number is exactly 10 digits and contains only numbers
        if (phoneNumber == null || !(phoneNumber.Length >= 9 && phoneNumber.Length <= 19))
        {
            return false;
        }

        foreach (char c in phoneNumber)
        {
            if (!char.IsDigit(c))
            {
                return false;
            }
        }

        return true;
    }
    public string SinglePhoneNumber(string phonenumber)
    {
        phonenumber = phonenumber.Replace("+", "");
        if (!IsValidPhoneNumber(phonenumber))
        {
            phonenumber = string.Empty;
        }
        if (HasSixConsecutiveZeros(phonenumber))
        {
            phonenumber = string.Empty;
        }
        phonenumber = ProcessPhoneNumber(phonenumber);
        return phonenumber;
    }
    public CellDataAndStatus[] MobileTel1_MobileTel2(string mobileTel1, string mobileTel2)
    {
        CellDataAndStatus[] mobileTel1_mobileTel2 = { new CellDataAndStatus(mobileTel1), new CellDataAndStatus(mobileTel2) };
        mobileTel1 = mobileTel1.Replace("+", "");
        mobileTel2 = mobileTel2.Replace("+", "");
        if (!IsValidPhoneNumber(mobileTel1))
        {
            mobileTel1 = string.Empty;
        }
        if (!IsValidPhoneNumber(mobileTel2))
        {
            mobileTel2 = string.Empty;
        }
        if (IsValidPhoneNumber(mobileTel1))
        {
            mobileTel1 = mobileTel1;
        }
        if (IsValidPhoneNumber(mobileTel2))
        {
            mobileTel2 = mobileTel2;
        }

        if (!string.IsNullOrWhiteSpace(mobileTel1) && !string.IsNullOrWhiteSpace(mobileTel2))
        {
            mobileTel1 = mobileTel1;
            mobileTel2 = mobileTel2;
        }


        if (!string.IsNullOrWhiteSpace(mobileTel1) && string.IsNullOrWhiteSpace(mobileTel2))
        {
            // First data already has content, no action needed
            mobileTel1 = mobileTel1;
            mobileTel2 = string.Empty;
            mobileTel1_mobileTel2[0].Passed = true;
        }
        if (string.IsNullOrWhiteSpace(mobileTel1) && !string.IsNullOrWhiteSpace(mobileTel2))
        {
            // First data already has content, no action needed
            mobileTel1 = mobileTel2;
            mobileTel2 = string.Empty;
            mobileTel1_mobileTel2[0].Passed = true;
        }



        if (string.IsNullOrWhiteSpace(mobileTel1))
        {
            mobileTel1 = string.Empty;
            mobileTel1_mobileTel2[0].Passed = true;
        }
        if (string.IsNullOrWhiteSpace(mobileTel2))
        {
            mobileTel2 = string.Empty;
            mobileTel1_mobileTel2[0].Passed = true;
        }

        if (HasSixConsecutiveZeros(mobileTel1))
        {
            mobileTel1 = string.Empty;
        }
        if (HasSixConsecutiveZeros(mobileTel2))
        {
            mobileTel2 = string.Empty;
        }
        mobileTel1_mobileTel2[0].Data = ProcessPhoneNumber(mobileTel1);
        mobileTel1_mobileTel2[1].Data = ProcessPhoneNumber(mobileTel2);

        return mobileTel1_mobileTel2;
    }
    public string ProcessPhoneNumber(string number)
    {
        if (string.IsNullOrEmpty(number))
            return number;

        // Replace leading 233 with 0
        if (number.StartsWith("+233"))
        {
            number = "0" + number.Substring(3);
        }
        else if (number.StartsWith("+2330"))
        {
            number = "0" + number.Substring(3);
        }
        else if (number.StartsWith("2330"))
        {
            number = "0" + number.Substring(3);
        }
        // Replace leading 233 with 0
        else if (number.StartsWith("233"))
        {
            number = "0" + number.Substring(3);
        }
        // Add leading zero if the number has 9 digits
        else if (number.Length == 9)
        {
            number = "0" + number;
        }

        return number;
    }
    public bool HasSixConsecutiveZeros(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;

        int consecutiveZeroCount = 0;

        foreach (char c in input)
        {
            if (c == '0')
            {
                consecutiveZeroCount++;
                if (consecutiveZeroCount >= 6)
                    return true;
            }
            else
            {
                consecutiveZeroCount = 0; // Reset count if a non-zero character is found
            }
        }
        return false;
    }
    public bool HasSixConsecutiveZerosAtEnd(string input)
    {
        if (string.IsNullOrEmpty(input) || input.Length < 6) return false;

        char lastChar = input[^1]; // Get last character
        if (!char.IsDigit(lastChar)) return false; // Ensure it's a number

        int count = 1;

        // Iterate backward to count consecutive occurrences
        for (int i = input.Length - 2; i >= 0; i--)
        {
            if (input[i] == lastChar)
            {
                count++;
                if (count > 5) return true; // Early exit if more than 5 repeats
            }
            else
            {
                break; // Stop if a different character is found
            }
        }

        return false;
    }
    string TextOnltLettersOnlyHypenAndSpace(string input)
    {
        StringBuilder result = new StringBuilder();

        foreach (char c in input)
        {
            // Keep letters, spaces, and commas
            if (char.IsLetter(c) || c == ' ' || c == ',')
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
    public CellDataAndStatus[] CleanUserIdentity(string title, string surname, string firstName, string middleName)
    {
        // Clean and remove special characters from each field.
        title = NewCleanText(title);
        title = NewRemoveSpecialCharacters(title);

        surname = NewCleanText(surname);
        surname = NewRemoveSpecialCharacters(surname);

        firstName = NewCleanText(firstName);
        firstName = NewRemoveSpecialCharacters(firstName);

        middleName = NewCleanText(middleName);
        middleName = NewRemoveSpecialCharacters(middleName);

        // Initialize the result array.
        CellDataAndStatus[] cds = {
        new CellDataAndStatus(title),
        new CellDataAndStatus(surname),
        new CellDataAndStatus(firstName),
        new CellDataAndStatus(middleName)
    };

        // Initially assume surname and first name are required.
        cds[1].Errors = new List<string> { "INVALID SURNAME AND FIRSTNAME" };

        var allFields = new List<string> { title, surname, firstName, middleName };

        // Split each field by commas and spaces, ignoring empty entries.
        var tokens = new List<string>();
        foreach (var field in allFields)
        {
            if (!string.IsNullOrWhiteSpace(field))
            {
                tokens.AddRange(field.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }
        }

        // -------------------------------------------------------------
        // 1. Extract Titles: find any token that is a valid title.
        // -------------------------------------------------------------
        var foundTitles = tokens
            .Where(token => Titles.Contains(token, StringComparer.OrdinalIgnoreCase))
            .ToList();

        // Use the first found title as the final title (if any).
        string finalTitle = foundTitles.FirstOrDefault() ?? string.Empty;

        // Remove all tokens that are recognized as titles.
        tokens = tokens
            .Where(token => !Titles.Contains(token, StringComparer.OrdinalIgnoreCase))
            .ToList();

        // -------------------------------------------------------------
        // 2. Process Surname and First Name:
        // They must be at least 3 characters long.
        // -------------------------------------------------------------
        string finalSurname = string.Empty;
        string finalFirstName = string.Empty;
        var middleTokens = new List<string>();

        foreach (var token in tokens)
        {
            if (string.IsNullOrWhiteSpace(finalSurname))
            {
                if (token.Length >= 3)
                {
                    finalSurname = token;
                    continue;
                }
                else
                {
                    middleTokens.Add(token);
                    continue;
                }
            }

            if (string.IsNullOrWhiteSpace(finalFirstName))
            {
                if (token.Length >= 3)
                {
                    finalFirstName = token;
                    continue;
                }
                else
                {
                    middleTokens.Add(token);
                    continue;
                }
            }

            middleTokens.Add(token);
        }

        // Check for surname in middleTokens if still empty or too short
        if (string.IsNullOrWhiteSpace(finalSurname) || finalSurname.Length < 3)
        {
            var candidate = middleTokens.FirstOrDefault(t => t.Length >= 3);
            if (candidate != null)
            {
                if (!string.IsNullOrWhiteSpace(finalSurname))
                    middleTokens.Add(finalSurname);
                finalSurname = candidate;
                middleTokens.Remove(candidate);
            }
        }

        // Check for first name in middleTokens if still empty or too short
        if (string.IsNullOrWhiteSpace(finalFirstName) || finalFirstName.Length < 3)
        {
            var candidate = middleTokens.FirstOrDefault(t => t.Length >= 3);
            if (candidate != null)
            {
                if (!string.IsNullOrWhiteSpace(finalFirstName))
                    middleTokens.Add(finalFirstName);
                finalFirstName = candidate;
                middleTokens.Remove(candidate);
            }
        }

        // -------------------------------------------------------------
        // 3. Combine Remaining Tokens into Middle Name
        // -------------------------------------------------------------
        string finalMiddleName = middleTokens.Any() ? string.Join(" ", middleTokens) : string.Empty;

        // -------------------------------------------------------------
        // 4. Clean Up and Assign Final Values
        // -------------------------------------------------------------
        cds[0].Data = finalTitle;
        cds[1].Data = finalSurname;
        cds[2].Data = finalFirstName;
        cds[3].Data = Regex.Replace(finalMiddleName, @"[^a-zA-Z0-9\s]", " ");

        // Validate surname and first name lengths
        bool isSurnameValid = finalSurname.Length >= 3;
        bool isFirstNameValid = finalFirstName.Length >= 3;

        if (isSurnameValid && isFirstNameValid)
        {
            cds[1].Passed = true;
            cds[1].Errors = null;
        }
        else
        {
            cds[1].Passed = false;
            cds[1].Errors = new List<string>
        {
            "INVALID SURNAME AND FIRSTNAME"
        };
        }

        return cds;
    }
    public string NormalizeCurrencyField(string input)
    {
        // Regex to keep digits and a single decimal point
        return Regex.Replace(input, @"[^\d.]", "");
    }
    //public CellDataAndStatus[] DisbursementDate_MaturityDate(string disbursementDate, string maturityDate, string fName)
    //{
    //    CellDataAndStatus[] disbursementDate_MaturityDate = [new CellDataAndStatus(disbursementDate), new CellDataAndStatus(maturityDate)];
    //    // Determine Passed and Data values based on input arguments
    //    bool disbursementDateEmpty = string.IsNullOrWhiteSpace(disbursementDate);
    //    bool maturityDateEmpty = string.IsNullOrWhiteSpace(maturityDate);
    //    //#0 if d_date > file date
    //    var fDate = GetFacilityDateFromFileName(fName);
    //    if (fDate.IsValid)
    //    {

    //        if (DateTime.ParseExact(disbursementDate_MaturityDate[0].Data!, "yyyyMMdd", null) > DateTime.Parse(fDate.LastDate))
    //        {
    //            disbursementDate_MaturityDate[0].Passed = false;
    //            disbursementDate_MaturityDate[0].Errors = new List<string>() { "DisbursementDate is greater than the reporting period" };
    //            return disbursementDate_MaturityDate;
    //        }

    //    }
        
    //        //#1
    //        if (!disbursementDateEmpty || !maturityDateEmpty)
    //    {
    //        disbursementDate_MaturityDate[0].Passed = false;
    //        disbursementDate_MaturityDate[1].Passed = false;
    //        ErrorWrapper("DisbursementDate And MaturityDate", "BOTH REQUIRE A VALID DATE");
    //        return disbursementDate_MaturityDate;
    //    }
    //    //#2
    //    // Console.WriteLine("Disbursement date is greater than maturity date.");
    //    if (DateTime.ParseExact(disbursementDate_MaturityDate[0].Data!, "yyyyMMdd", null) <= DateTime.ParseExact(disbursementDate_MaturityDate[1].Data!, "yyyyMMdd", null))
    //    {
    //        disbursementDate_MaturityDate[0].Passed = false;
    //        disbursementDate_MaturityDate[1].Passed = false;

    //        return disbursementDate_MaturityDate;
    //    }
    //    else
    //    {
    //        disbursementDate_MaturityDate[0].Passed = true;
    //        disbursementDate_MaturityDate[1].Passed = true;
    //        return disbursementDate_MaturityDate;
    //    }
    //}
    string[] ProcessName(string input)
    {
        string[] fullNames = new string[3];
        var names = input.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(name => name.Trim())
                         .ToList();

        fullNames[0] = names?.ElementAtOrDefault(0) ?? string.Empty;
        fullNames[1] = names?.ElementAtOrDefault(1) ?? string.Empty;
        return fullNames;
    }
    public CellDataAndStatus[] FacilityAmount_DisbursementAmt1(string facilityAmount, string disbursementAmt)
    {
        var facilityData = new CellDataAndStatus(facilityAmount);
        var disbursementData = new CellDataAndStatus(disbursementAmt);


        facilityData.Passed = false;
        disbursementData.Passed = false;


        var d_facilityAmountData = ValidateToDecimalCurrency(facilityAmount);
        var d_disbursementAmtData = ValidateToDecimalCurrency(disbursementAmt);


        facilityData.Data = d_facilityAmountData.ValidData.ToString();
        disbursementData.Data = d_disbursementAmtData.ValidData.ToString();


        if (d_facilityAmountData.IsGreaterThanZero && string.IsNullOrWhiteSpace(d_disbursementAmtData.ValidData.ToString()))
        {
            facilityData.Passed = true;
            disbursementData.Passed = true;

            disbursementData.Data = d_facilityAmountData.ValidData.ToString();
            facilityData.Data = d_facilityAmountData.ValidData.ToString();
            return [facilityData, disbursementData];
        }
        if (d_disbursementAmtData.IsGreaterThanZero && string.IsNullOrWhiteSpace(d_facilityAmountData.ValidData.ToString()))
        {
            facilityData.Passed = true;
            disbursementData.Passed = true;

            disbursementData.Data = d_disbursementAmtData.ValidData.ToString();
            facilityData.Data = d_disbursementAmtData.ValidData.ToString();
            return [facilityData, disbursementData];
        }



        if (!d_facilityAmountData.IsGreaterThanZero && !d_disbursementAmtData.IsGreaterThanZero)
        {
            facilityData.Passed = false;
            disbursementData.Passed = false;
        }





        if (d_facilityAmountData.IsGreaterThanZero && d_disbursementAmtData.IsGreaterThanZero)
        {
            facilityData.Passed = true;
            disbursementData.Passed = true;
        }
        return [facilityData, disbursementData];
    }
    public string RoundUpNumberOfDaysInArr(string input)
    {
        if (double.TryParse(input, out double value))
        {
            return Math.Ceiling((value * 12) / 365).ToString();
        }
        return string.Empty; // Return an empty string for invalid input
    }
    public CellDataAndStatus CleanToTwoDecimalPlaces(string input)
    {
        var cellData = new CellDataAndStatus(input);
        bool iscurrency = decimal.TryParse(input, out decimal result);
        if (!iscurrency || result == 0m)
        {
            cellData.Data = string.Empty;
            cellData.Passed = false;
            return cellData;
        }
        else
        {
            cellData.Data = Math.Round(result, 2, MidpointRounding.AwayFromZero).ToString();
            cellData.Passed = true;
            return cellData;
        }
    }
    /// <summary>
    /// Cleans the input string by removing numbers and special characters,
    /// but keeps letters, commas, and spaces. Also ensures that multiple spaces
    /// are replaced by a single space.
    /// </summary>
    /// <param name="input">The original string to clean.</param>
    /// <returns>A cleaned string with only letters, commas, and single spacing.</returns>
    public string CleanTextNames(string input)
    {
        if (input == null)
            return string.Empty;

        // Step 1: Remove any character that is NOT an uppercase/lowercase letter, comma, or space.
        // The regex [^A-Za-z, ] matches any character that is not A-Z, a-z, comma, or space.
        string result = Regex.Replace(input, @"[^A-Za-z, ]", "");

        // Step 2: Replace multiple consecutive spaces with a single space.
        result = Regex.Replace(result, @"\s+", " ");

        // Optionally, trim the result to remove any leading or trailing spaces.
        return result.Trim();
    }
    public string ValidateOccupation(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
            return string.Empty;

        bool isNumeric = data.All(char.IsDigit);

        return isNumeric ? string.Empty : data;
    }
    public string NewRemoveSpecialCharacters(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new StringBuilder(input.Length);

        foreach (char c in input)
        {
            if (char.IsLetterOrDigit(c) || c == ' ') // Allow letters, digits, and spaces
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
    public string NewCleanTextIDs(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        StringBuilder result = new StringBuilder();
        StringBuilder wordBuffer = new StringBuilder();
        bool lastWasSpace = false;

        foreach (char c in input)
        {
            if (char.IsWhiteSpace(c))
            {
                if (wordBuffer.Length > 0)
                {
                    string word = wordBuffer.ToString();
                    if (!WordsToRemove.Contains(word))
                    {
                        if (result.Length > 0) result.Append(' ');
                        result.Append(word);
                    }
                    wordBuffer.Clear();
                }

                if (!lastWasSpace)
                {
                    result.Append(' ');
                    lastWasSpace = true;
                }
            }
            else if (SpecialCharsIDs.Contains(c))
            {
                continue;
            }
            else
            {
                wordBuffer.Append(c);
                lastWasSpace = false;
            }
        }

        // Add the last word if needed
        if (wordBuffer.Length > 0)
        {
            string word = wordBuffer.ToString();
            if (!WordsToRemove.Contains(word))
            {
                if (result.Length > 0) result.Append(' ');
                result.Append(word);
            }
        }
        return ReplaceWordApostrophes(result).ToString().ToUpper().Replace(" ", "");
    }
    private readonly HashSet<char> SpecialCharsIDs = new HashSet<char>
    {
         ',', '?', '=', '*', '[', ']', '(', ')', '!', '$', '%', '^', '&', '+', ';', '_', '-', '.', '/','\\'
    };
    public CellDataAndStatus[] IncomeCurrency_Income_jointOrSoleAcc_NoParticipantsInAcc(string incomeCurrency, string income, string jointOrSoleAcc, string noParticipantsInAcc)
    {
        CellDataAndStatus[] incomeCurrency_Income_jointOrSoleAcc_NoParticipantsInAcc = [new CellDataAndStatus(incomeCurrency), new CellDataAndStatus(income), new CellDataAndStatus(jointOrSoleAcc), new CellDataAndStatus(noParticipantsInAcc)];

        incomeCurrency = incomeCurrency.ToUpper().Trim().Replace(" ", "");
        noParticipantsInAcc = noParticipantsInAcc.ToUpper().Trim().Replace(" ", "");
        income = income.ToUpper().Trim().Replace(" ", "").Replace("-", "").Replace("'", "").Replace("‘", "").Replace("‘", "").Replace(",","");

        jointOrSoleAcc = jointOrSoleAcc.ToUpper().Trim().Replace(" ", "");


        //CURRENCY
        if (incomeCurrency.StartsWith("GH") && incomeCurrency.Length < 9)
        {
            incomeCurrency = "GHS";
        }
        else if (incomeCurrency == "GHS" || incomeCurrency == "USD" || incomeCurrency == "GBP" || incomeCurrency == "EUR")
        {
            incomeCurrency = incomeCurrency.ToUpper();
        }
        else
        {
            incomeCurrency = string.Empty;
        }

        var d_income = CleanToTwoDecimalPlaces(income);
        if (incomeCurrency.Length == 3)
        {
            incomeCurrency = incomeCurrency.ToUpper();
        }
        else
        {
            incomeCurrency = string.Empty;
        }
        // Currency-without-amount gets the currency blanked. Amount-without-
        // currency is left as-is (no change needed here -- d_income is
        // computed independently of incomeCurrency above).
        if (!string.IsNullOrWhiteSpace(incomeCurrency) && string.IsNullOrWhiteSpace(d_income.Data))
        {
            incomeCurrency = string.Empty;
        }
        if (
            (jointOrSoleAcc == "S" || jointOrSoleAcc == "1" || jointOrSoleAcc == "01") &&
            (string.IsNullOrWhiteSpace(noParticipantsInAcc) || noParticipantsInAcc == "0" || noParticipantsInAcc == "1")
            )
        {
            jointOrSoleAcc = "01"; noParticipantsInAcc = "1";
        }
        bool response = int.TryParse(noParticipantsInAcc, out int _numberOfParticipant);
        if (response)
        {

            if (_numberOfParticipant > 1)
            {
                jointOrSoleAcc = "02";
                noParticipantsInAcc = _numberOfParticipant.ToString();
            }
            else if (_numberOfParticipant == 1)
            {
                jointOrSoleAcc = "01";
                noParticipantsInAcc = _numberOfParticipant.ToString();
            }
            else
            {
                jointOrSoleAcc = string.Empty;
                noParticipantsInAcc = string.Empty;
            }
        }
        else
        {
            jointOrSoleAcc = string.Empty;
            noParticipantsInAcc = string.Empty;
        }



        incomeCurrency_Income_jointOrSoleAcc_NoParticipantsInAcc[0].Data = incomeCurrency;
        incomeCurrency_Income_jointOrSoleAcc_NoParticipantsInAcc[1].Data = d_income.Data.ToString();
        incomeCurrency_Income_jointOrSoleAcc_NoParticipantsInAcc[2].Data = jointOrSoleAcc;
        incomeCurrency_Income_jointOrSoleAcc_NoParticipantsInAcc[3].Data = noParticipantsInAcc;
        return incomeCurrency_Income_jointOrSoleAcc_NoParticipantsInAcc;
    }
    public string LettersOnly_NoSpaces(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        StringBuilder result = new StringBuilder(input.Length);

        foreach (char c in input)
        {
            if ((c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z'))
            {
                result.Append(c);
            }
        }

        return result.ToString();
    }
    private readonly string[] ValidIDTypesOtherIdAndOtherIDType = { "STAFF", "STUD", "SERV", "NHIS" };
    public CellDataAndStatus[] OtherIdAndOtherIDType(string otherIDNum, string otherIDType)
    {
        CellDataAndStatus[] otherIdAndOtherIDType = [new CellDataAndStatus(otherIDType), new CellDataAndStatus(otherIDNum)];
        otherIDNum = otherIDNum.Replace(" ", "").ToUpper();
        otherIDType = otherIDType.Replace(" ", "").ToUpper();
        // Check if OtherIDType is valid and OtherIDNum is not empty
        if (ValidIDTypesOtherIdAndOtherIDType.Contains(otherIDType) && !string.IsNullOrEmpty(otherIDNum))
        {
            otherIdAndOtherIDType[0].Data = otherIDNum;
            otherIdAndOtherIDType[1].Data = otherIDType;
            return otherIdAndOtherIDType;
        }
        else
        {
            otherIdAndOtherIDType[0].Data = string.Empty;
            otherIdAndOtherIDType[1].Data = string.Empty;
            return otherIdAndOtherIDType;
        }
    }
    /// <summary>
    /// Valid = purely numeric, OR alphanumeric containing at least one digit
    /// (letters+digits mixed). Invalid = pure-alphabetic (letters only, no
    /// digits), empty, or containing any non-alphanumeric character
    /// (spaces, punctuation, symbols).
    /// </summary>
    public bool NumericOrAlphanumericWithDigit(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return false;
        }
        if (!Regex.IsMatch(data, @"^[A-Za-z0-9]+$"))
        {
            return false;
        }
        return data.Any(char.IsDigit);
    }
    /// <summary>
    /// Detects a Ghanaian phone number sitting in a cell that should hold an
    /// address (or similar free-text field), and blanks it. Skips any cell
    /// containing a letter -- those are treated as real address text, not a
    /// misplaced phone number, even if they also contain digits. For
    /// letter-free cells, non-digit formatting (spaces, dashes, '+',
    /// parentheses) is stripped before checking the digit string against:
    /// starts with "233" and length 12; starts with "0" and length 10;
    /// starts with "2", "3" or "5" and length 9. Non-matching letter-free
    /// cells (e.g. a plain house number or postal code) are left as-is.
    /// </summary>
    public string BlankIfPhoneNumber(string data)
    {
        if (string.IsNullOrWhiteSpace(data))
        {
            return data;
        }
        if (Regex.IsMatch(data, @"[A-Za-z]"))
        {
            return data;
        }
        string digitsOnly = Regex.Replace(data, @"[^0-9]", "");
        bool isPhoneNumber =
            (digitsOnly.StartsWith("233") && digitsOnly.Length == 12) ||
            (digitsOnly.StartsWith("0") && digitsOnly.Length == 10) ||
            ((digitsOnly.StartsWith("2") || digitsOnly.StartsWith("3") || digitsOnly.StartsWith("5")) && digitsOnly.Length == 9);

        return isPhoneNumber ? string.Empty : data;
    }
    /// <summary>
    /// Email must contain "@" and a real dot-suffix (e.g. "name@x.com",
    /// "name@x.co.uk"). Narrow exception: "name@gmail" or "name@yahoo" with
    /// no suffix at all gets ".com" appended, since those two providers are
    /// safe to default (Gmail has no other TLD; Yahoo's global default is
    /// .com) -- every other provider submitted without a suffix is blanked
    /// rather than guessed, since guessing the wrong one (e.g. one of
    /// Yahoo's or other providers' country-specific TLDs) would write an
    /// incorrect email into the record.
    /// </summary>
    public CellDataAndStatus CleanEmailAddress(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = RemoveSystemErroNames(data);
        if (string.IsNullOrWhiteSpace(data))
        {
            cellData.Data = string.Empty;
            return cellData;
        }
        data = Regex.Replace(data, @"^https?://", "", RegexOptions.IgnoreCase);
        data = data.TrimEnd('/', '\\').Trim();

        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        if (Regex.IsMatch(data, emailPattern))
        {
            cellData.Data = data.ToLower();
            return cellData;
        }

        string gmailYahooNoSuffixPattern = @"^[^@\s]+@(gmail|yahoo)$";
        if (Regex.IsMatch(data, gmailYahooNoSuffixPattern, RegexOptions.IgnoreCase))
        {
            cellData.Data = (data + ".com").ToLower();
            return cellData;
        }

        cellData.Data = string.Empty;
        return cellData;
    }
    /// <summary>
    /// Currency-without-amount gets the currency blanked; amount-without-
    /// currency is left as-is. Mirrors the pairing rule already applied to
    /// IncomeCurrency/Income.
    /// </summary>
    private readonly string[] ValidTurnoverCurrency = { "GHS", "USD", "EUR", "GBP", "NGN" };
    public CellDataAndStatus[] TurnoverCurrency_TurnoverAmount(string turnoverCurrency, string turnoverAmount)
    {
        CellDataAndStatus[] result = [new CellDataAndStatus(turnoverCurrency), new CellDataAndStatus(turnoverAmount)];

        turnoverCurrency = turnoverCurrency.ToUpper().Trim().Replace(" ", "");
        if (!ValidTurnoverCurrency.Contains(turnoverCurrency))
        {
            turnoverCurrency = string.Empty;
        }

        if (!string.IsNullOrWhiteSpace(turnoverCurrency) && string.IsNullOrWhiteSpace(turnoverAmount))
        {
            turnoverCurrency = string.Empty;
        }

        result[0].Data = turnoverCurrency;
        result[1].Data = turnoverAmount;
        return result;
    }
    public CellDataAndStatus[] ProofOfAddType_ProofOfAddNum(string proofOfAddType, string proofOfAddNum)
    {
        // ProofOfAddType is always blanked. ProofOfAddNum is kept only if
        // numeric-only or alphanumeric with at least one digit -- pure
        // letters or anything containing symbols/spaces is blanked.
        proofOfAddNum = proofOfAddNum.Replace(" ", "").ToUpper().Trim();

        CellDataAndStatus[] proofOfAddType_ProofOfAddNum = [new CellDataAndStatus(proofOfAddType), new CellDataAndStatus(proofOfAddNum)];
        proofOfAddType_ProofOfAddNum[0].Passed = true;
        proofOfAddType_ProofOfAddNum[1].Passed = true;

        proofOfAddType_ProofOfAddNum[0].Data = string.Empty;
        proofOfAddType_ProofOfAddNum[1].Data = NumericOrAlphanumericWithDigit(proofOfAddNum) ? proofOfAddNum : string.Empty;
        return proofOfAddType_ProofOfAddNum;
    }
    Dictionary<string, string> ValidEmpTypeNormarlise = new Dictionary<string, string>
        {
           { "101", "SALARIEDINDIVIDUAL" },
           { "102", "UNEMPLOYED" },
           { "103", "STUDENT" },
           { "104", "SELFEMPLOYED" },
           { "105", "HOMEMAKER" },
           { "106", "PENSIONER" }

        };
    public CellDataAndStatus EmpTypeNormarlise(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace(" ", "").ToUpper();
        if (data == "101" || data == "SALARIED" || data == "INDIVIDUAL" || data == "SALARY" || data == "SALARIEDINDIVIDUAL")
        {
            data = "101";
            cellData.Data = data;
            return cellData;
        }
        if (data == "102" || data == "UNEMPLOYED")
        {
            data = "102";
            cellData.Data = data;
            return cellData;
        }
        if (data == "103" || data == "STUDENT")
        {
            data = "103";
            cellData.Data = data;
            return cellData;
        }
        if (data == "104" || data == "SELFEMPLOYED")
        {
            data = "104";
            cellData.Data = data;
            return cellData;
        }
        if (data == "105" || data == "HOMEMAKER")
        {
            data = "105";
            cellData.Data = data;
            return cellData;
        }
        if (data == "106" || data == "PENSIONER")
        {
            data = "106";
            cellData.Data = data;
            return cellData;
        }
        cellData.Data = string.Empty;
        return cellData;
    }
    public string GetFileNamefromStringHelper()
    {
        return FILENAME;
    }
    public string NewCleanTextNames(string input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        StringBuilder result = new StringBuilder();
        StringBuilder wordBuffer = new StringBuilder();
        bool lastWasSpace = false;

        foreach (char c in input)
        {
            if (char.IsWhiteSpace(c))
            {
                if (wordBuffer.Length > 0)
                {
                    string word = wordBuffer.ToString();
                    if (!WordsToRemove.Contains(word))
                    {
                        if (result.Length > 0) result.Append(' ');
                        result.Append(word);
                    }
                    wordBuffer.Clear();
                }

                if (!lastWasSpace)
                {
                    result.Append(' ');
                    lastWasSpace = true;
                }
            }
            else if (SpecialCharsNames.Contains(c))
            {
                continue;
            }
            else
            {
                wordBuffer.Append(c);
                lastWasSpace = false;
            }
        }

        // Add the last word if needed
        if (wordBuffer.Length > 0)
        {
            string word = wordBuffer.ToString();
            if (!WordsToRemove.Contains(word))
            {
                if (result.Length > 0) result.Append(' ');
                result.Append(word);
            }
        }
        return ReplaceWordApostrophes(result).ToString().ToUpper();
    }
    private readonly HashSet<char> SpecialCharsNames = new HashSet<char>
    {
        '?', '=', '*', '[', ']', '(', ')' ,'!', '$', '%', '^', '&', '+',';'
    };
    public (string LastDate, bool IsValid) GetFacilityDateFromFileName(string fileName, bool isCompare_disBurDateWithFiledate = false)
    {
        fileName = GetExcelFileDate(fileName);
        FILENAME = fileName;
        // Attempt to parse the extracted date part.
        if (DateTime.TryParseExact(fileName, "MMyy", CultureInfo.InvariantCulture,
                                   DateTimeStyles.None, out DateTime parsedDate))
        {
            // Compute the last day of the month.
            int lastDay = DateTime.DaysInMonth(parsedDate.Year, parsedDate.Month);
            DateTime fullDate = new DateTime(parsedDate.Year, parsedDate.Month, lastDay);
            if (isCompare_disBurDateWithFiledate)
            {
                fullDate = fullDate.AddDays(15);
            }
           

            // Format the date as yyyyMMdd
            string formattedDate = fullDate.ToString("yyyyMMdd");

            return (formattedDate, true);
        }

        // The extracted segment does not represent a valid date.
        return (null, false);
    }
    public (string LastDate, bool IsValid) GetFacilityDateFromFileNameNew(string fileName, bool isCompare_disBurDateWithFiledate = false)
    {
        fileName = GetExcelFileDate(fileName);
        FILENAME = fileName;
        // Attempt to parse the extracted date part.
        if (DateTime.TryParseExact(fileName, "MMyy", CultureInfo.InvariantCulture,
                                   DateTimeStyles.None, out DateTime parsedDate))
        {
            // Compute the last day of the month.
            int lastDay = DateTime.DaysInMonth(parsedDate.Year, parsedDate.Month);
            DateTime fullDate = new DateTime(parsedDate.Year, parsedDate.Month, lastDay);
            if (isCompare_disBurDateWithFiledate)
            {
                fullDate = fullDate.AddDays(15);
            }


            // Format the date as yyyyMMdd
            string formattedDate = fullDate.ToString("yyyyMMdd");

            return (formattedDate, true);
        }

        // The extracted segment does not represent a valid date.
        return (null, false);
    }

    public string GetExcelFileDate(string filename)
    {
        if (string.IsNullOrEmpty(filename))
            return string.Empty;
        string _file = filename.Split("_")[0];
        if (_file.Length <= 4)
        {
           return string.Empty; // Return the entire string if it's 4 characters or less
        }

        // Use Substring to get the last 4 characters
        return _file.Substring(_file.Length - 4);

        //// Remove the file extension
        //string nameWithoutExtension = Path.GetFileNameWithoutExtension(filename);

        //// Extract the 4 characters starting from the 7th character from the end
        //int lengthToExtract = 4;
        //if (nameWithoutExtension.Length >= lengthToExtract + 4) // Ensure enough characters exist
        //{
        //    return nameWithoutExtension.Substring(nameWithoutExtension.Length - 10, lengthToExtract);
        //}

        //return string.Empty; // Return empty if the filename is too short
    }
    public (bool IsValid, string LettersPart, string NumbersPart, string fileDataType) ValidateFileName(string fileName)
    {
        bool isValid = false;
        string lettersPart = string.Empty;
        string numbersPart = string.Empty;
        string fileDataTypePart = string.Empty;


        if (string.IsNullOrEmpty(fileName))
        {
            return (isValid, lettersPart, numbersPart, fileDataTypePart);
        }

        int i = 0;
        fileName = Path.GetFileNameWithoutExtension(fileName);
        string[] fileKind = fileName.Split('_');
        fileName = fileKind.FirstOrDefault();
        if (fileKind.Count() == 2)
        {
            fileDataTypePart = fileKind[1];
        }
        

        // Validate the first part (letters only)
        while (i < fileName.Length && Char.IsLetter(fileName[i]))
        {
            lettersPart += fileName[i];
            i++;
        }

        // Validate that the next part is exactly 4 digits
        if (i >= fileName.Length || !Char.IsDigit(fileName[i]))
        {
            return (isValid, lettersPart, numbersPart, fileDataTypePart);
        }

        // Capture the 4 digits part
        while (i < fileName.Length && Char.IsDigit(fileName[i]) && numbersPart.Length < 4)
        {
            numbersPart += fileName[i];
            i++;
        }

        // Ensure there are exactly 4 digits
        if (numbersPart.Length != 4)
        {
            return (isValid, lettersPart, numbersPart, fileDataTypePart);
        }

        // Ensure we've consumed the entire string (no extra characters)
        if (i == fileName.Length && fileKind.Count() == 2 && fileDataTypePart.Length == 3)
        {
            isValid = true;
        }

        return (isValid, lettersPart, numbersPart, fileDataTypePart);
    }
    

    private const string DateFormat = "yyyyMMdd";
    private static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;

    // Helper method to format the date if the comparison condition holds.
    private static string FormatIf(bool condition, DateTime dt) =>
        condition ? dt.ToString(DateFormat) : string.Empty;

    // Helper method for safely parsing a string to DateTime using the exact format.
    private static bool TryParseDate(string s, out DateTime dt) =>
        DateTime.TryParseExact(s, DateFormat, InvariantCulture, DateTimeStyles.None, out dt);

    // CompareGreater implementations
    public string DateCompareGreater(DateTime dt1, DateTime dt2) =>
        FormatIf(dt1.Date > dt2.Date, dt1);

    public string DateCompareGreater(string s1, string s2)
    {
        if (TryParseDate(s1, out DateTime dt1) && TryParseDate(s2, out DateTime dt2))
            return DateCompareGreater(dt1, dt2);
        return string.Empty;
    }

    public string DateCompareGreater(DateTime dt, string s)
    {
        if (TryParseDate(s, out DateTime dt2))
            return DateCompareGreater(dt, dt2);
        return string.Empty;
    }

    public string DateCompareGreater(string s, DateTime dt)
    {
        if (TryParseDate(s, out DateTime dt1))
            return DateCompareGreater(dt1, dt);
        return string.Empty;
    }

    // CompareLess implementations
    public string DateCompareLess(DateTime dt1, DateTime dt2) =>
        FormatIf(dt1.Date < dt2.Date, dt1);

    public string DateCompareLess(string s1, string s2)
    {
        if (TryParseDate(s1, out DateTime dt1) && TryParseDate(s2, out DateTime dt2))
            return DateCompareLess(dt1, dt2);
        return string.Empty;
    }

    public string DateCompareLess(DateTime dt, string s)
    {
        if (TryParseDate(s, out DateTime dt2))
            return DateCompareLess(dt, dt2);
        return string.Empty;
    }

    public string DateCompareLess(string s, DateTime dt)
    {
        if (TryParseDate(s, out DateTime dt1))
            return DateCompareLess(dt1, dt);
        return string.Empty;
    }

    // CompareEqual implementations
    public string DateCompareEqual(DateTime dt1, DateTime dt2) =>
        FormatIf(dt1.Date == dt2.Date, dt1);
    public string DateCompareEqual(string s1, string s2)
    {
        if (TryParseDate(s1, out DateTime dt1) && TryParseDate(s2, out DateTime dt2))
            return DateCompareEqual(dt1, dt2);
        return string.Empty;
    }
   public string DateCompareEqual(DateTime dt, string s)
    {
        if (TryParseDate(s, out DateTime dt2))
            return DateCompareEqual(dt, dt2);
        return string.Empty;
    }
   public string DateCompareEqual(string s, DateTime dt)
    {
        if (TryParseDate(s, out DateTime dt1))
            return DateCompareEqual(dt1, dt);
        return string.Empty;
    }
   public string ClosureReasonConverter(string data)
    {
        Dictionary<string, string> catalogue = new Dictionary<string, string>
        {
            { "A", "By Credit Grantor without prejudice to the Subject" },
            { "B", "Balance Transfer" },
            { "C", "Death" },
            { "D", "End of Credit Facility Tenure" },
            { "E", "Merger of Credit Facility" },
            { "F", "Early Settlement by Subject" },
            { "G", "By Court Order" },
            { "H", "Lost Cards/Compromised Cards" },
            { "J", "Bankruptcy" },
            { "K", "Restructured/Rescheduled" }
        };

        var matchingCodes = catalogue
            .Where(kvp => kvp.Value.IndexOf(data, StringComparison.OrdinalIgnoreCase) >= 0)
            .Select(kvp => kvp.Key)
            .ToList();

        if (matchingCodes.Any())
        {
            foreach (var code in matchingCodes)
            {
                data = code;
            }
        }
        return string.Empty;
    }

















    //-------------------------------------------
    //  BUSINESS START
    //-------------------------------------------
    public bool AreAllCharactersSame(string str)
    {
        if (string.IsNullOrEmpty(str)) // Handle null or empty strings
            return true;

        char firstChar = str[0];

        for (int i = 1; i < str.Length; i++)
        {
            if (str[i] != firstChar)
            {
                return false;
            }
        }

        return true;
    }
    public string SpecialCharacterToExclude(string input)
    {
        // - . (period)
        return Regex.Replace(input, @"[^a-zA-Z0-9,.]", " ");
    }
    public CellDataAndStatus[] Busregnum_Tinum(string busregnum, string tinum) 
    {
       
        CellDataAndStatus[] busregnum_tinum = [ new CellDataAndStatus(busregnum), new CellDataAndStatus(tinum)];
        //if (busregnum.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        //{
        //    busregnum = string.Empty;
        //}
        //if (tinum.Any(c => !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c)))
        //{
        //    tinum = string.Empty;
        //}


        //busregnum = SpecialCharacterToExclude(busregnum);
        //tinum = SpecialCharacterToExclude(tinum);


        busregnum = RemoveHypens(busregnum);
        tinum = RemoveHypens(tinum);


        busregnum = busregnum.ToUpper().Trim().Replace(",", "");
        tinum = tinum.ToUpper().Trim().Replace(" ", "").Replace(",", "");

        busregnum = RemoveSystemErroNames(busregnum);
        tinum = RemoveSystemErroNames(tinum);

       // var checkDate = CheckDate(disbur_inputDate);

        if (busregnum.Length <= 4 || AreAllCharactersSame(busregnum))
            busregnum = string.Empty;
        if (tinum.Length <= 5 || AreAllCharactersSame(tinum))
            tinum = string.Empty;




       

        if (!string.IsNullOrWhiteSpace(busregnum) || !string.IsNullOrWhiteSpace(tinum))
        {
            busregnum_tinum[0].Data = busregnum;
            busregnum_tinum[1].Data = tinum;
            busregnum_tinum[0].Passed = true;
            busregnum_tinum[1].Passed = true;
            return busregnum_tinum;
        }
        busregnum_tinum[0].Passed = false;
        busregnum_tinum[1].Passed = false;
        busregnum_tinum[0].Errors = new List<string>() { "Empty or Invalid Busregnum or Tinum" };
        return busregnum_tinum;
    }
    public string SectorindcodeCatalogueCode(string input)
    {
        input = input.Replace(" ", "").Trim().Replace("&", "").Replace(",", "").Replace("/", "").ToUpper();
        if (input == "10" || input == "AGRICULTUREFORESTRYFISHING" || input == "AGRICULTUREFORESTRY" || input == "AGRICULTURE" || input == "FORESTRY" || input == "FISHING") { return "10"; }
        else if (input == "20" || input == "MININGQUARRYING" || input == "QUARRYING" || input == "QUARRYING") { return "20"; }
        else if (input == "30" || input == "MANUFACTURING") { return "30"; }
        else if (input == "40" || input == "CONSTRUCTION") { return "40"; }
        else if (input == "50" || input == "ELECTRICITYGASWATER" || input == "ELECTRICITY" || input == "WATER" || input == "GAS") { return "50"; }
        else if (input == "60" || input == "COMMERCEFINANCE" || input == "FINANCE" || input == "COMMERCE") { return "60"; }
        else if (input == "70" || input == "TRANSPORTSTORAGEANDCOMMUNICATION" || input == "TRANSPORTS" || input == "STORAGE" || input == "COMMUNICATION" || input == "STORAGEANDCOMMUNICATION" || input == "STORAGECOMMUNICATION") { return "70"; }
        else if (input == "80" || input == "SERVICES") { return "80"; }
        else if (input == "90" || input == "BANKING") { return "90"; }
        else if (input == "100" || input == "INSURANCE") { return "100"; }
        else if (input == "110" || input == "SECURITIESANDEXCHANGE") { return "110"; }
        else if (input == "120" || input == "PENSIONS") { return "120"; }
        else { return string.Empty; }
    }
    public string SubsecindcodeCatalogueCode(string input)
    {
        // Normalize the input by removing spaces, ampersands, commas, slashes, and converting to uppercase
        input = input.Replace(" ", "").Trim().Replace("&", "").Replace(",", "").Replace("/", "").ToUpper();

        // Agriculture and Forestry related
        if (input == "101" || input == "COCOAPRODUCTION") { return "101"; }
        else if (input == "102" || input == "LIVESTOCKBREEDING") { return "102"; }
        else if (input == "103" || input == "POULTRYFARMING") { return "103"; }
        else if (input == "104" || input == "OTHERAGRICULTURE") { return "104"; }
        else if (input == "105" || input == "FORESTRY") { return "105"; }
        else if (input == "106" || input == "LOGGING") { return "106"; }
        else if (input == "107" || input == "FISHING") { return "107"; }

        // Mining related
        else if (input == "201" || input == "BAUXITE") { return "201"; }
        else if (input == "202" || input == "DIAMONDS") { return "202"; }
        else if (input == "203" || input == "GOLD") { return "203"; }
        else if (input == "204" || input == "MANGANESE") { return "204"; }
        else if (input == "205" || input == "QUARRYING") { return "205"; }
        else if (input == "206" || input == "OTHERMININGACTIVITY") { return "206"; }

        // Manufacturing related
        else if (input == "301" || input == "FOODDRINKTOBACCO" || input == "FOODDRINKANDTOBACCO") { return "301"; }
        else if (input == "302" || input == "TEXTILESCLOTHINGFOOTWEAR" || input == "TEXTILESCLOTHINGANDFOOTWEAR") { return "302"; }
        else if (input == "303" || input == "SAWMILLINGWOODPROCESSING" || input == "SAWMILLINGANDWOODPROCESSING") { return "303"; }
        else if (input == "304" || input == "PAPERPULPAPER" || input == "PAPERPULPANDPAPER") { return "304"; }
        else if (input == "305" || input == "CHEMICALSANDFERTILIZERS" || input == "CHEMICALSANDFERTILISERS") { return "305"; }
        else if (input == "306" || input == "IRONANDSTEEL") { return "306"; }
        else if (input == "307" || input == "BOATSHIPBUILDINGANDREPAIRS" || input == "BOAT/SHIPBUILDINGANDREPAIRS") { return "307"; }
        else if (input == "308" || input == "MANUFACTURINGOFMOTORVEHICLES" || input == "MOTORVEHICLEMANUFACTURING") { return "308"; }
        else if (input == "309" || input == "OTHERUNCLASSIFIED") { return "309"; }

        // Construction related
        else if (input == "401" || input == "CONSTRUCTIONWORKS" || input == "CONSTRUCTION&WORKS" || input == "CONSTRUCTIONANDWORKS") { return "401"; }
        else if (input == "402" || input == "BUILDINGCONSTRUCTION") { return "402"; }

        // Utilities / Power
        else if (input == "501" || input == "ELECTRICLIGHTPOWER" || input == "ELECTRICLIGHT&POWER") { return "501"; }
        else if (input == "502" || input == "GASMANUFACTUREDISTRIBUTION" || input == "GASMANUFACTURE& DISTRIBUTION" || input == "GASMANUFACTUREANDDISTRIBUTION") { return "502"; }
        else if (input == "503" || input == "WATERSUPPLY") { return "503"; }

        // Imports / Exports related
        else if (input == "601" || input == "MOTORVEHICLEIMPORT") { return "601"; }
        else if (input == "602" || input == "MACHINERYHEAVYE" || input == "MACHINERYANDHEAVYE") { return "602"; }
        else if (input == "603" || input == "OTHERIMPORTITEMS") { return "603"; }
        else if (input == "604" || input == "COCOAEXPORTS") { return "604"; }
        else if (input == "605" || input == "TIMBEREXPORT") { return "605"; }
        else if (input == "606" || input == "OTHEREXPORTITEMS" || input == "OTHEEXPORTITEMS") { return "606"; }

        // Financial/Corporate related
        else if (input == "607" || input == "HIREPURCHASECOMPANY") { return "607"; }
        else if (input == "608" || input == "INSURANCECOMPANY") { return "608"; }
        else if (input == "609" || input == "BUILDINGBODIESANDCORPORATIONS") { return "609"; }

        // Transport related
        else if (input == "701" || input == "RAILWAYTRANSPORT") { return "701"; }
        else if (input == "702" || input == "ROADTRANSPORT") { return "702"; }
        else if (input == "703" || input == "OCEANANDOTHERWATER" || input == "OCEANANDOTHERVATER") { return "703"; }
        else if (input == "704" || input == "AIRTRANSPORT") { return "704"; }
        else if (input == "705" || input == "STORAGEANDWAREHOUSING" || input == "WAREHOUSING") { return "705"; }
        else if (input == "706" || input == "COMMUNICATIONS") { return "706"; }

        // Services related
        else if (input == "801" || input == "PRINTINGPUBLISHINGANDALLIED" || input == "PRINTINGANDPUBLISHING") { return "801"; }
        else if (input == "802" || input == "BUSINESSSERVICES") { return "802"; }
        else if (input == "803" || input == "RECREATIONSERVICES") { return "803"; }
        else if (input == "804" || input == "PERSONALSERVICES") { return "804"; }
        else if (input == "805" || input == "SALARYCREDIT") { return "805"; }
        else if (input == "806" || input == "OTHERSERVICESINCLUDINGGOVERNMENT") { return "806"; }

        // Banking and Finance related
        else if (input == "901" || input == "BANK") { return "901"; }
        else if (input == "902" || input == "MFI") { return "902"; }
        else if (input == "903" || input == "SAVINGSANDLOANS") { return "903"; }
        else if (input == "904" || input == "RURALANDCOMMUNITYBANKS") { return "904"; }
        else if (input == "905" || input == "FINANCEHOUSE") { return "905"; }
        else if (input == "906" || input == "FINANCIALHOLDINGCOMPANIES") { return "906"; }
        else if (input == "907" || input == "MORTGAGEFINANCINGCOMPANIES") { return "907"; }

        // Insurance related
        else if (input == "1001" || input == "LIFECOMPANIES") { return "1001"; }
        else if (input == "1002" || input == "NONLIFECOMPANIES") { return "1002"; }
        else if (input == "1003" || input == "REINSURANCECOMPANIES") { return "1003"; }

        // Securities / Investment related
        else if (input == "1101" || input == "SECURITIESEXCHANGES" || input == "SECURITIESEXCHANGE") { return "1101"; }
        else if (input == "1102" || input == "CUSTODIANS") { return "1102"; }
        else if (input == "1103" || input == "DEPOSITORIES") { return "1103"; }
        else if (input == "1104" || input == "PRIVATEFUNDS") { return "1104"; }
        else if (input == "1105" || input == "REGISTRARS") { return "1105"; }
        else if (input == "1106" || input == "FUNDMANAGERS") { return "1106"; }
        else if (input == "1107" || input == "INVESTMENTADVISORS") { return "1107"; }
        else if (input == "1108" || input == "TRUSTEES") { return "1108"; }
        else if (input == "1109" || input == "MUTUALFUNDS") { return "1109"; }
        else if (input == "1110" || input == "BROKERDEALER") { return "1110"; }
        else if (input == "1111" || input == "ISSUINGHOUSE") { return "1111"; }
        else if (input == "1112" || input == "PRIMARYDEALERS") { return "1112"; }
        else if (input == "1113" || input == "UNITTRUST") { return "1113"; }

        // Pensions related
        else if (input == "1201" || input == "PENSIONFUNDCUSTODIANS") { return "1201"; }
        else if (input == "1202" || input == "PENSIONSFUNDMANAGERS" || input == "PENSIONSFUNDMANAGERS") { return "1202"; }
        else
        {
            return string.Empty;
        }
    }
    public string[] Sectorindcode_Subsecindcode(string _sectorindcode, string _subsecindcode)
    {
        _sectorindcode = SectorindcodeCatalogueCode(_sectorindcode);
        _subsecindcode = SubsecindcodeCatalogueCode(_subsecindcode);

        string[] ListSubsecindcode = { "101", "102", "103", "104", "105", "106", "107", "201", "202", "203", "204",
            "205", "206", "301", "302", "303", "304", "305", "306", "307", "308", "309", "401", "402", "501", "502",
            "503", "601", "602", "603", "604", "605", "606", "607", "608", "609", "701", "702", "703", "704", "705",
            "706", "801", "802", "803", "804", "805", "806", "901", "902", "903", "904", "905", "906", "1001", "1002",
            "1003", "1101", "1102", "1103", "1104", "1105", "1106", "1107", "1108", "1109", "1110", "1111", "1112", "1113", "1201", "1202" };
        string[] _data = ["", ""];


       var response = ListSubsecindcode.Contains(_subsecindcode);
        if (response)
        {
            switch (_sectorindcode.Length)
            {
                case 2:
                    if (_subsecindcode.Substring(0,2) !=_sectorindcode)
                    {
                        _subsecindcode = string.Empty;
                    }
                    return [_sectorindcode, _subsecindcode];
                case 3:
                    if (_subsecindcode.Substring(0, 2) == _sectorindcode)
                    {
                        _subsecindcode = string.Empty;
                    }
                    return [_sectorindcode, _subsecindcode];
            }
        }
        return [_sectorindcode, _subsecindcode];
    }

    public (bool thisIsAGhanaCard, bool hasSixUniqueCharactersCharacters) IsGhanacardAndNotRepeatingCharcters(string input)
    {
        string cleaned = Regex.Replace(input, @"[ \/,_-]", "");

        bool thisIsAGhanaCard = cleaned.Length == 13 && cleaned.Substring(0, 3).All(char.IsLetter) && cleaned.Substring(3).All(char.IsDigit);

        bool hasSixRepeats = false;
        int repeatCount = 1;
        for (int i = 1; i < input.Length; i++)
        {
            if (input[i] == input[i - 1])
            {
                repeatCount++;
                if (repeatCount >= 6)
                {
                    hasSixRepeats = true;
                }
            }
            else
            {
                repeatCount = 1;
            }
           
        }
        return (thisIsAGhanaCard, hasSixRepeats);
    }



    public static bool IsValidEmailUsingAttribute(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return new EmailAddressAttribute().IsValid(email);
    }

    // Method 2: Using Regular Expression
    //public bool IsValidEmailUsingRegex(string email)
    //{
    //    if (string.IsNullOrWhiteSpace(email))
    //        return false;

    //    // Regular expression pattern for basic email validation
    //    string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

    //    try
    //    {
    //        return Regex.IsMatch(
    //            email,
    //            pattern,
    //            RegexOptions.IgnoreCase,
    //            TimeSpan.FromMilliseconds(250)
    //        );
    //    }
    //    catch (RegexMatchTimeoutException)
    //    {
    //        return false;
    //    }
    //}

    //-------------------------------------------
    //  BUSINESS END
    //-------------------------------------------

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var mailAddress = new MailAddress(email);
            return mailAddress.Address == email; // Ensures no extra formatting
        }
        catch (FormatException)
        {
            return false;
        }
    }
}






public class IDReallocator
{
    // Class to hold the final reallocated IDs.
    public class IDResult
    {
        public string NationalID { get; set; } = "";
        public string VoterID { get; set; } = "";
        public string DriversLicense { get; set; } = "";
        public string Passport { get; set; } = "";
        public string SSNIT { get; set; } = "";
        public string OtherIDs { get; set; } = "";
        public List<string> Errors { get; set; } = new List<string>();

        public override string ToString()
        {
            return $"NationalID: {NationalID}\n" +
                   $"VoterID: {VoterID}\n" +
                   $"DriversLicense: {DriversLicense}\n" +
                   $"Passport: {Passport}\n" +
                   $"SSNIT: {SSNIT}\n" +
                   $"OtherIDs: {OtherIDs}\n" +
                   $"Errors: {string.Join("; ", Errors)}";
        }
    }

    /// <summary>
    /// Validates that a candidate NationalID is exactly 13 characters,
    /// where the first three are letters and the remaining ten are digits.
    /// </summary>
    private bool IsValidNationalID(string id)
    {
        if (id.Length != 13)
            return false;
        // First three characters must be letters.
        for (int i = 0; i < 3; i++)
        {
            if (!char.IsLetter(id[i]))
                return false;
        }
        // Remaining ten characters must be digits.
        for (int i = 3; i < 13; i++)
        {
            if (!char.IsDigit(id[i]))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Reallocates IDs into their proper fields based on the following rules:
    /// - NationalID: 13 characters, first three letters and the rest digits.
    /// - VoterID: exactly 10 characters.
    /// - Driver's License: allowed lengths are 6, 7, 8, or 14 characters.
    /// - Passport: allowed lengths are 6, 7, or 8 characters.
    /// - SSNIT: allowed lengths are 8 or 13 characters.
    /// Any IDs that do not fit any primary column are concatenated into OtherIDs.
    /// If a field is not assigned a valid ID, it will remain as an empty string.
    /// </summary>
    public IDResult ReallocateIDs(params string[] ids)
    {
        IDResult result = new IDResult();

        // Check for null or empty input.
        if (ids == null || ids.Length == 0)
        {
            result.Errors.Add(" NO IDS PROVIDED ");
            return result;
        }

        // Gather all non-empty IDs (trimmed) into a list.
        List<string> unassigned = new List<string>();
        foreach (var id in ids)
        {
            if (!string.IsNullOrWhiteSpace(id))
                unassigned.Add(id.Trim());
        }

        // Helper function to assign an ID to a field.
        // For NationalID, we also log an error if a 13-character candidate fails the format.
        string AssignField(Func<string, bool> predicate, string fieldName)
        {
            for (int i = 0; i < unassigned.Count; i++)
            {
                string candidate = unassigned[i];

                if (fieldName == "NationalID")
                {
                    // For NationalID, if the candidate is 13 characters but not valid, log the error and skip.
                    if (candidate.Length == 13 && !IsValidNationalID(candidate))
                    {
                        result.Errors.Add($"{fieldName} MUST HAVE FIRST THREE LETTERS AND THEN TEN DIGITS: {candidate}");
                        continue;
                    }
                }

                if (predicate(candidate))
                {
                    unassigned.RemoveAt(i);
                    return candidate;
                }
            }
            return null;
        }

        // Process fields in the specified order.
        result.NationalID = AssignField(id => id.Length == 13 && IsValidNationalID(id), "NationalID") ?? "";
        result.VoterID = AssignField(id => id.Length == 10, "VoterID") ?? "";
        result.DriversLicense = AssignField(id => new[] { 6, 7, 8, 14 }.Contains(id.Length), "Driver's License") ?? "";
        result.Passport = AssignField(id => new[] { 6, 7, 8 }.Contains(id.Length), "Passport") ?? "";
        result.SSNIT = AssignField(id => (id.Length == 8 || id.Length == 13), "SSNIT") ?? "";

        // If none of the primary fields is valid, log an error.
        if (string.IsNullOrEmpty(result.NationalID) &&
            string.IsNullOrEmpty(result.VoterID) &&
            string.IsNullOrEmpty(result.DriversLicense) &&
            string.IsNullOrEmpty(result.Passport) &&
            string.IsNullOrEmpty(result.SSNIT))
        {
            result.Errors.Add(" NONE OF THE PRIMARY IDS ARE VALID. PLEASE PROVIDE IDS ");
        }

        // Concatenate any remaining IDs into the OtherIDs field.
        result.OtherIDs = string.Join(", ", unassigned);

        return result;
    }

  
}
public class CurrencyValidationResult
{
    public bool IsEmpty { get; set; }
    public bool IsValid { get; set; }
    public bool IsGreaterThanZero { get; set; }
    public bool IsZero { get; set; } // New property to check if the value is zero
    public decimal? ValidData { get; set; }
}