namespace CleanserBlazorUI.Converters;
public class BusinessDataTransformerDud
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
    public string Data(string data)
    {
        return "D";
    }
    public string Correctionindicator(string data)
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
        if (data.Length > 0)
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
    public CellDataAndStatus Branchcode(string data)
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
        data = data.ToUpper().Trim().Replace(" ", "").Replace(",", "");
        data = data.Replace("-", "").Replace("#", "").Replace("/", "");
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
    public CellDataAndStatus Sectorindcode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
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
        else if (data == "H" || data == "CONSULTANCYFIRMS" || data == "CONSULTANCY" || data == "PROFESSIONALBODIES" || data == "PROFESSIONAL" || data == "CONSULTANCYFIRMS/PROFESSIONALBODIES")
        {
            data = "CONSULTANCYFIRMS/PROFESSIONALBODIES";
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
            cellData.Errors = new List<string>() { "BUSINESSNAME: INVALID CHARACTERS" };
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
    public CellDataAndStatus Proofofaddtype(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Proofofaddnum(string data)
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
    public CellDataAndStatus Postaddrline1(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data.ToUpper());
        return cellData;
    }
    public CellDataAndStatus Postaddrline2(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus Postaddrline3(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus Postaddrline4(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = stringHelper.BlankIfPhoneNumber(data);
        return cellData;
    }
    public CellDataAndStatus Postaladdpostcode(string data)
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
    public CellDataAndStatus Emailaddress(string data)
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
    public CellDataAndStatus Oldcustomerid(string data)
    {
        var cellData = new CellDataAndStatus(data);
        //data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Oldaccountnum(string data)
    {
        var cellData = new CellDataAndStatus(data);
        //data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Oldsrn(string data)
    {
        var cellData = new CellDataAndStatus(data);
        //data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus Oldbranchcode(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        cellData.Data = data;
        return cellData;
    }
    public CellDataAndStatus ChequeNumber(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Trim().Replace(" ", "");
        var isLettere = stringHelper.IsContainingNonNumericCharacters(data);
        if (string.IsNullOrWhiteSpace(data) || isLettere)
        {
            cellData.Passed = false;
            cellData.Errors = new List<string>() { "ChequeNumber: NOT VALID" };
            return cellData;
        }
        string numberOfZeros = string.Empty;
        if (data.Length < 6)
        {
            for (int i = 0; i < 6 - data.Length; i++)
            {
                numberOfZeros += "0";
            }
        }
        cellData.Data = $"{numberOfZeros}{data.ToUpper()}";
        return cellData;
    }
    public CellDataAndStatus DateAccountOpened(string data)
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
            cellData.Data = string.Empty;
        }
        return cellData;
    }
    public CellDataAndStatus DateIssued(string data)
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
            cellData.Errors = new List<string>() { " DateIssued: INVALID DATE " };
            cellData.Passed = false;
        }
        return cellData;
    }
    public CellDataAndStatus DateBounced(string data)
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
            cellData.Errors = new List<string>() { " DateBounced: INVALID DATE " };
            cellData.Passed = false;
        }
        return cellData;
    }



    public CellDataAndStatus ReasonReturned(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Trim().Replace(" ", "").ToUpper();
        data = data.ToUpper();
        if (data == "11" || data == "INSUFFICIENTFUNDS" || data == "INSUFFICIENTFUNDS" || data == "INSUFFICIENT" || data == "INSUFFICIENTFUND")
        {
            cellData.Data = "INSUFFICIENT FUNDS";
            return cellData;
        }
        else if (data == "12" || data == "FRAUD")
        {
            cellData.Data = "FRAUD";
            return cellData;
        }
        else
        {
            cellData.Data = "INSUFFICIENT FUNDS";
            return cellData;
        }
    }
    public CellDataAndStatus Currency(string data)
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
    public CellDataAndStatus ChequeAmount(string data)
    {
        var cellData = new CellDataAndStatus(data);
        data = stringHelper.RemoveSystemErroNames(data);
        data = data.Replace(",", "");
        data = data.Trim().Replace(" ", "").Replace(",", "").Replace("'", "").Replace("(", "").Replace(")", "").Replace("-", "");
        var cl = stringHelper.IsContainingNonNumericCharacters(data);
        if (cl || string.IsNullOrWhiteSpace(data) || data == "0")
        {
            cellData.Passed = false;
            cellData.Errors = new List<string>() { " ChequeAmount: Not VALID " };
            return cellData;
        }
        else
        {
            cellData.Data = data.ToUpper();
            return cellData;
        }
    }

}
