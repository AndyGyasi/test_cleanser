using CleanserBlazorUI.Data;

namespace CleanserBlazorUI.Converters;
public class BusinessDataTransformer
{
    StringHelper stringHelper = new StringHelper();
    public string UNL_Busregnum(string data)
    {
        return data;
    }
    public string UNL_Tinum(string data)
    {
        return data;
    }
    public string UNL_NDIA_OldData(string data)
    {
        return data;
    }
    public string UNL_writtenoffamt(string data)
    {
        return data;
    }
    public string UNL_ssnum(string data)
    {
        return data;
    }
    public string UNL_otheridtype(string data)
    {
        return data;
    }
    public string UNL_otheridnum(string data)
    {
        return data;
    }
    public string UNL_proofofaddtype(string data)
    {
        return data;
    }
    public string UNL_proofofaddnum(string data)
    {
        return data;
    }
    public string Data(string data)
    {
        return "D";
    }
    public string CorrectionIndicator(string data)
    {
        if (data != "0" && data != "1" && data != "2")
        {
            return "0";
        }
        return data;
    }
    public CellDataAndStatus Facilityaccnum(string data)
    {
        var cellData = new CellDataAndStatus(data);
   data = data.TrimStart().TrimEnd();

        if (data.Length > 0 || stringHelper.AccountCustomerInValidCharacter(data))
        {
            if (data.Contains("+") || stringHelper.HasSixConsecutiveZerosAtEnd(data))
            {
                cellData.Data = data;
                cellData.Passed = false;
                cellData.Errors = new List<string>() { "Facilityaccnum: NOT VALID" };
                return cellData;
            }
            cellData.Data = data;
            cellData.Passed = true;
            return cellData;
        }
        else
        {
            cellData.Errors = new List<string>() { "Facilityaccnum: NOT VALID" };
            cellData.Data = data;
            cellData.Passed = false;
            return cellData;
        }
    }
    public CellDataAndStatus CustomerID(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = stringHelper.RemoveSystemErroNames(data);


        if (string.IsNullOrWhiteSpace(data))
        {
            cellData.Errors = new List<string>() { "CUSTOMERID: NOT VALID OR EMPTY" };
            cellData.Data = data;
            cellData.Passed = false;
            return cellData;
        }

        if (data.Length > 0 || !data.Contains("+"))
        {
            if (!data.Contains("+"))
            {
                cellData.Data = data;
                cellData.Passed = true;
                return cellData;
            }
            cellData.Data = data;
            cellData.Passed = false;
            return cellData;
        }
        else
        {
            cellData.Errors = new List<string>() { "CUSTOMERID: NOT VALID" };
            cellData.Data = data;
            cellData.Passed = false;
            return cellData;
        }
    }
    public CellDataAndStatus BranchCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data.ToUpper();
        return cellData;
    }
    public CellDataAndStatus Busregnum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("-", "").Replace("#", "").Replace("/", "");
        if (stringHelper.StartsWithFourXOrZero(data))
        {
            data = string.Empty;
        }
        cellData.Data = data.ToUpper();
        return cellData;
    }
    public CellDataAndStatus Prevregnum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = stringHelper.RemoveHypens(data);
        data = data.Replace("-", "").Replace("#", "").Replace("/", "");
        data = data.ToUpper().Trim().Replace(" ", "").Replace(",", "");
        cellData.Data = data.ToUpper();
        return cellData;
    }
    public CellDataAndStatus Tinum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("-", "").Replace("#", "").Replace("/", "");
        if (stringHelper.StartsWithFourXOrZero(data))
        {
            data = string.Empty;
        }
        cellData.Data = data.ToUpper();
        return cellData;
    }
    public CellDataAndStatus Subsecindcode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data.ToUpper();
        return cellData;
    }
    public CellDataAndStatus Sectorindcode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data.ToUpper();
        return cellData;
    }
    public CellDataAndStatus Bustype(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).ToUpper();
        data = data.ToUpper().Replace(" ", "").Trim();

        if (data == "A" || data == "SOLE" || data == "SOLEPROPRIETORSHIP" || data == "PROPRIETORSHIP")
        {
            data = "SOLE PROPRIETORSHIP";
        }
        else if (data == "B" || data == "LIMITEDPARTNERSHIP" || data == "LIMITED" || data == "PARTNERSHIP")
        {
            data = "LIMITED PARTNERSHIP";
        }
        else if (data == "C" || data == "COMPANYLIMITEDBYSHARES" || data == "SHARES")
        {
            data = "COMPANY LIMITED BY SHARES";
        }
        else if (data == "D" || data == "COMPANYLIMITEDBYGUARANTEE" || data == "GUARANTEE")
        {
            data = "COMPANY LIMITED BY GUARANTEE";
        }
        else if (data == "E" || data == "UNLIMITEDCOMPANY")
        {
            data = "UNLIMITED COMPANY";
        }
        else if (data == "F" || data == "COOPERATIVE")
        {
            data = "COOPERATIVE";
        }
        else if (data == "G" || data == "FOREIGN" || data == "FOREIGNCOMPANY" || data == "EXTERNALCOMPANY" || data == "EXTERNAL" || data == "FOREIGN/EXTERNALCOMPANY")
        {
            data = "FOREIGN / EXTERNAL COMPANY";
        }
        else if (data == "H" || data == "CONSULTANCY FIRMS" || data == "CONSULTANCY" || data == "PROFESSIONAL BODIES" || data == "PROFESSIONAL" || data == "CONSULTANCY FIRMS/PROFESSIONAL BODIES")
        {
            data = "CONSULTANCY FIRMS/PROFESSIONAL BODIES";
        }
        else if (data == "J" || data == "SOCIALORGANIZATION" || data == "SOCIAL")
        {
            data = "SOCIAL ORGANIZATION";
        }
        else if (data == "K" || data == "INTERNATIONAL" || data == "ORGANIZATIONS" || data == "INTERNATIONALORGANIZATIONS")
        {
            data = "INTERNATIONAL ORGANIZATIONS";
        }
        else if (data == "L" || data == "NGO")
        {
            data = "NGO";
        }
        else
        {
            data = string.Empty;
        }

        cellData.Data = data;
        return cellData ?? new CellDataAndStatus(data);
    }
    public CellDataAndStatus Registrationdate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        var b1 = date.TheDate.Year > DateTime.Now.Year;
        var b11 = date.IsFutureDate;
        var b2 = date.TheDate.Year < 1900;
        var b22 = date.TheDate.Year > 1900;
        if (date.IsValidFormat && date.TheDate.Year > 1900 && !date.IsFutureDate)
        {
            cellData.Data = data;
        }
        else
        {
            cellData.Data = string.Empty;
        }
        return cellData;
    }
    public CellDataAndStatus Commencementdate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        var b1 = date.TheDate.Year > DateTime.Now.Year;
        var b11 = date.IsFutureDate;
        var b2 = date.TheDate.Year < 1900;
        var b22 = date.TheDate.Year > 1900;
        if (date.IsValidFormat && date.TheDate.Year > 1900 && !date.IsFutureDate)
        {
            cellData.Data = data;
        }
        else
        {
            cellData.Data = string.Empty;
        }
        return cellData;
    }
    public CellDataAndStatus Businessname(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace(".", " ").Replace(",", " ");
        if (stringHelper.ContainsInvalidCharactersBusiness(data))
        {
            cellData.Passed = false;
            cellData.Errors = new List<string>() { "BUSINESSNAME: INVALID CHARACTERS" };
            cellData.Data = data;
            return cellData;
        }
        cellData.Data = stringHelper.CleanString(data); ;
        return cellData;
    }
    public CellDataAndStatus Tradingname(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace(".", " ").Replace(",", " ");
        if (stringHelper.ContainsInvalidCharactersBusiness(data))
        {
            cellData.Passed = false;
            cellData.Errors = new List<string>() { "Tradingname: INVALID CHARACTERS" };
            cellData.Data = data;
            return cellData;
        }
        cellData.Data = stringHelper.CleanString(data); ;
        return cellData;
    }
    public CellDataAndStatus Turnovercurrency(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).ToUpper(); // Single uppercase conversion [[2]][[3]]

        // Valid currencies based on ISO 4217 and Ghana's context [[5]][[8]][[10]]
        var validCurrencies = new HashSet<string>
    {
        "GHS",  // Ghanaian Cedi (primary) [[2]][[3]][[5]]
        "USD",  // Major international trade [[7]][[9]]
        "EUR",  // Eurozone transactions [[4]]
        "GBP",  // UK remittances [[5]]
        "NGN"   // Nigerian Naira for regional trade [[5]]
    };

        cellData.Data = validCurrencies.Contains(data) ? data : string.Empty;
        return cellData;
    }
    public CellDataAndStatus Turnoveramount(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Prevbusname(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus ProofOfAddType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus ProofOfAddNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Curlocadd1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus Curlocadd2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus Curlocadd3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus Curlocadd4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus Curlocaddrpostalcode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus PostAddrLine1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus PostAddrLine2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus PostAddrLine3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus PostAddrLine4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus PostalAddPostCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus Websiteadd(string data)
    {
        var celldata = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (string.IsNullOrWhiteSpace(data))
        {
            celldata.Data = string.Empty;
            return celldata;
        }
        data = data.Trim();
        data = Regex.Replace(data, @"^https?:\/\/", "", RegexOptions.IgnoreCase);
        string urlPattern = @"^(https?:\/\/)?([\w\-]+(\.[\w\-]+)+)([\w\-,.@?^=%&:/~+#]*[\w\-@?^=%&/~+#])?$";
        data = data.TrimEnd('/', '\\');
        bool isemailValid = Regex.IsMatch(data, urlPattern);
        if (isemailValid)
        {
            celldata.Data = data;
            return celldata;
        }
        celldata.Data = string.Empty;
        return celldata;
    }
    public CellDataAndStatus EmailAddress(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (stringHelper.IsValidEmail(data))
        {
            data = data.ToUpper();
        }
        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Officetel1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Officetel2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Officefaxnum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = stringHelper.SinglePhoneNumber(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus OldCustomerID(string data)
    {
        var cellData = new CellDataAndStatus(data);
        //data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus OldAccountNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        //data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus OldSRN(string data)
    {
        var cellData = new CellDataAndStatus(data);
        //data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus OldBranchCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        //data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CreditFacilityType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).Trim().Replace(" ", "").ToUpper();

        // Only these 8 codes have a fixed CreditFacilityType regardless of
        // subscriber type. Every other code/value, and blanks, fall through
        // to the "W" sentinel below and get resolved into the subscriber's
        // bank/non-bank code (X/W) downstream, based on SubCategoryCode.
        if (data == "102" || data == "AUTOLOAN" || data == "A") { data = "A"; }
        else if (data == "103" || data == "BANKGUARANTEE" || data == "Q") { data = "Q"; }
        else if (data == "106" || data == "CREDITCARD" || data == "C") { data = "C"; }
        else if (data == "109" || data == "HOUSINGLOAN" || data == "H") { data = "H"; }
        else if (data == "111" || data == "LETTEROFCREDIT" || data == "Y") { data = "Y"; }
        else if (data == "118" || data == "MORTGAGE" || data == "H") { data = "H"; }
        else if (data == "121" || data == "OVERDRAFT" || data == "V") { data = "V"; }
        else if (data == "128" || data == "STUDENTLOAN" || data == "STUDENT" || data == "T") { data = "T"; }
        else
        {
            // Genuinely blank, or a value that matches none of the 8 fixed
            // codes above -- per spec, both cases are treated as "blank"
            // and resolved to the bank/non-bank code (W/X) once the
            // subscriber's type is known. This transformer is a pure,
            // synchronous string function with no access to that context
            // (it's determined later, from an async DB lookup), so "W" is
            // used here as an unambiguous sentinel -- never used as an
            // input or output anywhere in the mapping above -- for the
            // caller to resolve into the real W/X value.
            data = "W";
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PurposeOfFacility(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).Trim().Replace(" ", "").ToUpper();

        if (data == "A" || data == "CRISISLOAN" || data == "CRISIS" || data == "CRISIS") { data = "A"; }
        else if (data == "B" || data == "HOME" || data == "HOMELOANS") { data = "B"; }
        else if (data == "S" || data == "STUDY" || data == "STUDYLOAN") { data = "S"; }
        else if (data == "C" || data == "OTHERASSETACQUISITIONFINANCING") { data = "C"; }
        else if (data == "D" || data == "PROJECT" || data == "PROJECTFINANCE") { data = "D"; }
        else if (data == "E" || data == "CAPITAL" || data == "CAPITALFINANCE") { data = "E"; }
        else if (data == "F" || data == "MACHINERY" || data == "EQUIPMENT" || data == "EQUIPMENTANDMACHINERYFINANCE") { data = "F"; }
        else if (data == "G" || data == "WORKING" || data == "CAPITAL" || data == "WORKINGCAPITALFINANCE") { data = "G"; }
        else if (data == "H" || data == "SUBSCRIPTIONFINANCE" || data == "SUBSCRIPTION") { data = "H"; }
        else if (data == "P" || data == "PERSONAL" || data == "PERSONALFINANCE") { data = "P"; }
        else if (data == "J" || data == "FINANCEFORTRADING" || data == "FINANCEINSECURITIES" || data == "FORTRADING" || data == "INSECURITIES" ||
                data == "FINANCEFORTRADINGINSECURITIES") { data = "J"; }
        else if (data == "K" || data == "CONSOLIDATION" || data == "CONSOLIDATIONLOAN") { data = "K"; }
        else if (data == "L" || data == "OTHER") { data = "L"; }
        else
        {
            data = string.Empty;
        }
        cellData.Data = data.ToUpper();
        return cellData;
    }
    public CellDataAndStatus FacilityTerm(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);

        if (string.IsNullOrWhiteSpace(data))
            return new CellDataAndStatus(string.Empty); // Return empty if input is null or whitespace

        // Normalize input: trim and convert to lowercase for easier matching
        string normalizedData = data.Trim().ToLower();

        // Match patterns like "10 months", "10 mths", "10 m", "2 years", "5 days", etc.
        var match = Regex.Match(normalizedData, @"(\d+)\s*(days?|d|weeks?|w|months?|m|years?|y)", RegexOptions.IgnoreCase);

        if (match.Success)
        {
            // Extract the number and the unit
            double value = double.Parse(match.Groups[1].Value);
            string unit = match.Groups[2].Value;

            // Determine the conversion factor based on the unit
            if (unit.StartsWith("d")) // Days to months — round up so e.g. 28 days = 1 month, not 0
            {
                cellData.Data = ((int)Math.Ceiling(value / 30.44)).ToString();
            }
            else if (unit.StartsWith("w")) // Weeks to months — round up so partial months count as 1
            {
                cellData.Data = ((int)Math.Ceiling((value * 7) / 30.44)).ToString();
            }
            else if (unit.StartsWith("m")) // Already in months
            {
                cellData.Data = ((int)value).ToString();
            }
            else if (unit.StartsWith("y")) // Years to months
            {
                cellData.Data = ((int)(value * 12)).ToString();
            }
            else
            {
                cellData.Data = ((int)value).ToString(); // Default to treating as months
            }

            return cellData;
        }

        // If input doesn't match expected pattern, attempt to extract any number
        var numberOnlyMatch = Regex.Match(normalizedData, @"\d+");
        if (numberOnlyMatch.Success)
        {
            double number = double.Parse(numberOnlyMatch.Value);
            cellData.Data = ((int)number).ToString(); // Treat as months by default
            return cellData;
        }

        return new CellDataAndStatus(string.Empty); // Return empty if no valid numeric data found
    }
    public CellDataAndStatus DefPaymentStartDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {

            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus AmountCurrency(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.ToUpper();
        if (data == "GH" || data == "GHC" || data == "CEDIS" || data == "GHA" || data == "GHS")
        {
            data = "GHS";
        }//GHS, USD, EUR, GBP
        else if (data == "USD" || data == "EUR" || data == "GBP")
        {
            data = data.ToUpper();
        }
        else
        {
            data = string.Empty;
            cellData.Passed = false;
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus FacilityAmount(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus DisbursementDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Errors = new List<string>() { " DISBURSEMENTDATE: INVALID DATE " };
            cellData.Passed = false;
        }
        return cellData;
    }
    public CellDataAndStatus DisbursementAmt(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus MaturityDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Errors = new List<string>() { " MATURITYDATE: INVALID DATE " };
            cellData.Passed = false;
        }
        return cellData;
    }
    public CellDataAndStatus SchdInstalAmount(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.ToUpper().Trim().Replace(" ", "").Replace("-", "").Replace("'", "").Replace("‘", "").Replace("‘", "");


        data = stringHelper.NormalizeDecimalOrComma(data);
        cellData = stringHelper.CleanToTwoDecimalPlaces(data);

        var F_Amt = stringHelper.ValidateDecimalInput(cellData.Data).Value;
        var D_Amt = stringHelper.ValidateDecimalInput(cellData.Data).Value;

        return cellData;
    }
    public CellDataAndStatus RepaymentFreq(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).ToUpper().Replace(" ", "").Trim();

        if (string.IsNullOrWhiteSpace(data))
        {
            cellData.Data = string.Empty; // Return an empty string for null or empty input
        }

        switch (data)
        {
            case "10":
            case "WEEKLY":
            case "01":
                data = "01";
                break;


            case "11":
            case "BI-MONTHLY":
            case "BIMONTHLY":
            case "02":
                data = "02";
                break;

            case "12":
            case "MONTHLY":
            case "03":
                data = "03";
                break;

            case "13":
            case "QUARTERLY":
            case "04":
                data = "04";
                break;

            case "14":
            case "TRIANNUALLY":
                data = string.Empty; // RETURN Empty string FOR THIS case
                break;

            case "15":
            case "SEMIANNUALLY":
                data = "05";
                break;

            case "16":
            case "ANNUAL":
                data = "06";
                break;

            case "17":
            case "VARIABLE":
                data = string.Empty; // RETURN Empty string FOR THIS case
                break;

            case "18":
            case "BULLET":
                data = "07";
                break;

            case "DEMAND":
            case "19":
                data = string.Empty; // RETURN Empty string FOR THIS case
                break;

            case "20":
            case "UNSPECIFIED":
                data = string.Empty; // RETURN Empty string FOR THIS case
                break;

            case "21":
            case "BALLOON":
                data = string.Empty; // RETURN Empty string FOR THIS case
                break;

            default:
                data = string.Empty;
                break;
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus LastPaymentAmount(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.ToUpper().Trim().Replace(" ", "").Replace("-", "").Replace("'", "").Replace("‘", "").Replace("‘", "").Replace(",", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        cellData = stringHelper.CleanToTwoDecimalPlaces(data);

        var F_Amt = stringHelper.ValidateDecimalInput(cellData.Data).Value;
        var D_Amt = stringHelper.ValidateDecimalInput(cellData.Data).Value;

        return cellData;
    }
    public CellDataAndStatus LastPaymentDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {

            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus NextPaymentDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {

            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus CurBal(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace(",", "");
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CurBalIndicator(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = "D";
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus AssetClassification(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Trim().Replace(" ", "").Replace("–", "-");
        data = stringHelper.RemoveSystemErroNames(data).ToUpper();
        if (string.IsNullOrWhiteSpace(data))
        {
            data = string.Empty;
        }

        // Normalize input to lowercase string for easy matching
        string value = data.ToString().Trim().ToLower();

        // Try to parse an integer if entered
        if (int.TryParse(value, out int days))
        {
            if (days >= 1 && days <= 30) data = "A";
            if (days >= 31 && days <= 90) data = "B";
            if (days >= 91 && days <= 180) data = "C";
            if (days >= 181 && days <= 360) data = "D";
            if (days > 360) data = "E";
        }

        // Handle various string inputs (full and short forms)
        if (value.Contains("current") || value.Contains("a")) data = "A";
        else if (value.Contains("olem") || value.Contains("b")) data = "B";
        else if (value.Contains("sub") || value.Contains("c")) data = "C";
        else if (value.Contains("doubt") || value.Contains("d")) data = "D";
        else if (value.Contains("loss") || value.Contains("over 360") || value.Contains("e")) data = "E";

        // Handle common variations
        else if (value.Contains("1-30") || value.Contains("1to30")) data = "A";
        else if (value.Contains("31-90") || value.Contains("31to90")) data = "B";
        else if (value.Contains("91-180") || value.Contains("91to180")) data = "C";
        else if (value.Contains("181-360") || value.Contains("181to360")) data = "D";
        else
        {
            data = string.Empty; // Return empty if no match
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus AmountInArrears(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus ArrearsStartDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {

            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus NDIA(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = stringHelper.RoundUpNumberOfDaysInArr(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PaymentHistoryProfile(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Trim().Replace(" ", "");
        data = stringHelper.RemoveSystemErroNames(data).ToUpper();
        if (string.IsNullOrWhiteSpace(data))
        {
            data = string.Empty;
        }

        // Normalize input to lowercase string for easy matching
        string value = data.ToString().Trim().ToLower();

        // Try to parse an integer if entered
        if (int.TryParse(value, out int days))
        {
            if (days >= 1 && days <= 30) data = "0";
            if (days >= 31 && days <= 60) data = "1";
            if (days >= 61 && days <= 90) data = "2";
            if (days >= 91 && days <= 120) data = "3";
            if (days >= 121 && days <= 180) data = "4";
            if (days > 180) data = "5";
        }

        // Handle various string inputs (full and short forms)
        if (value.Contains("current")) data = "0";

        // Handle common variations
        else if (value.Contains("1-30") || value.Contains("1to30")) data = "0";
        else if (value.Contains("31-60") || value.Contains("31to90")) data = "1";
        else if (value.Contains("61-90") || value.Contains("91to180")) data = "2";
        else if (value.Contains("91-120") || value.Contains("91to180")) data = "3";
        else if (value.Contains("121-180") || value.Contains("91to180")) data = "4";
        else if (value.Contains("181-360") || value.Contains("181to360")) data = "5";
        else
        {
            data = string.Empty; // Return empty if no match
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus AmtOverdue1to30days(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus AmtOverdue31to60days(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus AmtOverdue61to90days(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus AmtOverdue91to120days(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus Amtoverdue121To150Days(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus AmtOverdue151to180days(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus AmtOverdue181orMore(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("’", "").Replace("-", "");
        data = stringHelper.NormalizeDecimalOrComma(data);
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data; ;
        return cellData;
    }
    public CellDataAndStatus LegalFlag(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).ToUpper();
        if (data == "101" || data == "YES")
        {
            cellData.Data = "101";
            return cellData;
        }
        if (data == "102" || data == "NO")
        {
            cellData.Data = "102";
            return cellData;
        }
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus FacilityStatusCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus FacilityStatusDate(string data, string _filename)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);

        var file_date_LastDate = stringHelper.GetFacilityDateFromFileName(_filename).LastDate;
        var facility_date = stringHelper.CheckDate(data);
        var file_date = stringHelper.CheckDate(file_date_LastDate);
        if ((facility_date.IsValidFormat && file_date.IsValidFormat) && facility_date.TheDate > file_date.TheDate)
        {
            cellData.Data = file_date_LastDate;
            return cellData;
        }
        else if (facility_date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus ClosedDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsValidFormat)
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {

            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus ClosureReason(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).Trim().Replace(" ", "").ToUpper();

        if (data == "A" || data == "BYCREDITGRANTORWITHOUTPREJUDICETOTHESUBJECT") { data = "A"; }
        else if (data == "B" || data == "BALANCETRANSFER") { data = "B"; }
        else if (data == "C" || data == "DEATH") { data = "C"; }
        else if (data == "D" || data == "ENDOFCREDITFACILITYTENURE") { data = "D"; }
        else if (data == "E" || data == "MERGEROFCREDITFACILITY") { data = "E"; }
        else if (data == "F" || data == "EARLYSETTLEMENTBYSUBJECT") { data = "F"; }
        else if (data == "G" || data == "BYCOURTORDER") { data = "G"; }
        else if (data == "H" || data == "LOSTCARDS/COMPROMISEDCARDS") { data = "H"; }
        else if (data == "J" || data == "BANKRUPTCY") { data = "J"; }
        else if (data == "K" || data == "RESTRUCTURED/RESCHEDULED") { data = "K"; }

        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus WrittenOffAmt(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus ReasonForWrittenOff(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).Trim().Replace(" ", "").ToUpper();

        if (data == "A" || data == " PARTSETTLEMENT") { data = "A"; }
        else if (data == "B" || data == "DEATH") { data = "B"; }
        else if (data == "C" || data == "UNABLETOLOCATE") { data = "C"; }
        else if (data == "D" || data == "GOVERNMENTCONCESSION") { data = "D"; }
        else if (data == "E" || data == "BANKRUPTCY") { data = "E"; }
        else if (data == "F" || data == "OTHER") { data = "F"; }



        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus DateRestructured(string data, string f_code)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (f_code == "E" || f_code == "R")
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }

    }
    public CellDataAndStatus ReasonForRestructure(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).Trim().Replace(" ", "").ToUpper();

        if (data == "T" || data == "REQUESTFORTOPUPS") { data = "T"; }
        else if (data == "E" || data == "IRREGULARREPAYMENTS") { data = "E"; }
        else if (data == "L" || data == "LOSSOFJOB") { data = "L"; }
        else if (data == "D" || data == "BUSINESSDOWNTURN") { data = "D"; }
        else if (data == "F" || data == "FORCEMAJEURE") { data = "F"; }
        else if (data == "C" || data == "OTHER") { data = "C"; }


        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CreditCollateralInd(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Trim().ToUpper().Replace(" ", "");
        if (data == "Y" || data == "101" || data == "YES")
        {
            data = "101";
            cellData.Data = data;
            return cellData;
        }
        if (data == "N" || data == "102" || data == "NO")
        {
            data = "102";
            cellData.Data = data;
            return cellData;
        }
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus SecurityType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).ToUpper().Trim().Replace(" ", "");

        switch (data)
        {
            case "LAND":
            case "A":
                data = "A";
                break;

            case "SHARES":
            case "B":
                data = "B";
                break;

            case "GOVERNMENTBONDS":
            case "GOVERNMENTSECURITIES":
            case "C":
                data = "C";
                break;

            case "BUILDING":
            case "D":
                data = "D";
                break;

            case "E":
            case "CASH":
            case "FIXEDDEPOSIT":
                data = "E";
                break;

            case "BANKGUARANTEE":
            case "F":
                data = "F";
                break;

            case "SALARYASSIGNMENT":
            case "G":
                data = "G";
                break;

            case "TERMINALBENEFITSASSIGNMENT":
            case "H":
                data = "H";
                break;

            case "BULLIONS":
            case "J":
                data = "J";
                break;

            case "GENERALPLANT&MACHINERY":
            case "K":
                data = "K";
                break;

            case "VEHICLES":
            case "L":
                data = "L";
                break;

            case "CORPORATEGUARANTEE":
            case "M":
                data = "M";
                break;

            case "INDIVIDUALGUARANTEE":
            case "INDIVIDUAL":
            case "N":
                data = "N";
                break;

            case "GOVERNMENTGUARANTEE":
            case "P":
                data = "P";
                break;

            case "OTHERS":
            case "Q":
                data = "Q";
                break;

            default:
                data = string.Empty;
                break;
        }

        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus NatureOfCharge(string data)
    {
        data = data.ToUpper();
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (data == "FIXED" || data == "OUTRIGHT" || data == "OUTRIGHTPAYMENT" || data == "A")
        {
            cellData.Data = "FIXED";
            return cellData;
        }
        if (data == "FLOAT" || data == "INSTALMENTS" || data == "B")
        {
            cellData.Data = "FLOAT";
            return cellData;
        }
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus SecurityValue(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.ToUpper().Trim().Replace(" ", "").Replace("-", "").Replace("'", "").Replace("‘", "").Replace("‘", "");
        data = stringHelper.CleanToTwoDecimalPlaces(data).Data;
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CollRegRefNum(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus SpecialCommentsCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);

        HashSet<string> targetNumbers = new HashSet<string> { "101", "102", "103", "104", "105", "106", "107", "108", "109", "110", "111", "112", "113", "114", "115" };

        // Assume 'data' is the variable you want to check
        if (targetNumbers.Contains(data))
        {
            cellData.Data = data;
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus NatureOfGuarantor1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        // Define the codes and their descriptions in a Dictionary for fast lookups
        var codes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
           { "101", "INDIVIDUAL" },
 { "102", "COMMERCIALENTITY"},
 { "103", "NOGUARANTOR" }

        };
        var descriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in codes)
        {
            descriptions[pair.Value] = pair.Key;
        }
        // Check if the input matches a code or description
        if (codes.TryGetValue(data, out string? description))
        {
            data = data.ToLower();
        }
        else if (descriptions.TryGetValue(data, out string? code))
        {
            data = code;
        }
        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus NameOfComGuarantor1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        //data = stringHelper.ConvertBusinessShortFormToLongForm(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus BusRegOfGuarantor1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (string.IsNullOrWhiteSpace(data))
            data = string.Empty;

        if (data.Length >= 5 && Regex.IsMatch(data, @"^[a-zA-Z0-9]+$") && !Regex.IsMatch(data, @"^[a-zA]+$"))
        {
            data = data.ToUpper();
            cellData.Data = data;
            return cellData;
        }
        else
        {
            data = string.Empty;
            cellData.Data = data;
            return cellData;
        }

    }
    public CellDataAndStatus G1Surname(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1FirstName(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1MiddleNames(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1NatID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1VotID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1DrivLic(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1PassNum(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1SSN(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }

    public CellDataAndStatus G1Gender(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus G1DOB(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsEighteenOrAbove && date.IsValidFormat && DateTime.Now.Year - date.TheDate.Year <= 100)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Data = string.Empty;
            cellData.Passed = true;
        }
        return cellData;
    }
    public CellDataAndStatus G1Add1(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1Add2(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1Add3(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G1HomeTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G1WorkTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus NatureOfGuarantor2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        // Define the codes and their descriptions in a Dictionary for fast lookups
        var codes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
           { "101", "INDIVIDUAL" },
 { "102", "COMMERCIALENTITY"},
 { "103", "NOGUARANTOR" }

        };
        var descriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in codes)
        {
            descriptions[pair.Value] = pair.Key;
        }
        // Check if the input matches a code or description
        if (codes.TryGetValue(data, out string? description))
        {
            data = data.ToLower();
        }
        else if (descriptions.TryGetValue(data, out string? code))
        {
            data = code;
        }
        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus NameOfComGuarantor2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        //data = stringHelper.ConvertBusinessShortFormToLongForm(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus BusRegOfGuarantor2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (string.IsNullOrWhiteSpace(data))
            data = string.Empty;

        if (data.Length >= 5 && Regex.IsMatch(data, @"^[a-zA-Z0-9]+$") && !Regex.IsMatch(data, @"^[a-zA]+$"))
        {
            data = data.ToUpper();
            cellData.Data = data;
            return cellData;
        }
        else
        {
            data = string.Empty;
            cellData.Data = data;
            return cellData;
        }

    }
    public CellDataAndStatus G1Mobile(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2Surname(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2FirstName(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2MiddleNames(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2NatID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2VotID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2DrivLic(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2PassNum(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2SSN(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2Gender(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus G2DOB(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsEighteenOrAbove && date.IsValidFormat && DateTime.Now.Year - date.TheDate.Year <= 100)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Data = string.Empty;
            cellData.Passed = true;
        }
        return cellData;
    }
    public CellDataAndStatus NatureOfGuarantor3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        // Define the codes and their descriptions in a Dictionary for fast lookups
        var codes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
           { "101", "INDIVIDUAL" },
 { "102", "COMMERCIALENTITY"},
 { "103", "NOGUARANTOR" }

        };
        var descriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in codes)
        {
            descriptions[pair.Value] = pair.Key;
        }
        // Check if the input matches a code or description
        if (codes.TryGetValue(data, out string? description))
        {
            data = data.ToLower();
        }
        else if (descriptions.TryGetValue(data, out string? code))
        {
            data = code;
        }
        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus NameOfComGuarantor3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        //data = stringHelper.ConvertBusinessShortFormToLongForm(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus BusRegOfGuarantor3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (string.IsNullOrWhiteSpace(data))
            data = string.Empty;

        if (data.Length >= 5 && Regex.IsMatch(data, @"^[a-zA-Z0-9]+$") && !Regex.IsMatch(data, @"^[a-zA]+$"))
        {
            data = data.ToUpper();
            cellData.Data = data;
            return cellData;
        }
        else
        {
            data = string.Empty;
            cellData.Data = data;
            return cellData;
        }

    }
    public CellDataAndStatus NatureOfGuarantor4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        // Define the codes and their descriptions in a Dictionary for fast lookups
        var codes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
           { "101", "INDIVIDUAL" },
 { "102", "COMMERCIALENTITY"},
 { "103", "NOGUARANTOR" }

        };
        var descriptions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var pair in codes)
        {
            descriptions[pair.Value] = pair.Key;
        }
        // Check if the input matches a code or description
        if (codes.TryGetValue(data, out string? description))
        {
            data = data.ToLower();
        }
        else if (descriptions.TryGetValue(data, out string? code))
        {
            data = code;
        }
        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus NameOfComGuarantor4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        //data = stringHelper.ConvertBusinessShortFormToLongForm(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus BusRegOfGuarantor4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (string.IsNullOrWhiteSpace(data))
            data = string.Empty;

        if (data.Length >= 5 && Regex.IsMatch(data, @"^[a-zA-Z0-9]+$") && !Regex.IsMatch(data, @"^[a-zA]+$"))
        {
            data = data.ToUpper();
            cellData.Data = data;
            return cellData;
        }
        else
        {
            data = string.Empty;
            cellData.Data = data;
            return cellData;
        }

    }
    public CellDataAndStatus G2Add1(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2Add2(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2Add3(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G2HomeTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G2WorkTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G2Mobile(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3Surname(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3FirstName(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3MiddleNames(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3NatID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3VotID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3DrivLic(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3PassNum(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3SSN(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3Gender(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus G3DOB(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsEighteenOrAbove && date.IsValidFormat && DateTime.Now.Year - date.TheDate.Year <= 100)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Data = string.Empty;
            cellData.Passed = true;
        }
        return cellData;
    }
    public CellDataAndStatus G3Add1(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3Add2(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3Add3(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G3HomeTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G3WorkTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G3Mobile(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4Surname(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4FirstName(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4MiddleNames(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4NatID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4VotID(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4DrivLic(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4PassNum(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4SSN(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4Gender(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus G4DOB(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        var date = stringHelper.CheckDate(data);
        if (date.IsEighteenOrAbove && date.IsValidFormat && DateTime.Now.Year - date.TheDate.Year <= 100)
        {
            cellData.Passed = true;
            cellData.Data = data;
        }
        else
        {
            cellData.Data = string.Empty;
            cellData.Passed = true;
        }
        return cellData;
    }
    public CellDataAndStatus G4Add1(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4Add2(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4Add3(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }
    public CellDataAndStatus G4HomeTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G4WorkTel(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = data.Replace("+", "");
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("(", "").Replace(")", "").Replace("'", "");
        if (stringHelper.IsValidPhoneNumber(data))
        {
            cellData.Data = stringHelper.ProcessPhoneNumber(data);
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    public CellDataAndStatus G4Mobile(string data) { var cellData = new CellDataAndStatus(data); data = stringHelper.RemoveSystemErroNames(data); cellData.Data = data; return cellData; }

}
