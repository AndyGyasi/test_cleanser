namespace CleanserBlazorUI.Converters;
public class IndividualJudgementDataTransformer
{
    StringHelper stringHelper = new StringHelper();
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
    public CellDataAndStatus NatIDNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus VotersIDNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus DriverLicNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PassportNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus SSNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus EzwichNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus OtherIDType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus OtherIDNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
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
        Dictionary<string, string> currencyCodes = new Dictionary<string, string>
        {
           {"USA", "AMERICAN"},
           {"EUR", "EUROZONE (MULTIPLE NATIONALITIES)"},
           {"GBR", "BRITISH"},
           {"JPN", "JAPANESE"},
           {"CHN", "CHINESE"},
           {"IND", "INDIAN"},
           {"IN", "INDIAN"},
           {"AUS", "AUSTRALIAN"},
           {"CAN", "CANADIAN"},
           {"CHE", "SWISS"},
           {"NZL", "NEW ZEALANDER"},
           {"NGA", "NIGERIAN"},
           {"ZAF", "SOUTH AFRICAN"},
           {"KEN", "KENYAN"},
           {"EGY", "EGYPTIAN"},
           {"ETH", "ETHIOPIAN"},
           {"GHA", "GHANAIAN"},
           {"GHANA", "GHANAIAN"},
           {"GHS", "GHANAIAN"},
           {"GH", "GHANAIAN"},
           {"XOF", "WEST AFRICAN (MULTIPLE NATIONALITIES)"},
           {"TZA", "TANZANIAN"},
           {"UGA", "UGANDAN"},
           {"MAR", "MOROCCAN"},
           {"KOR", "SOUTH KOREAN"},
           {"ARE", "EMIRATI"},
           {"TUR", "TURKISH"},
           {"SGP", "SINGAPOREAN"},
           {"NIG", "NIGERIAN"},
           {"NG", "NIGERIAN"},
           {"CIV", "IVORIAN"},
           {"PE", "PERUVIAN"},
           {"PER", "PERUVIAN"},
           {"TT", "TRINIDADIANS"},
           {"TTO", "TRINIDADIANS"},
           {"LK", "SRILANKAN"},
           {"LKA", "SRILANKAN"},
           {"SL", "SIERRALEONEAN"},
           {"SLA", "SIERRALEONEAN"},
           {"BJ", "BENINOIS"},
           {"BEN", "BENINOIS"},
           {"CN", "CHINESE"},
           {"CI", "IVORIAN"},
           {"GNQ", "EQUATOGUINEAN" },
           {"GQ", "EQUATOGUINEAN" },
           {"GAB", "GABONESE" },
           {"GA", "GABONESE" },
           {"SN", "SENEGALESE" },
           {"SEN", "SENEGALESE" },
           {"TOG", "TOGOLESE"},
           {"FRA", "FRENCH"}};

        if (currencyCodes.ContainsKey(data))
        {
            cellData.Data = currencyCodes[data];
            return cellData;
        }
        else
        {
            cellData.Data = data;
            return cellData;
        }
    }
    public CellDataAndStatus DateOfBrith(string data)
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
    public CellDataAndStatus Title(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = stringHelper.NewFilterNameByTitle(data);
        cellData.Data = data.ToUpper();
        return cellData;
    }
    public CellDataAndStatus Surname(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
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
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus PreviousNames(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Alias(string data)
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
        cellData.Data = data.ToUpper();
        return cellData;
    }
    public CellDataAndStatus CurResAddr1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus CurResAddr2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus CurResAddr3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus CurResAddr4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus CurResAddrPostalCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
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
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus PrevResAddr2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus PrevResAddr3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus PrevResAddr4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
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
    public CellDataAndStatus EmailAddress(string data)
    {
        return stringHelper.CleanEmailAddress(data);
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
    public CellDataAndStatus EmpType(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus EmpPayrollNum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.NumericOrAlphanumericWithDigit(data) ? data : string.Empty;
        return cellData;
    }
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
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus EmpAddr2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus EmpAddr3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus EmpAddr4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus EmpAddrPostalCode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        cellData.Data = string.Empty;
        return cellData;
    }
    public CellDataAndStatus DateOfEmp(string data)
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

    // ── Court/case fields ────────────────────────────────────────────
    // Per spec: CourtName, CourtLocation, CourtType, CaseNumber and
    // CaseFilingDate are left exactly as submitted -- no cleaning.
    public CellDataAndStatus CourtName(string data)
    {
        return new CellDataAndStatus(data) { Data = data };
    }
    public CellDataAndStatus CourtLocation(string data)
    {
        return new CellDataAndStatus(data) { Data = data };
    }
    public CellDataAndStatus CourtType(string data)
    {
        return new CellDataAndStatus(data) { Data = data };
    }
    public CellDataAndStatus CaseNumber(string data)
    {
        return new CellDataAndStatus(data) { Data = data };
    }
    public CellDataAndStatus CaseFilingDate(string data)
    {
        return new CellDataAndStatus(data) { Data = data };
    }
    // CaseType and CaseReason are always fixed in the CLEAN output,
    // regardless of whatever the raw file contains.
    public CellDataAndStatus CaseType(string data)
    {
        return new CellDataAndStatus(data) { Data = "JUDGMENT BY DEFAULT" };
    }
    public CellDataAndStatus CaseReason(string data)
    {
        return new CellDataAndStatus(data) { Data = "DEBT RECOVERY" };
    }
    public CellDataAndStatus Currency(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.ToUpper();
        if (data == "GH" || data == "GHC" || data == "CEDIS" || data == "GHA" || data == "GHS")
        {
            data = "GHS";
        }
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
    public CellDataAndStatus Amount(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Trim().Replace(" ", "").Replace(",", "").Replace("'", "").Replace("(", "").Replace(")", "").Replace("-", "");
        var cl = stringHelper.IsContainingNonNumericCharacters(data);
        if (cl || string.IsNullOrWhiteSpace(data) || data == "0")
        {
            cellData.Passed = false;
            cellData.Errors = new List<string>() { " Amount: Not VALID " };
            return cellData;
        }
        else
        {
            cellData.Data = data.ToUpper();
            return cellData;
        }
    }
}
