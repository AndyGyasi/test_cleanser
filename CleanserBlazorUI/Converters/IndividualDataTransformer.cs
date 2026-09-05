namespace CleanserBlazorUI.Converters;
public class IndividualDataTransformer
{
    StringHelper stringHelper = new StringHelper();
    public string UNL_natidnum(string data)
    {
        return data;
    }
    public string UNL_NDIA_OldData(string data)
    {
        return data;
    }
    public string UNL_votersidnum(string data)
    {
        return data;
    }
    public string UNL_driverlicnum(string data)
    {
        return data;
    }
    public string UNL_passportnum(string data)
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
    public string UNL_surname(string data)
    {
        return data;
    }
    public string UNL_firstname(string data)
    {
        return data;
    }
    public string UNL_middlenames(string data)
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
    public string UNL_writtenoffamt(string data)
    {
        return data;
    }
    //defective
    public string Data(string data) //1
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
    public CellDataAndStatus CreditFacilityAccNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
           data = data.TrimStart().TrimEnd();
        if (data.Length > 0)
        {
            if (data.Contains("+") || stringHelper.HasSixConsecutiveZerosAtEnd(data))
            {
                cellData.Data = data;
                cellData.Passed = false;
                cellData.Errors = new List<string>() { "CREDITFACILITYACCNUM: NOT VALID" };
                return cellData;
            }
            cellData.Data = data;
            cellData.Passed = true;
            return cellData;
        }
        else
        {
            cellData.Errors = new List<string>() { "CREDITFACILITYACCNUM: NOT VALID" };
            cellData.Data = data;
            cellData.Passed = false;
            return cellData;
        }
    }
    public CellDataAndStatus CustomerID(string data)
    {
        var cellData = new CellDataAndStatus(data);
           data = data.TrimStart().TrimEnd();
        if (data.Length > 0)
        {
            if (data.Contains("+") || stringHelper.HasSixConsecutiveZerosAtEnd(data))
            {
                cellData.Data = data;
                cellData.Passed = false;
                cellData.Errors = new List<string>() { "CustomerID: NOT VALID" };
                return cellData;
            }
            cellData.Data = data;
            cellData.Passed = true;
            return cellData;
        }
        else
        {
            cellData.Errors = new List<string>() { "CustomerID: NOT VALID" };
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

    //Required  Conditional
    public CellDataAndStatus NatIDNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //Required Conditional
    public CellDataAndStatus VotersIDNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //Required Conditional
    public CellDataAndStatus DriverLicNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //Required Conditional
    public CellDataAndStatus PassportNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //If Available
    public CellDataAndStatus SSNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //If Available
    public CellDataAndStatus EzwichNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //If Available
    public CellDataAndStatus OtherIDType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //Required Conditional
    public CellDataAndStatus OtherIDNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //If Available
    public CellDataAndStatus TINum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace("-", "").Replace(" ", "").Replace("(", "").Replace(")", "");
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Gender(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData = stringHelper.NormalizeGender(data);
        return cellData;
    }
    //[Default: S]
    public CellDataAndStatus MaritalStatus(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData = stringHelper.NormarlizeMaritalStatus(data);
        return cellData;
    }
    public CellDataAndStatus Nationality(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        // Normalise input before lookup — handles mixed case (e.g. "gha", "Ghana", "GHA" all resolve correctly)
        string normalisedInput = data.Trim().ToUpperInvariant();

        // Keys are always uppercase ISO codes OR common full-name variants subscribers send
        Dictionary<string, string> nationalityCodes = new Dictionary<string, string>
        {
            // ── Americas ──────────────────────────────────────────────────
            {"USA",         "AMERICAN"},
            {"US",          "AMERICAN"},
            {"AMERICAN",    "AMERICAN"},
            {"CAN",         "CANADIAN"},
            {"CA",          "CANADIAN"},
            {"CANADIAN",    "CANADIAN"},
            {"PE",          "PERUVIAN"},
            {"PER",         "PERUVIAN"},
            {"PERUVIAN",    "PERUVIAN"},
            {"TT",          "TRINIDADIAN"},
            {"TTO",         "TRINIDADIAN"},
            {"TRINIDADIAN", "TRINIDADIAN"},
            // ── Europe ────────────────────────────────────────────────────
            {"EUR",         "EUROPEAN"},
            {"GBR",         "BRITISH"},
            {"GB",          "BRITISH"},
            {"UK",          "BRITISH"},
            {"BRITISH",     "BRITISH"},
            {"FRA",         "FRENCH"},
            {"FR",          "FRENCH"},
            {"FRENCH",      "FRENCH"},
            {"CHE",         "SWISS"},
            {"CH",          "SWISS"},
            {"SWISS",       "SWISS"},
            {"TUR",         "TURKISH"},
            {"TR",          "TURKISH"},
            {"TURKISH",     "TURKISH"},
            // ── Asia & Pacific ────────────────────────────────────────────
            {"JPN",         "JAPANESE"},
            {"JP",          "JAPANESE"},
            {"JAPANESE",    "JAPANESE"},
            {"CHN",         "CHINESE"},
            {"CN",          "CHINESE"},
            {"CHINESE",     "CHINESE"},
            {"IND",         "INDIAN"},
            {"IN",          "INDIAN"},
            {"INDIAN",      "INDIAN"},
            {"AUS",         "AUSTRALIAN"},
            {"AU",          "AUSTRALIAN"},
            {"AUSTRALIAN",  "AUSTRALIAN"},
            {"NZL",         "NEW ZEALANDER"},
            {"NZ",          "NEW ZEALANDER"},
            {"SGP",         "SINGAPOREAN"},
            {"SG",          "SINGAPOREAN"},
            {"SINGAPOREAN", "SINGAPOREAN"},
            {"KOR",         "SOUTH KOREAN"},
            {"KR",          "SOUTH KOREAN"},
            {"LK",          "SRI LANKAN"},
            {"LKA",         "SRI LANKAN"},
            {"SRILANKAN",   "SRI LANKAN"},
            {"SRI LANKAN",  "SRI LANKAN"},
            // ── Middle East ───────────────────────────────────────────────
            {"ARE",         "EMIRATI"},
            {"AE",          "EMIRATI"},
            {"EMIRATI",     "EMIRATI"},
            // ── West Africa ───────────────────────────────────────────────
            {"GHA",         "GHANAIAN"},
            {"GH",          "GHANAIAN"},
            {"GHANA",       "GHANAIAN"},
            {"GHS",         "GHANAIAN"},
            {"GHANAIAN",    "GHANAIAN"},
            {"NGA",         "NIGERIAN"},
            {"NIG",         "NIGERIAN"},
            {"NG",          "NIGERIAN"},
            {"NIGERIA",     "NIGERIAN"},
            {"NIGERIAN",    "NIGERIAN"},
            {"CIV",         "IVORIAN"},
            {"CI",          "IVORIAN"},
            {"IVORIAN",     "IVORIAN"},
            {"BJ",          "BENINOIS"},
            {"BEN",         "BENINOIS"},
            {"BENIN",       "BENINOIS"},
            {"BENINOIS",    "BENINOIS"},
            {"SN",          "SENEGALESE"},
            {"SEN",         "SENEGALESE"},
            {"SENEGAL",     "SENEGALESE"},
            {"SENEGALESE",  "SENEGALESE"},
            {"TOG",         "TOGOLESE"},
            {"TG",          "TOGOLESE"},
            {"TOGO",        "TOGOLESE"},
            {"TOGOLESE",    "TOGOLESE"},
            {"GNQ",         "EQUATOGUINEAN"},
            {"GQ",          "EQUATOGUINEAN"},
            {"GAB",         "GABONESE"},
            {"GA",          "GABONESE"},
            {"GABONESE",    "GABONESE"},
            {"SL",          "SIERRA LEONEAN"},
            {"SLA",         "SIERRA LEONEAN"},
            {"SLE",         "SIERRA LEONEAN"},
            {"SIERRALEONEAN","SIERRA LEONEAN"},
            {"SIERRA LEONEAN","SIERRA LEONEAN"},
            {"XOF",         "WEST AFRICAN"},
            // ── East & Southern Africa ────────────────────────────────────
            {"KEN",         "KENYAN"},
            {"KE",          "KENYAN"},
            {"KENYAN",      "KENYAN"},
            {"TZA",         "TANZANIAN"},
            {"TZ",          "TANZANIAN"},
            {"TANZANIAN",   "TANZANIAN"},
            {"UGA",         "UGANDAN"},
            {"UG",          "UGANDAN"},
            {"UGANDAN",     "UGANDAN"},
            {"ZAF",         "SOUTH AFRICAN"},
            {"ZA",          "SOUTH AFRICAN"},
            {"SOUTH AFRICAN","SOUTH AFRICAN"},
            // ── North Africa ──────────────────────────────────────────────
            {"EGY",         "EGYPTIAN"},
            {"EG",          "EGYPTIAN"},
            {"EGYPTIAN",    "EGYPTIAN"},
            {"MAR",         "MOROCCAN"},
            {"MA",          "MOROCCAN"},
            {"MOROCCAN",    "MOROCCAN"},
            {"ETH",         "ETHIOPIAN"},
            {"ET",          "ETHIOPIAN"},
            {"ETHIOPIAN",   "ETHIOPIAN"},
        };

        // Check if normalised input resolves in the dictionary
        if (nationalityCodes.ContainsKey(normalisedInput))
        {
            cellData.Data = nationalityCodes[normalisedInput];
            return cellData;
        }
        else
        {
            // Pass through as-is — unknown codes are preserved, not wiped
            cellData.Data = data.Trim();
            return cellData;
        }
    }
    public CellDataAndStatus DateOfBirth(string data)
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
            cellData.Errors = new List<string>() { " DATEOFBIRTH: INVALID DATE " };
            cellData.Passed = false;
        }
        return cellData;
    }
    //TODO :Company names are not allowed
    public CellDataAndStatus Title(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = stringHelper.NewFilterNameByTitle(data);
        cellData.Data = data.ToUpper();
        return cellData;
    }
    //TODO :Company names are not allowed
    public CellDataAndStatus Surname(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;//stringHelper.NewCleanTextNames(data);
        return cellData;
    }

    //TODO :Company names are not allowed
    public CellDataAndStatus FirstName(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus MiddleNames(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data; //stringHelper.NewCleanTextNames(data);
        return cellData;
    }
    public CellDataAndStatus PreviousNames(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data; //stringHelper.NewCleanTextNames(data);
        return cellData;
    }
    public CellDataAndStatus Alias(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data; //stringHelper.NewCleanTextNames(data);
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
        cellData.Data = data.ToUpper();
        return cellData;
    }
    public CellDataAndStatus CurResAddr1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CurResAddr2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CurResAddr3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CurResAddr4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus CurResAddrPostalCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    //===================================================END++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    public CellDataAndStatus DateMovedCurrRes(string data)
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
    public CellDataAndStatus PrevResAddr1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PrevResAddr2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PrevResAddr3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PrevResAddr4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PrevResAddrPostalCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus OwnerOrTenant(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Trim().Replace(" ", "").ToUpper();
        data = data.ToUpper();
        if (data == "0")
        {
            cellData.Data = "O";
            return cellData;
        }
        if (data == "O" || data == "OWNER" || data == "0")
        {
            cellData.Data = "O";
            return cellData;
        }
        if (data == "T" || data == "TENANT")
        {
            cellData.Data = "T";
            return cellData;
        }
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus PostAddrLine1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PostAddrLine2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PostAddrLine3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PostAddrLine4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PostalAddPostCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus EmailAddress(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        if (string.IsNullOrWhiteSpace(data))
        {
            cellData.Data = string.Empty;
            return cellData;
        }
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        data = Regex.Replace(data, @"^https?://", "", RegexOptions.IgnoreCase);
        // Remove trailing slashes or backslashes
        data = data.TrimEnd('/', '\\');
        bool isemailValid = Regex.IsMatch(data, emailPattern);
        if (!string.IsNullOrWhiteSpace(data) && isemailValid)
        {
            data = data.ToLower();
        }
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus HomeTel(string data)
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
    public CellDataAndStatus MobileTel1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus MobileTel2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus WorkTel(string data)
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
    public CellDataAndStatus NumOfDependants(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);

        if (int.TryParse(data, out int num) && num > 0)
        {
            data = int.Parse(data).ToString();
            cellData.Data = data;
            return cellData;
        }
        else
        {
            cellData.Data = string.Empty;
            return cellData;
        }
    }
    //TODO? default
    public CellDataAndStatus EmpType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }

    //EE
    public CellDataAndStatus EmpPayrollNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Paypoint(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    //EE
    public CellDataAndStatus EmpName(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data.Replace(",", " ").Replace("~", " ").Replace("`", " ").Replace(";", " ").Replace("!", " ").Replace("@", " ").Replace("#", " ").Replace(":", " ")
            .Replace("$", " ").Replace("%", " ").Replace("^", "").Replace("&", " & ").Replace("*", " ").Replace("_", " ").Replace("+", " ").Replace(".", " ");
        return cellData;
    }
    public CellDataAndStatus EmpAddr1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus EmpAddr2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus EmpAddr3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus EmpAddr4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus EmpAddrPostalCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty; ;
        return cellData;
    }
    public CellDataAndStatus DateOfEmp(string data, string cellHeader)
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
    public CellDataAndStatus Occupation(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = stringHelper.ValidateOccupation(data);
        data = stringHelper.ReplaceAlphanumericWithSpace(data);
        if (data.Length > 1)
        {
            cellData.Data = data;
        }
        else
        {
            cellData.Data = string.Empty;
        }
        return cellData;
    }
    public CellDataAndStatus IncomeCurrency(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Income(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus JointOrSoleAcc(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus NoParticipantsInAcc(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus OldCustomerID(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus OldAccountNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }

    public CellDataAndStatus OldSRN(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus OldBranchCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    //Default: 122

    //TODO? default or empty string
    public CellDataAndStatus CreditFacilityType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).Trim().Replace(" ", "").ToUpper();

        // Only these 8 codes have a fixed CreditFacilityType. Every other
        // code/value, and blanks, default to "P" -- final for individual
        // files (unlike business, individual files have no bank/non-bank
        // resolution step downstream).
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
            data = "P";
        }
        cellData.Data = data;
        return cellData;
    }
    // private string RemoveSpaces(string input) => string.Concat(input.Where(c => !char.IsWhiteSpace(c)));
    //TODO? default
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

    //(FacilityAmount-DisbursementAmt)
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
        data = data.ToUpper().Trim().Replace(" ", "").Replace("-", "").Replace("'", "").Replace("‘", "").Replace("‘", "").Replace(",","");


        data = stringHelper.NormalizeDecimalOrComma(data);
        cellData = stringHelper.CleanToTwoDecimalPlaces(data);

        var F_Amt = stringHelper.ValidateDecimalInput(cellData.Data).Value;
        var D_Amt = stringHelper.ValidateDecimalInput(cellData.Data).Value;

        return cellData;
    }

    //Negative remove
    public CellDataAndStatus RepaymentFreq(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).ToUpper().Replace(" ","").Trim();

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
            cellData.Data = string.Empty;
            return cellData;
        }

        // Normalize to lowercase for string matching
        string value = data.Trim().ToLower();

        // ── Valid final codes (0–5) passed directly by subscriber — preserve as-is ──
        // Must be checked BEFORE the day-range integer parse to avoid misreading "1"–"5"
        // as day counts (1 day, 2 days, etc.) and converting them incorrectly.
        var validFinalCodes = new HashSet<string> { "0", "1", "2", "3", "4", "5" };
        if (validFinalCodes.Contains(value))
        {
            cellData.Data = value;
            return cellData;
        }

        // ── Day-count integers: convert to standard code ──────────────────────────
        // Subscribers sometimes send the number of days in arrears rather than the code.
        if (int.TryParse(value, out int days))
        {
            if (days >= 1 && days <= 30)        cellData.Data = "0";
            else if (days >= 31 && days <= 60)  cellData.Data = "1";
            else if (days >= 61 && days <= 90)  cellData.Data = "2";
            else if (days >= 91 && days <= 120)  cellData.Data = "3";
            else if (days >= 121 && days <= 180) cellData.Data = "4";
            else if (days > 180)                 cellData.Data = "5";
            else                                 cellData.Data = string.Empty;
            return cellData; // early return — integer input fully handled
        }

        // ── Descriptive string inputs ─────────────────────────────────────────────
        if      (value.Contains("current") || value.Contains("1-30")    || value.Contains("1to30"))    cellData.Data = "0";
        else if (value.Contains("31-60")   || value.Contains("31to60"))                                cellData.Data = "1";
        else if (value.Contains("61-90")   || value.Contains("61to90"))                                cellData.Data = "2";
        else if (value.Contains("91-120")  || value.Contains("91to120"))                               cellData.Data = "3";
        else if (value.Contains("121-180") || value.Contains("121to180"))                              cellData.Data = "4";
        else if (value.Contains("181")     || value.Contains("181to360") || value.Contains("over 180")) cellData.Data = "5";
        else
        {
            // Input does not match any known pattern — preserve original rather than silently wiping
            cellData.Data = data.Trim();
        }

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
    public CellDataAndStatus AmtOverdue121to150days(string data)
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
    //string statustype, string currentBalance, string amountOverdue, string writtenOffAmount, string accountInArrears, string nDIA
    public CellDataAndStatus FacilityStatusCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus FacilityStatusDate(string data, string _filename)
    {
        if (!string.IsNullOrWhiteSpace(data))
        {

        }
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
        //var cellData = new CellDataAndStatus(data);
        //data = stringHelper.RemoveSystemErroNames(data);
        //var date = stringHelper.CheckDate(data);

        //if (date.IsValidFormat)
        //{
        //    cellData.Data = data;
        //    return cellData;
        //}
        //else
        //{

        //    cellData.Data = string.Empty;
        //    return cellData;
        //}
    }
    public CellDataAndStatus ClosedDate(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);

        if (string.IsNullOrWhiteSpace(data))
        {
            cellData.Data = string.Empty;
            return cellData;
        }

        // Attempt to normalise common subscriber date formats into yyyyMMdd
        // before passing to CheckDate(), which only accepts yyyyMMdd.
        // Formats handled: yyyy-MM-dd, dd/MM/yyyy, dd-MM-yyyy, MM/dd/yyyy,
        //                  yyyyMMdd (already correct), dd MMM yyyy, etc.
        string normalised = TryNormaliseDateToYyyyMMdd(data.Trim()) ?? data.Trim();

        var date = stringHelper.CheckDate(normalised);
        if (date.IsValidFormat)
        {
            cellData.Data = normalised; // always store in yyyyMMdd
            return cellData;
        }

        // Not a recognisable date — preserve original value rather than silently wiping
        cellData.Data = data.Trim();
        return cellData;
    }

    /// <summary>
    /// Attempts to parse a date string in various common formats and return it
    /// as yyyyMMdd. Returns null if no recognised format matches.
    /// </summary>
    private string? TryNormaliseDateToYyyyMMdd(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;

        string[] formats = new[]
        {
            "yyyyMMdd",       // already correct — 20240115
            "yyyy-MM-dd",     // ISO 8601   — 2024-01-15
            "yyyy/MM/dd",     // slash ISO  — 2024/01/15
            "dd/MM/yyyy",     // UK style   — 15/01/2024
            "dd-MM-yyyy",     // UK dashes  — 15-01-2024
            "MM/dd/yyyy",     // US style   — 01/15/2024
            "dd MMM yyyy",    // verbose    — 15 Jan 2024
            "dd MMMM yyyy",   // full month — 15 January 2024
            "d/M/yyyy",       // no-pad UK  — 5/1/2024
            "d-M-yyyy",       // no-pad dash
        };

        if (DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime parsed))
        {
            return parsed.ToString("yyyyMMdd");
        }

        // Fallback: let .NET try any recognised format
        if (DateTime.TryParse(input, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime fallback))
        {
            return fallback.ToString("yyyyMMdd");
        }

        return null;
    }
    public CellDataAndStatus ClosureReason(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data).Trim().Replace(" ", "").ToUpper();

        if(data == "A" || data == "BYCREDITGRANTORWITHOUTPREJUDICETOTHESUBJECT") { data = "A"; }
        else if(data == "B" || data == "BALANCETRANSFER") { data = "B"; }
        else if(data == "C" || data == "DEATH") { data = "C"; }
        else if(data == "D" || data == "ENDOFCREDITFACILITYTENURE") { data = "D"; }
        else if(data == "E" || data == "MERGEROFCREDITFACILITY") { data = "E"; }
        else if(data == "F" || data == "EARLYSETTLEMENTBYSUBJECT") { data = "F"; }
        else if(data == "G" || data == "BYCOURTORDER") { data = "G"; }
        else if(data == "H" || data == "LOSTCARDS/COMPROMISEDCARDS") { data = "H"; }
        else if(data == "J" || data == "BANKRUPTCY") { data = "J"; }
        else if(data == "K" || data == "RESTRUCTURED/RESCHEDULED") { data = "K"; }

        else
        {
            data = string.Empty;
        }
        cellData.Data = data;
        return cellData;
    }

    //CurrentBalance, AmountOverdue, WrittenOffAmount, AccountInArrears, NDIA"
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

        if(data == "A" || data == " PARTSETTLEMENT") { data = "A"; }
        else if(data == "B" || data == "DEATH") { data = "B"; }
        else if(data == "C" || data == "UNABLETOLOCATE") { data = "C"; }
        else if(data == "D" || data == "GOVERNMENTCONCESSION") { data = "D"; }
        else if(data == "E" || data == "BANKRUPTCY") { data = "E"; }
        else if(data == "F" || data == "OTHER") { data = "F"; }



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

        if(data == "T" || data == "REQUESTFORTOPUPS") { data = "T"; }
        else if(data == "E" || data == "IRREGULARREPAYMENTS") { data = "E"; }
        else if(data == "L" || data == "LOSSOFJOB") { data = "L"; }
        else if(data == "D" || data == "BUSINESSDOWNTURN") { data = "D"; }
        else if(data == "F" || data == "FORCEMAJEURE") { data = "F"; }
        else if(data == "C" || data == "OTHER") { data = "C"; }


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
    public CellDataAndStatus NatureOfGuarantor(string data)
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

    // data = stringHelper.RemoveSystemErroNames(data);

    //================================================================================================================================================
    public CellDataAndStatus NameOfComGuarantor(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        //data = stringHelper.ConvertBusinessShortFormToLongForm(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus BusRegOfGuarantor(string data)
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
        data = stringHelper.RemoveSystemErroNames(data);
        cellData = stringHelper.NormalizeGender(data);
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
        data = stringHelper.RemoveSystemErroNames(data);
        cellData = stringHelper.NormalizeGender(data);
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
        data = stringHelper.RemoveSystemErroNames(data);
        cellData = stringHelper.NormalizeGender(data);
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
        data = stringHelper.RemoveSystemErroNames(data);
        cellData = stringHelper.NormalizeGender(data);
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